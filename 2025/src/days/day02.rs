use std::time::Instant;

use aoc_helpers::math::get_digits_count;
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

fn parse_input(file_contents: String) -> Vec<(i64, i64)> {
    let mut turns: Vec<(i64, i64)> = Vec::new();
    for line in file_contents.split(',') {
        if let Some((left, right)) = line.split_once('-')
            && let Ok(l) = left.parse::<i64>()
            && let Ok(r) = right.parse::<i64>()
        {
            turns.push((l, r));
        }
    }
    turns
}

fn solve_part_1(input: &[(i64, i64)]) -> i64 {
    let mut total = 0;

    for &(start, end) in input {
        for number in start..=end {
            let digits_count = get_digits_count(number);
            if !digits_count.is_multiple_of(2) {
                continue;
            }

            // The pattern will always be two numbers repeated,
            // so we can compare one half against the other
            let pattern_length = digits_count / 2;
            let left = number / 10i64.pow(pattern_length);
            let right = number % 10i64.pow(pattern_length);
            if left == right {
                total += number;
            }
        }
    }

    total
}

fn solve_part_2(input: &[(i64, i64)]) -> i64 {
    let mut total = 0;

    for &(start, end) in input {
        'outer: for number in start..=end {
            let digits_count = get_digits_count(number);

            // The max pattern length will be half of the number
            'inner: for pattern_length in 1..=digits_count / 2 {
                if !digits_count.is_multiple_of(pattern_length) {
                    continue;
                }

                let groups_count = digits_count / pattern_length;
                let pattern_power = 10i64.pow(pattern_length);
                let mut divisor = 10i64.pow(pattern_length * (groups_count - 1));
                let mut remainder = number % divisor;
                let start_pattern = number / divisor;
                divisor /= pattern_power;
                for _ in 1..groups_count {
                    let found_pattern = remainder / divisor;
                    if found_pattern != start_pattern {
                        continue 'inner;
                    }
                    remainder %= divisor;
                    divisor /= pattern_power;
                }

                total += number;
                continue 'outer;
            }
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
            "11-22,95-115,998-1012,1188511880-1188511890,222220-222224,\
            1698522-1698528,446443-446449,38593856-38593862,565653-565659,\
            824824821-824824827,2121212118-2121212124",
            1227775554,
        )];

        for (input, expected) in data {
            let input = parse_input(input.to_string());
            assert_eq!(solve_part_1(&input), expected);
        }
    }

    #[test]
    fn test_part_2() {
        let data: [(&str, i64); 1] = [(
            "11-22,95-115,998-1012,1188511880-1188511890,222220-222224,\
            1698522-1698528,446443-446449,38593856-38593862,565653-565659,\
            824824821-824824827,2121212118-2121212124",
            4174379265,
        )];

        for (input, expected) in data {
            let input = parse_input(input.to_string());
            assert_eq!(solve_part_2(&input), expected);
        }
    }
}
