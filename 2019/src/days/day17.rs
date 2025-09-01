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

    let mut position = Point2d::new(0, 0);
    while let Ok(status) = computer.run_interactive(1) {
        match status {
            IntCodeStatus::OutputWaiting => {
                if let Some(status) = computer.output.pop_front() {
                    let value = status as u8 as char;
                    match value {
                        '\n' => {
                            position.x = 0;
                            position.y += 1;
                        },
                        '<' | '^' | 'v' | '>' => {
                            display.pixels.insert(position, '#');
                            position.x += 1;
                        }
                        '#' | '.' => {
                            display.pixels.insert(position, value);
                            position.x += 1;
                        },
                        _ => unreachable!("invalid character"),
                    };
                }
            },
            IntCodeStatus::InputRequired => unreachable!(),
            IntCodeStatus::ProgramHalted => break
        }
    }

    let mut total = 0;
    for (position, value) in &display.pixels {
        if *value == '#' {
            let mut neighbour_count = 0;
            for neighbour in position.neighbours() {
                if let Some(neighbour_value) = display.pixels.get(&neighbour) && *neighbour_value == '#' {
                    neighbour_count += 1;
                }
            }
            if neighbour_count > 2 {
                trace!("found intersection at {} -> {} x {} = {}", position, position.x, position.y, position.x * position.y);
                total += position.x * position.y;
            }
        }
    }
    total
}

fn solve_part_2(input: &[i64]) -> i64 {
    -1
}
