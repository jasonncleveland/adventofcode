use std::time::Instant;

use log::debug;

pub fn solve(file_contents: &str) -> (String, String) {
    let parse_timer = Instant::now();
    let input = parse_input(file_contents);
    debug!("File parse: ({:?})", parse_timer.elapsed());

    let part1_timer = Instant::now();
    let part1 = solve_part_1(&input);
    debug!("Part 1: {} ({:?})", part1, part1_timer.elapsed());

    let part2_timer = Instant::now();
    let part2 = solve_part_2(file_contents);
    debug!("Part 2: {} ({:?})", part2, part2_timer.elapsed());

    (part1.to_string(), part2.to_string())
}

fn parse_input(file_contents: &str) -> (Vec<Vec<i64>>, Vec<char>) {
    let mut turns: Vec<Vec<i64>> = Vec::new();
    let mut operations: Vec<char> = Vec::new();
    let mut lines: Vec<&str> = file_contents.lines().collect();
    if let Some(os) = lines.pop() {
        for operation in os.split_whitespace() {
            if let Some(o) = operation.chars().next() {
                operations.push(o);
            }
        }
    }
    while let Some(line) = lines.pop() {
        let mut numbers = Vec::<i64>::new();
        for number in line.split_whitespace() {
            if let Ok(n) = number.parse::<i64>() {
                numbers.push(n);
            }
        }
        turns.push(numbers);
    }
    (turns, operations)
}

fn solve_part_1(input: &(Vec<Vec<i64>>, Vec<char>)) -> i64 {
    let mut total = 0;

    for (j, operation) in input.1.iter().enumerate() {
        let mut sum = match operation {
            '+' => 0,
            '*' => 1,
            _ => unreachable!(),
        };
        for i in 0..input.0.len() {
            if let Some(ns) = input.0.get(i)
                && let Some(v) = ns.get(j)
            {
                match operation {
                    '+' => sum += v,
                    '*' => sum *= v,
                    _ => unreachable!(),
                }
            }
        }
        total += sum;
    }
    total
}
fn solve_part_2(file_contents: &str) -> i64 {
    let mut total = 0;

    let mut lines_chars: Vec<Vec<char>> = Vec::new();
    for line in file_contents.lines() {
        lines_chars.push(line.chars().collect());
    }

    let mut things: Vec<Vec<String>> = Vec::new();
    if let Some(fl) = lines_chars.first() {
        let length = fl.len();

        let mut start_index = 0;
        for current_index in 0..length {
            let mut all_blank = true;
            for i in 0..lines_chars.len() {
                if let Some(l) = lines_chars.get(i)
                    && let Some(&c) = l.get(current_index)
                    && c != ' '
                {
                    all_blank = false;
                    break;
                }
            }
            if all_blank {
                let mut found: Vec<String> = Vec::new();
                for i in 0..lines_chars.len() {
                    let mut str = String::new();
                    if let Some(l) = lines_chars.get(i) {
                        for i in start_index..current_index {
                            if let Some(&c) = l.get(i) {
                                str.push(c);
                            }
                        }
                    }
                    found.push(str);
                }
                things.push(found);
                start_index = current_index + 1;
            }
            if current_index == length - 1 {
                let mut found: Vec<String> = Vec::new();
                for i in 0..lines_chars.len() {
                    let mut str = String::new();
                    if let Some(l) = lines_chars.get(i) {
                        for i in start_index..=current_index {
                            if let Some(&c) = l.get(i) {
                                str.push(c);
                            }
                        }
                    }
                    found.push(str);
                }
                things.push(found);
                start_index = current_index + 1;
            }
        }
    }

    for mut thing in things {
        if let Some(operation) = thing.pop() {
            let mut found_numbers: Vec<i64> = Vec::new();
            let op_length = operation.len();
            for i in 0..op_length {
                let mut f = String::new();
                for number in &thing {
                    if let Some(last) = number.chars().nth(i) {
                        f.push(last);
                    }
                }
                if let Ok(v) = f.trim().parse::<i64>() {
                    found_numbers.push(v);
                }
            }
            match operation.trim() {
                "+" => {
                    total += found_numbers.iter().sum::<i64>();
                }
                "*" => {
                    total += found_numbers.iter().product::<i64>();
                }
                _ => unreachable!(),
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
            "123 328  51 64
 45 64  387 23
  6 98  215 314
*   +   *   +  ",
            4_277_556,
        )];

        for (input, expected) in data {
            let input = parse_input(input);
            assert_eq!(solve_part_1(&input), expected);
        }
    }

    #[test]
    fn test_part_2() {
        let data: [(&str, i64); 1] = [(
            "123 328  51 64 \n 45 64  387 23 \n  6 98  215 314\n*   +   *   +  ",
            3_263_827,
        )];

        for (input, expected) in data {
            assert_eq!(solve_part_2(input), expected);
        }
    }
}
