use std::collections::{HashMap, HashSet, VecDeque};
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
    let part2 = solve_part_2(&input);
    debug!("Part 2: {} ({:?})", part2, part2_timer.elapsed());

    (part1.to_string(), part2.to_string())
}

fn parse_input(file_contents: String) -> HashMap<i64, Program> {
    let mut programs: HashMap<i64, Program> = HashMap::new();
    for line in file_contents.lines() {
        if let Some((program, neighbours)) = line.split_once(" <-> ")
            && let Ok(program_id) = program.parse::<i64>()
        {
            let mut neighbour_ids = HashSet::new();
            for neighbour in neighbours.split(", ") {
                if let Ok(neighbour_id) = neighbour.parse::<i64>() {
                    neighbour_ids.insert(neighbour_id);
                }
            }
            programs.entry(program_id).or_insert(Program {
                id: program_id,
                neighbours: neighbour_ids,
            });
        }
    }
    programs
}

fn solve_part_1(input: &HashMap<i64, Program>) -> i64 {
    let mut queue: VecDeque<Program> = VecDeque::new();
    let mut visited: HashSet<i64> = HashSet::new();

    if let Some(start) = input.get(&0) {
        queue.push_back(start.clone());
        visited.insert(start.id);
    }

    let mut total = 0;
    while let Some(program) = queue.pop_front() {
        total += 1;

        for neighbour_id in program.neighbours {
            if !visited.contains(&neighbour_id)
                && let Some(neighbour) = input.get(&neighbour_id)
            {
                visited.insert(neighbour_id);
                queue.push_back(neighbour.clone());
            }
        }
    }
    total
}

fn solve_part_2(input: &HashMap<i64, Program>) -> i64 {
    let mut queue: VecDeque<Program> = VecDeque::new();
    let mut visited: HashSet<i64> = HashSet::new();

    let mut program_ids: Vec<i64> = input.keys().cloned().collect();

    let mut total = 0;
    while let Some(start_program_id) = program_ids.pop() {
        // Get group origin
        if let Some(start_program) = input.get(&start_program_id) {
            queue.push_back(start_program.clone());
            visited.insert(start_program.id);
        }
        total += 1;

        while let Some(program) = queue.pop_front() {
            // Remove program from possible group origins
            if let Some(index) = program_ids.iter().position(|&p| p == program.id) {
                program_ids.remove(index);
            }

            for neighbour_id in program.neighbours {
                if !visited.contains(&neighbour_id)
                    && let Some(neighbour) = input.get(&neighbour_id)
                {
                    visited.insert(neighbour_id);
                    queue.push_back(neighbour.clone());
                }
            }
        }
    }
    total
}

#[derive(Clone, Debug)]
struct Program {
    id: i64,
    neighbours: HashSet<i64>,
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let data: [(&str, i64); 1] = [(
            "0 <-> 2
1 <-> 1
2 <-> 0, 3, 4
3 <-> 2, 4
4 <-> 2, 3, 6
5 <-> 6
6 <-> 4, 5",
            6,
        )];

        for (input, expected) in data {
            let input = parse_input(input.to_string());
            assert_eq!(solve_part_1(&input), expected);
        }
    }

    #[test]
    fn test_part_2() {
        let data: [(&str, i64); 1] = [(
            "0 <-> 2
1 <-> 1
2 <-> 0, 3, 4
3 <-> 2, 4
4 <-> 2, 3, 6
5 <-> 6
6 <-> 4, 5",
            2,
        )];

        for (input, expected) in data {
            let input = parse_input(input.to_string());
            assert_eq!(solve_part_2(&input), expected);
        }
    }
}
