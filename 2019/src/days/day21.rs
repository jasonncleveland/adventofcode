use std::time::Instant;

use log::{debug, trace};

use crate::shared::intcode::{IntCodeComputer, IntCodeDisplay, IntCodeStatus};
use crate::shared::io::parse_int_list;
use crate::shared::point2d::Point2d;

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
    let mut computer = IntCodeComputer::new(input);
    let mut display = IntCodeDisplay::new();
    display.set_default_character(' ');

    // Input commands
    let mut commands = String::new();

    // Jump if any of A, B, or C are unsafe
    commands.push_str("NOT A J\n");
    commands.push_str("NOT B T\n");
    commands.push_str("OR T J\n");
    commands.push_str("NOT C T\n");
    commands.push_str("OR T J\n");
    // Jump if D is safe
    commands.push_str("AND D J\n");
    // Walk (and pray)
    commands.push_str("WALK\n");

    let mut position = Point2d::new(0, 0);
    while let Ok(status) = computer.run_interactive(1) {
        match status {
            IntCodeStatus::OutputWaiting => {
                if let Some(status) = computer.output.pop_front() {
                    if status > u8::MAX as i64 {
                        trace!("received hull damage: {}", status);
                        return status;
                    } else {
                        let value = status as u8 as char;
                        match value {
                            '\n' => {
                                position.x = 0;
                                position.y += 1;
                            },
                            _ => {
                                display.pixels.insert(position, value);
                                position.x += 1;
                            },
                        };
                    }
                }
            },
            IntCodeStatus::InputRequired => {
                display.to_string().lines().for_each(|l| trace!("{}", l));
                display.clear();
                for command in commands.lines() {
                    trace!("inputting command: {}", command);
                    for character in command.chars().map(|c| c as i64) {
                        computer.input.push_back(character);
                    }
                    computer.input.push_back('\n' as i64);
                }
            },
            IntCodeStatus::ProgramHalted => {
                display.to_string().lines().for_each(|l| trace!("{}", l));
                display.clear();
                break;
            },
        }
    }
    unreachable!();
}

fn solve_part_2(input: &[i64]) -> i64 {
    unimplemented!();
}
