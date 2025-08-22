use std::collections::HashMap;
use std::time::Instant;

use log::{debug, trace};

use crate::shared::colour::Colour;
use crate::shared::direction::{get_next_direction, Direction};
use crate::shared::intcode::{IntCodeComputer, IntCodeError};
use crate::shared::io::parse_int_list;
use crate::shared::point::Point2d;

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

fn solve_part_1(input: &[i64]) -> usize {
    let mut robot = IntCodeComputer::new(input);
    let mut tiles: HashMap<Point2d, Colour> = HashMap::new();

    // The starting tile is black
    robot.input.push_back(0);
    paint_tiles(&mut tiles, &mut robot);

    // Count number of unique painted tiles
    tiles.len()
}

fn solve_part_2(input: &[i64]) -> String {
    let mut robot = IntCodeComputer::new(input);
    let mut tiles: HashMap<Point2d, Colour> = HashMap::new();

    // The starting tile is white
    robot.input.push_back(1);
    paint_tiles(&mut tiles, &mut robot);

    print_tiles(&tiles)
}

fn paint_tiles(tiles: &mut HashMap<Point2d, Colour>, robot: &mut IntCodeComputer) {
    let mut current_position = Point2d::new(0, 0);
    let mut current_direction = Direction::Up;

    loop {
        match robot.process_instruction() {
            Ok(result) => match result {
                true => continue,
                false => {
                    // Stop when program halts
                    trace!("program halted: {:?}", robot.output);
                    break;
                },
            },
            Err(IntCodeError::NoInputGiven) => {
                // Get colour and direction
                let next_colour = match robot.output.pop_front() {
                    Some(0) => Colour::Black,
                    Some(1) => Colour::White,
                    _ => unreachable!(),
                };
                let turn_direction = match robot.output.pop_front() {
                    Some(0) => Direction::Left,
                    Some(1) => Direction::Right,
                    _ => unreachable!(),
                };

                // Paint the current tile
                trace!("Painting hull at {} {}", current_position, next_colour);
                tiles.insert(current_position, next_colour);

                // Move to next tile
                let next_direction = get_next_direction(&current_direction, &turn_direction);
                trace!("Changing direction from {} to {}", current_direction, next_direction);
                current_direction = next_direction;

                let next_position = current_position.next(&current_direction);
                trace!("Moving robot from {} to {}", current_position, next_position);
                current_position = next_position;

                // Get colour of current position
                let current_colour = match tiles.get(&current_position) {
                    Some(Colour::Black) => 0,
                    Some(Colour::White) => 1,
                    None => 0,
                };
                robot.input.push_back(current_colour);
            },
            Err(error) => panic!("Unexpected error: {}", error),
        }
    }
}

fn print_tiles(tiles: &HashMap<Point2d, Colour>) -> String {
    // Get boundaries
    let mut min_x = i64::MAX;
    let mut min_y = i64::MAX;
    let mut max_x = i64::MIN;
    let mut max_y = i64::MIN;

    for point in tiles.keys() {
        if let Some(c) = tiles.get(point) && *c != Colour::White {
            continue;
        }

        if point.x < min_x {
            min_x = point.x;
        }
        if point.y < min_y {
            min_y = point.y;
        }
        if point.x > max_x {
            max_x = point.x;
        }
        if point.y > max_y {
            max_y = point.y;
        }
    }

    // Get tile values
    let mut output = String::new();
    for y in min_y..=max_y {
        output.push('\n');
        for x in min_x..=max_x {
            let value = match tiles.get(&Point2d::new(x, y)) {
                Some(Colour::Black) => '.',
                Some(Colour::White) => '#',
                None => '.',
            };
            output.push(value);
        }
    }

    output
}
