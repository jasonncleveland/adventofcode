use std::cmp::min;
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
    let output_signal = simulate_phases(input, 100);

    let mut power = 1;
    let mut output = 0;
    for i in 0..8 {
        output += output_signal[7 - i] * power;
        power *= 10;
    }
    output
}

fn solve_part_2(input: &[i64]) -> i64 {
    let mut extended_input_signal: Vec<i64> = Vec::with_capacity(input.len() * 10_000);
    for _ in 0..10_000 {
        extended_input_signal.extend_from_slice(input);
    }

    let mut power = 1;
    let mut output_offset: usize = 0;
    for i in 0..7 {
        output_offset += input[6 - i] as usize * power;
        power *= 10;
    }

    let important_digits = extended_input_signal.len() - output_offset + 1;
    let len = extended_input_signal.len();
    for _ in 0..100 {
        let mut total = 0;
        for i in 0..important_digits {
            total += extended_input_signal[len - 1 - i];
            extended_input_signal[len - 1 - i] = total % 10;
        }
    }

    let mut power = 1;
    let mut output = 0;
    for i in 0..8 {
        output += extended_input_signal[output_offset + 7 - i] * power;
        power *= 10;
    }
    output
}

fn simulate_phases(input: &[i64], phase_count: usize) -> Vec<i64> {
    // Pre-compute indexes
    // Base pattern [0, 1, 0, -1]
    let mut indexes: Vec<(Vec<usize>, Vec<usize>)> = Vec::with_capacity(input.len());
    for offset in 0..input.len() {
        let repeat_count = 1 + offset;

        // Add items that match pattern X * 1
        let mut add_indexes: Vec<usize> = Vec::new();
        let mut add_index = offset;
        while add_index < input.len() {
            for i in add_index..min(input.len(), add_index + repeat_count) {
                add_indexes.push(i);
            }
            add_index += repeat_count * 4;
        }

        // Subtract items that match patten X * -1
        let mut subtract_indexes: Vec<usize> = Vec::new();
        let mut subtract_index = offset + repeat_count * 2;
        while subtract_index < input.len() {
            for i in subtract_index..min(input.len(), subtract_index + repeat_count) {
                subtract_indexes.push(i);
            }
            subtract_index += repeat_count * 4;
        }

        indexes.push((add_indexes, subtract_indexes));
    }

    // Memory allocation is expensive so allocate two arrays and swap between
    let mut input_signal = input.to_vec();
    let mut output_signal: Vec<i64> = vec![0; input_signal.len()];
    for _ in 0..phase_count {
        simulate_phase(&input_signal, &mut output_signal, &indexes);
        swap(&mut input_signal, &mut output_signal);
    }
    input_signal
}

fn simulate_phase(input_signal: &[i64], output_signal: &mut [i64], indexes: &[(Vec<usize>, Vec<usize>)]) {
    for offset in 0..input_signal.len() {
        let mut sum = 0;

        // Add items that match pattern X * 1
        for i in &indexes[offset].0 {
            sum += input_signal[*i];
        }

        // Subtract items that match patten X * -1
        for i in &indexes[offset].1 {
            sum -= input_signal[*i];
        }

        let last_digit = sum.abs() % 10;
        output_signal[offset] = last_digit;
    }
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
        let input: [&str; 3] = [
            "03036732577212944063491565474664",
            "02935109699940807407585447034323",
            "03081770884921959731165446850517",
        ];
        let expected: [i64; 3] = [
            84462026,
            78725270,
            53553731,
        ];

        for i in 0..input.len() {
            let parsed = parse_input(input[i].to_string());
            assert_eq!(solve_part_2(&parsed), expected[i]);
        }
    }
}
