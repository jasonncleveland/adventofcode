use std::time::Instant;

use log::debug;

pub fn solve(file_contents: String) -> (String, String) {
    let parse_timer = Instant::now();
    let input = parse_input(file_contents);
    debug!("File parse: ({:?})", parse_timer.elapsed());

    let part1_timer = Instant::now();
    let part1 = solve_part_1(input);
    debug!("Part 1: {} ({:?})", part1, part1_timer.elapsed());

    let part2_timer = Instant::now();
    let part2 = solve_part_2(input);
    debug!("Part 2: {} ({:?})", part2, part2_timer.elapsed());

    (part1.to_string(), part2.to_string())
}

fn parse_input(file_contents: String) -> (i64, i64) {
    if let Some((line1, line2)) = file_contents.split_once('\n')
        && let Some(first) = line1.split_whitespace().last()
        && let Ok(a) = first.parse::<i64>()
        && let Some(second) = line2.split_whitespace().last()
        && let Ok(b) = second.parse::<i64>()
    {
        return (a, b);
    }
    unreachable!();
}

fn solve_part_1(input: (i64, i64)) -> i64 {
    let (mut a, mut b) = input;

    let mut total = 0;
    const MASK: i64 = 1 << 16;
    for _ in 0..40_000_000 {
        a = a * 16807 % 2147483647;
        b = b * 48271 % 2147483647;
        if a % MASK == b % MASK {
            total += 1;
        }
    }
    total
}

fn solve_part_2(input: (i64, i64)) -> i64 {
    let (mut a, mut b) = input;

    let mut total = 0;
    const MASK: i64 = 1 << 16;
    for _ in 0..5_000_000 {
        loop {
            a = a * 16807 % 2147483647;
            if a % 4 == 0 {
                break;
            }
        }
        loop {
            b = b * 48271 % 2147483647;
            if b % 8 == 0 {
                break;
            }
        }
        if a % MASK == b % MASK {
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
        let data: [(&str, i64); 1] = [(
            "Generator A starts with 65
Generator B starts with 8921",
            588,
        )];

        for (input, expected) in data {
            let input = parse_input(input.to_string());
            assert_eq!(solve_part_1(input), expected);
        }
    }

    #[test]
    fn test_part_2() {
        let data: [(&str, i64); 1] = [(
            "Generator A starts with 65
Generator B starts with 8921",
            309,
        )];

        for (input, expected) in data {
            let input = parse_input(input.to_string());
            assert_eq!(solve_part_2(input), expected);
        }
    }
}
