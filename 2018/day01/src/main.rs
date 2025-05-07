use std::collections::HashSet;
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

fn parse_input(file_contents: &str) -> Vec<i64> {
    let mut numbers: Vec<i64> = Vec::new();
    for line in file_contents.lines() {
        numbers.push(line.parse::<i64>().unwrap());
    }
    numbers
}

fn part1(file_contents: &str) -> i64 {
    let numbers = parse_input(file_contents);

    let mut frequency: i64 = 0;
    for value in numbers {
        frequency += value;
    }
    frequency
}

fn part2(file_contents: &str) -> i64 {
    let numbers = parse_input(file_contents);

    let mut occurrences: HashSet<i64> = HashSet::new();
    let mut frequency: i64 = 0;
    occurrences.insert(frequency);
    let mut i: usize = 0;
    loop {
        frequency += numbers[i % numbers.len()];
        if occurrences.contains(&frequency) {
            return frequency;
        }
        occurrences.insert(frequency);
        i += 1;
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part1() {
        let input: [&str; 4] = [
            "+1\n-2\n+3\n+1",
            "+1\n+1\n+1",
            "+1\n+1\n-2",
            "-1\n-2\n-3",
        ];
        let expected: [i64; 4] = [
            3,
            3,
            0,
            -6,
        ];

        for i in 0..input.len() {
            assert_eq!(part1(input[i]), expected[i]);
        }
    }

    #[test]
    fn test_part2() {
        let input: [&str; 5] = [
            "+1\n-2\n+3\n+1",
            "+1\n-1",
            "+3\n+3\n+4\n-2\n-4",
            "-6\n+3\n+8\n+5\n-6",
            "+7\n+7\n-2\n-7\n-4",
        ];
        let expected: [i64; 5] = [
            2,
            0,
            10,
            5,
            14,
        ];

        for i in 0..input.len() {
            assert_eq!(part2(input[i]), expected[i]);
        }
    }
}
