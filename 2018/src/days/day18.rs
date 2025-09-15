use std::collections::HashMap;
use std::time::Instant;

use aoc_helpers::io::parse_char_grid;
use aoc_helpers::point2d::Point2d;
use log::{debug, trace};

pub fn solve(file_contents: String) -> (String, String) {
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
    let mut landscape = input.clone();
    for _ in 0..10 {
        landscape = simulate_landscape(&landscape);
    }

    let mut trees = 0;
    let mut lumberyards = 0;
    for &value in landscape.values() {
        match value {
            '|' => trees += 1,
            '#' => lumberyards += 1,
            _ => continue,
        }
    }

    trees * lumberyards
}

fn solve_part_2(input: &HashMap<Point2d, char>) -> i64 {
    -1
}

fn simulate_landscape(landscape: &HashMap<Point2d, char>) -> HashMap<Point2d, char> {
    let mut future: HashMap<Point2d, char> = HashMap::new();

    for key in landscape.keys() {
        trace!("checking key {}: {:?}", key, landscape.get(key));

        if let Some(&c) = landscape.get(key) {
            trace!("value {} found at {}", c, key);

            let mut open = 0;
            let mut trees = 0;
            let mut lumberyards = 0;

            for neighbour in key.neighbours8() {
                trace!("checking neighbour {}: {:?}", neighbour, landscape.get(&neighbour));
                if let Some(&n) = landscape.get(&neighbour) {
                    match n {
                        '.' => open += 1,
                        '|' => trees += 1,
                        '#' => lumberyards += 1,
                        _ => unreachable!("invalid character found"),
                    }
                }
            }
            trace!("found {} open, {} trees, and {} lumberyards", open, trees, lumberyards);

            if let Some(&c) = landscape.get(key) {
                match c {
                    '.' => {
                        if trees >= 3 {
                            future.insert(*key, '|');
                        } else {
                            future.insert(*key, '.');
                        }
                    },
                    '|' => {
                        if lumberyards >= 3 {
                            future.insert(*key, '#');
                        } else {
                            future.insert(*key, '|');
                        }
                    },
                    '#' => {
                        if lumberyards >= 1 && trees >= 1 {
                            future.insert(*key, '#');
                        } else {
                            future.insert(*key, '.');
                        }
                    },
                    _ => unreachable!("invalid character found"),
                }
            }
        }
    }

    future
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let input: [&str; 1] = [
            ".#.#...|#.
.....#|##|
.|..|...#.
..|#.....#
#.#|||#|#|
...#.||...
.|....|...
||...#|.#|
|.||||..|.
...#.|..|.",
        ];
        let expected: [i64; 1] = [
            1147,
        ];

        for i in 0..input.len() {
            let input = parse_char_grid(input[i].to_string());
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
            let input = parse_char_grid(input[i].to_string());
            assert_eq!(solve_part_2(&input), expected[i]);
        }
    }
}
