use std::collections::{HashMap, HashSet, VecDeque};
use std::time::Instant;

use aoc_helpers::point2d::Point2d;
use log::debug;

use crate::shared::knot_hash::calculate_knot_hash;

pub fn solve(file_contents: &str) -> (String, String) {
    let parse_timer = Instant::now();
    debug!("File parse: ({:?})", parse_timer.elapsed());

    let part1_timer = Instant::now();
    let part1 = solve_part_1(file_contents);
    debug!("Part 1: {} ({:?})", part1, part1_timer.elapsed());

    let part2_timer = Instant::now();
    let part2 = solve_part_2(file_contents);
    debug!("Part 2: {} ({:?})", part2, part2_timer.elapsed());

    (part1.to_string(), part2.to_string())
}

fn solve_part_1(input: &str) -> usize {
    let memory = generate_memory(input);
    memory.values().filter(|c| **c == '#').count()
}

fn solve_part_2(input: &str) -> i64 {
    let memory = generate_memory(input);

    let mut queue: VecDeque<Point2d> = VecDeque::new();
    let mut visited: HashSet<Point2d> = HashSet::new();

    let mut total = 0;
    for x in 0..128i64 {
        for y in 0..128i64 {
            let start_coordinate = Point2d::new(x, y);
            if visited.contains(&start_coordinate) {
                continue;
            }

            if let Some(&block) = memory.get(&start_coordinate)
                && block == '.'
            {
                continue;
            }

            queue.push_back(start_coordinate);
            visited.insert(start_coordinate);
            total += 1;

            while let Some(coordinate) = queue.pop_front() {
                for neighbour in coordinate.neighbours() {
                    if !visited.contains(&neighbour)
                        && let Some(&block) = memory.get(&neighbour)
                        && block == '#'
                    {
                        visited.insert(neighbour);
                        queue.push_back(neighbour);
                    }
                }
            }
        }
    }
    total
}

fn generate_memory(input: &str) -> HashMap<Point2d, char> {
    let mut memory: HashMap<Point2d, char> = HashMap::new();
    let mut coordinate = Point2d::new(0, 0);

    for i in 0..128 {
        let hash_input = format!("{}-{}", input, i);
        let knot_hash = calculate_knot_hash(&hash_input);

        for hex in knot_hash.chars() {
            let binary = match hex {
                '0' => vec!['.', '.', '.', '.'],
                '1' => vec!['.', '.', '.', '#'],
                '2' => vec!['.', '.', '#', '.'],
                '3' => vec!['.', '.', '#', '#'],
                '4' => vec!['.', '#', '.', '.'],
                '5' => vec!['.', '#', '.', '#'],
                '6' => vec!['.', '#', '#', '.'],
                '7' => vec!['.', '#', '#', '#'],
                '8' => vec!['#', '.', '.', '.'],
                '9' => vec!['#', '.', '.', '#'],
                'a' => vec!['#', '.', '#', '.'],
                'b' => vec!['#', '.', '#', '#'],
                'c' => vec!['#', '#', '.', '.'],
                'd' => vec!['#', '#', '.', '#'],
                'e' => vec!['#', '#', '#', '.'],
                'f' => vec!['#', '#', '#', '#'],
                other => unreachable!("invalid hex value: {}", other),
            };
            for block in binary {
                memory.insert(coordinate, block);
                coordinate.x += 1;
            }
        }
        coordinate.x = 0;
        coordinate.y += 1;
    }
    memory
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let data: [(&str, usize); 1] = [("flqrgnkx", 8108)];

        for (input, expected) in data {
            assert_eq!(solve_part_1(input), expected);
        }
    }

    #[test]
    fn test_part_2() {
        let data: [(&str, i64); 1] = [("flqrgnkx", 1242)];

        for (input, expected) in data {
            assert_eq!(solve_part_2(input), expected);
        }
    }
}
