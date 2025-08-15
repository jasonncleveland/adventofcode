use std::time::Instant;

use crate::shared::intcode::initialize_computer;

pub fn solve(file_contents: String) -> (String, String) {
    let parse_timer = Instant::now();
    let input = parse_input(file_contents);
    println!("File parse: ({:?})", parse_timer.elapsed());

    let part1_timer = Instant::now();
    let part1 = solve_part_1(&input);
    println!("Part 1: {} ({:?})", part1, part1_timer.elapsed());

    let part2_timer = Instant::now();
    let part2 = solve_part_2(&input);
    println!("Part 2: {} ({:?})", part2, part2_timer.elapsed());

    (part1.to_string(), part2.to_string())
}

pub fn parse_input(file_contents: String) -> Vec<usize> {
    let mut result: Vec<usize> = Vec::new();
    for line in file_contents.split(',') {
        if let Ok(value) = line.parse::<usize>() {
            result.push(value);
        }
    }
    result
}

fn solve_part_1(input: &[usize]) -> usize {
    let mut computer = initialize_computer(input);
    computer.memory[1] = 12;
    computer.memory[2] = 2;
    computer.run();
    computer.memory[0]
}

fn solve_part_2(input: &[usize]) -> usize {
    for noun in 0..100 {
        for verb in 0..100 {
            let mut computer = initialize_computer(input);
            computer.memory[1] = noun;
            computer.memory[2] = verb;
            computer.run();
            if computer.memory[0] == 19690720 {
                println!("Found noun: {} verb: {}", noun, verb);
                return 100 * noun + verb
            }
        }
    }
    panic!("Could not find a noun and verb")
}
