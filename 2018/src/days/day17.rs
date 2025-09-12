use std::collections::{HashMap, HashSet};
use std::time::Instant;

use aoc_helpers::direction::Direction;
use aoc_helpers::point2d::Point2d;
use log::debug;

pub fn solve(file_contents: String) -> (String, String) {
    let parse_timer = Instant::now();
    let (map, min_y, max_y) = parse_input(file_contents);
    debug!("File parse: ({:?})", parse_timer.elapsed());

    let part1_timer = Instant::now();
    let part1 = solve_part_1(&map, min_y, max_y);
    debug!("Part 1: {} ({:?})", part1, part1_timer.elapsed());

    let part2_timer = Instant::now();
    let part2 = solve_part_2(&map, min_y, max_y);
    debug!("Part 2: {} ({:?})", part2, part2_timer.elapsed());

    (part1.to_string(), part2.to_string())
}

fn parse_input(file_contents: String) -> (HashMap<Point2d, char>, i64, i64) {
    let mut ground: HashMap<Point2d, char> = HashMap::new();

    let mut min_y = i64::MAX;
    let mut max_y = i64::MIN;
    for line in file_contents.lines() {
        let items = line.split(", ").map(|s| s.split_once("=")).collect::<Vec<_>>();
        if let Some(Some(left)) = items.first() {
            // This has a single value
            if let Some(Some(right)) = items.last() {
                // This has a range value
                match left.0 {
                    "x" => {
                        if let Ok(x) = left.1.parse::<i64>()
                            && let Some((Ok(y1), Ok(y2))) = right.1.split_once("..").map(|s| (s.0.parse::<i64>(), s.1.parse::<i64>())) {
                            for y in y1..=y2 {
                                if y < min_y {
                                    min_y = y;
                                }
                                if y > max_y {
                                    max_y = y;
                                }
                                ground.insert(Point2d::new(x, y), '#');
                            }
                        }
                    },
                    "y" => {
                        if let Ok(y) = left.1.parse::<i64>()
                            && let Some((Ok(x1), Ok(x2))) = right.1.split_once("..").map(|s| (s.0.parse::<i64>(), s.1.parse::<i64>())) {
                            for x in x1..=x2 {
                                ground.insert(Point2d::new(x, y), '#');
                            }
                        }
                    },
                    _ => panic!("invalid coordinate type")
                }
            }
        }
    }

    (ground, min_y, max_y)
}

fn solve_part_1(map: &HashMap<Point2d, char>, min_y: i64, max_y: i64) -> usize {
    let mut map_copy = map.clone();
    let water_source = Point2d::new(500, 0);

    let mut visited: HashSet<Point2d> = HashSet::new();
    flood_vertical(&mut map_copy, water_source, &Direction::Down, min_y, max_y, &mut visited);

    map_copy.values().filter(|c| **c == '~' || **c == '|').count()
}

fn solve_part_2(map: &HashMap<Point2d, char>, min_y: i64, max_y: i64) -> usize {
    let mut map_copy = map.clone();
    let water_source = Point2d::new(500, 0);

    let mut visited: HashSet<Point2d> = HashSet::new();
    flood_vertical(&mut map_copy, water_source, &Direction::Down, min_y, max_y, &mut visited);

    map_copy.values().filter(|c| **c == '~').count()
}

fn flood_vertical(map: &mut HashMap<Point2d, char>, coordinate: Point2d, direction: &Direction, min_y: i64, max_y: i64, visited: &mut HashSet<Point2d>) -> bool {
    if visited.contains(&coordinate) {
        return false;
    }
    visited.insert(coordinate);

    if coordinate.y > max_y {
        // Stop if we reach the max y value
        return false;
    }

    match map.get(&coordinate) {
        None => {
            // Insert flowing water
            if coordinate.y >= min_y && coordinate.y <= max_y {
                map.insert(coordinate, '|');
            }

            let down = coordinate.next(direction);
            match map.get(&down) {
                // Attempt to flood if there is nothing
                None => {
                    let is_settled = flood_vertical(map, down, direction, min_y, max_y, visited);

                    if can_settle(map, coordinate) {
                        settle_water(map, coordinate);
                        return true;
                    } else if is_settled {
                        // Flood left and right if we can't settle on the current level but previous level is settled
                        let left = coordinate.next(&Direction::Left);
                        flood_horizontal(map, left, &Direction::Left, min_y, max_y, visited);

                        let right = coordinate.next(&Direction::Right);
                        flood_horizontal(map, right, &Direction::Right, min_y, max_y, visited);

                        // Check if we can settle after flowing left and right
                        if can_settle(map, coordinate) {
                            settle_water(map, coordinate);
                            return true;
                        }
                    }
                    false
                },
                // Attempt to settle if we hit a solid base
                Some('#') | Some('~') => {
                    if can_settle(map, coordinate) {
                        settle_water(map, coordinate);
                        return true;
                    } else {
                        // Flood left and right if we can't settle
                        let left = coordinate.next(&Direction::Left);
                        flood_horizontal(map, left, &Direction::Left, min_y, max_y, visited);

                        let right = coordinate.next(&Direction::Right);
                        flood_horizontal(map, right, &Direction::Right, min_y, max_y, visited);

                        // Check if we can settle after flowing left and right
                        if can_settle(map, coordinate) {
                            settle_water(map, coordinate);
                            return true;
                        }
                    }
                    false
                },
                // Stop if we run into flowing water
                Some('|') => false,
                Some(other) => unreachable!("invalid character found {}", other),
            }
        },
        Some(other) => unreachable!("invalid character found {}", other),
    }
}

