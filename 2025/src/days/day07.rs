use std::collections::{HashMap, HashSet, VecDeque};
use std::time::Instant;

use aoc_helpers::direction::Direction;
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
    let part2 = solve_part_2(&input);
    debug!("Part 2: {} ({:?})", part2, part2_timer.elapsed());

    (part1.to_string(), part2.to_string())
}

fn solve_part_1(input: &HashMap<Point2d, char>) -> i64 {
    let mut total = 0;

    let mut queue: VecDeque<Point2d> = VecDeque::new();
    let mut visited: HashSet<Point2d> = HashSet::new();

    if let Some((start, _)) = input.iter().find(|(_, v)| **v == 'S') {
        queue.push_back(*start);
        visited.insert(*start);

        while let Some(point) = queue.pop_front() {
            if let Some(value) = input.get(&point) {
                match value {
                    'S' | '.' => {
                        let next = point.next(&Direction::Down);
                        if visited.insert(next) {
                            queue.push_back(next);
                        }
                    }
                    '^' => {
                        total += 1;
                        let left = point.next(&Direction::Left);
                        let right = point.next(&Direction::Right);
                        if visited.insert(left) {
                            queue.push_back(left);
                        }
                        if visited.insert(right) {
                            queue.push_back(right);
                        }
                    }
                    _ => unreachable!(),
                }
            }
        }
    }

    total
}

fn solve_part_2(input: &HashMap<Point2d, char>) -> i64 {
    let mut cache: HashMap<Point2d, i64> = HashMap::new();

    if let Some((start, _)) = input.iter().find(|(_, v)| **v == 'S') {
        return traverse_rec(input, start, &mut cache);
    }
    unreachable!();
}

fn traverse_rec(
    input: &HashMap<Point2d, char>,
    point: &Point2d,
    cache: &mut HashMap<Point2d, i64>,
) -> i64 {
    if let Some(&cached) = cache.get(point) {
        return cached;
    }

    input.get(point).map_or(1, |value| match value {
        'S' | '.' => {
            let next = point.next(&Direction::Down);
            let result = traverse_rec(input, &next, cache);
            cache.insert(*point, result);
            result
        }
        '^' => {
            let left = point.next(&Direction::Left);
            let right = point.next(&Direction::Right);
            let result = traverse_rec(input, &left, cache) + traverse_rec(input, &right, cache);
            cache.insert(*point, result);
            result
        }
        _ => unreachable!(),
    })
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let data: [(&str, i64); 1] = [(
            ".......S.......
...............
.......^.......
...............
......^.^......
...............
.....^.^.^.....
...............
....^.^...^....
...............
...^.^...^.^...
...............
..^...^.....^..
...............
.^.^.^.^.^...^.
...............",
            21,
        )];

        for (input, expected) in data {
            let input = parse_char_grid(input);
            assert_eq!(solve_part_1(&input), expected);
        }
    }

    #[test]
    fn test_part_2() {
        let data: [(&str, i64); 1] = [(
            ".......S.......
...............
.......^.......
...............
......^.^......
...............
.....^.^.^.....
...............
....^.^...^....
...............
...^.^...^.^...
...............
..^...^.....^..
...............
.^.^.^.^.^...^.
...............",
            40,
        )];

        for (input, expected) in data {
            let input = parse_char_grid(input);
            assert_eq!(solve_part_2(&input), expected);
        }
    }
}
