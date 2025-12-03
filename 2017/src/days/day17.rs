use std::collections::VecDeque;
use std::time::Instant;

use aoc_helpers::io::parse_int;
use log::debug;

pub fn solve(file_contents: &str) -> (String, String) {
    let parse_timer = Instant::now();
    let input = parse_int(file_contents);
    debug!("File parse: ({:?})", parse_timer.elapsed());

    let part1_timer = Instant::now();
    let part1 = solve_part_1(input);
    debug!("Part 1: {} ({:?})", part1, part1_timer.elapsed());

    let part2_timer = Instant::now();
    let part2 = solve_part_2(input);
    debug!("Part 2: {} ({:?})", part2, part2_timer.elapsed());

    (part1.to_string(), part2.to_string())
}

fn solve_part_1(input: i64) -> i64 {
    let mut buffer: VecDeque<i64> = VecDeque::new();
    buffer.push_back(0);

    for i in 1..=2017 {
        buffer.rotate_left(input as usize % buffer.len());
        buffer.push_back(i);
    }

    buffer.rotate_left(1);
    if let Some(&result) = buffer.back() {
        return result;
    }
    unreachable!();
}

fn solve_part_2(input: i64) -> i64 {
    let mut index = 0;
    let mut after_zero = 0;

    for i in 1..=50_000_000 {
        // Calculate the index that the new value should be inserted at
        index = ((index + input) % i) + 1;
        if index == 1 {
            // Keep track of the most recent number inserted at index 1
            after_zero = i;
        }
    }
    after_zero
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let data: [(&str, i64); 1] = [("3", 638)];

        for (input, expected) in data {
            let input = parse_int(input);
            assert_eq!(solve_part_1(input), expected);
        }
    }

    #[test]
    fn test_part_2() {
        let data: [(&str, i64); 1] = [("3", 1222153)];

        for (input, expected) in data {
            let input = parse_int(input);
            assert_eq!(solve_part_2(input), expected);
        }
    }
}
