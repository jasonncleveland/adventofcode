use std::collections::{HashSet, VecDeque};
use std::time::Instant;

use log::{debug, trace};
use crate::shared::direction::Direction;
use crate::shared::intcode::{IntCodeComputer, IntCodeError};
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

fn solve_part_1(input: &[i64]) -> usize {
    let directions = vec![Direction::Up, Direction::Down, Direction::Left, Direction::Right];

    let mut queue: VecDeque<Droid> = VecDeque::new();
    let mut visited: HashSet<Point2d> = HashSet::new();
    queue.push_back(Droid::new(input));
    visited.insert(Point2d::new(0, 0));

    while let Some(mut droid) = queue.pop_front() {
        loop {
            match droid.computer.process_instruction() {
                Ok(result) => match result {
                    true => {
                        if droid.computer.output.len() > 0
                            && let Some(status) = droid.computer.output.pop_front() {
                            match status {
                                0 => {
                                    trace!("Droid hit a wall at {}", droid.position);
                                    break;
                                },
                                1 => {
                                    trace!("Droid has successfully moved to {}", droid.position);
                                    continue;
                                },
                                2 => {
                                    debug!("Found target after {} moves", droid.steps);
                                    return droid.steps;
                                },
                                _ => unreachable!(),
                            };
                        }
                    },
                    false => {
                        // Stop when program halts
                        trace!("program halted: {:?}", droid.computer.output);
                        break;
                    },
                },
                Err(IntCodeError::NoInputGiven) => {
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
                Err(error) => panic!("Unexpected error: {}", error),
            }
        }
    }
    unreachable!();
}

fn solve_part_2(input: &[i64]) -> i64 {
    -1
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
