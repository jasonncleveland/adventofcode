use std::collections::{HashSet, VecDeque};
use std::f64::consts::PI;
use std::time::Instant;

use aoc_helpers::math::greatest_common_divisor;
use aoc_helpers::point2d::Point2d;
use log::{debug, trace};

pub fn solve(file_contents: String) -> (String, String) {
    let parse_timer = Instant::now();
    let input = parse_input(file_contents);
    debug!("File parse: ({:?})", parse_timer.elapsed());

    let part1_timer = Instant::now();
    let (part1, origin) = solve_part_1(&input);
    debug!("Part 1: {} ({:?})", part1, part1_timer.elapsed());

    let part2_timer = Instant::now();
    let part2 = solve_part_2(&input, &origin);
    debug!("Part 2: {} ({:?})", part2, part2_timer.elapsed());

    (part1.to_string(), part2.to_string())
}

fn parse_input(file_contents: String) -> Vec<Point2d> {
    let mut points: Vec<Point2d> = Vec::new();
    for (y, line) in file_contents.lines().enumerate() {
        for (x, c) in line.chars().enumerate() {
            if c == '#' {
                points.push(Point2d::new(x as i64, y as i64));
            }
        }
    }
    points
}

fn solve_part_1(input: &Vec<Point2d>) -> (usize, Point2d) {
    let mut max_visible_count = usize::MIN;
    let mut best_point: Point2d = Point2d::new(0, 0);

    #[derive(Eq, Hash, PartialEq)]
    struct SlopeIntercept {
        x: i64,
        y: i64,
        slope: (i64, i64),
        intercept: (i64, i64),
        direction: (i64, i64),
    }

    for start in input {
        trace!("checking how many points are visible from {}", start);
        let mut found_slopes: HashSet<SlopeIntercept> = HashSet::new();
        for end in input {
            if start == end {
                continue;
            }

            // There can be two valid asteroids on opposite sides of the same slope
            // so we need to keep track of the directionality to avoid ignoring
            // any valid asteroids
            let directionality = get_directionality(start, end);

            // If the X values are equal then we have a straight vertical line
            if start.x == end.x {
                found_slopes.insert(SlopeIntercept {
                    x: start.x,
                    y: i64::MAX,
                    slope: (i64::MAX, i64::MAX),
                    intercept: (i64::MAX, i64::MAX),
                    direction: directionality,
                });
                continue;
            }

            // If the Y values are equal then we have a straight horizontal line
            if start.y == end.y {
                found_slopes.insert(SlopeIntercept {
                    x: i64::MAX,
                    y: start.y,
                    slope: (i64::MAX, i64::MAX),
                    intercept: (i64::MAX, i64::MAX),
                    direction: directionality,
                });
                continue;
            }

            // Calculate slope and intercept as fractions to preserve precision
            // Slope: m = (y₂ - y₁)/(x₂ - x₁)
            let mut slope_numerator = end.y - start.y;
            let mut slope_denominator = end.x - start.x;

            // If the numbers have the same sign, make them positive
            if slope_numerator * slope_denominator > 0 {
                slope_numerator = slope_numerator.abs();
                slope_denominator = slope_denominator.abs();
            }

            // Reduce the slope fraction
            let slope_gcd = greatest_common_divisor(slope_numerator.abs(), slope_denominator.abs());
            slope_numerator /= slope_gcd;
            slope_denominator /= slope_gcd;

            // Y-Intersect: b = y₁ - x₁(y₂ - y₁)/(x₂ - x₁)
            // Y-Intersect: b = y₁ - x₁m
            let mut intercept_numerator = start.y * slope_denominator - start.x * slope_numerator;
            let mut intercept_denominator = slope_denominator;

            // If the numbers have the same sign, make them positive
            if intercept_numerator * intercept_denominator > 0 {
                intercept_numerator = intercept_numerator.abs();
                intercept_denominator = intercept_denominator.abs();
            }

            // Ignore the intercept if it is 0
            if intercept_numerator == 0 {
                found_slopes.insert(SlopeIntercept {
                    x: i64::MAX,
                    y: i64::MAX,
                    slope: (slope_numerator, slope_denominator),
                    intercept: (i64::MAX, i64::MAX),
                    direction: directionality,
                });
            } else {
                // Reduce the intercept fraction
                let intercept_gcd = greatest_common_divisor(intercept_numerator.abs(), intercept_denominator.abs());
                intercept_numerator /= intercept_gcd;
                intercept_denominator /= intercept_gcd;
                found_slopes.insert(SlopeIntercept {
                    x: i64::MAX,
                    y: i64::MAX,
                    slope: (slope_numerator, slope_denominator),
                    intercept: (intercept_numerator, intercept_denominator),
                    direction: directionality,
                });
            }
        }

        trace!("found {} points visible from {}", found_slopes.len(), start);
        if found_slopes.len() > max_visible_count {
            max_visible_count = found_slopes.len();
            best_point = *start;
        }
    }

    trace!("Best point is {} with {} visible points", best_point, max_visible_count);
    (max_visible_count, best_point)
}

