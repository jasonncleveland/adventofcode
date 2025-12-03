use std::time::Instant;

use aoc_helpers::hex::{HexCoordinate, HexDirection};
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

fn parse_input(file_contents: &str) -> Vec<HexDirection> {
    file_contents
        .split(',')
        .map(|direction| match direction {
            "nw" => HexDirection::NorthWest,
            "n" => HexDirection::North,
            "ne" => HexDirection::NorthEast,
            "sw" => HexDirection::SouthWest,
            "s" => HexDirection::South,
            "se" => HexDirection::SouthEast,
            _ => unreachable!(),
        })
        .collect()
}

fn solve_part_1(directions: &[HexDirection]) -> i64 {
    let mut coordinate = HexCoordinate::new(0, 0, 0);
    for direction in directions {
        coordinate.move_hex(direction);
    }
    coordinate.distance(&HexCoordinate::new(0, 0, 0))
}

fn solve_part_2(directions: &[HexDirection]) -> i64 {
    let mut max_distance = i64::MIN;
    let origin = HexCoordinate::new(0, 0, 0);
    let mut coordinate = HexCoordinate::new(0, 0, 0);
    for direction in directions {
        coordinate.move_hex(direction);
        let distance = coordinate.distance(&origin);
        if distance > max_distance {
            max_distance = distance;
        }
    }
    max_distance
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let data: [(&str, i64); 4] = [
            ("ne,ne,ne", 3),
            ("ne,ne,sw,sw", 0),
            ("ne,ne,s,s", 2),
            ("se,sw,se,sw,sw", 3),
        ];

        for (input, expected) in data {
            let input = parse_input(input);
            assert_eq!(solve_part_1(&input), expected);
        }
    }

    #[test]
    fn test_part_2() {
        let data: [(&str, i64); 4] = [
            ("ne,ne,ne", 3),
            ("ne,ne,sw,sw", 2),
            ("ne,ne,s,s", 2),
            ("se,sw,se,sw,sw", 3),
        ];

        for (input, expected) in data {
            let input = parse_input(input);
            assert_eq!(solve_part_2(&input), expected);
        }
    }
}
