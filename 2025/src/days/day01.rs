use std::time::Instant;

use aoc_helpers::direction::Direction;
use log::debug;

pub fn solve(file_contents: String) -> (String, String) {
    let parse_timer = Instant::now();
    let input = parse_input(file_contents);
    debug!("File parse: ({:?})", parse_timer.elapsed());

    let part1_timer = Instant::now();
    let part1 = solve_part_1(&input);
    debug!("Part 1: {} ({:?})", part1, part1_timer.elapsed());

    let part2_timer = Instant::now();
    let part2 = solve_part_2(&input);
    debug!("Part 2: {} ({:?})", part2, part2_timer.elapsed());

    (part1.to_string(), part2.to_string())
}

fn parse_input(file_contents: String) -> Vec<DialTurn> {
    let mut turns: Vec<DialTurn> = Vec::new();
    for line in file_contents.lines() {
        let mut chars = line.chars();
        if let Some(direction) = chars.next()
            && let Ok(amount) = chars.as_str().parse::<i64>()
        {
            turns.push(DialTurn {
                direction: match direction {
                    'L' => Direction::Left,
                    'R' => Direction::Right,
                    _ => unreachable!(),
                },
                amount,
            })
        }
    }
    turns
}

fn solve_part_1(input: &[DialTurn]) -> i64 {
    let mut total = 0;

    let mut dial_position = 50;
    for turn in input {
        match turn.direction {
            Direction::Left => {
                dial_position -= turn.amount % 100;
                if dial_position < 0 {
                    // Loop around to 99
                    dial_position += 100;
                }
            }
            Direction::Right => {
                dial_position += turn.amount % 100;
                if dial_position > 99 {
                    // Loop around to 0
                    dial_position %= 100;
                }
            }
            _ => unreachable!(),
        }

        // Increment if we stop on 0
        if dial_position == 0 {
            total += 1;
        }
    }

    total
}

fn solve_part_2(input: &[DialTurn]) -> i64 {
    let mut total = 0;

    let mut dial_position = 50;
    for turn in input {
        let mut turn_amount = turn.amount;
        if turn_amount > 99 {
            // If the turn is 100 or more then we have a full rotation
            // Simplify the logic by skipping full rotations
            total += turn.amount / 100;
            turn_amount %= 100;
        }
        match turn.direction {
            Direction::Left => {
                if dial_position - turn_amount >= 0 {
                    dial_position -= turn_amount;
                } else {
                    for i in 0..turn_amount {
                        dial_position -= 1;
                        if dial_position < 0 {
                            dial_position = 99;
                        }

                        // Increment if we pass through 0
                        if dial_position == 0 && i != turn_amount - 1 {
                            total += 1;
                        }
                    }
                }
            }
            Direction::Right => {
                if dial_position + turn_amount < 100 {
                    dial_position += turn_amount;
                } else {
                    for i in 0..turn_amount {
                        dial_position += 1;
                        if dial_position > 99 {
                            dial_position = 0;
                        }

                        // Increment if we pass through 0
                        if dial_position == 0 && i != turn_amount - 1 {
                            total += 1;
                        }
                    }
                }
            }
            _ => unreachable!(),
        }

        // Increment if we stop on 0
        if dial_position == 0 {
            total += 1;
        }
    }

    total
}

#[derive(Debug)]
struct DialTurn {
    direction: Direction,
    amount: i64,
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let data: [(&str, i64); 1] = [(
            "L68
L30
R48
L5
R60
L55
L1
L99
R14
L82",
            3,
        )];

        for (input, expected) in data {
            let input = parse_input(input.to_string());
            assert_eq!(solve_part_1(&input), expected);
        }
    }

    #[test]
    fn test_part_2() {
        let data: [(&str, i64); 1] = [(
            "L68
L30
R48
L5
R60
L55
L1
L99
R14
L82",
            6,
        )];

        for (input, expected) in data {
            let input = parse_input(input.to_string());
            assert_eq!(solve_part_2(&input), expected);
        }
    }
}
