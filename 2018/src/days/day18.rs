use std::collections::HashMap;
use std::mem::swap;
use std::time::Instant;

use aoc_helpers::io::parse_char_vec;
use log::{debug, trace};

pub fn solve(file_contents: String) -> (String, String) {
    let parse_timer = Instant::now();
    let input = parse_char_vec(file_contents);
    debug!("File parse: ({:?})", parse_timer.elapsed());

    let part1_timer = Instant::now();
    let part1 = solve_part_1(&input);
    debug!("Part 1: {} ({:?})", part1, part1_timer.elapsed());

    let part2_timer = Instant::now();
    let part2 = solve_part_2(&input);
    debug!("Part 2: {} ({:?})", part2, part2_timer.elapsed());

    (part1.to_string(), part2.to_string())
}

fn solve_part_1(input: &[Vec<char>]) -> i64 {
    let size = input.len();
    let mut current: Vec<Vec<char>> = vec![vec!['.'; size]; size];
    for y in 0..size {
        for x in 0..size {
            current[y][x] = input[y][x];
        }
    }
    let mut future: Vec<Vec<char>> = vec![vec!['.'; size]; size];

    for _ in 0..10 {
        simulate_landscape(&current, &mut future, size);
        swap(&mut current, &mut future);
    }

    let mut trees = 0;
    let mut lumberyards = 0;
    for row in current {
        for value in row {
            match value {
                '|' => trees += 1,
                '#' => lumberyards += 1,
                _ => continue,
            }
        }
    }

    trees * lumberyards
}

fn solve_part_2(input: &[Vec<char>]) -> i64 {
    let size = input.len();
    let mut current: Vec<Vec<char>> = vec![vec!['.'; size]; size];
    for y in 0..size {
        for x in 0..size {
            current[y][x] = input[y][x];
        }
    }
    let mut future: Vec<Vec<char>> = vec![vec!['.'; size]; size];
    let mut states: HashMap<(i64, i64, i64), (usize, usize)> = HashMap::new();

    const MAX: usize = 1_000_000_000;
    let mut minute = 0;
    loop {
        simulate_landscape(&current, &mut future, size);
        swap(&mut current, &mut future);
        minute += 1;
        let mut open = 0;
        let mut trees = 0;
        let mut lumberyards = 0;
        for row in &current {
            for &value in row {
                match value {
                    '.' => open += 1,
                    '|' => trees += 1,
                    '#' => lumberyards += 1,
                    _ => continue,
                }
            }
        }

        // Store the last time this state was seen and the delta between when it was last seen
        let key = (open, trees, lumberyards);
        if let Some((last_seen, delta)) = states.get(&key) {
            // We have seen the same state twice after the same number of states
            if minute - last_seen == *delta {
                trace!("found loop every {} steps", delta);
                let remaining = MAX - minute;
                let cycles = remaining / delta;
                let offset = remaining % delta;
                trace!("{remaining} steps remaining resulting in {cycles} cycles and {offset} extra iterations");

                // Process the remaining minutes needed to reach 1,000,000,000
                for _ in 0..offset {
                    simulate_landscape(&current, &mut future, size);
                    swap(&mut current, &mut future);
                }

                trees = 0;
                lumberyards = 0;
                for row in &current {
                    for &value in row {
                        match value {
                            '|' => trees += 1,
                            '#' => lumberyards += 1,
                            _ => continue,
                        }
                    }
                }
                return trees * lumberyards;
            }
            states.insert(key, (minute, minute - last_seen));
        } else {
            states.insert(key, (minute, 0));
        }
    }
}

fn simulate_landscape(current: &[Vec<char>], future: &mut [Vec<char>], size: usize) {

    for y in 0..size {
        for x in 0..size {
            let mut trees = 0;
            let mut lumberyards = 0;

            // Up left
            if x > 0 && y > 0 {
                match current[y - 1][x - 1] {
                    '|' => trees += 1,
                    '#' => lumberyards += 1,
                    _ => (),
                }
            }
            // Up
            if y > 0 {
                match current[y - 1][x] {
                    '|' => trees += 1,
                    '#' => lumberyards += 1,
                    _ => (),
                }
            }
            // Up right
            if y > 0 && x < size - 1 {
                match current[y - 1][x + 1] {
                    '|' => trees += 1,
                    '#' => lumberyards += 1,
                    _ => (),
                }
            }
            // Left
            if x > 0 {
                match current[y][x - 1] {
                    '|' => trees += 1,
                    '#' => lumberyards += 1,
                    _ => (),
                }
            }
            // Right
            if x < size - 1 {
                match current[y][x + 1] {
                    '|' => trees += 1,
                    '#' => lumberyards += 1,
                    _ => (),
                }
            }
            // Down left
            if y < size - 1 && x > 0 {
                match current[y + 1][x - 1] {
                    '|' => trees += 1,
                    '#' => lumberyards += 1,
                    _ => (),
                }
            }
            // Down
            if y < size - 1 {
                match current[y + 1][x] {
                    '|' => trees += 1,
                    '#' => lumberyards += 1,
                    _ => (),
                }
            }
            // Down right
            if y < size - 1 && x < size - 1 {
                match current[y + 1][x + 1] {
                    '|' => trees += 1,
                    '#' => lumberyards += 1,
                    _ => (),
                }
            }

            match current[y][x] {
                '.' => {
                    if trees >= 3 {
                        future[y][x] = '|';
                    } else {
                        future[y][x] = '.';
                    }
                },
                '|' => {
                    if lumberyards >= 3 {
                        future[y][x] = '#';
                    } else {
                        future[y][x] = '|';
                    }
                },
                '#' => {
                    if lumberyards >= 1 && trees >= 1 {
                        future[y][x] = '#';
                    } else {
                        future[y][x] = '.';
                    }
                },
                _ => unreachable!("invalid character found"),
            }
        }
    }
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
            let input = parse_char_vec(input[i].to_string());
            assert_eq!(solve_part_1(&input), expected[i]);
        }
    }
}
