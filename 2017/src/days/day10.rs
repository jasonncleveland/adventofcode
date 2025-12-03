use std::collections::VecDeque;
use std::time::Instant;

use aoc_helpers::io::parse_int_list;
use log::debug;

use crate::shared::knot_hash::{calculate_knot_hash, reverse_numbers};

pub fn solve(file_contents: &str) -> (String, String) {
    let parse_timer = Instant::now();
    let input = parse_int_list(file_contents, ',');
    debug!("File parse: ({:?})", parse_timer.elapsed());

    let part1_timer = Instant::now();
    let part1 = solve_part_1(&input, 256);
    debug!("Part 1: {} ({:?})", part1, part1_timer.elapsed());

    let part2_timer = Instant::now();
    let part2 = solve_part_2(file_contents);
    debug!("Part 2: {} ({:?})", part2, part2_timer.elapsed());

    (part1.to_string(), part2.to_string())
}

fn solve_part_1(lengths: &[i64], size: i64) -> i64 {
    let mut numbers: VecDeque<i64> = (0..size).collect();
    let mut current_position = 0;

    for (skip_size, length) in lengths.iter().enumerate() {
        reverse_numbers(
            &mut numbers,
            current_position,
            current_position + *length as usize,
        );
        current_position += *length as usize + skip_size;
    }

    numbers[0] * numbers[1]
}

fn solve_part_2(input: &str) -> String {
    calculate_knot_hash(input)
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let data: [(&str, i64, i64); 1] = [("3,4,1,5", 5, 12)];

        for (input, size, expected) in data {
            let input = parse_int_list(input, ',');
            assert_eq!(solve_part_1(&input, size), expected);
        }
    }

    #[test]
    fn test_part_2() {
        let data: [(&str, &str); 4] = [
            ("", "a2582a3a0e66e6e86e3812dcb672a272"),
            ("AoC 2017", "33efeb34ea91902bb2f59c9920caa6cd"),
            ("1,2,3", "3efbe78a8d82f29979031a4aa0b16a9d"),
            ("1,2,4", "63960835bcdc130f0b66d7ff4f6a5a8e"),
        ];

        for (input, expected) in data {
            assert_eq!(solve_part_2(input), expected);
        }
    }
}
