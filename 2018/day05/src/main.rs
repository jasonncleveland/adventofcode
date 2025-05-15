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

fn parse_input(file_contents: &str) -> Vec<char> {
    Vec::from_iter(file_contents.chars())
}

fn part1(file_contents: &str) -> i64 {
    let mut units: Vec<char> = parse_input(file_contents);
    let mut index = 1;
    while index < units.len() {
        if index < 1 {
            index = 1;
        }
        let lchar = units[index - 1] as u8;
        let rchar = units[index] as u8;
        if lchar.abs_diff(rchar) == 32 {
            units.remove(index);
            units.remove(index - 1);
            index = index - 1;
        } else {
            index += 1;
        }
    }
    units.len() as i64
}

fn part2(file_contents: &str) -> i64 {
    -1
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part1() {
        let input: [&str; 5] = [
            "aA",
            "abBA",
            "abAB",
            "aabAAB",
            "dabAcCaCBAcCcaDA",
        ];
        let expected: [i64; 5] = [
            0,
            0,
            4,
            6,
            10,
        ];

        for i in 0..input.len() {
            assert_eq!(part1(input[i]), expected[i]);
        }
    }

    #[test]
    fn test_part2() {
        let input: [&str; 0] = [
        ];
        let expected: [i64; 0] = [
        ];

        for i in 0..input.len() {
            assert_eq!(part2(input[i]), expected[i]);
        }
    }
}
