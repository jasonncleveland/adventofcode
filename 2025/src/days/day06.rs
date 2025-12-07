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
    let part2 = solve_part_2(&input);
    debug!("Part 2: {} ({:?})", part2, part2_timer.elapsed());

    (part1.to_string(), part2.to_string())
}

fn parse_input(file_contents: &str) -> Vec<ArithmeticBlock> {
    let mut blocks: Vec<ArithmeticBlock> = Vec::new();
    let mut index = 0;

    let mut lines_chars: Vec<Vec<char>> = file_contents
        .lines()
        .map(|line| line.chars().collect())
        .collect();

    let mut block: Vec<Vec<char>> = vec![Vec::new(); lines_chars.len()];
    let mut digit_count = 0;
    loop {
        // Check if the current column is empty
        let mut empty_column = true;
        for line_index in 0..lines_chars.len() {
            if let Some(line) = lines_chars.get_mut(line_index)
                && let Some(&c) = line.get(index)
                && c != ' '
            {
                empty_column = false;
                break;
            }
        }

        let mut found_characters = false;
        for (line_index, line) in lines_chars.iter_mut().enumerate() {
            if let Some(&c) = line.get(index) {
                found_characters = true;
                if !empty_column {
                    block[line_index].push(c);
                }
            }
        }
        if !empty_column {
            digit_count += 1;
        }

        // Stop parsing a block when we find an empty column
        if empty_column {
            if let Some(possible_operations) = block.pop()
                && let Some(&operation) = possible_operations.iter().find(|&&c| c != ' ')
            {
                blocks.push(ArithmeticBlock {
                    digits: block.clone(),
                    digit_count,
                    operation,
                });
            }
            block = vec![Vec::new(); lines_chars.len()];
            digit_count = 0;
        }

        // Stop when there are no more characters
        if !found_characters {
            break;
        }

        index += 1;
    }

    blocks
}

fn solve_part_1(input: &Vec<ArithmeticBlock>) -> i64 {
    let mut total = 0;

    for block in input {
        // Read the digits horizontally and convert to numbers
        let result = block
            .digits
            .iter()
            .map(|l| l.iter().filter(|c| **c != ' ').collect::<String>())
            .map(|l| l.parse::<i64>())
            .collect::<Result<Vec<_>, _>>();

        if let Ok(numbers) = result {
            // Add the sum or product of the found numbers to the total
            total += match block.operation {
                '+' => numbers.iter().sum::<i64>(),
                '*' => numbers.iter().product::<i64>(),
                _ => unreachable!(),
            }
        }
    }

    total
}
fn solve_part_2(input: &Vec<ArithmeticBlock>) -> i64 {
    let mut total = 0;

    for block in input {
        let mut numbers: Vec<i64> = Vec::new();
        for digit_index in 0..block.digit_count {
            let mut number = String::new();
            for line_index in 0..block.digits.len() {
                if let Some(line) = block.digits.get(line_index)
                    && let Some(&digit) = line.get(digit_index)
                    && digit != ' '
                {
                    number.push(digit);
                }
            }
            if let Ok(value) = number.parse::<i64>() {
                numbers.push(value);
            }
        }

        // Add the sum or product of the found numbers to the total
        total += match block.operation {
            '+' => numbers.iter().sum::<i64>(),
            '*' => numbers.iter().product::<i64>(),
            _ => unreachable!(),
        }
    }

    total
}

#[derive(Debug)]
struct ArithmeticBlock {
    digits: Vec<Vec<char>>,
    digit_count: usize,
    operation: char,
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
            let input = parse_input(input);
            assert_eq!(solve_part_2(&input), expected);
        }
    }
}
