use std::time::Instant;

use log::debug;

use crate::shared::device::{parse_device_instructions, Device, Instruction};

pub fn solve(file_contents: String) -> (String, String) {
    let parse_timer = Instant::now();
    let input = parse_device_instructions(file_contents);
    debug!("File parse: ({:?})", parse_timer.elapsed());

    let part1_timer = Instant::now();
    let part1 = solve_part_1(&input);
    debug!("Part 1: {} ({:?})", part1, part1_timer.elapsed());

    let part2_timer = Instant::now();
    let part2 = solve_part_2(&input);
    debug!("Part 2: {} ({:?})", part2, part2_timer.elapsed());

    (part1.to_string(), part2.to_string())
}

fn solve_part_1(instructions: &[Instruction]) -> usize {
    let mut device = Device::new(instructions);
    device.process_instructions();
    device.registers[0]
}

fn solve_part_2(input: &[Instruction]) -> i64 {
    -1
}
