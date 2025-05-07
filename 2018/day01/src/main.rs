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
}

fn read_file(file_name: &str) -> String {
    fs::read_to_string(file_name)
        .expect("Something went wrong reading the file")
}

fn part1(file_contents: &str) -> i64 {
    let mut frequency: i64 = 0;
    for line in file_contents.lines() {
        let value: i64 = line.parse().unwrap();
        frequency += value;
    }
    frequency
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
}
