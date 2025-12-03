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

fn solve_part_1(grid: &HashMap<Point2d, char>) -> String {
    let mut found_characters = String::new();
    if let Some((&start_position, _)) = grid.iter().find(|(k, v)| k.y == 0 && **v == '|') {
        let mut current_direction = Direction::Down;

        let mut queue: VecDeque<Point2d> = VecDeque::new();
        let mut visited: HashSet<Point2d> = HashSet::new();

        queue.push_back(start_position);
        visited.insert(start_position);

        'outer: while let Some(current_position) = queue.pop_front() {
            if let Some(&current_value) = grid.get(&current_position)
                && current_value.is_ascii_uppercase()
            {
                found_characters.push(current_value);
            }

            let next_position = current_position.next(&current_direction);
            if let Some(&next_value) = grid.get(&next_position)
                && next_value != ' '
            {
                visited.insert(next_position);
                queue.push_back(next_position);
            } else {
                // Attempt to turn left and right to search for the next valid space
                let directions = vec![Direction::Right, Direction::Left];
                for direction in directions {
                    let turn_direction = current_direction.next(&direction);
                    let turn_position = current_position.next(&turn_direction);
                    if let Some(&turn_value) = grid.get(&turn_position)
                        && turn_value != ' '
                    {
                        current_direction = turn_direction;
                        visited.insert(turn_position);
                        queue.push_back(turn_position);
                        continue 'outer;
                    }
                }
            }
        }
    }
    found_characters
}

fn solve_part_2(grid: &HashMap<Point2d, char>) -> i64 {
    let mut steps = 0;
    if let Some((&start_position, _)) = grid.iter().find(|(k, v)| k.y == 0 && **v == '|') {
        let mut current_direction = Direction::Down;

        let mut queue: VecDeque<Point2d> = VecDeque::new();
        let mut visited: HashSet<Point2d> = HashSet::new();

        queue.push_back(start_position);
        visited.insert(start_position);

        'outer: while let Some(current_position) = queue.pop_front() {
            steps += 1;

            let next_position = current_position.next(&current_direction);
            if let Some(&next_value) = grid.get(&next_position)
                && next_value != ' '
            {
                visited.insert(next_position);
                queue.push_back(next_position);
            } else {
                // Attempt to turn left and right to search for the next valid space
                let directions = vec![Direction::Right, Direction::Left];
                for direction in directions {
                    let turn_direction = current_direction.next(&direction);
                    let turn_position = current_position.next(&turn_direction);
                    if let Some(&turn_value) = grid.get(&turn_position)
                        && turn_value != ' '
                    {
                        current_direction = turn_direction;
                        visited.insert(turn_position);
                        queue.push_back(turn_position);
                        continue 'outer;
                    }
                }
            }
        }
    }
    steps
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let data: [(&str, &str); 1] = [(
            "     |
     |  +--+
     A  |  C
 F---|----E|--+
     |  |  |  D
     +B-+  +--+
",
            "ABCDEF",
        )];

        for (input, expected) in data {
            let input = parse_char_grid(input);
            assert_eq!(solve_part_1(&input), expected);
        }
    }

    #[test]
    fn test_part_2() {
        let data: [(&str, i64); 1] = [(
            "     |
     |  +--+
     A  |  C
 F---|--|-E---+
     |  |  |  D
     +B-+  +--+
",
            38,
        )];

        for (input, expected) in data {
            let input = parse_char_grid(input);
            assert_eq!(solve_part_2(&input), expected);
        }
    }
}
