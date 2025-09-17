use std::collections::{HashMap, HashSet, VecDeque};
use std::time::Instant;

use aoc_helpers::direction::{Direction, get_directions};
use aoc_helpers::point2d::Point2d;
use log::{debug, trace};

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

fn parse_input(file_contents: String) -> HashMap<Point2d, char> {
    let mut maze: HashMap<Point2d, char> = HashMap::new();
    let start = Point2d::new(0, 0);
    maze.insert(start, 'X');

    let directions = file_contents.chars().collect::<Vec<char>>();
    generate_maze_rec(&directions, &mut maze, start, &mut 0);

    maze
}

fn generate_maze_rec(directions: &[char], maze: &mut HashMap<Point2d, char>, coordinate: Point2d, index: &mut usize) {
    let mut current = coordinate;
    while *index < directions.len() {
        let c = directions[*index];
        *index += 1;

        if c == '(' {
            // We found a group so need to recurse into it
            generate_maze_rec(directions, maze, current, index);
        } else if c == ')' {
            // The group has ended so return to the parent group
            return;
        } else if c == '|' {
            // We found a new option so reset to the location at the start of the group
            current = coordinate;
        } else if c.is_ascii_uppercase() {
            // If we find a direction then move the coordinate through the door to the new location
            let direction = match c {
                'N' => &Direction::Up,
                'E' => &Direction::Right,
                'S' => &Direction::Down,
                'W' => &Direction::Left,
                _ => unreachable!(),
            };
            let door_shape = match c {
                'N' | 'S' => '-',
                'E' | 'W' => '|',
                _ => unreachable!(),
            };
            let door = current.next(direction);
            maze.insert(door, door_shape);
            let next = door.next(direction);
            maze.insert(next, '.');
            trace!("moving in direction {} from {} to {} through door at {}", direction, current, next, door);
            current = next;
        } else if c == '^' || c == '$' {
            continue;
        } else {
            unreachable!("invalid character found: {}", c);
        }
    }
}

fn solve_part_1(maze: &HashMap<Point2d, char>) -> i64 {
    let mut queue: VecDeque<(Point2d, i64)> = VecDeque::new();
    let mut visited: HashSet<Point2d> = HashSet::new();

    queue.push_back((Point2d::new(0, 0), 0));
    visited.insert(Point2d::new(0, 0));

    let mut max_doors = i64::MIN;
    while let Some((coordinate, distance)) = queue.pop_front() {
        trace!("checking location: {} doors passed: {}", coordinate, distance);

        if distance > max_doors {
            max_doors = distance;
        }

        for direction in get_directions() {
            trace!("checking direction: {}", direction);
            let door = coordinate.next(&direction);

            if maze.get(&door).is_none() {
                continue;
            }

            let neighbour = door.next(&direction);

            if visited.contains(&neighbour) {
                trace!("already checked neighbour at {}", neighbour);
                continue;
            }
            visited.insert(neighbour);

            if let Some(d) = maze.get(&door) && let Some(n) = maze.get(&door) {
                trace!("found door {} leading to neighbour {} at {}", d, n, neighbour);
                queue.push_back((neighbour, distance + 1));
            }
        }
    }
    max_doors
}

fn solve_part_2(maze: &HashMap<Point2d, char>) -> i64 {
    let mut long_paths = 0;

    let mut queue: VecDeque<(Point2d, i64)> = VecDeque::new();
    let mut visited: HashSet<Point2d> = HashSet::new();

    queue.push_back((Point2d::new(0, 0), 0));
    visited.insert(Point2d::new(0, 0));

    while let Some((coordinate, distance)) = queue.pop_front() {
        trace!("checking location: {} doors passed: {}", coordinate, distance);

        // If the number of doors passed through to get to this location is 1000 or more, increment
        if distance >= 1000 {
            long_paths += 1;
        }

        for direction in get_directions() {
            trace!("checking direction: {}", direction);
            let door = coordinate.next(&direction);

            if maze.get(&door).is_none() {
                continue;
            }

            let neighbour = door.next(&direction);

            if visited.contains(&neighbour) {
                trace!("already checked neighbour at {}", neighbour);
                continue;
            }
            visited.insert(neighbour);

            if let Some(d) = maze.get(&door) && let Some(n) = maze.get(&door) {
                trace!("found door {} leading to neighbour {} at {}", d, n, neighbour);
                queue.push_back((neighbour, distance + 1));
            }
        }
    }

    long_paths
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let input: [&str; 5] = [
            "^WNE$",
            "^ENWWW(NEEE|SSE(EE|N))$",
            "^ENNWSWW(NEWS|)SSSEEN(WNSE|)EE(SWEN|)NNN$",
            "^ESSWWN(E|NNENN(EESS(WNSE|)SSS|WWWSSSSE(SW|NNNE)))$",
            "^WSSEESWWWNW(S|NENNEEEENN(ESSSSW(NWSW|SSEN)|WSWWN(E|WWS(E|SS))))$",
        ];
        let expected: [i64; 5] = [
            3,
            10,
            18,
            23,
            31,
        ];

        for i in 0..input.len() {
            let input = parse_input(input[i].to_string());
            assert_eq!(solve_part_1(&input), expected[i]);
        }
    }
}
