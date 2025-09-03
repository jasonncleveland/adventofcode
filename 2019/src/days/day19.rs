use std::collections::HashSet;
use std::time::Instant;

use log::debug;

use crate::shared::intcode::IntCodeComputer;
use crate::shared::io::parse_int_list;

pub fn solve(file_contents: String) -> (String, String) {
    let parse_timer = Instant::now();
    let input = parse_int_list(file_contents);
    debug!("File parse: ({:?})", parse_timer.elapsed());

    let part1_timer = Instant::now();
    let part1 = solve_part_1(&input);
    debug!("Part 1: {} ({:?})", part1, part1_timer.elapsed());

    let part2_timer = Instant::now();
    let part2 = solve_part_2(&input);
    debug!("Part 2: {} ({:?})", part2, part2_timer.elapsed());

    (part1.to_string(), part2.to_string())
}

fn solve_part_1(input: &[i64]) -> i64 {
    let computer = IntCodeComputer::new(input);

    let mut total = 0;
    for x in 0..50 {
        for y in 0..50 {
            if check_coordinate(&mut computer.clone(), x, y) {
                total += 1;
            }
        }
    }
    total
}

fn solve_part_2(input: &[i64]) -> i64 {
    let computer = IntCodeComputer::new(input);

    let mut start_x = i64::MAX;
    let mut end_x = i64::MIN;
    let mut y = 10;

    // Calculate the x range for the starting row
    // Start with y=10 since it is low enough to have few valid squares close to x=0
    for x in 0..10 {
        let mut computer_clone = computer.clone();
        if check_coordinate(&mut computer_clone, x, 10) {
            if x < start_x {
                start_x = x;
            }
            if x > end_x {
                end_x = x;
            }
        }
    }

    // Loop until we find a valid 100x100 location
    loop {
        y += 1;
        // let mut row_total = 0;
        let mut min_x = i64::MAX;
        let mut max_x = i64::MIN;

        // Calculate the width of the current row
        // The width varies by 1 at most so we only need to check around the start and end coordinates
        let relevant_coordinates: HashSet<i64> = HashSet::from_iter(vec![start_x, start_x + 1, end_x - 1, end_x, end_x + 1, end_x + 2]);
        for x in relevant_coordinates {
            if x < 0 {
                continue;
            }

            let mut computer_clone = computer.clone();
            if check_coordinate(&mut computer_clone, x, y) {
                if x < min_x {
                    min_x = x;
                }
                if x > max_x {
                    max_x = x;
                }
            }
        }

        let row_total = max_x - min_x + 1;
        if row_total >= 100 {
            // The valid square will always have the top right corner at the end of the row
            // We only need to check the bottom left corner to determine if the square is valid
            if check_coordinate(&mut computer.clone(), max_x - 99, y + 99) {
                return (max_x - 99) * 10_000 + y;
            }
        }
        start_x = min_x;
        end_x = max_x;
    }
}

fn check_coordinate(computer: &mut IntCodeComputer, x: i64, y: i64) -> bool {
    computer.input.push_back(x);
    computer.input.push_back(y);

    computer.run();

    if let Some(result) = computer.output.pop_back() {
        return result == 1;
    }
    false
}
