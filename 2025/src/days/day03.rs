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

fn parse_input(file_contents: &str) -> Vec<Vec<i64>> {
    let mut turns: Vec<Vec<i64>> = Vec::new();
    for line in file_contents.lines() {
        turns.push(line.chars().map(|c| c as i64 - '0' as i64).collect());
    }
    turns
}

fn solve_part_1(input: &[Vec<i64>]) -> i64 {
    input.iter().map(|bank| find_largest_number(bank, 2)).sum()
}

fn solve_part_2(input: &[Vec<i64>]) -> i64 {
    input.iter().map(|bank| find_largest_number(bank, 12)).sum()
}

#[inline]
fn find_largest_number(digits: &[i64], digits_count: usize) -> i64 {
    // Find digits
    let mut found_digits: Vec<i64> = Vec::new();
    let mut index = 0;
    let mut num_digits = digits_count;
    for _ in 0..digits_count {
        let to_search = digits.len() - num_digits;
        let mut highest_digit = -1;
        let mut highest_index = 0;
        for i in index..=to_search {
            if let Some(&result) = digits.get(i)
                && result > highest_digit
            {
                highest_index = i;
                highest_digit = result;
            }
        }
        found_digits.push(highest_digit);
        index = highest_index + 1;
        num_digits -= 1;
    }

    // Compute number
    let mut exponent = 1;
    let mut computed_num = 0;
    for digit in found_digits.iter().rev() {
        computed_num += digit * exponent;
        exponent *= 10;
    }
    computed_num
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let data: [(&str, i64); 1] = [(
            "987654321111111
811111111111119
234234234234278
818181911112111",
            357,
        )];

        for (input, expected) in data {
            let input = parse_input(input);
            assert_eq!(solve_part_1(&input), expected);
        }
    }

    #[test]
    fn test_part_2() {
        let data: [(&str, i64); 1] = [(
            "987654321111111
811111111111119
234234234234278
818181911112111",
            3_121_910_778_619,
        )];

        for (input, expected) in data {
            let input = parse_input(input);
            assert_eq!(solve_part_2(&input), expected);
        }
    }
}
