use std::time::Instant;

use aoc_helpers::io::parse_int_list;
use log::{debug, trace};

use crate::shared::intcode::IntCodeComputer;

pub fn solve(file_contents: &str) -> (String, String) {
    let parse_timer = Instant::now();
    let input = parse_int_list(file_contents, ',');
    debug!("File parse: ({:?})", parse_timer.elapsed());

    let part1_timer = Instant::now();
    let part1 = solve_part_1(&input);
    debug!("Part 1: {} ({:?})", part1, part1_timer.elapsed());

    let part2_timer = Instant::now();
    let part2 = solve_part_2(&input);
    debug!("Part 2: {} ({:?})", part2, part2_timer.elapsed());

    (part1.to_string(), part2.to_string())
}

fn solve_part_1(input: &[i64]) -> i64 {
    let mut computer = IntCodeComputer::new(input);
    computer.memory[1] = 12;
    computer.memory[2] = 2;
    computer.run();
    computer.memory[0]
}

fn solve_part_2(input: &[i64]) -> i64 {
    for noun in 0..100 {
        for verb in 0..100 {
            trace!("Noun: {}, Verb: {}", noun, verb);
            let mut computer = IntCodeComputer::new(input);
            computer.memory[1] = noun;
            computer.memory[2] = verb;
            computer.run();
            if computer.memory[0] == 19690720 {
                trace!("Found noun: {} verb: {}", noun, verb);
                return 100 * noun + verb;
            }
        }
    }
    panic!("Could not find a noun and verb")
}
