use log::debug;

use std::time::Instant;

use crate::shared::intcode::initialize_computer;
use crate::shared::io::parse_int_list;

pub fn solve(file_contents: String) -> (String, String) {
    let parse_timer = Instant::now();
    let input = parse_int_list(file_contents);
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
    let mut computer = initialize_computer(input);
    computer.input.push(1);
    computer.run();
    if let Some(result) = computer.output.last() {
        return *result;
    }
    unreachable!();
}

fn solve_part_2(input: &[i64]) -> i64 {
    let mut computer = initialize_computer(input);
    computer.input.push(5);
    computer.run();
    if let Some(result) = computer.output.last() {
        return *result;
    }
    unreachable!();
}