fn settle_water(map: &mut HashMap<Point2d, char>, origin: Point2d) {
    // Settle origin
    map.insert(origin, '~');

    let mut settle_direction = |mut coordinate: Point2d, direction: &Direction| {
        loop {
            match map.get(&coordinate) {
                // Settle water as long as there is nothing or flowing water
                None | Some('|') => {
                    map.insert(coordinate, '~');
                    coordinate = coordinate.next(direction);
                },
                // Stop if a wall is found
                Some('#') => break,
                Some(other) => unreachable!("invalid character found {}", other),
            }
        }
    };

    // Settle left and right
    let left = origin.next(&Direction::Left);
    settle_direction(left, &Direction::Left);

    let right = origin.next(&Direction::Right);
    settle_direction(right, &Direction::Right);
}

fn can_settle(map: &mut HashMap<Point2d, char>, origin: Point2d) -> bool {
    let can_settle_direction = |mut coordinate: Point2d, direction: &Direction| {
        loop {
            match map.get(&coordinate) {
                // Continue to check as long as there is a solid base below
                None | Some('|') => {
                    let down = coordinate.next(&Direction::Down);
                    match map.get(&down) {
                        None | Some('|') => return false,
                        Some('#') | Some('~') => coordinate = coordinate.next(direction),
                        Some(other) => unreachable!("invalid character found {}", other),
                    }
                },
                // Stop if a wall is found
                Some('#') => break,
                Some(other) => unreachable!("invalid character found {}", other),
            }
        }
        true
    };

    // Check left and right
    let left = origin.next(&Direction::Left);
    let right = origin.next(&Direction::Right);

    can_settle_direction(left, &Direction::Left) && can_settle_direction(right, &Direction::Right)
}

fn flood_horizontal(map: &mut HashMap<Point2d, char>, coordinate: Point2d, direction: &Direction, min_y: i64, max_y: i64, visited: &mut HashSet<Point2d>) {
    if visited.contains(&coordinate) {
        return;
    }
    visited.insert(coordinate);


    match map.get(&coordinate) {
        // Attempt to flood if there is nothing
        None => {
            let down = coordinate.next(&Direction::Down);
            match map.get(&down) {
                // Flood vertically if there is nothing below
                None => {
                    // Insert flowing water
                    if coordinate.y >= min_y && coordinate.y <= max_y {
                        map.insert(coordinate, '|');
                    }

                    let is_settled = flood_vertical(map, down, &Direction::Down, min_y, max_y, visited);
                    if is_settled {
                        let next = coordinate.next(direction);
                        flood_horizontal(map, next, direction, min_y, max_y, visited);
                    }
                },
                // Flood as long as there is a solid base below
                Some('#') | Some('~') => {
                    // Insert flowing water
                    if coordinate.y >= min_y && coordinate.y <= max_y {
                        map.insert(coordinate, '|');
                    }

                    let next = coordinate.next(direction);
                    flood_horizontal(map, next, direction, min_y, max_y, visited);
                },
                Some(other) => unreachable!("invalid character found {}", other),
            }
        },
        // Stop if we hit a wall
        Some('#') => {},
        Some(other) => unreachable!("invalid character found {}", other),
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let input: [&str; 1] = [
            "x=495, y=2..7
y=7, x=495..501
x=501, y=3..7
x=498, y=2..4
x=506, y=1..2
x=498, y=10..13
x=504, y=10..13
y=13, x=498..504",
        ];
        let expected: [usize; 1] = [
            57,
        ];

        for i in 0..input.len() {
            let (map, min_y, max_y) = parse_input(input[i].to_string());
            assert_eq!(solve_part_1(&map, min_y, max_y), expected[i]);
        }
    }

    #[test]
    fn test_part_2() {
        let input: [&str; 1] = [
            "x=495, y=2..7
y=7, x=495..501
x=501, y=3..7
x=498, y=2..4
x=506, y=1..2
x=498, y=10..13
x=504, y=10..13
y=13, x=498..504",
        ];
        let expected: [usize; 1] = [
            29,
        ];

        for i in 0..input.len() {
            let (map, min_y, max_y) = parse_input(input[i].to_string());
            assert_eq!(solve_part_2(&map, min_y, max_y), expected[i]);
        }
    }
}
