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

fn parse_input(file_contents: String) -> Vec<(i64, i64)> {
    let mut turns: Vec<(i64, i64)> = Vec::new();
    for line in file_contents.split(',') {
        if let Some((left, right)) = line.split_once('-')
            && let Ok(l) = left.parse::<i64>()
            && let Ok(r) = right.parse::<i64>()
        {
            turns.push((l, r));
        }
    }
    turns
}

fn solve_part_1(input: &[(i64, i64)]) -> i64 {
    let mut total = 0;

    for &(start, end) in input {
        for i in start..=end {
            let str_repr = i.to_string();
            let chars = str_repr.chars().collect::<Vec<char>>();
            if !chars.len().is_multiple_of(2) {
                continue;
            }
            let len = chars.len() / 2;
            let mut left: Vec<char> = Vec::with_capacity(len);
            let mut right: Vec<char> = Vec::with_capacity(len);
            for i in 0..len {
                left.push(chars[i]);
                right.push(chars[len + i]);
            }
            let a = left.iter().collect::<String>();
            let b = right.iter().collect::<String>();
            if a == b {
                total += i;
            }
        }
    }

    total
}

fn solve_part_2(input: &[(i64, i64)]) -> i64 {
    let mut total = 0;

    for &(start, end) in input {
        'outer: for i in start..=end {
            let str_repr = i.to_string();
            let mut chars = str_repr.chars();
            let mut pattern: Vec<char> = Vec::new();
            let mut possible_patterns: Vec<Vec<char>> = Vec::new();
            let mut pattern_len = chars.clone().count() / 2;
            if let Some(first) = chars.next() {
                pattern.push(first);
                possible_patterns.push(pattern.clone());
                pattern_len -= 1;
                while let Some(next) = chars.next()
                    && pattern_len > 0
                {
                    pattern.push(next);
                    possible_patterns.push(pattern.clone());
                    pattern_len -= 1;
                }
            }

            'inner: for p in possible_patterns {
                let str_len = str_repr.chars().count();
                if p.len() == str_len || str_len % p.len() != 0 {
                    continue;
                }

                for (i, c) in str_repr.chars().enumerate() {
                    if c != p[i % p.len()] {
                        continue 'inner;
                    }
                }

                total += i;
                continue 'outer;
            }
        }
    }

    total
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let data: [(&str, i64); 1] = [(
            "11-22,95-115,998-1012,1188511880-1188511890,222220-222224,\
            1698522-1698528,446443-446449,38593856-38593862,565653-565659,\
            824824821-824824827,2121212118-2121212124",
            1227775554,
        )];

        for (input, expected) in data {
            let input = parse_input(input.to_string());
            assert_eq!(solve_part_1(&input), expected);
        }
    }

    #[test]
    fn test_part_2() {
        let data: [(&str, i64); 1] = [(
            "11-22,95-115,998-1012,1188511880-1188511890,222220-222224,\
            1698522-1698528,446443-446449,38593856-38593862,565653-565659,\
            824824821-824824827,2121212118-2121212124",
            4174379265,
        )];

        for (input, expected) in data {
            let input = parse_input(input.to_string());
            assert_eq!(solve_part_2(&input), expected);
        }
    }
}
