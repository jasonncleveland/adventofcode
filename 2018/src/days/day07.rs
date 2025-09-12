use std::collections::HashMap;
use std::time::Instant;

use log::debug;

pub fn solve(file_contents: String) -> (String, String) {
    let parse_timer = Instant::now();
    let input = parse_input(file_contents);
    debug!("File parse: ({:?})", parse_timer.elapsed());

    let part1_timer = Instant::now();
    let part1 = solve_part_1(&input);
    debug!("Part 1: {} ({:?})", part1, part1_timer.elapsed());

    let part2_timer = Instant::now();
    let part2 = solve_part_2(&input, 5, 60);
    debug!("Part 2: {} ({:?})", part2, part2_timer.elapsed());

    (part1.to_string(), part2.to_string())
}

fn parse_input(file_contents: String) -> HashMap<char, Vec<char>> {
    let mut incoming: HashMap<char, Vec<char>> = HashMap::new();
    for line in file_contents.lines() {
        let chars = line.chars().collect::<Vec<char>>();
        incoming.entry(chars[5]).or_default();
        incoming.entry(chars[36]).or_default().push(chars[5]);
    }
    incoming
}

fn solve_part_1(input: &HashMap<char, Vec<char>>) -> String {
    let mut incoming: HashMap<char, Vec<char>> = input.clone();

    sort_steps(&mut incoming)
}

fn solve_part_2(input: &HashMap<char, Vec<char>>, workers: usize, delay: u8) -> i64 {
    let mut incoming: HashMap<char, Vec<char>> = input.clone();

    sort_steps_multi(&mut incoming, workers, delay)
}

fn sort_steps(incoming: &mut HashMap<char, Vec<char>>) -> String {
    let mut result: String = String::new();

    loop {
        // Janky topological sort attempt
        let next_step = get_next_step(incoming);
        if next_step == 'x' {
            break;
        }
        result.push(next_step);
        remove_contains_node(incoming, &next_step);
    }

    result
}

fn sort_steps_multi(incoming: &mut HashMap<char, Vec<char>>, workers: usize, delay: u8) -> i64 {
    let mut result: String = String::new();

    let mut pool: Vec<Process> = Vec::with_capacity(workers);
    for _ in 0..workers {
        pool.push(Process {
            ticks_remaining: 0,
            process: 'x'
        });
    }

    let mut tick = 0i64;
    let mut jobs = incoming.keys().len();
    loop {
        if jobs == 0 {
            break;
        }

        // Assign steps
        for worker in pool.iter_mut() {
            // Attempt to assign step to worker if they have no current process
            if worker.ticks_remaining == 0 {
                let next_available_step = get_next_step(incoming);
                if next_available_step != 'x' {
                    let duration = next_available_step as u8 - b'A' + 1 + delay;
                    worker.ticks_remaining = duration;
                    worker.process = next_available_step;
                }
            }
        }

        // Progress steps
        for worker in pool.iter_mut() {
            // Process one tick of the step
            if worker.process != 'x' {
                worker.ticks_remaining -= 1;
                if worker.ticks_remaining == 0 {
                    // Mark node as completed
                    remove_contains_node(incoming, &worker.process);
                    result.push(worker.process);
                    worker.process = 'x';
                    jobs -= 1;
                }
            }
        }

        tick += 1;
    }

    tick
}

fn get_next_step(incoming: &mut HashMap<char, Vec<char>>) -> char {
    let incoming_clone = incoming.clone();
    let mut empty_nodes = get_empty_nodes(&incoming_clone);
    // Default to alphabetical order if there are multiple nodes
    empty_nodes.sort();
    if empty_nodes.is_empty() {
        return 'x';
    }

    let result = empty_nodes.first();
    if result.is_none() {
        return 'x';
    }
    let char = result.unwrap();
    remove_node(incoming, char);
    **char
}

fn get_empty_nodes(incoming: &HashMap<char, Vec<char>>) -> Vec<&char> {
    incoming
        .iter()
        .filter(|(_, v)| v.is_empty())
        .map(|(k, _)| k)
        .collect()
}

fn remove_node(graph: &mut HashMap<char, Vec<char>>, node: &char) {
    graph.remove(node);
}

fn remove_contains_node(graph: &mut HashMap<char, Vec<char>>, node: &char) {
    graph
        .iter_mut()
        .filter(|(_, v)| v.contains(node))
        .for_each(|(_, v)| { v.swap_remove(v.iter().position(|c| c == node).unwrap()); });
}

#[derive(Debug)]
struct Process {
    ticks_remaining: u8,
    process: char,
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let input: [&str; 1] = [
            "Step C must be finished before step A can begin.
Step C must be finished before step F can begin.
Step A must be finished before step B can begin.
Step A must be finished before step D can begin.
Step B must be finished before step E can begin.
Step D must be finished before step E can begin.
Step F must be finished before step E can begin.",
        ];
        let expected: [&str; 1] = [
            "CABDFE",
        ];

        for i in 0..input.len() {
            let input = parse_input(input[i].to_string());
            assert_eq!(solve_part_1(&input), expected[i]);
        }
    }

    #[test]
    fn test_part_2() {
        let input: [&str; 1] = [
            "Step C must be finished before step A can begin.
Step C must be finished before step F can begin.
Step A must be finished before step B can begin.
Step A must be finished before step D can begin.
Step B must be finished before step E can begin.
Step D must be finished before step E can begin.
Step F must be finished before step E can begin.",
        ];
        let expected: [i64; 1] = [
            15,
        ];

        for i in 0..input.len() {
            let input = parse_input(input[i].to_string());
            assert_eq!(solve_part_2(&input, 2, 0), expected[i]);
        }
    }
}
