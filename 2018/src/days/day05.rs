use std::collections::VecDeque;
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

fn parse_input(file_contents: String) -> VecDeque<char> {
    file_contents.chars().collect()
}

fn solve_part_1(units: &VecDeque<char>) -> i64 {
    let mut units_copy = units.to_owned();
    react_polymer(&mut units_copy)
}

fn solve_part_2(units: &VecDeque<char>) -> i64 {
    let mut min = i64::MAX;
    for upper in 'A'..='Z' {
        let lower = ((upper as u8) + 32) as char;
        let mut units_copy = units.to_owned();
        let original_length = units_copy.len();
        units_copy.retain(|c| *c != lower && *c != upper);
        if units_copy.len() != original_length {
            let result = react_polymer(&mut units_copy);
            if result < min {
                min = result;
            }
        }
    }
    min
}

fn react_polymer(units: &mut VecDeque<char>) -> i64 {
    let mut index = 1;
    while index < units.len() {
        if index < 1 {
            index = 1;
        }
        let lchar = units[index - 1] as u8;
        let rchar = units[index] as u8;
        if lchar.abs_diff(rchar) == 32 {
            units.remove(index);
            units.remove(index - 1);
            index -= 1;
        } else {
            index += 1;
        }
    }
    units.len() as i64
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let input: [&str; 5] = ["aA", "abBA", "abAB", "aabAAB", "dabAcCaCBAcCcaDA"];
        let expected: [i64; 5] = [0, 0, 4, 6, 10];

        for i in 0..input.len() {
            let input = parse_input(input[i].to_string());
            assert_eq!(solve_part_1(&input), expected[i]);
        }
    }

    #[test]
    fn test_part_2() {
        let input: [&str; 1] = ["dabAcCaCBAcCcaDA"];
        let expected: [i64; 1] = [4];

        for i in 0..input.len() {
            let input = parse_input(input[i].to_string());
            assert_eq!(solve_part_2(&input), expected[i]);
        }
    }
}
