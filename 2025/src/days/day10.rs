use std::collections::VecDeque;
use std::ops::Add;
use std::time::Instant;

use log::debug;
use z3::ast::{Bool, Int};
use z3::{Optimize, SatResult};

pub fn solve(file_contents: &str) -> (String, String) {
    let parse_timer = Instant::now();
    let input = parse_input(file_contents);
    debug!("File parse: ({:?})", parse_timer.elapsed());

    let part1_timer = Instant::now();
    let part1 = solve_part_1(&input);
    debug!("Part 1: {} ({:?})", part1, part1_timer.elapsed());

    let part2_timer = Instant::now();
    let part2 = solve_part_2(&input);
    debug!("Part 2: {} ({:?})", part2, part2_timer.elapsed());

    (part1.to_string(), part2.to_string())
}

fn parse_input(file_contents: &str) -> Vec<MachineInformation> {
    let mut machines: Vec<MachineInformation> = Vec::new();

    for line in file_contents.lines() {
        let parts = line.split_whitespace().collect::<Vec<&str>>();
        if let Some(diagram) = parts.first()
            && let Some(joltage) = parts.last()
        {
            let indicator_light_count = diagram.len() - 2;
            // Convert the diagram to a binary bitmask
            let diagram_values: usize =
                diagram[1..diagram.len() - 1]
                    .chars()
                    .enumerate()
                    .fold(0, |acc, (i, c)| {
                        let shift = indicator_light_count - i - 1;
                        let state = usize::from(c == '#');
                        let mask = state << shift;
                        acc + mask
                    });
            let joltage_values: Vec<i64> = joltage[1..joltage.len() - 1]
                .split(',')
                .filter_map(|j| j.parse().ok())
                .collect();

            // Convert the button actions to a bitmask and indexes
            let mut button_indexes: Vec<Vec<usize>> = Vec::new();
            let mut button_masks: Vec<usize> = Vec::new();
            for i in 1..(parts.len() - 1) {
                if let Some(actions) = parts.get(i) {
                    let mut action = 0;
                    let mut indexes: Vec<usize> = Vec::new();
                    for b in actions[1..actions.len() - 1]
                        .split(',')
                        .filter_map(|j| j.parse::<usize>().ok())
                    {
                        action += 1 << (indicator_light_count - b - 1);
                        indexes.push(b);
                    }
                    button_indexes.push(indexes);
                    button_masks.push(action);
                }
            }

            machines.push(MachineInformation {
                button_indexes,
                button_masks,
                diagram: diagram_values,
                joltage_requirements: joltage_values,
            });
        }
    }

    machines
}

fn solve_part_1(input: &Vec<MachineInformation>) -> i64 {
    let mut total = 0;

    for machine in input {
        let mut queue: VecDeque<(usize, i64)> = VecDeque::new();
        queue.push_back((0, 0));

        while let Some((state, steps)) = queue.pop_front() {
            if state == machine.diagram {
                total += steps;
                break;
            }

            for button in &machine.button_masks {
                // XOR the state with the button to toggle the bitmask
                // E.g.
                //     000110 (state)
                // XOR 010111 (bitmask)
                //   = 010001 (result)
                queue.push_back((state ^ button, steps + 1));
            }
        }
    }

    total
}

fn solve_part_2(input: &Vec<MachineInformation>) -> i64 {
    let mut total = 0;

    // Solve the problem by generating an equation
    for machine in input {
        // Generate variables for the buttons
        let mut button_variables: Vec<Int> = Vec::new();
        for i in 0..machine.button_masks.len() {
            button_variables.push(Int::new_const(format!("button{i}")));
        }

        // Generate solve constraints
        let mut joltage_constraints: Vec<Bool> = Vec::new();
        for (i, &joltage) in machine.joltage_requirements.iter().enumerate() {
            let mut buttons: Vec<&Int> = Vec::new();
            for (j, button) in machine.button_indexes.iter().enumerate() {
                if button.contains(&i) {
                    buttons.push(&button_variables[j]);
                }
            }

            let result = buttons.iter().fold(Int::from(0), |t, &v| t.add(v));
            joltage_constraints.push(result.eq(Int::from(joltage)));
        }

        let zero = Int::from(0);
        let greater_than_zero_constraints: Vec<Bool> =
            button_variables.iter().map(|b| b.ge(&zero)).collect();

        // Add constraints to solver
        let optimizer = Optimize::new();
        optimizer.assert(&Bool::and(&greater_than_zero_constraints));
        optimizer.assert(&Bool::and(&joltage_constraints));

        // Set minimize operation
        optimizer.minimize(&zero.add(&Int::add(&button_variables)));

        // Solve the equations
        let result = optimizer.check(&joltage_constraints);
        if result == SatResult::Sat
            && let Some(model) = optimizer.get_model()
        {
            let mut steps = 0;
            for button in &button_variables {
                if let Some(result) = model.eval(button, true)
                    && let Some(value) = result.as_i64()
                {
                    steps += value;
                }
            }
            total += steps;
        }
    }

    total
}

#[derive(Clone, Debug)]
struct MachineInformation {
    button_indexes: Vec<Vec<usize>>,
    button_masks: Vec<usize>,
    diagram: usize,
    joltage_requirements: Vec<i64>,
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let data: [(&str, i64); 1] = [(
            "[.##.] (3) (1,3) (2) (2,3) (0,2) (0,1) {3,5,4,7}
[...#.] (0,2,3,4) (2,3) (0,4) (0,1,2) (1,2,3,4) {7,5,12,7,2}
[.###.#] (0,1,2,3,4) (0,3,4) (0,1,2,4,5) (1,2) {10,11,11,5,10,5}",
            7,
        )];

        for (input, expected) in data {
            let input = parse_input(input);
            assert_eq!(solve_part_1(&input), expected);
        }
    }

    #[test]
    fn test_part_2() {
        let data: [(&str, i64); 1] = [(
            "[.##.] (3) (1,3) (2) (2,3) (0,2) (0,1) {3,5,4,7}
[...#.] (0,2,3,4) (2,3) (0,4) (0,1,2) (1,2,3,4) {7,5,12,7,2}
[.###.#] (0,1,2,3,4) (0,3,4) (0,1,2,4,5) (1,2) {10,11,11,5,10,5}",
            33,
        )];

        for (input, expected) in data {
            let input = parse_input(input);
            assert_eq!(solve_part_2(&input), expected);
        }
    }
}
