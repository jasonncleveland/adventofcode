use std::collections::{HashSet, VecDeque};
use std::time::Instant;

use log::{debug, trace};

use crate::shared::direction::Direction;
use crate::shared::intcode::{IntCodeComputer, IntCodeStatus};
use crate::shared::io::parse_int_list;
use crate::shared::point2d::Point2d;

pub fn solve(file_contents: String) -> (String, String) {
    let parse_timer = Instant::now();
    let input = parse_int_list(file_contents);
    debug!("File parse: ({:?})", parse_timer.elapsed());

    let part1_timer = Instant::now();
    let (part1, droid) = solve_part_1(&input);
    debug!("Part 1: {} ({:?})", part1, part1_timer.elapsed());

    let part2_timer = Instant::now();
    let part2 = solve_part_2(droid);
    debug!("Part 2: {} ({:?})", part2, part2_timer.elapsed());

    (part1.to_string(), part2.to_string())
}

fn solve_part_1(input: &[i64]) -> (usize, Droid) {
    let directions = [Direction::Up, Direction::Down, Direction::Left, Direction::Right];

    let mut queue: VecDeque<Droid> = VecDeque::new();
    let mut visited: HashSet<Point2d> = HashSet::new();
    queue.push_back(Droid::new(input));
    visited.insert(Point2d::new(0, 0));

    while let Some(mut droid) = queue.pop_front() {
        while let Ok(status) = droid.computer.run_interactive(1) {
            match status {
                IntCodeStatus::OutputWaiting => {
                    if let Some(status) = droid.computer.output.pop_front() {
                        match status {
                            // Droid hit a wall
                            0 => break,
                            // Droid successfully moved
                            1 => continue,
                            // Droid found target location
                            2 => return (droid.steps, droid),
                            _ => unreachable!(),
                        };
                    }
                },
                IntCodeStatus::InputRequired => {
                    for (i, direction) in directions.iter().enumerate() {
                        let mut droid_clone = droid.clone();
                        let next_position = droid_clone.position.next(direction);
                        if !visited.contains(&next_position) {
                            trace!("moving in direction {} {} from {} to {}", i + 1, direction, droid_clone.position, next_position);
                            visited.insert(next_position);
                            droid_clone.computer.input.push_back(i as i64 + 1);
                            droid_clone.position = next_position;
                            droid_clone.steps += 1;
                            queue.push_back(droid_clone);
                        }
                    }
                    break;
                },
                IntCodeStatus::ProgramHalted => break
            }
        }
    }
    unreachable!();
}

fn solve_part_2(droid: Droid) -> i64 {
    let directions = [Direction::Up, Direction::Down, Direction::Left, Direction::Right];

    // Spread oxygen
    let mut visited: HashSet<Point2d> = HashSet::new();
    visited.insert(droid.position);

    let mut locations: Vec<Droid> = vec![droid];

    let mut minutes = 0;
    loop {
        let mut next_locations: Vec<Droid> = Vec::new();
        for droid in &mut locations {
            while let Ok(status) = droid.computer.run_interactive(1) {
                match status {
                    IntCodeStatus::OutputWaiting => {
                        if let Some(status) = droid.computer.output.pop_front() {
                            match status {
                                // Droid hit a wall
                                0 => break,
                                // Droid successfully moved
                                1 => continue,
                                _ => unreachable!(),
                            };
                        }
                    },
                    IntCodeStatus::InputRequired => {
                        for (i, direction) in directions.iter().enumerate() {
                            let mut droid_clone = droid.clone();
                            let next_position = droid_clone.position.next(direction);
                            if !visited.contains(&next_position) {
                                trace!("moving in direction {} {} from {} to {}", i + 1, direction, droid_clone.position, next_position);
                                visited.insert(next_position);
                                droid_clone.computer.input.push_back(i as i64 + 1);
                                droid_clone.position = next_position;
                                droid_clone.steps += 1;
                                next_locations.push(droid_clone);
                            }
                        }
                        break;
                    },
                    IntCodeStatus::ProgramHalted => break
                }
            }
        }

        if next_locations.is_empty() {
            return minutes;
        }

        locations = next_locations.clone();
        minutes += 1;
    }
}

#[derive(Clone, Debug)]
struct Droid {
    computer: IntCodeComputer,
    position: Point2d,
    steps: usize,
}

impl Droid {
    fn new(program: &[i64]) -> Self {
        Droid {
            computer: IntCodeComputer::new(program),
            position: Point2d::new(0, 0),
            steps: 0,
        }
    }
}
