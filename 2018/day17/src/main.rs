use std::collections::{HashMap, HashSet};
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

fn parse_input(file_contents: &str) -> (HashMap<Coordinate, char>, i64, i64) {
    let mut ground: HashMap<Coordinate, char> = HashMap::new();

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
                                ground.insert((x, y), '#');
                            }
                        }
                    },
                    "y" => {
                        if let Ok(y) = left.1.parse::<i64>()
                            && let Some((Ok(x1), Ok(x2))) = right.1.split_once("..").map(|s| (s.0.parse::<i64>(), s.1.parse::<i64>())) {
                            for x in x1..=x2 {
                                ground.insert((x, y), '#');
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

fn part1(file_contents: &str) -> i64 {
    let (mut map, min_y, max_y) = parse_input(file_contents);

    let water_source: Coordinate = (500, 0);

    let mut visited: HashSet<Coordinate> = HashSet::new();
    flood_vertical(&mut map, water_source, &Direction::Down, min_y, max_y, &mut visited);

    map.values().filter(|c| **c == '~' || **c == '|').count() as i64
}

fn part2(file_contents: &str) -> i64 {
    let (mut map, min_y, max_y) = parse_input(file_contents);

    let water_source: Coordinate = (500, 0);

    let mut visited: HashSet<Coordinate> = HashSet::new();
    flood_vertical(&mut map, water_source, &Direction::Down, min_y, max_y, &mut visited);

    map.values().filter(|c| **c == '~').count() as i64
}

fn flood_vertical(map: &mut HashMap<Coordinate, char>, coordinate: Coordinate, direction: &Direction, min_y: i64, max_y: i64, visited: &mut HashSet<Coordinate>) -> bool {
    if visited.contains(&coordinate) {
        return false;
    }
    visited.insert(coordinate);

    if coordinate.1 > max_y {
        // Stop if we reach the max y value
        return false;
    }

    match map.get(&coordinate) {
        None => {
            // Insert flowing water
            if coordinate.1 >= min_y && coordinate.1 <= max_y {
                map.insert(coordinate, '|');
            }

            let down = get_next_coordinate(coordinate, direction);
            match map.get(&down) {
                // Attempt to flood if there is nothing
                None => {
                    let is_settled = flood_vertical(map, down, direction, min_y, max_y, visited);

                    if can_settle(map, coordinate) {
                        settle_water(map, coordinate);
                        return true;
                    } else if is_settled {
                        // Flood left and right if we can't settle on the current level but previous level is settled
                        let left = get_next_coordinate(coordinate, &Direction::Left);
                        flood_horizontal(map, left, &Direction::Left, min_y, max_y, visited);

                        let right = get_next_coordinate(coordinate, &Direction::Right);
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
                        let left = get_next_coordinate(coordinate, &Direction::Left);
                        flood_horizontal(map, left, &Direction::Left, min_y, max_y, visited);

                        let right = get_next_coordinate(coordinate, &Direction::Right);
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

fn settle_water(map: &mut HashMap<Coordinate, char>, origin: Coordinate) {
    // Settle origin
    map.insert(origin, '~');

    let mut settle_direction = |mut coordinate: Coordinate, direction: &Direction| {
        loop {
            match map.get(&coordinate) {
                // Settle water as long as there is nothing or flowing water
                None | Some('|') => {
                    map.insert(coordinate, '~');
                    coordinate = get_next_coordinate(coordinate, direction);
                },
                // Stop if a wall is found
                Some('#') => break,
                Some(other) => unreachable!("invalid character found {}", other),
            }
        }
    };

    // Settle left and right
    let left = get_next_coordinate(origin, &Direction::Left);
    settle_direction(left, &Direction::Left);

    let right = get_next_coordinate(origin, &Direction::Right);
    settle_direction(right, &Direction::Right);
}

fn can_settle(map: &mut HashMap<Coordinate, char>, origin: Coordinate) -> bool {
    let can_settle_direction = |mut coordinate: Coordinate, direction: &Direction| {
        loop {
            match map.get(&coordinate) {
                // Continue to check as long as there is a solid base below
                None | Some('|') => {
                    let down = get_next_coordinate(coordinate, &Direction::Down);
                    match map.get(&down) {
                        None | Some('|') => return false,
                        Some('#') | Some('~') => coordinate = get_next_coordinate(coordinate, direction),
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
    let left = get_next_coordinate(origin, &Direction::Left);
    let right = get_next_coordinate(origin, &Direction::Right);

    can_settle_direction(left, &Direction::Left) && can_settle_direction(right, &Direction::Right)
}

fn flood_horizontal(map: &mut HashMap<Coordinate, char>, coordinate: Coordinate, direction: &Direction, min_y: i64, max_y: i64, visited: &mut HashSet<Coordinate>) {
    if visited.contains(&coordinate) {
        return;
    }
    visited.insert(coordinate);


    match map.get(&coordinate) {
        // Attempt to flood if there is nothing
        None => {
            let down = get_next_coordinate(coordinate, &Direction::Down);
            match map.get(&down) {
                // Flood vertically if there is nothing below
                None => {
                    // Insert flowing water
                    if coordinate.1 >= min_y && coordinate.1 <= max_y {
                        map.insert(coordinate, '|');
                    }

                    let is_settled = flood_vertical(map, down, &Direction::Down, min_y, max_y, visited);
                    if is_settled {
                        let next = get_next_coordinate(coordinate, direction);
                        flood_horizontal(map, next, direction, min_y, max_y, visited);
                    }
                },
                // Flood as long as there is a solid base below
                Some('#') | Some('~') => {
                    // Insert flowing water
                    if coordinate.1 >= min_y && coordinate.1 <= max_y {
                        map.insert(coordinate, '|');
                    }

                    let next = get_next_coordinate(coordinate, direction);
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

fn get_next_coordinate(coordinate: Coordinate, direction: &Direction) -> Coordinate {
    match direction {
        Direction::Down => (coordinate.0, coordinate.1 + 1),
        Direction::Left => (coordinate.0 - 1, coordinate.1),
        Direction::Right => (coordinate.0 + 1, coordinate.1),
    }
}

#[derive(Debug)]
enum Direction {
    Down,
    Left,
    Right,
}

type Coordinate = (i64, i64);

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part1() {
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
        let expected: [i64; 1] = [
            57,
        ];

        for i in 0..input.len() {
            assert_eq!(part1(input[i]), expected[i]);
        }
    }

    #[test]
    fn test_part2() {
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
        let expected: [i64; 1] = [
            29,
        ];

        for i in 0..input.len() {
            assert_eq!(part2(input[i]), expected[i]);
        }
    }
}
