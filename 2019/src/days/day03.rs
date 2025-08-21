use std::cmp::{max, min};
use std::time::Instant;

use log::debug;

use crate::shared::line::LineSegment2d;
use crate::shared::point::Point2d;

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

pub fn parse_input(file_contents: String) -> (Vec<LineData>, Vec<LineData>) {
    let mut first: Vec<LineData> = Vec::new();
    let mut second: Vec<LineData> = Vec::new();
    if let Some((first_line, second_line)) = file_contents.split_once('\n') {
        // Parse the first line instructions
        first = get_line_segments(first_line);
        // Parse the second line instructions
        second = get_line_segments(second_line);
    }
    (first, second)
}

fn get_line_segments(instructions: &str) -> Vec<LineData> {
    let mut segments: Vec<LineData> = Vec::new();
    let mut steps = 0;
    let mut previous = Point2d::new(0, 0);
    for instruction in instructions.split(',') {
        if let Ok(num) = instruction[1..].parse::<i64>() {
            steps += num;
            let mut current = previous;
            match &instruction[0..1] {
                "L" => current.x -= num,
                "R" => current.x += num,
                "U" => current.y -= num,
                "D" => current.y += num,
                other => panic!("Unknown direction: {}", other),
            }
            segments.push(LineData { line: LineSegment2d::new(previous, current), steps });
            previous = current;
        }
    }
    segments
}

fn solve_part_1(input: &(Vec<LineData>, Vec<LineData>)) -> i64 {
    find_closest_intersection(input, choose_shortest_manhattan_distance)
}

fn solve_part_2(input: &(Vec<LineData>, Vec<LineData>)) -> i64 {
    find_closest_intersection(input, choose_shortest_total_steps)
}

fn find_closest_intersection(input: &(Vec<LineData>, Vec<LineData>), get_distance: impl Fn(Point2d, &LineData, &LineData) -> i64) -> i64 {
    let mut min_distance = i64::MAX;
    for first in input.0.iter() {
        for second in input.1.iter() {
            if first.line.start.x == first.line.end.x && second.line.start.x == second.line.end.x
                || first.line.start.y == first.line.end.y && second.line.start.y == second.line.end.y {
                // Ignore parallel lines
                continue;
            }

            if first.line.start.x == first.line.end.x {
                let min_x = min(second.line.start.x, second.line.end.x);
                let max_x = max(second.line.start.x, second.line.end.x);
                let min_y = min(first.line.start.y, first.line.end.y);
                let max_y = max(first.line.start.y, first.line.end.y);

                if min_y <= second.line.start.y && second.line.start.y <= max_y
                    && min_x <= first.line.start.x && first.line.start.x <= max_x {
                    let intersection = Point2d::new(first.line.start.x, second.line.start.y);
                    let distance = get_distance(intersection, first, second);
                    if distance == 0 {
                        // Ignore intersection at origin point
                        continue;
                    }
                    if distance < min_distance {
                        min_distance = distance;
                    }
                }
            }
            if first.line.start.y == first.line.end.y {
                let min_y = min(second.line.start.y, second.line.end.y);
                let max_y = max(second.line.start.y, second.line.end.y);
                let min_x = min(first.line.start.x, first.line.end.x);
                let max_x = max(first.line.start.x, first.line.end.x);

                if min_x <= second.line.start.x && second.line.start.x <= max_x
                    && min_y <= first.line.start.y && first.line.start.y <= max_y {
                    let intersection = Point2d::new(second.line.start.x, first.line.start.y);
                    let distance = get_distance(intersection, first, second);
                    if distance == 0 {
                        // Ignore intersection at origin point
                        continue;
                    }
                    if distance < min_distance {
                        min_distance = distance;
                    }
                }
            }
        }
    }
    min_distance
}

fn choose_shortest_manhattan_distance(intersection: Point2d, _: &LineData, _: &LineData) -> i64 {
    intersection.manhattan(&Point2d::new(0, 0))
}

fn choose_shortest_total_steps(intersection: Point2d, first: &LineData, second: &LineData) -> i64 {
    let first_steps = first.steps;
    let second_steps = second.steps;
    let mut first_offset = 0;
    let mut second_offset = 0;
    if first.line.start.x == first.line.end.x {
        first_offset = (first.line.end.y - intersection.y).abs();
        second_offset = (second.line.end.x - intersection.x).abs();
    } else if first.line.start.y == first.line.end.y {
        first_offset = (first.line.end.x - intersection.x).abs();
        second_offset = (second.line.end.y - intersection.y).abs();
    }
    first_steps + second_steps - first_offset - second_offset
}

#[derive(Debug)]
pub struct LineData {
    line: LineSegment2d,
    steps: i64,
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let input: [&str; 3] = [
            "R8,U5,L5,D3
U7,R6,D4,L4",
            "R75,D30,R83,U83,L12,D49,R71,U7,L72
U62,R66,U55,R34,D71,R55,D58,R83",
            "R98,U47,R26,D63,R33,U87,L62,D20,R33,U53,R51
U98,R91,D20,R16,D67,R40,U7,R15,U6,R7",
        ];
        let expected: [i64; 3] = [
            6,
            159,
            135,
        ];

        for i in 0..input.len() {
            let parsed = parse_input(input[i].to_string());
            assert_eq!(solve_part_1(&parsed), expected[i]);
        }
    }

    #[test]
    fn test_part_2() {
        let input: [&str; 3] = [
            "R8,U5,L5,D3
U7,R6,D4,L4",
            "R75,D30,R83,U83,L12,D49,R71,U7,L72
U62,R66,U55,R34,D71,R55,D58,R83",
            "R98,U47,R26,D63,R33,U87,L62,D20,R33,U53,R51
U98,R91,D20,R16,D67,R40,U7,R15,U6,R7",
        ];
        let expected: [i64; 3] = [
            30,
            610,
            410,
        ];

        for i in 0..input.len() {
            let parsed = parse_input(input[i].to_string());
            assert_eq!(solve_part_2(&parsed), expected[i]);
        }
    }
}
