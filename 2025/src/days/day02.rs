use std::time::Instant;

use log::debug;

pub fn solve(file_contents: &str) -> (String, String) {
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

fn parse_input(file_contents: &str) -> Vec<(i64, i64)> {
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

                // A repeating patter will produce a sequence of 1s and 0s
                // e.g. 123123123 / 123 = 1001001
                let pattern_power = 10i64.pow(pattern_length);
                let pattern = number % pattern_power;
                let groups_count = digits_count / pattern_length;
                if pattern > 0 {
                    let mut n = number;
                    for _ in 0..groups_count {
                        if n / pattern % pattern_power != 1 {
                            continue 'inner;
                        }
                        n /= pattern_power;
                    }
                    total += number;
                    continue 'outer;
                }
            }
        }
    }

    total
}

#[inline]
const fn get_digits_count(number: i64) -> u32 {
    // Log10 is slow to calculate digits so it is faster to calculate manually
    let mut digits_count = 1;
    let mut power = 10;
    while number > power {
        digits_count += 1;
        power *= 10;
    }
    digits_count
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
            let input = parse_input(input);
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
            let input = parse_input(input);
            assert_eq!(solve_part_2(&input), expected);
        }
    }
}
