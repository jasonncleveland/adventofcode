use std::collections::HashMap;
use std::time::Instant;

use aoc_helpers::direction::Direction;
use aoc_helpers::io::parse_int;
use aoc_helpers::point2d::Point2d;
use log::debug;

pub fn solve(file_contents: &str) -> (String, String) {
    let parse_timer = Instant::now();
    let input = parse_int(file_contents);
    debug!("File parse: ({:?})", parse_timer.elapsed());

    let part1_timer = Instant::now();
    let part1 = solve_part_1(input);
    debug!("Part 1: {} ({:?})", part1, part1_timer.elapsed());

    let part2_timer = Instant::now();
    let part2 = solve_part_2(input);
    debug!("Part 2: {} ({:?})", part2, part2_timer.elapsed());

    (part1.to_string(), part2.to_string())
}

fn solve_part_1(number: i64) -> i64 {
    if number == 1 {
        return 0;
    }

    let mut corner = 1;
    let origin = Point2d::new(0, 0);
    let mut point = Point2d::new(0, 0);
    loop {
        let width = corner - 1;
        let squared = corner * corner;
        if number <= squared {
            let row = (squared - number) / width;
            let offset = (squared - number) % width;
            let location = match row {
                0 => Point2d::new(point.x - offset, point.y),
                1 => Point2d::new(point.x - width, point.y - offset),
                2 => Point2d::new(point.x - width + offset, point.y - width),
                3 => Point2d::new(point.x, point.y - width + offset),
                _ => unreachable!(),
            };
            return location.manhattan(&origin);
        }
        point.x += 1;
        point.y += 1;
        corner += 2;
    }
}

fn solve_part_2(number: i64) -> i64 {
    let mut current = Point2d::new(0, 0);
    let mut direction = Direction::Up;
    let mut width = 0;

    let mut squares: HashMap<Point2d, i64> = HashMap::new();
    squares.insert(current, 1);

    loop {
        current = current.next(&Direction::Right);
        if let Some(result) = find_and_store_value(&mut squares, current, number) {
            return result;
        }

        width += 2;
        // Need to move up one less than width to account for being on right square
        let mut to_move = width - 1;
        for _ in 0..4 {
            for _ in 0..to_move {
                current = current.next(&direction);
                if let Some(result) = find_and_store_value(&mut squares, current, number) {
                    return result;
                }
            }
            direction = direction.next(&Direction::Left);
            to_move = width;
        }
    }
}

fn find_and_store_value(
    squares: &mut HashMap<Point2d, i64>,
    location: Point2d,
    number: i64,
) -> Option<i64> {
    let mut total = 0;
    for neighbour in location.neighbours8() {
        if let Some(value) = squares.get(&neighbour) {
            total += value;
        }
    }
    if total > number {
        return Some(total);
    }
    squares.insert(location, total);
    None
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let input: [&str; 4] = ["1", "12", "23", "1024"];
        let expected: [i64; 4] = [0, 3, 2, 31];

        for i in 0..input.len() {
            let input = parse_int(input[i]);
            assert_eq!(solve_part_1(input), expected[i]);
        }
    }

    #[test]
    fn test_part_2() {
        let input: [&str; 5] = ["1", "5", "50", "250", "500"];
        let expected: [i64; 5] = [2, 10, 54, 304, 747];

        for i in 0..input.len() {
            let input = parse_int(input[i]);
            assert_eq!(solve_part_2(input), expected[i]);
        }
    }
}
