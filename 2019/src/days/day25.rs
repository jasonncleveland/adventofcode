use std::{env, io};
use std::collections::{HashSet, VecDeque};
use std::time::Instant;

use log::{debug, info};

use crate::shared::direction::Direction;
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

    (part1.to_string(), "Merry Christmas!".to_string())
}

fn solve_part_1(input: &[i64]) -> String {
    let mut computer = IntCodeComputer::new(input);

    // Set the environment variable AOC_MANUAL_INPUT=true to have manual input
    if let Ok(env_var) = env::var("AOC_MANUAL_INPUT") && let Ok(value) = env_var.parse::<bool>() {
        info!("accepting manual input: {} {:?}", env_var, value);
        run_manual(&mut computer)
    } else {
        run(&mut computer)
    }
}

fn run_manual(computer: &mut IntCodeComputer) -> String {
    let mut display = IntCodeDisplay::new();
    display.set_default_character(' ');

    while let Ok(status) = computer.run_interactive(1) {
        match status {
            IntCodeStatus::OutputWaiting => {
                if let Some(status) = computer.output.pop_front() {
                    display.write_character(status as u8 as char);
                }
            },
            IntCodeStatus::InputRequired => {
                let output = display.to_string();
                output.lines().for_each(|l| info!("{}", l));
                let mut input = String::new();
                if let Ok(_) = io::stdin().read_line(&mut input) {
                    info!("{}", input);
                    for c in input.chars() {
                        computer.input.push_back(c as i64);
                    }
                }
                display.clear();
            },
            IntCodeStatus::ProgramHalted => {
                let output = display.to_string();
                output.lines().for_each(|l| info!("{}", l));
                if output.contains("Pressure-Sensitive Floor") {
                    return output.chars().filter(|char| char.is_ascii_digit()).collect();
                }
                break;
            },
        }
    }
    unreachable!();
}

fn run(computer: &mut IntCodeComputer) -> String {
    let mut display = IntCodeDisplay::new();
    display.set_default_character(' ');

    let mut queue: VecDeque<(IntCodeComputer, Point2d, Vec<String>)> = VecDeque::new();
    let mut visited: HashSet<(Point2d, Vec<String>)> = HashSet::new();

    queue.push_back((computer.clone(), Point2d::new(0, 0), vec![]));
    visited.insert((Point2d::new(0, 0), vec![]));

    while let Some((mut robot, point, inventory)) = queue.pop_front() {
        display.clear();

        while let Ok(status) = robot.run_interactive(1) {
            match status {
                IntCodeStatus::OutputWaiting => {
                    if let Some(status) = robot.output.pop_front() {
                        display.write_character(status as u8 as char);
                    }
                },
                IntCodeStatus::InputRequired => {
                    let output = display.to_string();
                    // If we see the pressure sensitive floor and input is required, our weight is incorrect
                    if output.contains("Pressure-Sensitive Floor") {
                        break;
                    }

                    let mut taking_directions = false;
                    let mut taking_items = false;
                    let mut directions: Vec<String> = Vec::new();
                    let mut items: Vec<String> = vec![String::new()];
                    for line in output.lines() {
                        if taking_directions {
                            if line.starts_with(' ') {
                                taking_directions = false;
                            } else {
                                directions.push(line[2..7].trim().to_string());
                            }
                        }
                        if taking_items {
                            if line.starts_with(' ') {
                                taking_items = false;
                            } else {
                                items.push(line[2..].trim().to_string());
                            }
                        }
                        if line.starts_with("Doors here lead:") {
                            taking_directions = true;
                        }
                        if line.starts_with("Items here:") {
                            taking_items = true;
                        }
                    }

                    for direction in &directions {
                        let next_direction = match direction.as_str() {
                            "north" => Direction::Up,
                            "south" => Direction::Down,
                            "east" => Direction::Right,
                            "west" => Direction::Left,
                            _ => unreachable!("invalid direction received: {}", direction),
                        };

                        let next = point.next(&next_direction);
                        for item in &items {
                            let mut inv_copy = inventory.clone();
                            if item != "" {
                                inv_copy.push(item.to_owned());
                            }
                            if visited.contains(&(next, inv_copy.clone())) {
                                continue;
                            }
                            visited.insert((next, inv_copy.clone()));

                            // Ignore lethal items
                            match item.as_str() {
                                "infinite loop" => continue,
                                "giant electromagnet" => continue,
                                "molten lava" => continue,
                                "photons" => continue,
                                "escape pod" => continue,
                                _ => (),
                            };

                            let mut computer_clone = robot.clone();
                            let mut instructions = String::new();
                            if item != "" {
                                instructions.push_str(format!("take {item}\n").as_str());
                            }
                            instructions.push_str(direction);
                            instructions.push('\n');
                            for character in instructions.chars().map(|c| c as i64) {
                                computer_clone.input.push_back(character);
                            }
                            queue.push_back((computer_clone, next, inv_copy));
                        }
                    }
                    break;
                },
                IntCodeStatus::ProgramHalted => {
                    let output = display.to_string();
                    // If we see the pressure sensitive floor and the program has halted, our weight is correct
                    if output.contains("Pressure-Sensitive Floor") {
                        return output.chars().filter(|char| char.is_ascii_digit()).collect();
                    }
                    break;
                },
            }
        }
    }
    unreachable!();
}
