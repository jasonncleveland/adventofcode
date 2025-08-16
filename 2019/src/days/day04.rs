use std::time::Instant;

use log::debug;

pub fn solve(file_contents: String) -> (String, String) {
    let parse_timer = Instant::now();
    let input = parse_input(file_contents);
    debug!("File parse: ({:?})", parse_timer.elapsed());

    let part1_timer = Instant::now();
    let part1 = solve_part_1(input);
    debug!("Part 1: {} ({:?})", part1, part1_timer.elapsed());

    let part2_timer = Instant::now();
    let part2 = solve_part_2(input);
    debug!("Part 2: {} ({:?})", part2, part2_timer.elapsed());

    (part1.to_string(), part2.to_string())
}

pub fn parse_input(file_contents: String) -> (i64, i64) {
    if let Some(range) = file_contents.split_once('-')
        && let Ok(start) = range.0.parse::<i64>()
        && let Ok(end) = range.1.parse::<i64>() {
        return (start, end);
    }
    panic!("failed to parse input");
}

fn solve_part_1(input: (i64, i64)) -> i64 {
    let (start, end) = input;

    let mut total = 0;
    for mut password in start..=end {
        let mut last_digit = -1;
        let mut offset = 100_000;

        // Password must be exactly 6 digits
        if !(100_000..999_999).contains(&password) {
            continue;
        }

        let mut is_increasing = true;
        let mut found_repeat = false;
        for _ in 0..6 {
            let digit = password / offset;
            // Password must have at least one set of repeating digits
            if digit == last_digit {
                found_repeat = true;
            }

            // Password must have always increasing digits
            if digit < last_digit {
                is_increasing = false;
                break;
            }

            password %= offset;
            offset /= 10;
            last_digit = digit;
        }
        if is_increasing && found_repeat {
            total += 1;
        }
    }
    total
}

fn solve_part_2(input: (i64, i64)) -> i64 {
    let (start, end) = input;

    let mut total = 0;
    for mut password in start..=end {
        let mut last_digit = -1;
        let mut offset = 100_000;

        // Password must be exactly 6 digits
        if !(100_000..999_999).contains(&password) {
            continue;
        }

        let mut is_increasing = true;
        let mut found_repeat = false;
        let mut repeat_count = 0;
        for _ in 0..6 {
            let digit = password / offset;
            // Password must have at least one group of two repeating digits
            if digit == last_digit {
                repeat_count += 1;
            } else {
                if repeat_count == 2 {
                    found_repeat = true;
                }
                repeat_count = 1;
            }

            // Password must have always increasing digits
            if digit < last_digit {
                is_increasing = false;
                break;
            }

            password %= offset;
            offset /= 10;
            last_digit = digit;
        }
        // Check after all digits have been processed in case repeat is last two digits
        if repeat_count == 2 {
            found_repeat = true;
        }

        if is_increasing && found_repeat {
            total += 1;
        }
    }
    total
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let input: [&str; 8] = [
            "1234-1234",
            "1234567-1234567",
            "122345-122345",
            "111123-111123",
            "135679-135679",
            "111111-111111",
            "223450-223450",
            "123789-123789",
        ];
        let expected: [i64; 8] = [
            0,
            0,
            1,
            1,
            0,
            1,
            0,
            0,
        ];

        for i in 0..input.len() {
            let parsed = parse_input(input[i].to_string());
            assert_eq!(solve_part_1(parsed), expected[i]);
        }
    }

    #[test]
    fn test_part_2() {
        let input: [&str; 11] = [
            "1234-1234",
            "1234567-1234567",
            "122345-122345",
            "111123-111123",
            "135679-135679",
            "111111-111111",
            "223450-223450",
            "123789-123789",
            "112233-112233",
            "123444-123444",
            "111122-111122",
        ];
        let expected: [i64; 11] = [
            0,
            0,
            1,
            0,
            0,
            0,
            0,
            0,
            1,
            0,
            1,
        ];

        for i in 0..input.len() {
            let parsed = parse_input(input[i].to_string());
            assert_eq!(solve_part_2(parsed), expected[i]);
        }
    }
}
