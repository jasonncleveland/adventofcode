use std::env;
use std::fs;
use std::time;

fn main() {
    let args: Vec<String> = env::args().collect();

    let start = time::Instant::now();
    let line = fs::read_to_string(&args[1])
        .expect("Should have been able to read the file");
    println!("File read: ({:?})", start.elapsed());

    let part1_timer = time::Instant::now();
    let part1 = solve_part1(&line);
    println!("Part 1: {} ({:?})", part1, part1_timer.elapsed());

    let part2_timer = time::Instant::now();
    let part2 = solve_part2(&line);
    println!("Part 2: {} ({:?})", part2, part2_timer.elapsed());
}

fn solve_part1(line: &str) -> i64 {
    let (numbers, min, max) = parse_input(&line);

    let mut min_fuel = i64::MAX;
    for position in min..max {
        let mut total = 0;
        for number in numbers.iter() {
            let fuel = (number - position).abs();
            total += fuel;
        }
        if total < min_fuel {
            min_fuel = total;
        }
    }

    return min_fuel;
}

fn solve_part2(line: &str) -> i64 {
    let (numbers, min, max) = parse_input(&line);

    let mut min_fuel = i64::MAX;
    for position in min..max {
        let mut total = 0;
        for number in numbers.iter() {
            let mut fuel = (number - position).abs();
            fuel = fuel * (fuel + 1) / 2;
            total += fuel;
        }
        if total < min_fuel {
            min_fuel = total;
        }
    }

    return min_fuel;
}

fn parse_input(line: &str) -> (Vec<i64>, i64, i64) {
    let split: Vec<&str> = line.split(',').collect();

    let mut max = i64::MIN;
    let mut min = i64::MAX;
    let mut numbers: Vec<i64> = vec![];

    for item in split.iter() {
        let number: i64 = item.parse().expect("Not a valid number");
        if number > max {
            max = number;
        }
        if number < min {
            min = number;
        }
        numbers.push(number);
    }

    return (numbers, min, max);
}
