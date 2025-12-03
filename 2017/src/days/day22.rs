use std::collections::HashMap;
use std::time::Instant;

use aoc_helpers::direction::Direction;
use aoc_helpers::grid::get_dimensions;
use aoc_helpers::io::parse_char_grid;
use aoc_helpers::point2d::Point2d;
use log::debug;

pub fn solve(file_contents: &str) -> (String, String) {
    let parse_timer = Instant::now();
    let input = parse_char_grid(file_contents);
    debug!("File parse: ({:?})", parse_timer.elapsed());

    let part1_timer = Instant::now();
    let part1 = solve_part_1(&input);
    debug!("Part 1: {} ({:?})", part1, part1_timer.elapsed());

    let part2_timer = Instant::now();
    let part2 = solve_part_2(&input, 10_000_000);
    debug!("Part 2: {} ({:?})", part2, part2_timer.elapsed());

    (part1.to_string(), part2.to_string())
}

fn solve_part_1(grid: &HashMap<Point2d, char>) -> i64 {
    let (width, height) = get_dimensions(grid);

    let mut infections = 0;
    let mut network = grid.clone();
    let mut current_direction = Direction::Up;
    let mut current_position = Point2d::new(width / 2, height / 2);
    for _ in 0..10_000 {
        match network.get(&current_position) {
            Some('.') | None => {
                infections += 1;
                network.insert(current_position, '#');
                current_direction = current_direction.next(&Direction::Left);
            }
            Some('#') => {
                network.insert(current_position, '.');
                current_direction = current_direction.next(&Direction::Right);
            }
            _ => unreachable!(),
        };
        current_position = current_position.next(&current_direction);
    }
    infections
}

fn solve_part_2(grid: &HashMap<Point2d, char>, iterations: usize) -> i64 {
    let (width, height) = get_dimensions(grid);

    let mut infections = 0;
    let mut network = grid.clone();
    let mut current_direction = Direction::Up;
    let mut current_position = Point2d::new(width / 2, height / 2);
    for _ in 0..iterations {
        match network.get(&current_position) {
            Some('.') | None => {
                network.insert(current_position, 'W');
                current_direction = current_direction.next(&Direction::Left);
            }
            Some('W') => {
                infections += 1;
                network.insert(current_position, '#');
            }
            Some('#') => {
                network.insert(current_position, 'F');
                current_direction = current_direction.next(&Direction::Right);
            }
            Some('F') => {
                network.insert(current_position, '.');
                current_direction = current_direction.opposite();
            }
            _ => unreachable!(),
        };
        current_position = current_position.next(&current_direction);
    }
    infections
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let data: [(&str, i64); 1] = [(
            "..#
#..
...",
            5587,
        )];

        for (input, expected) in data {
            let input = parse_char_grid(input);
            assert_eq!(solve_part_1(&input), expected);
        }
    }

    #[test]
    fn test_part_2() {
        let data: [(&str, i64); 1] = [(
            "..#
#..
...",
            26,
        )];

        for (input, expected) in data {
            let input = parse_char_grid(input);
            assert_eq!(solve_part_2(&input, 100), expected);
        }
    }
}
