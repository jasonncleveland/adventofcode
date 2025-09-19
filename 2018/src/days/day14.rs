use std::collections::VecDeque;
use std::time::Instant;

use aoc_helpers::io::parse_int;
use log::debug;

pub fn solve(file_contents: String) -> (String, String) {
    let parse_timer = Instant::now();
    let input = parse_int(file_contents.clone());
    debug!("File parse: ({:?})", parse_timer.elapsed());

    let part1_timer = Instant::now();
    let part1 = solve_part_1(input);
    debug!("Part 1: {} ({:?})", part1, part1_timer.elapsed());

    let part2_timer = Instant::now();
    let part2 = solve_part_2(file_contents.clone());
    debug!("Part 2: {} ({:?})", part2, part2_timer.elapsed());

    (part1.to_string(), part2.to_string())
}

fn solve_part_1(input: i64) -> String {
    let end = input as usize;

    let mut recipes: Vec<usize> = vec![3, 7];
    let mut special_recipes: Vec<usize> = vec![];

    let mut first_elf_index = 0;
    let mut second_elf_index = 1;

    loop {
        let new_recipe = recipes[first_elf_index] + recipes[second_elf_index];
        if new_recipe > 9 {
            recipes.push(new_recipe / 10);
            if recipes.len() > end {
                special_recipes.push(new_recipe / 10);
            }
            if recipes.len() == end + 10 {
                break;
            }
        }
        recipes.push(new_recipe % 10);
        if recipes.len() > end {
            special_recipes.push(new_recipe % 10);
        }
        if recipes.len() == end + 10 {
            break;
        }
        first_elf_index = (first_elf_index + 1 + recipes[first_elf_index]) % recipes.len();
        second_elf_index = (second_elf_index + 1 + recipes[second_elf_index]) % recipes.len();
    }

    let mut output = String::with_capacity(10);
    for recipe in special_recipes {
        output.push_str(recipe.to_string().as_str());
    }
    output
}

fn solve_part_2(input: String) -> usize {
    let expected = input
        .chars()
        .map(|c| c.to_digit(10).unwrap() as usize)
        .collect::<Vec<usize>>();

    let mut recipes: Vec<usize> = vec![3, 7];
    let mut stack: VecDeque<usize> = VecDeque::from(expected.clone());

    let mut first_elf_index = 0;
    let mut second_elf_index = 1;

    let mut is_checking = false;
    let mut found_index = 0;
    loop {
        let new_recipe = recipes[first_elf_index] + recipes[second_elf_index];
        if new_recipe > 9 {
            recipes.push(new_recipe / 10);
            if is_checking && new_recipe / 10 != *stack.front().unwrap() {
                is_checking = false;
                stack = VecDeque::from(expected.clone());
            }
            if new_recipe / 10 == *stack.front().unwrap() {
                is_checking = true;
                stack.pop_front();
                found_index = recipes.len();
            }
            if stack.is_empty() {
                break;
            }
        }
        recipes.push(new_recipe % 10);
        if is_checking && new_recipe % 10 != *stack.front().unwrap() {
            is_checking = false;
            stack = VecDeque::from(expected.clone());
        }
        if new_recipe % 10 == *stack.front().unwrap() {
            is_checking = true;
            stack.pop_front();
            found_index = recipes.len();
        }
        if stack.is_empty() {
            break;
        }
        first_elf_index = (first_elf_index + 1 + recipes[first_elf_index]) % recipes.len();
        second_elf_index = (second_elf_index + 1 + recipes[second_elf_index]) % recipes.len();
    }

    found_index - expected.len()
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let input: [&str; 4] = ["9", "5", "18", "2018"];
        let expected: [&str; 4] = ["5158916779", "0124515891", "9251071085", "5941429882"];

        for i in 0..input.len() {
            let input = parse_int(input[i].to_string());
            assert_eq!(solve_part_1(input), expected[i]);
        }
    }

    #[test]
    fn test_part_2() {
        let input: [&str; 4] = ["51589", "01245", "92510", "59414"];
        let expected: [usize; 4] = [9, 5, 18, 2018];

        for i in 0..input.len() {
            assert_eq!(solve_part_2(input[i].to_string()), expected[i]);
        }
    }
}
