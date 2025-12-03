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

pub fn parse_input(file_contents: &str) -> Vec<i64> {
    let mut result: Vec<i64> = Vec::new();
    for line in file_contents.lines() {
        if let Ok(value) = line.parse::<i64>() {
            result.push(value);
        }
    }
    result
}

fn solve_part_1(input: &Vec<i64>) -> i64 {
    let mut total = 0;
    for mass in input {
        total += calculate_fuel(*mass);
    }
    total
}

fn solve_part_2(input: &Vec<i64>) -> i64 {
    let mut total = 0;
    for mass in input {
        total += calculate_fuel_residual(*mass);
    }
    total
}

fn calculate_fuel(mass: i64) -> i64 {
    mass / 3 - 2
}

fn calculate_fuel_residual(mass: i64) -> i64 {
    let mut remaining_mass = mass;
    let mut total = 0;
    loop {
        let fuel = calculate_fuel(remaining_mass);
        if fuel <= 0 {
            break;
        }
        remaining_mass = fuel;
        total += fuel;
    }
    total
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_calculate_fuel() {
        let input: [i64; 4] = [12, 14, 1969, 100756];
        let expected: [i64; 4] = [2, 2, 654, 33583];

        for i in 0..input.len() {
            assert_eq!(calculate_fuel(input[i]), expected[i]);
        }
    }

    #[test]
    fn test_calculate_fuel_rec() {
        let input: [i64; 4] = [12, 14, 1969, 100756];
        let expected: [i64; 4] = [2, 2, 966, 50346];

        for i in 0..input.len() {
            assert_eq!(calculate_fuel_residual(input[i]), expected[i]);
        }
    }
}
