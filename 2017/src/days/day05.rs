use std::time::Instant;

use aoc_helpers::io::parse_int_list;
use log::debug;

pub fn solve(file_contents: &str) -> (String, String) {
    let parse_timer = Instant::now();
    let input = parse_int_list(file_contents, '\n');
    debug!("File parse: ({:?})", parse_timer.elapsed());

    let part1_timer = Instant::now();
    let part1 = solve_part_1(&input);
    debug!("Part 1: {} ({:?})", part1, part1_timer.elapsed());

    let part2_timer = Instant::now();
    let part2 = solve_part_2(&input);
    debug!("Part 2: {} ({:?})", part2, part2_timer.elapsed());

    (part1.to_string(), part2.to_string())
}

fn solve_part_1(input: &[i64]) -> i64 {
    let mut total = 0;

    let mut index: i64 = 0;

    let mut input_copy = input.to_owned();
    while let Some(offset) = input_copy.get_mut(index as usize) {
        total += 1;
        index += *offset;
        *offset += 1;
    }

    total
}

fn solve_part_2(input: &[i64]) -> i64 {
    let mut total = 0;

    let mut index: i64 = 0;

    let mut input_copy = input.to_owned();
    while let Some(offset) = input_copy.get_mut(index as usize) {
        total += 1;
        index += *offset;
        if *offset >= 3 {
            *offset -= 1;
        } else {
            *offset += 1;
        }
    }

    total
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let data: [(&str, i64); 1] = [("0\n3\n0\n1\n-3", 5)];

        for (input, expected) in data {
            let input = parse_int_list(input, '\n');
            assert_eq!(solve_part_1(&input), expected);
        }
    }

    #[test]
    fn test_part_2() {
        let data: [(&str, i64); 1] = [("0\n3\n0\n1\n-3", 10)];

        for (input, expected) in data {
            let input = parse_int_list(input, '\n');
            assert_eq!(solve_part_2(&input), expected);
        }
    }
}
