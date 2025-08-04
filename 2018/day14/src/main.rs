use std::collections::VecDeque;
use std::env;
use std::fs;
use std::time;

fn main() {
    let args: Vec<String> = env::args().collect();
    if args.len() < 2 {
        panic!("Must pass filename as argument");
    }

    let input_timer = time::Instant::now();
    let file_name = &args[1];
    let file_contents = read_file(file_name);
    println!("File read: ({:?})", input_timer.elapsed());

    let part1_timer = time::Instant::now();
    let part1 = part1(&file_contents);
    println!("Part 1: {} ({:?})", part1, part1_timer.elapsed());

    let part2_timer = time::Instant::now();
    let part2 = part2(&file_contents);
    println!("Part 2: {} ({:?})", part2, part2_timer.elapsed());
}

fn read_file(file_name: &str) -> String {
    fs::read_to_string(file_name)
        .expect("Something went wrong reading the file")
}

fn part1(file_contents: &str) -> String {
    let end = file_contents.parse::<usize>().unwrap();

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

fn part2(file_contents: &str) -> usize {
    let expected = file_contents.chars().map(|c| c.to_digit(10).unwrap() as usize).collect::<Vec<usize>>();

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
    fn test_part1() {
        let input: [&str; 4] = [
            "9",
            "5",
            "18",
            "2018",
        ];
        let expected: [&str; 4] = [
            "5158916779",
            "0124515891",
            "9251071085",
            "5941429882",
        ];

        for i in 0..input.len() {
            assert_eq!(part1(input[i]), expected[i]);
        }
    }

    #[test]
    fn test_part2() {
        let input: [&str; 4] = [
            "51589",
            "01245",
            "92510",
            "59414",
        ];
        let expected: [usize; 4] = [
            9,
            5,
            18,
            2018,
        ];

        for i in 0..input.len() {
            assert_eq!(part2(input[i]), expected[i]);
        }
    }
}
