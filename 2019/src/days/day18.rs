use std::collections::{HashMap, HashSet, VecDeque};
use std::time::Instant;

use log::debug;

use crate::shared::io::parse_char_grid;
use crate::shared::point2d::Point2d;

pub fn solve(file_contents: String) -> (String, String) {
    let parse_timer = Instant::now();
    let (grid, start) = parse_char_grid(file_contents.clone(), '@');
    debug!("File parse: ({:?})", parse_timer.elapsed());

    let part1_timer = Instant::now();
    let part1 = solve_part_1(&grid, &start);
    debug!("Part 1: {} ({:?})", part1, part1_timer.elapsed());

    let part2_timer = Instant::now();
    let part2 = solve_part_2(&grid, &start);
    debug!("Part 2: {} ({:?})", part2, part2_timer.elapsed());

    (part1.to_string(), part2.to_string())
}

fn solve_part_1(grid: &HashMap<Point2d, char>, start: &Point2d) -> i64 {
    let key_count = grid.values().filter(|c| c.is_ascii_lowercase()).count();

    let mut queue: VecDeque<(Point2d, i64, Vec<char>)> = VecDeque::new();
    let mut visited: HashSet<(Point2d, Vec<char>)> = HashSet::new();

    queue.push_back((start.to_owned(), 0, Vec::with_capacity(key_count)));
    visited.insert((start.to_owned(), Vec::with_capacity(key_count)));

    while let Some((position, steps, keys)) = queue.pop_front() {

        if keys.len() == key_count {
            return steps;
        }

        for next_position in position.neighbours() {
            if visited.contains(&(next_position.to_owned(), keys.clone())) {
                continue;
            }

            match grid.get(&next_position) {
                None => continue,
                Some('#') => continue,
                Some('.') => {
                    queue.push_back((next_position, steps + 1, keys.clone()));
                    visited.insert((next_position, keys.clone()));
                },
                Some(value) => {
                    if value.is_ascii_uppercase() {
                        if keys.contains(&value.to_ascii_lowercase()) {
                            queue.push_back((next_position, steps + 1, keys.clone()));
                            visited.insert((next_position, keys.clone()));
                        }
                    }
                    if value.is_ascii_lowercase() {
                        let mut next_keys = keys.clone();
                        if !next_keys.contains(value) {
                            next_keys.push(*value);
                        }
                        next_keys.sort();
                        queue.push_back((next_position, steps + 1, next_keys.clone()));
                        visited.insert((next_position, next_keys.clone()));
                    }
                },
            }
        }
    }
    unreachable!();
}

fn solve_part_2(grid: &HashMap<Point2d, char>, start: &Point2d) -> i64 {
    -1
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let input: [&str; 5] = [
            "#########
#b.A.@.a#
#########",
            "########################
#f.D.E.e.C.b.A.@.a.B.c.#
######################.#
#d.....................#
########################",
            "########################
#...............b.C.D.f#
#.######################
#.....@.a.B.c.d.A.e.F.g#
########################",
            "#################
#i.G..c...e..H.p#
########.########
#j.A..b...f..D.o#
########@########
#k.E..a...g..B.n#
########.########
#l.F..d...h..C.m#
#################",
            "########################
#@..............ac.GI.b#
###d#e#f################
###A#B#C################
###g#h#i################
########################",
        ];
        let expected: [i64; 5] = [
            8,
            86,
            132,
            136,
            81,
        ];

        for i in 0..input.len() {
            let (grid, start) = parse_char_grid(input[i].to_string(), '@');
            assert_eq!(solve_part_1(&grid, &start), expected[i]);
        }
    }

    #[test]
    fn test_part_2() {
        let input: [&str; 1] = [
            "",
        ];
        let expected: [i64; 1] = [
            0
        ];

        for i in 0..input.len() {
            let (grid, start) = parse_char_grid(input[i].to_string(), '@');
            assert_eq!(solve_part_2(&grid, &start), expected[i]);
        }
    }
}
