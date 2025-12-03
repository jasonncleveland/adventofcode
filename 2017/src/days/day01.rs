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

fn parse_input(file_contents: &str) -> Vec<i64> {
    let mut numbers: Vec<i64> = Vec::new();
    for c in file_contents.chars() {
        if let Some(value) = c.to_digit(10) {
            numbers.push(value as i64);
        }
    }
    numbers
}

fn solve_part_1(numbers: &[i64]) -> i64 {
    let mut total = 0;

    if let Some(&last) = numbers.last() {
        let mut previous = last;
        for &number in numbers {
            if number == previous {
                total += number;
            }
            previous = number;
        }
    }

    total
}

fn solve_part_2(numbers: &[i64]) -> i64 {
    let mut total = 0;

    let len = numbers.len();
    let step = numbers.len() / 2;
    for (i, &number) in numbers.iter().enumerate() {
        if numbers[i] == numbers[(i + step) % len] {
            total += number;
        }
    }

    total
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let input: [&str; 4] = ["1122", "1111", "1234", "91212129"];
        let expected: [i64; 4] = [3, 4, 0, 9];

        for i in 0..input.len() {
            let input = parse_input(input[i]);
            assert_eq!(solve_part_1(&input), expected[i]);
        }
    }

    #[test]
    fn test_part_2() {
        let input: [&str; 5] = ["1212", "1221", "123425", "123123", "12131415"];
        let expected: [i64; 5] = [6, 0, 4, 12, 4];

        for i in 0..input.len() {
            let input = parse_input(input[i]);
            assert_eq!(solve_part_2(&input), expected[i]);
        }
    }
}
