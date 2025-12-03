use std::time::Instant;

use aoc_helpers::io::parse_int_list;
use log::debug;

pub fn solve(file_contents: &str) -> (String, String) {
    let parse_timer = Instant::now();
    let input = parse_int_list(file_contents, ' ');
    debug!("File parse: ({:?})", parse_timer.elapsed());

    let part1_timer = Instant::now();
    let part1 = solve_part_1(&input);
    debug!("Part 1: {} ({:?})", part1, part1_timer.elapsed());

    let part2_timer = Instant::now();
    let part2 = solve_part_2(&input);
    debug!("Part 2: {} ({:?})", part2, part2_timer.elapsed());

    (part1.to_string(), part2.to_string())
}

fn solve_part_1(numbers: &[i64]) -> i64 {
    let (_, total) = calculate_metadata_sum_rec(numbers, 0);
    total
}

fn solve_part_2(numbers: &[i64]) -> usize {
    let (_, total) = calculate_root_value_rec(numbers, 0);
    total
}

fn calculate_metadata_sum_rec(numbers: &[i64], start_index: usize) -> (usize, i64) {
    let mut index = start_index;
    let child_node_count = numbers[index];
    index += 1;
    let metadata_entries_count = numbers[index];
    index += 1;
    let mut total = 0;
    for _ in 0..child_node_count {
        let (i, t) = calculate_metadata_sum_rec(numbers, index);
        index = i;
        total += t;
    }
    for _ in 0..metadata_entries_count {
        total += numbers[index];
        index += 1;
    }
    (index, total)
}

fn calculate_root_value_rec(numbers: &[i64], start_index: usize) -> (usize, usize) {
    let mut index = start_index;
    let child_node_count = numbers[index] as usize;
    index += 1;
    let metadata_entries_count = numbers[index];
    index += 1;

    let mut child_node_values: Vec<usize> = Vec::with_capacity(child_node_count);
    for _ in 0..child_node_count {
        let (i, t) = calculate_root_value_rec(numbers, index);
        index = i;
        child_node_values.push(t);
    }

    let mut metadata_entries: Vec<usize> = Vec::with_capacity(metadata_entries_count as usize);
    for _ in 0..metadata_entries_count {
        metadata_entries.push(numbers[index] as usize);
        index += 1;
    }

    let mut total = 0;
    if child_node_count == 0 {
        total = metadata_entries.iter().sum();
    } else {
        for entry in metadata_entries {
            let offset_index = entry - 1;
            if offset_index < child_node_count {
                total += child_node_values[entry - 1];
            }
        }
    }
    (index, total)
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let input: [&str; 1] = ["2 3 0 3 10 11 12 1 1 0 1 99 2 1 1 2"];
        let expected: [i64; 1] = [138];

        for i in 0..input.len() {
            let input = parse_int_list(input[i], ' ');
            assert_eq!(solve_part_1(&input), expected[i]);
        }
    }

    #[test]
    fn test_part_2() {
        let input: [&str; 1] = ["2 3 0 3 10 11 12 1 1 0 1 99 2 1 1 2"];
        let expected: [usize; 1] = [66];

        for i in 0..input.len() {
            let input = parse_int_list(input[i], ' ');
            assert_eq!(solve_part_2(&input), expected[i]);
        }
    }
}
