use std::collections::HashSet;
use std::time::Instant;

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

fn parse_input(file_contents: String) -> Vec<i64> {
    let mut numbers: Vec<i64> = Vec::new();
    for line in file_contents.lines() {
        if let Ok(value) = line.parse::<i64>() {
            numbers.push(value);
        }
    }
    numbers
}

fn solve_part_1(numbers: &[i64]) -> i64 {
    let mut frequency: i64 = 0;
    for value in numbers {
        frequency += value;
    }
    frequency
}

fn solve_part_2(numbers: &[i64]) -> i64 {
    let mut occurrences: HashSet<i64> = HashSet::new();
    let mut frequency: i64 = 0;
    occurrences.insert(frequency);
    let mut i: usize = 0;
    loop {
        frequency += numbers[i % numbers.len()];
        if occurrences.contains(&frequency) {
            return frequency;
        }
        occurrences.insert(frequency);
        i += 1;
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let input: [&str; 4] = ["+1\n-2\n+3\n+1", "+1\n+1\n+1", "+1\n+1\n-2", "-1\n-2\n-3"];
        let expected: [i64; 4] = [3, 3, 0, -6];

        for i in 0..input.len() {
            let input = parse_input(input[i].to_string());
            assert_eq!(solve_part_1(&input), expected[i]);
        }
    }

    #[test]
    fn test_part_2() {
        let input: [&str; 5] = [
            "+1\n-2\n+3\n+1",
            "+1\n-1",
            "+3\n+3\n+4\n-2\n-4",
            "-6\n+3\n+8\n+5\n-6",
            "+7\n+7\n-2\n-7\n-4",
        ];
        let expected: [i64; 5] = [2, 0, 10, 5, 14];

        for i in 0..input.len() {
            let input = parse_input(input[i].to_string());
            assert_eq!(solve_part_2(&input), expected[i]);
        }
    }
}
