use std::collections::HashMap;
use std::time::Instant;

use aoc_helpers::io::parse_char_grid;
use aoc_helpers::point2d::Point2d;
use log::debug;

pub fn solve(file_contents: &str) -> (String, String) {
    let parse_timer = Instant::now();
    let input = parse_char_grid(file_contents);
    debug!("File parse: ({:?})", parse_timer.elapsed());

    let part1_timer = Instant::now();
    let part1 = solve_part_1(&input);
    debug!("Part 1: {} ({:?})", part1, part1_timer.elapsed());

    let part2_timer = Instant::now();
    let part2 = solve_part_2(&input);
    debug!("Part 2: {} ({:?})", part2, part2_timer.elapsed());

    (part1.to_string(), part2.to_string())
}

fn solve_part_1(input: &HashMap<Point2d, char>) -> i64 {
    let mut total = 0;

    for (point, &value) in input {
        if value != '@' {
            continue;
        }

        let mut neighbouring_rolls = 0;
        for neighbour in point.neighbours8() {
            if matches!(input.get(&neighbour), Some('@')) {
                neighbouring_rolls += 1;
            }
        }
        if neighbouring_rolls < 4 {
            total += 1;
        }
    }

    total
}

fn solve_part_2(input: &HashMap<Point2d, char>) -> i64 {
    let mut total = 0;

    let mut input_copy = input.clone();
    loop {
        let mut to_remove: Vec<Point2d> = Vec::new();
        for (&point, &value) in &input_copy {
            if value != '@' {
                continue;
            }

            let mut neighbouring_rolls = 0;
            for neighbour in point.neighbours8() {
                if matches!(input_copy.get(&neighbour), Some('@')) {
                    neighbouring_rolls += 1;
                }
            }
            if neighbouring_rolls < 4 {
                total += 1;
                to_remove.push(point);
            }
        }

        // Stop when there are no more rolls to remove
        if to_remove.is_empty() {
            break;
        }

        // Remove accessible rolls
        for point in &to_remove {
            input_copy.remove(point);
        }
    }

    total
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let data: [(&str, i64); 1] = [(
            "..@@.@@@@.
@@@.@.@.@@
@@@@@.@.@@
@.@@@@..@.
@@.@@@@.@@
.@@@@@@@.@
.@.@.@.@@@
@.@@@.@@@@
.@@@@@@@@.
@.@.@@@.@.",
            13,
        )];

        for (input, expected) in data {
            let input = parse_char_grid(input);
            assert_eq!(solve_part_1(&input), expected);
        }
    }

    #[test]
    fn test_part_2() {
        let data: [(&str, i64); 1] = [(
            "..@@.@@@@.
@@@.@.@.@@
@@@@@.@.@@
@.@@@@..@.
@@.@@@@.@@
.@@@@@@@.@
.@.@.@.@@@
@.@@@.@@@@
.@@@@@@@@.
@.@.@@@.@.",
            43,
        )];

        for (input, expected) in data {
            let input = parse_char_grid(input);
            assert_eq!(solve_part_2(&input), expected);
        }
    }
}
