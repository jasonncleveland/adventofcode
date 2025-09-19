use std::time::Instant;

use aoc_helpers::direction::{Direction, get_next_direction};
use aoc_helpers::io::parse_int_list;
use aoc_helpers::point2d::Point2d;
use log::{debug, trace};

use crate::shared::intcode::{IntCodeComputer, IntCodeDisplay, IntCodeStatus};

pub fn solve(file_contents: String) -> (String, String) {
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

fn solve_part_1(input: &[i64]) -> usize {
    let mut robot = IntCodeComputer::new(input);
    let mut screen = IntCodeDisplay::new();

    // The starting tile is black
    robot.input.push_back(0);
    paint_tiles(&mut screen, &mut robot);

    // Count number of unique painted tiles
    screen.pixels.len()
}

fn solve_part_2(input: &[i64]) -> String {
    let mut robot = IntCodeComputer::new(input);
    let mut screen = IntCodeDisplay::new();

    // The starting tile is white
    robot.input.push_back(1);
    paint_tiles(&mut screen, &mut robot);

    screen.to_string()
}

fn paint_tiles(screen: &mut IntCodeDisplay, robot: &mut IntCodeComputer) {
    let mut current_position = Point2d::new(0, 0);
    let mut current_direction = Direction::Up;

    while let Ok(status) = robot.run_interactive(1) {
        match status {
            IntCodeStatus::OutputWaiting => continue,
            IntCodeStatus::InputRequired => {
                // Get colour and direction
                let next_colour = match robot.output.pop_front() {
                    Some(0) => '.',
                    Some(1) => '#',
                    _ => unreachable!(),
                };
                let turn_direction = match robot.output.pop_front() {
                    Some(0) => Direction::Left,
                    Some(1) => Direction::Right,
                    _ => unreachable!(),
                };

                // Paint the current tile
                trace!("Painting hull at {} {}", current_position, next_colour);
                screen.pixels.insert(current_position, next_colour);

                // Move to next tile
                let next_direction = get_next_direction(&current_direction, &turn_direction);
                trace!(
                    "Changing direction from {} to {}",
                    current_direction, next_direction
                );
                current_direction = next_direction;

                let next_position = current_position.next(&current_direction);
                trace!(
                    "Moving robot from {} to {}",
                    current_position, next_position
                );
                current_position = next_position;

                // Get colour of current position
                let current_colour = match screen.pixels.get(&current_position) {
                    Some('.') => 0,
                    Some('#') => 1,
                    _ => 0,
                };
                robot.input.push_back(current_colour);
            }
            IntCodeStatus::ProgramHalted => break,
        }
    }
}
