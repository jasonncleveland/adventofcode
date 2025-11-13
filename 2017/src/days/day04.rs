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

fn parse_input(file_contents: String) -> Vec<Vec<String>> {
    let mut passphrases: Vec<Vec<String>> = Vec::new();
    for line in file_contents.lines() {
        passphrases.push(line.split_whitespace().map(String::from).collect());
    }
    passphrases
}

fn solve_part_1(passphrases: &Vec<Vec<String>>) -> i64 {
    let mut total = 0;

    for passphrase in passphrases {
        let mut unique: HashSet<String> = HashSet::new();
        for word in passphrase {
            unique.insert(word.to_owned());
        }
        if unique.len() == passphrase.len() {
            total += 1;
        }
    }

    total
}

fn solve_part_2(passphrases: &Vec<Vec<String>>) -> i64 {
    let mut total = 0;

    for passphrase in passphrases {
        let mut unique: HashSet<String> = HashSet::new();
        for word in passphrase {
            let mut chars = word.chars().collect::<Vec<char>>();
            chars.sort();
            unique.insert(chars.iter().collect::<String>());
        }
        if unique.len() == passphrase.len() {
            total += 1;
        }
    }

    total
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let data: [(&str, i64); 3] = [
            ("aa bb cc dd ee", 1),
            ("aa bb cc dd aa", 0),
            ("aa bb cc dd aaa", 1),
        ];

        for (input, expected) in data {
            let input = parse_input(input.to_string());
            assert_eq!(solve_part_1(&input), expected);
        }
    }

    #[test]
    fn test_part_2() {
        let data: [(&str, i64); 5] = [
            ("abcde fghij", 1),
            ("abcde xyz ecdab", 0),
            ("a ab abc abd abf abj", 1),
            ("iiii oiii ooii oooi oooo", 1),
            ("oiii ioii iioi iiio", 0),
        ];

        for (input, expected) in data {
            let input = parse_input(input.to_string());
            assert_eq!(solve_part_2(&input), expected);
        }
    }
}
