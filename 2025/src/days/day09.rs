use std::cmp::{max, min};
use std::collections::VecDeque;
use std::time::Instant;

use aoc_helpers::io::parse_int_list;
use aoc_helpers::line::LineSegment2d;
use aoc_helpers::point2d::Point2d;
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

fn parse_input(file_contents: &str) -> Vec<Point2d> {
    let mut coordinates: Vec<Point2d> = Vec::new();

    for line in file_contents.lines() {
        let numbers = parse_int_list(line, ',');
        if let Some(&x) = numbers.first()
            && let Some(&y) = numbers.last()
        {
            coordinates.push(Point2d::new(x, y));
        }
    }
    coordinates
}

fn solve_part_1(coordinates: &Vec<Point2d>) -> i64 {
    let mut largest_area = i64::MIN;

    for first in coordinates {
        for second in coordinates {
            if first == second || second.x < first.x || second.y < first.y {
                continue;
            }
            let width = second.x - first.x + 1;
            let height = second.y - first.y + 1;
            let area = width * height;
            if area > largest_area {
                largest_area = area;
            }
        }
    }

    largest_area
}

fn solve_part_2(coordinates: &Vec<Point2d>) -> i64 {
    // Generate list of perimeter lines
    let mut lines: Vec<LineSegment2d> = Vec::new();
    let mut corners = VecDeque::from(coordinates.clone());
    if let Some(&last) = coordinates.last() {
        let mut first = last;
        while let Some(second) = corners.pop_front() {
            if second.x == first.x {
                lines.push(if second.y < first.y {
                    LineSegment2d::new(second, first)
                } else {
                    LineSegment2d::new(first, second)
                });
            } else if second.y == first.y {
                lines.push(if second.x < first.x {
                    LineSegment2d::new(second, first)
                } else {
                    LineSegment2d::new(first, second)
                });
            }
            first = second;
        }
    }

    let mut largest_area = i64::MIN;

    // Search all combinations of rectangles
    for first in coordinates {
        for second in coordinates {
            if first == second {
                continue;
            }

            let top_left = Point2d::new(min(first.x, second.x), min(first.y, second.y));
            let bottom_right = Point2d::new(max(first.x, second.x), max(first.y, second.y));

            let width = bottom_right.x - top_left.x + 1;
            let height = bottom_right.y - top_left.y + 1;
            let area = width * height;
            if area > largest_area {
                // Check if the line segment intersects the rectangle
                let is_invalid = lines.iter().any(|line| {
                    line.start.x < bottom_right.x
                        && line.start.y < bottom_right.y
                        && line.end.x > top_left.x
                        && line.end.y > top_left.y
                });

                if !is_invalid {
                    largest_area = area;
                }
            }
        }
    }

    largest_area
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let data: [(&str, i64); 1] = [(
            "7,1
11,1
11,7
9,7
9,5
2,5
2,3
7,3",
            50,
        )];

        for (input, expected) in data {
            let input = parse_input(input);
            assert_eq!(solve_part_1(&input), expected);
        }
    }

    #[test]
    fn test_part_2() {
        let data: [(&str, i64); 1] = [(
            "7,1
11,1
11,7
9,7
9,5
2,5
2,3
7,3",
            24,
        )];

        for (input, expected) in data {
            let input = parse_input(input);
            assert_eq!(solve_part_2(&input), expected);
        }
    }
}
