use std::collections::{HashMap, HashSet};
use std::time::Instant;

use aoc_helpers::io::parse_char_grid;
use aoc_helpers::point2d::Point2d;
use aoc_helpers::point3d::Point3d;
use log::{debug, trace};

pub fn solve(file_contents: &str) -> (String, String) {
    let parse_timer = Instant::now();
    let input = parse_char_grid(file_contents);
    debug!("File parse: ({:?})", parse_timer.elapsed());

    let part1_timer = Instant::now();
    let part1 = solve_part_1(&input);
    debug!("Part 1: {} ({:?})", part1, part1_timer.elapsed());

    let part2_timer = Instant::now();
    let part2 = solve_part_2(&input, 200);
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
                    let value = match c {
                        '#' => 1,
                        '.' => 0,
                        _ => unreachable!(),
                    };
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

fn solve_part_2(input: &HashMap<Point2d, char>, cycles: i64) -> i64 {
    let mut area: HashMap<i64, Vec<Vec<char>>> = HashMap::new();

    let mut level = vec![vec!['.'; 5]; 5];
    for (&point, &value) in input {
        level[point.y as usize][point.x as usize] = value;
    }
    area.insert(0, level);

    let mut min_level = 0;
    let mut max_level = 0;
    let mut total = 0;
    for cycle in 0..cycles {
        // Add extra levels every two cycles
        if cycle % 2 == 0 {
            min_level -= 1;
            area.insert(min_level, vec![vec!['.'; 5]; 5]);
            max_level += 1;
            area.insert(max_level, vec![vec!['.'; 5]; 5]);
        }
        total = process_round_expanded(&mut area, min_level, max_level);
    }
    total
}

fn process_round(area: &mut HashMap<Point2d, char>) {
    let mut future: HashMap<Point2d, char> = HashMap::new();

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
                    future.insert(point, '.');
                } else if c == '.' && (adjacent_bugs == 1 || adjacent_bugs == 2) {
                    // Area becomes infested if exactly 1 or 2 bugs are adjacent
                    future.insert(point, '#');
                } else {
                    // Remains the same
                    future.insert(point, c);
                }
            }
        }
    }

    for (point, c) in future {
        area.insert(point, c);
    }
}

fn process_round_expanded(
    area: &mut HashMap<i64, Vec<Vec<char>>>,
    min_level: i64,
    max_level: i64,
) -> i64 {
    let mut future: HashMap<i64, Vec<Vec<char>>> = HashMap::new();

    for z in min_level..=max_level {
        trace!("checking level {}", z);
        let mut next = vec![vec!['.'; 5]; 5];
        if let Some(level) = area.get(&z) {
            for y in 0..5 {
                for x in 0..5 {
                    // Ignore middle point
                    if x == 2 && y == 2 {
                        continue;
                    }

                    let point = Point3d::new(x as i64, y as i64, z);
                    trace!("checking point: {}", point);

                    // Get neighbours and count bugs
                    let mut adjacent_bugs = 0;
                    for neighbour in get_neighbours(&point) {
                        trace!("checking neighbour {} of {}", point, neighbour);
                        if let Some(level) = area.get(&neighbour.z)
                            && level[neighbour.y as usize][neighbour.x as usize] == '#'
                        {
                            adjacent_bugs += 1;
                        }
                    }
                    trace!("found bugs adjacent to {}: {}", point, adjacent_bugs);

                    let current_value = level[y][x];
                    if current_value == '#' {
                        // Bug dies unless there is exactly one other bug adjacent
                        if adjacent_bugs == 1 {
                            next[y][x] = '#';
                        } else {
                            next[y][x] = '.';
                        }
                    } else if current_value == '.' {
                        // Area becomes infested if exactly 1 or 2 bugs are adjacent
                        if adjacent_bugs == 1 || adjacent_bugs == 2 {
                            next[y][x] = '#';
                        } else {
                            next[y][x] = '.';
                        }
                    } else {
                        unreachable!("we should not reach here but panic if we do");
                    }
                }
            }
        }
        future.insert(z, next);
    }

    let mut total = 0;
    for (z, level) in future {
        for row in &level {
            for &c in row {
                if c == '#' {
                    total += 1;
                }
            }
        }
        area.insert(z, level);
    }
    total
}

