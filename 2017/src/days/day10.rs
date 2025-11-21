use std::collections::VecDeque;
use std::time::Instant;

use aoc_helpers::io::parse_int_list;
use log::{debug, trace};

pub fn solve(file_contents: String) -> (String, String) {
    let parse_timer = Instant::now();
    let input = parse_int_list(file_contents.clone(), ',');
    debug!("File parse: ({:?})", parse_timer.elapsed());

    let part1_timer = Instant::now();
    let part1 = solve_part_1(&input, 256);
    debug!("Part 1: {} ({:?})", part1, part1_timer.elapsed());

    let part2_timer = Instant::now();
    let part2 = solve_part_2(file_contents.clone());
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

fn solve_part_2(input: String) -> String {
    let mut numbers: VecDeque<i64> = (0..256).collect();
    let mut current_position = 0;
    let mut skip_size = 0;

    let mut lengths: VecDeque<usize> = input.chars().map(|c| c as usize).collect();
    lengths.extend([17, 31, 73, 47, 23]);

    for _ in 0..64 {
        for length in &lengths {
            reverse_numbers(&mut numbers, current_position, current_position + length);
            current_position += length + skip_size;
            skip_size += 1;
        }
    }

    let mut hash = String::new();
    for i in 0..16 {
        let mut reduced = 0;
        for j in 0..16 {
            reduced ^= numbers[i * 16 + j];
        }
        hash.push_str(&format!("{:02x}", reduced));
    }
    hash
}

fn reverse_numbers(numbers: &mut VecDeque<i64>, start: usize, end: usize) {
    trace!(
        "reversing numbers from start index {} to end index {} of {:?}",
        start, end, numbers
    );
    let mut i = start;
    let mut j = end - 1;
    while i < j {
        numbers.swap(i % numbers.len(), j % numbers.len());
        i += 1;
        j -= 1;
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let data: [(&str, i64, i64); 1] = [("3,4,1,5", 5, 12)];

        for (input, size, expected) in data {
            let input = parse_int_list(input.to_string(), ',');
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
            assert_eq!(solve_part_2(input.to_string()), expected);
        }
    }
}
