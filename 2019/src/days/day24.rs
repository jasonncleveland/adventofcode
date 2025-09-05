use std::collections::{HashMap, HashSet};
use std::time::Instant;

use log::{debug, trace};

use crate::shared::io::parse_char_grid;
use crate::shared::point2d::Point2d;

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
    let mut area = input.clone();
    let mut seen: HashSet<i64> = HashSet::new();

    loop {
        process_round(&mut area);

        // Generate a bit mask to store the seen state
        let mut bit_mask = 0;
        for y in 0..5 {
            for x in 0..5 {
                if let Some(c) = area.get(&Point2d::new(x, y)) {
                    let shift = y * 5 + x;
                    let value = match c { '#' => 1, '.' => 0, _ => unreachable!() };
                    bit_mask += value << shift;
                }
            }
        }

        if seen.contains(&bit_mask) {
            trace!("found repeated state: {:b} {}", bit_mask, bit_mask);
            return bit_mask;
        }
        seen.insert(bit_mask);
    }
}

fn solve_part_2(input: &HashMap<Point2d, char>) -> i64 {
    unimplemented!()
}

fn process_round(area: &mut HashMap<Point2d, char>) {
    let mut future = [[0; 5]; 5];

    for y in 0..5 {
        for x in 0..5 {
            let point = Point2d::new(x as i64, y as i64);
            if let Some(&c) = area.get(&point) {
                let mut adjacent_bugs = 0;
                for neighbour in point.neighbours() {
                    match area.get(&neighbour) {
                        Some('#') => adjacent_bugs += 1,
                        Some('.') | None => continue,
                        _ => unreachable!(),
                    }
                }

                if c == '#' && adjacent_bugs != 1 {
                    // Bug dies unless there is exactly one other bug adjacent
                    future[y][x] = 0;
                } else if c == '.' && (adjacent_bugs == 1 || adjacent_bugs == 2) {
                    // Area becomes infested if exactly 1 or 2 bugs are adjacent
                    future[y][x] = 1;
                } else {
                    // Remains the same
                    future[y][x] = match c {
                        '#' => 1,
                        '.' => 0,
                        _ => unreachable!()
                    };
                }
            }
        }
    }

    for y in 0..5 {
        for x in 0..5 {
            let point = Point2d::new(x as i64, y as i64);
            area.insert(point, match future[y][x] {
                0 => '.',
                1 => '#',
                _ => unreachable!()
            });
        }
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let input: [&str; 1] = [
            "....#
#..#.
#..##
..#..
#....",
        ];
        let expected: [i64; 1] = [
            2129920,
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
