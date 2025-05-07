use std::collections::HashMap;
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

fn part1(file_contents: &str) -> i64 {
    let mut twos: i64 = 0;
    let mut threes: i64 = 0;
    for line in file_contents.lines() {
        let mut occurrences: HashMap<char, i64> = HashMap::new();
        for letter in line.chars() {
            occurrences.entry(letter).and_modify(|e| *e += 1).or_insert(1);
        }

        let mut found_two: bool = false;
        let mut found_three: bool = false;
        for (_, count) in occurrences {
            if !found_two && count == 2 {
                found_two = true;
                twos += 1;
            }
            if !found_three && count == 3 {
                found_three = true;
                threes += 1;
            }
        }
    }
    twos * threes
}

fn part2(file_contents: &str) -> i64 {
    -1
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part1() {
        let input: [&str; 1] = [
            "abcdef\nbababc\nabbcde\nabcccd\naabcdd\nabcdee\nababab",
        ];
        let expected: [i64; 1] = [
            12,
        ];

        for i in 0..input.len() {
            assert_eq!(part1(input[i]), expected[i]);
        }
    }

    #[test]
    fn test_part2() {
        let input: [&str; 1] = [
            "",
        ];
        let expected: [i64; 1] = [
            0,
        ];

        for i in 0..input.len() {
            assert_eq!(part2(input[i]), expected[i]);
        }
    }
}