fn solve_part_2(input: &[Point2d], origin: &Point2d) -> i64 {
    let mut targets: Vec<PolarCoordinate> = Vec::new();
    for target in input {
        if target == origin {
            continue;
        }

        let distance = origin.euclidean(target);
        let angle = match target.x < origin.x {
            true => PI * 2.0 - origin.angle(target),
            false => origin.angle(target).abs(),
        };

        targets.push(PolarCoordinate {
            x: target.x,
            y: target.y,
            distance,
            angle,
        });
    }
    targets.sort_by(|a, b| a.partial_cmp(b).unwrap());

    let mut count = 0;
    let mut last_angle = f64::NAN;
    let mut queue: VecDeque<PolarCoordinate> = VecDeque::from(targets);
    while let Some(target) = queue.pop_front() {
        if target.angle == last_angle && queue.len() > 1 {
            // We can only destroy one asteroid at a time per angle
            queue.push_back(target);
            continue;
        }

        trace!("Destroying asteroid at ({}, ({}))", target.x, target.y);
        last_angle = target.angle;
        count += 1;

        if count == 200 {
            trace!("200th asteroid destroyed: {:?}", target);
            return target.x * 100 + target.y;
        }
    }
    0
}

fn get_directionality(start: &Point2d, end: &Point2d) -> (i64, i64) {
    let x = match start.x - end.x {
        ..0 => 1,
        1.. => -1,
        0 => 0,
    };
    let y = match start.y - end.y {
        ..0 => 1,
        1.. => -1,
        0 => 0,
    };
    (x, y)
}

#[derive(Debug, PartialEq, PartialOrd)]
struct PolarCoordinate {
    angle: f64,
    distance: f64,
    x: i64,
    y: i64,
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let input: [&str; 5] = [
            ".#..#
.....
#####
....#
...##",
            "......#.#.
#..#.#....
..#######.
.#.#.###..
.#..#.....
..#....#.#
#..#....#.
.##.#..###
##...#..#.
.#....####",
            "#.#...#.#.
.###....#.
.#....#...
##.#.#.#.#
....#.#.#.
.##..###.#
..#...##..
..##....##
......#...
.####.###.",
            ".#..#..###
####.###.#
....###.#.
..###.##.#
##.##.#.#.
....###..#
..#.#..#.#
#..#.#.###
.##...##.#
.....#.#..",
            ".#..##.###...#######
##.############..##.
.#.######.########.#
.###.#######.####.#.
#####.##.#.##.###.##
..#####..#.#########
####################
#.####....###.#.#.##
##.#################
#####.##.###..####..
..######..##.#######
####.##.####...##..#
.#####..#.######.###
##...#.##########...
#.##########.#######
.####.#.###.###.#.##
....##.##.###..#####
.#.#.###########.###
#.#.#.#####.####.###
###.##.####.##.#..##",
        ];
        let expected: [usize; 5] = [
            8,
            33,
            35,
            41,
            210,
        ];

        for i in 0..input.len() {
            let parsed = parse_input(input[i].to_string());
            assert_eq!(solve_part_1(&parsed).0, expected[i]);
        }
    }

    #[test]
    fn test_part_2() {
        let input: [(Point2d, &str); 1] = [
            (Point2d::new(11, 13), ".#..##.###...#######
##.############..##.
.#.######.########.#
.###.#######.####.#.
#####.##.#.##.###.##
..#####..#.#########
####################
#.####....###.#.#.##
##.#################
#####.##.###..####..
..######..##.#######
####.##.####...##..#
.#####..#.######.###
##...#.##########...
#.##########.#######
.####.#.###.###.#.##
....##.##.###..#####
.#.#.###########.###
#.#.#.#####.####.###
###.##.####.##.#..##"),
        ];
        let expected: [i64; 1] = [
            802,
        ];

        for i in 0..input.len() {
            let parsed = parse_input(input[i].1.to_string());
            assert_eq!(solve_part_2(&parsed, &input[i].0), expected[i]);
        }
    }
}
