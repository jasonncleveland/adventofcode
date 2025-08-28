use std::mem::swap;
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
    let mut sequence: Vec<i64> = Vec::new();
    for line in file_contents.trim_start_matches("\u{feff}").chars() {
        if let Some(digit) = line.to_digit(10) {
            sequence.push(digit as i64)
        }
    }
    sequence
}

fn solve_part_1(input: &[i64]) -> i64 {
    let mut input_signal = input.to_vec();
    for _ in 0..100 {
        let mut output_signal = simulate_phase(&input_signal);
        swap(&mut input_signal, &mut output_signal);
    }

    let mut power = 1;
    let mut output = 0;
    for i in 0..8 {
        output += input_signal[7 - i] * power;
        power *= 10;
    }
    output
}

fn solve_part_2(input: &[i64]) -> i64 {
    -1
}

fn simulate_phase(input_signal: &[i64]) -> Vec<i64> {
    let base_pattern = [0, 1, 0, -1];

    let mut output_signal: Vec<i64> = Vec::with_capacity(input_signal.len());
    for offset in 0..input_signal.len() {
        let repeat = 1 + offset;
        let mut sum = 0;
        for (i, value) in input_signal.iter().enumerate() {
            let pattern_value = base_pattern[((i + 1) / repeat) % base_pattern.len()];
            let calculated_value = value * pattern_value;
            sum += calculated_value;
        }
        let last_digit = sum.abs() % 10;
        output_signal.push(last_digit);
    }
    output_signal
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let input: [&str; 3] = [
            "80871224585914546619083218645595",
            "19617804207202209144916044189917",
            "69317163492948606335995924319873",
        ];
        let expected: [i64; 3] = [
            24176176,
            73745418,
            52432133,
        ];

        for i in 0..input.len() {
            let parsed = parse_input(input[i].to_string());
            assert_eq!(solve_part_1(&parsed), expected[i]);
        }
    }

    #[test]
    fn test_part_2() {
        let input: [&str; 1] = [
            "",
        ];
        let expected: [i64; 1] = [
            0,
        ];

        for i in 0..input.len() {
            let parsed = parse_input(input[i].to_string());
            assert_eq!(solve_part_2(&parsed), expected[i]);
        }
    }
}
