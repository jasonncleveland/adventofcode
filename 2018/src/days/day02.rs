use std::collections::HashMap;
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

fn parse_input(file_contents: String) -> Vec<String> {
    file_contents
        .lines()
        .map(|l| l.to_string())
        .collect()
}

fn solve_part_1(lines: &[String]) -> i64 {
    let mut twos: i64 = 0;
    let mut threes: i64 = 0;
    for line in lines {
        let mut occurrences: HashMap<char, i64> = HashMap::new();
        for letter in line.chars() {
            occurrences.entry(letter).and_modify(|e| *e += 1).or_insert(1);
        }

        let mut found_two: bool = false;
        let mut found_three: bool = false;
        for (_, count) in occurrences {
            if !found_two && count == 2 {
                found_two = true;
                twos += 1;
            }
            if !found_three && count == 3 {
                found_three = true;
                threes += 1;
            }
        }
    }
    twos * threes
}

fn solve_part_2(lines: &[String]) -> String {
    for i in 0..lines.len() {
        for j in 0..lines.len() {
            if i == j {
                continue;
            }
            let first: Vec<char> = lines[i].chars().collect();
            let second: Vec<char> = lines[j].chars().collect();
            let mut same: Vec<char> = Vec::new();
            let mut diff = 0;
            for k in 0..first.len() {
                if first[k] != second[k] {
                    diff += 1;
                } else {
                    same.push(first[k]);
                }
            }
            if diff == 1 {
                return same.iter().collect::<String>();
            }
        }
    }
    String::new()
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let input: [&str; 1] = [
            "abcdef\nbababc\nabbcde\nabcccd\naabcdd\nabcdee\nababab",
        ];
        let expected: [i64; 1] = [
            12,
        ];

        for i in 0..input.len() {
            let input = parse_input(input[i].to_string());
            assert_eq!(solve_part_1(&input), expected[i]);
        }
    }

    #[test]
    fn test_part_2() {
        let input: [&str; 1] = [
            "abcde\nfghij\nklmno\npqrst\nfguij\naxcye\nwvxyz",
        ];
        let expected: [&str; 1] = [
            "fgij",
        ];

        for i in 0..input.len() {
            let input = parse_input(input[i].to_string());
            assert_eq!(solve_part_2(&input), expected[i]);
        }
    }
}
