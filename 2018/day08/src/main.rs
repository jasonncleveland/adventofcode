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

fn parse_input(file_contents: &str) -> Vec<usize> {
    let mut numbers: Vec<usize> = Vec::new();
    for number in file_contents.split(' ') {
        numbers.push(number.parse::<usize>().unwrap());
    }
    numbers
}

fn part1(file_contents: &str) -> usize {
    let numbers = parse_input(file_contents);

    let (_, total) = calculate_metadata_sum_rec(&numbers, 0);
    total
}

fn part2(file_contents: &str) -> usize {
    let numbers = parse_input(file_contents);

    let (_, total) = calculate_root_value_rec(&numbers, 0);
    total
}

fn calculate_metadata_sum_rec(numbers: &Vec<usize>, start_index: usize) -> (usize, usize) {
    let mut index: usize = start_index;
    let child_node_count = numbers[index];
    index += 1;
    let metadata_entries_count = numbers[index];
    index += 1;
    let mut total = 0;
    for _ in 0..child_node_count {
        let result = calculate_metadata_sum_rec(numbers, index);
        index = result.0;
        total += result.1;
    }
    for _ in 0..metadata_entries_count {
        total += numbers[index];
        index += 1;
    }
    (index, total)
}

fn calculate_root_value_rec(numbers: &Vec<usize>, start_index: usize) -> (usize, usize) {
    let mut index: usize = start_index;
    let child_node_count = numbers[index];
    index += 1;
    let metadata_entries_count = numbers[index];
    index += 1;

    let mut child_node_values: Vec<usize> = Vec::with_capacity(child_node_count);
    for _ in 0..child_node_count {
        let result = calculate_root_value_rec(numbers, index);
        index = result.0;
        child_node_values.push(result.1);
    }

    let mut metadata_entries: Vec<usize> = Vec::with_capacity(metadata_entries_count);
    for _ in 0..metadata_entries_count {
        metadata_entries.push(numbers[index]);
        index += 1;
    }

    let mut total = 0;
    if child_node_count == 0 {
        total = metadata_entries.iter().sum();
    } else {
        for entry in metadata_entries {
            let offset_index = entry - 1;
            if offset_index < child_node_count {
                total += child_node_values[entry - 1];
            }
        }
    }
    (index, total)
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part1() {
        let input: [&str; 1] = [
            "2 3 0 3 10 11 12 1 1 0 1 99 2 1 1 2",
        ];
        let expected: [usize; 1] = [
            138,
        ];

        for i in 0..input.len() {
            assert_eq!(part1(input[i]), expected[i]);
        }
    }

    #[test]
    fn test_part2() {
        let input: [&str; 1] = [
            "2 3 0 3 10 11 12 1 1 0 1 99 2 1 1 2",
        ];
        let expected: [usize; 1] = [
            66,
        ];

        for i in 0..input.len() {
            assert_eq!(part2(input[i]), expected[i]);
        }
    }
}
