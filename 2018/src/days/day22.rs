use std::collections::HashMap;
use std::time::Instant;

use aoc_helpers::point2d::Point2d;
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

fn parse_input(file_contents: String) -> ScanInfo {
    if let Some((first, second)) = file_contents.split_once('\n')
        && let Some((_, depth)) = first.split_once(": ")
        && let Some((_, target)) = second.split_once(": ")
        && let Some((xs, ys)) = target.split_once(',')
        && let Ok(d) = depth.parse::<i64>()
        && let Ok(x) = xs.parse::<i64>()
        && let Ok(y) = ys.parse::<i64>()
    {
        return ScanInfo {
            depth: d,
            target: Point2d::new(x, y),
        };
    }
    unreachable!();
}

fn solve_part_1(input: &ScanInfo) -> i64 {
    let mut cave: HashMap<Point2d, char> = HashMap::new();
    let mut cache = Cache::new();
    let mut risk_level = 0;
    for x in 0..=input.target.x {
        for y in 0..=input.target.y {
            let coordinate = Point2d::new(x, y);
            trace!("calculating risk level for point {}", coordinate);
            let region_type = calculate_region_type(&coordinate, &input.target, input.depth, &mut cache);
            trace!("region type: {:?}", region_type);
            match region_type {
                Region::Rocky => {
                    cave.insert(coordinate, '.');
                    risk_level += 0;
                },
                Region::Wet => {
                    cave.insert(coordinate, '=');
                    risk_level += 1;
                },
                Region::Narrow => {
                    cave.insert(coordinate, '|');
                    risk_level += 2;
                },
            }
        }
    }
    risk_level
}

fn solve_part_2(input: &ScanInfo) -> i64 {
    -1
}

fn calculate_geologic_index(point: &Point2d, target: &Point2d, depth: i64, cache: &mut Cache) -> i64 {
    trace!("calculating geologic index of {} with target {}", point, target);
    if cache.geologic_index.contains_key(point) {
        return cache.geologic_index[point];
    }

    let geologic_index: i64;
    if point.x == 0 && point.y == 0 || point == target {
        geologic_index = 0;
    } else if point.y == 0 {
        geologic_index = point.x * 16807;
    } else if point.x == 0 {
        geologic_index = point.y * 48271;
    } else {
        let up = calculate_erosion_level(&Point2d::new(point.x - 1, point.y), target, depth, cache);
        let left = calculate_erosion_level(&Point2d::new(point.x, point.y - 1), target, depth, cache);
        geologic_index = up * left;
    }

    cache.geologic_index.insert(*point, geologic_index);
    geologic_index
}

fn calculate_erosion_level(point: &Point2d, target: &Point2d, depth: i64, cache: &mut Cache) -> i64 {
    trace!("calculating erosion level of {} with target {}", point, target);
    if cache.erosion_level.contains_key(point) {
        return cache.erosion_level[point];
    }

    let geologic_index = calculate_geologic_index(point, target, depth, cache);
    trace!("({} + {}) % 20183 = {} % 3 = {}", geologic_index, depth, (geologic_index + depth) % 20183, ((geologic_index + depth) % 20183) % 3);
    let erosion_level = (geologic_index + depth) % 20183;
    cache.erosion_level.insert(*point, erosion_level);
    erosion_level
}

fn calculate_region_type(point: &Point2d, target: &Point2d, depth: i64, cache: &mut Cache) -> Region {
    trace!("calculating region type of {} with target {}", point, target);
    let erosion_level = calculate_erosion_level(point, target, depth, cache);
    match erosion_level % 3 {
        0 => Region::Rocky,
        1 => Region::Wet,
        2 => Region::Narrow,
        _ => unreachable!(),
    }
}

#[derive(Debug)]
struct ScanInfo {
    depth: i64,
    target: Point2d,
}

#[derive(Debug, PartialEq)]
enum Region {
    Rocky,
    Narrow,
    Wet,
}

struct Cache {
    geologic_index: HashMap<Point2d, i64>,
    erosion_level: HashMap<Point2d, i64>,
}

impl Cache {
    fn new() -> Cache {
        Self {
            geologic_index: HashMap::new(),
            erosion_level: HashMap::new(),
        }
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_calculate_region_type() {
        let input: [Point2d; 5] = [
            Point2d::new(0, 0),
            Point2d::new(1, 0),
            Point2d::new(0, 1),
            Point2d::new(1, 1),
            Point2d::new(10, 10),
        ];
        let expected: [Region; 5] = [
            Region::Rocky,
            Region::Wet,
            Region::Rocky,
            Region::Narrow,
            Region::Rocky,
        ];

        let target = Point2d::new(10, 10);
        let depth = 510;
        for i in 0..input.len() {
            let mut cache = Cache::new();
            assert_eq!(calculate_region_type(&input[i], &target, depth, &mut cache), expected[i], "{}", input[i]);
        }
    }

    #[test]
    fn test_part_1() {
        let input: [&str; 1] = [
            "depth: 510
target: 10,10",
        ];
        let expected: [i64; 1] = [
            114,
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
