use std::time::Instant;

use aoc_helpers::io::parse_char_list;
use log::{debug, trace};

pub fn solve(file_contents: String) -> (String, String) {
    let parse_timer = Instant::now();
    let input = parse_char_list(file_contents);
    debug!("File parse: ({:?})", parse_timer.elapsed());

    let part1_timer = Instant::now();
    let part1 = solve_part_1(&input);
    debug!("Part 1: {} ({:?})", part1, part1_timer.elapsed());

    let part2_timer = Instant::now();
    let part2 = solve_part_2(&input);
    debug!("Part 2: {} ({:?})", part2, part2_timer.elapsed());

    (part1.to_string(), part2.to_string())
}

fn solve_part_1(input: &[char]) -> i64 {
    let (count, _) = parse_stream(input);
    count
}

fn solve_part_2(input: &[char]) -> i64 {
    let (_, count) = parse_stream(input);
    count
}

fn parse_stream(input: &[char]) -> (i64, i64) {
    let mut total = 0;
    let mut depth = 0;
    let mut index = 0;
    let mut in_garbage = false;
    let mut garbage_count = 0;
    while index < input.len() {
        if in_garbage {
            match input[index] {
                '>' => {
                    trace!("exiting garbage at depth {}", depth);
                    in_garbage = false;
                }
                '!' => {
                    trace!("ignoring next character");
                    index += 1;
                }
                other => {
                    trace!("ignoring unknown character in garbage: {}", other);
                    garbage_count += 1;
                }
            }
        } else {
            match input[index] {
                '{' => {
                    trace!("entering new group at depth {}", depth);
                    depth += 1;
                }
                '}' => {
                    trace!("exiting group at depth {}", depth);
                    total += depth;
                    depth -= 1;
                }
                '<' => {
                    trace!("entering garbage at depth {}", depth);
                    in_garbage = true;
                }
                other => {
                    trace!("ignoring unknown character in group: {}", other);
                }
            }
        }
        index += 1;
    }
    (total, garbage_count)
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let data: [(&str, i64); 8] = [
            ("{}", 1),
            ("{{{}}}", 6),
            ("{{},{}}", 5),
            ("{{{},{},{{}}}}", 16),
            ("{<a>,<a>,<a>,<a>}", 1),
            ("{{<ab>},{<ab>},{<ab>},{<ab>}}", 9),
            ("{{<!!>},{<!!>},{<!!>},{<!!>}}", 9),
            ("{{<a!>},{<a!>},{<a!>},{<ab>}}", 3),
        ];

        for (input, expected) in data {
            let input = parse_char_list(input.to_string());
            assert_eq!(solve_part_1(&input), expected);
        }
    }

    #[test]
    fn test_part_2() {
        let data: [(&str, i64); 7] = [
            ("{<>}", 0),
            ("{<random characters>}", 17),
            ("{<<<<>}", 3),
            ("{<{!>}>}", 2),
            ("{<!!>}", 0),
            ("{<!!!>>}", 0),
            ("{<{o\"i!a,<{i<a>}", 10),
        ];

        for (input, expected) in data {
            let input = parse_char_list(input.to_string());
            assert_eq!(solve_part_2(&input), expected);
        }
    }
}