fn get_neighbours(point: &Point3d) -> Vec<Point3d> {
    // Inner edges
    if point.x == 2 && point.y == 1 {
        // (2, 1)
        trace!("found top inner edge z: {} -> {}", point.z, point.z - 1);
        vec![
            // 5 top neighbours on lower level
            Point3d::new(0, 0, point.z + 1),
            Point3d::new(1, 0, point.z + 1),
            Point3d::new(2, 0, point.z + 1),
            Point3d::new(3, 0, point.z + 1),
            Point3d::new(4, 0, point.z + 1),
            // Left
            Point3d::new(point.x - 1, point.y, point.z),
            // Right
            Point3d::new(point.x + 1, point.y, point.z),
            // Up
            Point3d::new(point.x, point.y - 1, point.z),
        ]
    } else if point.x == 2 && point.y == 3 {
        // (2, 3)
        trace!("found bottom inner edge z: {} -> {}", point.z, point.z - 1);
        vec![
            // 5 bottom neighbours on lower level
            Point3d::new(0, 4, point.z + 1),
            Point3d::new(1, 4, point.z + 1),
            Point3d::new(2, 4, point.z + 1),
            Point3d::new(3, 4, point.z + 1),
            Point3d::new(4, 4, point.z + 1),
            // Left
            Point3d::new(point.x - 1, point.y, point.z),
            // Right
            Point3d::new(point.x + 1, point.y, point.z),
            // Down
            Point3d::new(point.x, point.y + 1, point.z),
        ]
    } else if point.x == 1 && point.y == 2 {
        // (1, 2)
        trace!("found left inner edge z: {} -> {}", point.z, point.z - 1);
        vec![
            // 5 right neighbours on lower level
            Point3d::new(0, 0, point.z + 1),
            Point3d::new(0, 1, point.z + 1),
            Point3d::new(0, 2, point.z + 1),
            Point3d::new(0, 3, point.z + 1),
            Point3d::new(0, 4, point.z + 1),
            // Left
            Point3d::new(point.x - 1, point.y, point.z),
            // Up
            Point3d::new(point.x, point.y - 1, point.z),
            // Down
            Point3d::new(point.x, point.y + 1, point.z),
        ]
    } else if point.x == 3 && point.y == 2 {
        // (3, 2)
        trace!("found right inner edge z: {} -> {}", point.z, point.z - 1);
        vec![
            // 5 left neighbours on lower level
            Point3d::new(4, 0, point.z + 1),
            Point3d::new(4, 1, point.z + 1),
            Point3d::new(4, 2, point.z + 1),
            Point3d::new(4, 3, point.z + 1),
            Point3d::new(4, 4, point.z + 1),
            // Right
            Point3d::new(point.x + 1, point.y, point.z),
            // Up
            Point3d::new(point.x, point.y - 1, point.z),
            // Down
            Point3d::new(point.x, point.y + 1, point.z),
        ]
    // Outer corners
    } else if point.x == 0 && point.y == 0 {
        // (0, 0)
        trace!("found top left corner z: {} -> {}", point.z, point.z + 1);
        vec![
            // Left on upper level
            Point3d::new(1, 2, point.z - 1),
            // Up on upper level
            Point3d::new(2, 1, point.z - 1),
            // Right
            Point3d::new(point.x + 1, point.y, point.z),
            // Down
            Point3d::new(point.x, point.y + 1, point.z),
        ]
    } else if point.x == 4 && point.y == 0 {
        // (4, 0)
        trace!("found top right corner z: {} -> {}", point.z, point.z + 1);
        vec![
            // Right on upper level
            Point3d::new(3, 2, point.z - 1),
            // Up on upper level
            Point3d::new(2, 1, point.z - 1),
            // Left
            Point3d::new(point.x - 1, point.y, point.z),
            // Down
            Point3d::new(point.x, point.y + 1, point.z),
        ]
    } else if point.x == 0 && point.y == 4 {
        // (0, 4)
        trace!("found bottom left corner z: {} -> {}", point.z, point.z + 1);
        vec![
            // Left on upper level
            Point3d::new(1, 2, point.z - 1),
            // Down on upper level
            Point3d::new(2, 3, point.z - 1),
            // Right
            Point3d::new(point.x + 1, point.y, point.z),
            // Up
            Point3d::new(point.x, point.y - 1, point.z),
        ]
    } else if point.x == 4 && point.y == 4 {
        // (4, 4)
        trace!(
            "found bottom right corner z: {} -> {}",
            point.z,
            point.z + 1
        );
        vec![
            // Right on upper level
            Point3d::new(3, 2, point.z - 1),
            // Down on upper level
            Point3d::new(2, 3, point.z - 1),
            // Left
            Point3d::new(point.x - 1, point.y, point.z),
            // Up
            Point3d::new(point.x, point.y - 1, point.z),
        ]
    // Outer edges
    } else if point.x == 0 && point.y > 0 && point.y < 4 {
        trace!("found left outer edge z: {} -> {}", point.z, point.z + 1);
        vec![
            // Left on upper level
            Point3d::new(1, 2, point.z - 1),
            // Right
            Point3d::new(point.x + 1, point.y, point.z),
            // Up
            Point3d::new(point.x, point.y - 1, point.z),
            // Down
            Point3d::new(point.x, point.y + 1, point.z),
        ]
    } else if point.x == 4 && point.y > 0 && point.y < 4 {
        // (0, 0)
        trace!("found right outer edge z: {} -> {}", point.z, point.z + 1);
        vec![
            // Right on upper level
            Point3d::new(3, 2, point.z - 1),
            // Left
            Point3d::new(point.x - 1, point.y, point.z),
            // Up
            Point3d::new(point.x, point.y - 1, point.z),
            // Down
            Point3d::new(point.x, point.y + 1, point.z),
        ]
    } else if point.y == 0 && point.x > 0 && point.x < 4 {
        trace!("found top outer edge z: {} -> {}", point.z, point.z + 1);
        vec![
            // Top on upper level
            Point3d::new(2, 1, point.z - 1),
            // Left
            Point3d::new(point.x - 1, point.y, point.z),
            // Right
            Point3d::new(point.x + 1, point.y, point.z),
            // Down
            Point3d::new(point.x, point.y + 1, point.z),
        ]
    } else if point.y == 4 && point.x > 0 && point.x < 4 {
        trace!("found bottom outer edge z: {} -> {}", point.z, point.z + 1);
        vec![
            // Bottom on upper level
            Point3d::new(2, 3, point.z - 1),
            // Left
            Point3d::new(point.x - 1, point.y, point.z),
            // Right
            Point3d::new(point.x + 1, point.y, point.z),
            // Up
            Point3d::new(point.x, point.y - 1, point.z),
        ]
    // Inner squares
    } else {
        vec![
            // Left
            Point3d::new(point.x - 1, point.y, point.z),
            // Right
            Point3d::new(point.x + 1, point.y, point.z),
            // Up
            Point3d::new(point.x, point.y - 1, point.z),
            // Down
            Point3d::new(point.x, point.y + 1, point.z),
        ]
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let input: [&str; 1] = ["....#
#..#.
#..##
..#..
#...."];
        let expected: [i64; 1] = [2129920];

        for i in 0..input.len() {
            let input = parse_char_grid(input[i]);
            assert_eq!(solve_part_1(&input), expected[i]);
        }
    }

    #[test]
    fn test_part_2() {
        let input: [&str; 1] = ["....#
#..#.
#..##
..#..
#...."];
        let expected: [i64; 1] = [99];

        for i in 0..input.len() {
            let input = parse_char_grid(input[i]);
            assert_eq!(solve_part_2(&input, 10), expected[i]);
        }
    }
}
