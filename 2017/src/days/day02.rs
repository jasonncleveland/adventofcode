use std::time::Instant;

use log::debug;

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

fn parse_input(file_contents: String) -> Vec<Vec<i64>> {
    let mut grid: Vec<Vec<i64>> = Vec::new();
    for line in file_contents.lines() {
        let mut row: Vec<i64> = Vec::new();
        for number in line.split_whitespace() {
            if let Ok(value) = number.parse::<i64>() {
                row.push(value);
            }
        }
        grid.push(row);
    }
    grid
}

fn solve_part_1(grid: &Vec<Vec<i64>>) -> i64 {
    let mut total = 0;
    for row in grid {
        let mut min = i64::MAX;
        let mut max = i64::MIN;
        for &number in row {
            if number < min {
                min = number;
            }
            if number > max {
                max = number;
            }
        }
        total += max - min
    }
    total
}

fn solve_part_2(grid: &Vec<Vec<i64>>) -> i64 {
    let mut total = 0;
    for row in grid {
        'outer: for &first in row {
            for &second in row {
                if first == second {
                    continue;
                }

                if first % second == 0 {
                    total += first / second;
                    break 'outer;
                }
            }
        }
    }
    total
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let input: [&str; 1] = ["5 1 9 5
7 5 3
2 4 6 8"];
        let expected: [i64; 1] = [18];

        for i in 0..input.len() {
            let input = parse_input(input[i].to_string());
            assert_eq!(solve_part_1(&input), expected[i]);
        }
    }

    #[test]
    fn test_part_2() {
        let input: [&str; 1] = ["5 9 2 8
9 4 7 3
3 8 6 5"];
        let expected: [i64; 1] = [9];

        for i in 0..input.len() {
            let input = parse_input(input[i].to_string());
            assert_eq!(solve_part_2(&input), expected[i]);
        }
    }
}
