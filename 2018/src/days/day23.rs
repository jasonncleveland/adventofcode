use std::time::Instant;

use aoc_helpers::point3d::Point3d;
use log::{debug, trace};

pub fn solve(file_contents: String) -> (String, String) {
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

fn parse_input(file_contents: String) -> Vec<NanoBot> {
    let mut bots: Vec<NanoBot> = Vec::new();
    for line in file_contents.lines() {
        let mut numbers: Vec<i64> = Vec::with_capacity(4);
        for item in line.split(',') {
            let cleaned = item.chars().filter(|&c| c == '-' || c.is_ascii_digit()).collect::<String>();
            if let Ok(number) = cleaned.parse::<i64>() {
                numbers.push(number);
            }
        }
        bots.push(NanoBot {
            position: Point3d::new(numbers[0], numbers[1], numbers[2]),
            radius: numbers[3],
        });
    }
    bots
}

fn solve_part_1(input: &[NanoBot]) -> i64 {
    let mut max_radius = i64::MIN;
    let mut max_bots = i64::MIN;
    for bot in input {
        if bot.radius > max_radius {
            max_radius = bot.radius;
            max_bots = count_nano_bots_in_range(input, bot);
        }
    }
    max_bots
}

fn solve_part_2(input: &[NanoBot]) -> i64 {
    -1
}

fn count_nano_bots_in_range(bots: &[NanoBot], origin: &NanoBot) -> i64 {
    trace!("counting nanobots in range of {} ({})", origin.position, origin.radius);
    let mut total = 0;
    for target in bots {
        let manhattan_distance = origin.position.manhattan(&target.position);
        if manhattan_distance <= origin.radius {
            total += 1;
        }
        trace!("The nanobot at {} is distance {} away, and so is {}in range", target.position, manhattan_distance, match manhattan_distance <= origin.radius { true => "", false => "not "});
    }
    trace!("{} in range", total);
    total
}

#[derive(Debug)]
struct NanoBot {
    position: Point3d,
    radius: i64,
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let input: [&str; 1] = [
            "pos=<0,0,0>, r=4
pos=<1,0,0>, r=1
pos=<4,0,0>, r=3
pos=<0,2,0>, r=1
pos=<0,5,0>, r=3
pos=<0,0,3>, r=1
pos=<1,1,1>, r=1
pos=<1,1,2>, r=1
pos=<1,3,1>, r=1",
        ];
        let expected: [i64; 1] = [
            7,
        ];

        for i in 0..input.len() {
            let input = parse_input(input[i].to_string());
            assert_eq!(solve_part_1(&input), expected[i]);
        }
    }

    #[test]
    fn test_part_2() {
        let input: [&str; 1] = [
            "",
        ];
        let expected: [i64; 1] = [
            0,
        ];

        for i in 0..input.len() {
            let input = parse_input(input[i].to_string());
            assert_eq!(solve_part_2(&input), expected[i]);
        }
    }
}
