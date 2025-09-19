use std::collections::{HashMap, HashSet, VecDeque};
use std::time::Instant;

use aoc_helpers::direction::Direction;
use aoc_helpers::point2d::Point2d;
use log::debug;

pub fn solve(file_contents: String) -> (String, String) {
    let parse_timer = Instant::now();
    let (battlefield, units) = parse_input(file_contents);
    debug!("File parse: ({:?})", parse_timer.elapsed());

    let part1_timer = Instant::now();
    let part1 = solve_part_1(&battlefield, &units);
    debug!("Part 1: {} ({:?})", part1, part1_timer.elapsed());

    let part2_timer = Instant::now();
    let part2 = solve_part_2(&battlefield, &units);
    debug!("Part 2: {} ({:?})", part2, part2_timer.elapsed());

    (part1.to_string(), part2.to_string())
}

fn parse_input(file_contents: String) -> (HashMap<Point2d, char>, HashMap<Point2d, Unit>) {
    let mut battlefield: HashMap<Point2d, char> = HashMap::new();
    let mut units: HashMap<Point2d, Unit> = HashMap::new();

    let lines = file_contents.lines();

    for (y, row) in lines.enumerate() {
        for (x, column) in row.chars().enumerate() {
            let coordinate = Point2d::new(x as i64, y as i64);
            match column {
                'G' | 'E' => {
                    units.insert(
                        coordinate,
                        Unit {
                            position: coordinate,
                            faction: column,
                            hp: 200,
                            power: 3,
                        },
                    );
                    battlefield.insert(coordinate, column);
                }
                other => {
                    battlefield.insert(coordinate, other);
                }
            };
        }
    }

    (battlefield, units)
}

fn solve_part_1(battlefield: &HashMap<Point2d, char>, units: &HashMap<Point2d, Unit>) -> i64 {
    let mut battlefield_copy = battlefield.clone();
    let mut units_copy = units.clone();

    simulate_battle(&mut battlefield_copy, &mut units_copy, 3)
}

fn solve_part_2(battlefield: &HashMap<Point2d, char>, units: &HashMap<Point2d, Unit>) -> i64 {
    let starting_elf_count = units.iter().filter(|(_, u)| u.faction == 'E').count();

    let mut elf_power = 4;
    let mut highest_elf_power_with_death = 3;
    let mut lowest_elf_power_without_death = 200;
    let mut results: HashMap<i64, i64> = HashMap::new();
    loop {
        let mut battlefield_copy = battlefield.clone();
        let mut units_copy = units.clone();
        let result = simulate_battle(&mut battlefield_copy, &mut units_copy, elf_power);
        results.insert(elf_power, result);

        // Use binary search to minimize the number of simulations performed
        let elf_count = units_copy.iter().filter(|(_, u)| u.faction == 'E').count();
        if elf_count == starting_elf_count {
            lowest_elf_power_without_death = elf_power;
        } else {
            highest_elf_power_with_death = elf_power;
        }
        let difference = lowest_elf_power_without_death - highest_elf_power_with_death;
        if difference == 0 || difference == 1 {
            // If the difference is 0 or 1 then we have found the correct value
            if let Some(result) = results.get(&lowest_elf_power_without_death) {
                return *result;
            }
        }
        elf_power = highest_elf_power_with_death + difference / 2;
    }
}

fn simulate_battle(
    battlefield: &mut HashMap<Point2d, char>,
    units: &mut HashMap<Point2d, Unit>,
    elf_power: i64,
) -> i64 {
    // Set the attack power for all elf units
    units
        .iter_mut()
        .filter(|(_, u)| u.faction == 'E')
        .for_each(|(_, u)| u.power = elf_power);

    let mut round = 1;
    loop {
        if let Some(result) = process_round(battlefield, units)
            && result
        {
            // If a unit is unable to find a move then the last completed round is the previous round
            if let Some(hp_remaining) = units.values().map(|u| u.hp).reduce(|a, b| a + b) {
                return (round - 1) * hp_remaining;
            }
        }
        round += 1;
    }
}

fn process_round(
    battlefield: &mut HashMap<Point2d, char>,
    units: &mut HashMap<Point2d, Unit>,
) -> Option<bool> {
    let units_copy = units.clone();
    let mut coordinates = units_copy.keys().collect::<Vec<&Point2d>>();
    // Sort the units by position so that the top-left-most unit is first
    // coordinates.sort_by(|a, b| a.cmp(b));
    coordinates.sort();

    for coordinate in coordinates {
        if let Some(result) = units.get(coordinate) {
            let mut unit = result.clone();
            // Movement phase
            if !in_range_of_target(battlefield, &unit) {
                if let Some(next_step) = find_target(battlefield, &unit) {
                    // Move towards the nearest target
                    battlefield.insert(next_step, unit.faction);
                    battlefield.insert(*coordinate, '.');
                    unit.position = next_step;
                    units.remove(coordinate);
                    units.insert(next_step, unit.clone());
                } else {
                    // If a unit cannot find a target, check and stop if all targets have been eliminated
                    let goblin_count = units.iter().filter(|(_, u)| u.faction == 'G').count();
                    let elf_count = units.iter().filter(|(_, u)| u.faction == 'E').count();
                    if elf_count == 0 || goblin_count == 0 {
                        return Some(true);
                    }
                }
            }

            // Attack phase
            if let Some(target_coordinate) = get_prioritized_target(battlefield, units, &unit) {
                let target = units.get_mut(&target_coordinate)?;
                target.hp -= unit.power;
                if target.hp <= 0 {
                    units.remove(&target_coordinate);
                    battlefield.insert(target_coordinate, '.');
                }
            }
        }
    }
    None
}

fn get_neighbouring_coordinates(coordinate: Point2d) -> Vec<Point2d> {
    vec![
        coordinate.next(&Direction::Up),    // up
        coordinate.next(&Direction::Left),  // left
        coordinate.next(&Direction::Right), // right
        coordinate.next(&Direction::Down),  // down
    ]
}

fn in_range_of_target(battlefield: &HashMap<Point2d, char>, unit: &Unit) -> bool {
    for coordinate in get_neighbouring_coordinates(unit.position) {
        if let Some(next_tile) = battlefield.get(&coordinate)
            && *next_tile == get_opposite_faction(unit.faction)
        {
            return true;
        }
    }
    false
}

fn find_target(battlefield: &HashMap<Point2d, char>, unit: &Unit) -> Option<Point2d> {
    let mut targets: HashMap<Point2d, Vec<Point2d>> = HashMap::new();

    let mut queue: VecDeque<(Point2d, Point2d, u8)> = VecDeque::new();
    let mut visited: HashSet<Point2d> = HashSet::new();

    queue.push_back((unit.position, Point2d::new(0, 0), 0));
    visited.insert(unit.position);

    let mut max_path = u8::MAX;
    while let Some((position, origin, path)) = queue.pop_front() {
        if path > max_path {
            // If we have already found the shortest path length to a target then cull any longer paths
            continue;
        }

        for coordinate in get_neighbouring_coordinates(position) {
            if !visited.contains(&coordinate)
                && let Some(next_tile) = battlefield.get(&coordinate)
            {
                if *next_tile == get_opposite_faction(unit.faction) {
                    // If the current tile is an opponent, add the previous tile to the list of possible targets
                    match targets.contains_key(&position) {
                        true => {
                            targets.get_mut(&position)?.push(origin);
                        }
                        false => {
                            targets.insert(position, vec![origin]);
                        }
                    }
                    max_path = path;
                } else if *next_tile == '.' {
                    // Store the first move to help calculate the best move
                    let path_origin = if origin == Point2d::new(0, 0) {
                        coordinate
                    } else {
                        origin
                    };
                    visited.insert(coordinate);
                    queue.push_back((coordinate, path_origin, path + 1));
                }
            }
        }
    }

    // Find the closest target with ties broken by reading order
    let targets_clone = targets.clone();
    let mut possible_targets = targets_clone.keys().collect::<Vec<&Point2d>>();
    // possible_targets.sort_by(|a, b| a.cmp(b));
    possible_targets.sort();

    if let Some(first_target) = possible_targets.first()
        && let Some(possible_moves) = targets.get_mut(first_target)
    {
        // If there are multiple paths to the target then take the first one in reading order
        // possible_moves.sort_by(|a, b| a.cmp(b));
        possible_moves.sort();
        if let Some(first_coordinate) = possible_moves.first() {
            return Some(*first_coordinate);
        }
    }
    None
}

fn get_prioritized_target(
    battlefield: &HashMap<Point2d, char>,
    units: &HashMap<Point2d, Unit>,
    unit: &Unit,
) -> Option<Point2d> {
    let mut targets: Vec<&Unit> = Vec::new();

    for coordinate in get_neighbouring_coordinates(unit.position) {
        if let Some(next_tile) = battlefield.get(&coordinate)
            && *next_tile == get_opposite_faction(unit.faction)
        {
            targets.push(units.get(&coordinate)?);
        }
    }

    if targets.is_empty() {
        return None;
    }

    // Sort targets by lowest hp then position
    targets.sort_by(|a, b| (a.hp, a.position).cmp(&(b.hp, b.position)));
    Some(targets.first()?.position)
}

fn get_opposite_faction(faction: char) -> char {
    match faction {
        'E' => 'G',
        'G' => 'E',
        _ => panic!("invalid faction"),
    }
}

#[derive(Clone, Debug)]
struct Unit {
    position: Point2d,
    faction: char,
    hp: i64,
    power: i64,
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let input: [&str; 6] = [
            "#######
#.G...#
#...EG#
#.#.#G#
#..G#E#
#.....#
#######",
            "#######
#G..#E#
#E#E.E#
#G.##.#
#...#E#
#...E.#
#######",
            "#######
#E..EG#
#.#G.E#
#E.##E#
#G..#.#
#..E#.#
#######",
            "#######
#E.G#.#
#.#G..#
#G.#.G#
#G..#.#
#...E.#
#######",
            "#######
#.E...#
#.#..G#
#.###.#
#E#G#G#
#...#G#
#######",
            "#########
#G......#
#.E.#...#
#..##..G#
#...##..#
#...#...#
#.G...G.#
#.....G.#
#########",
        ];
        let expected: [i64; 6] = [27730, 36334, 39514, 27755, 28944, 18740];

        for i in 0..input.len() {
            let (battlefield, units) = parse_input(input[i].to_string());
            assert_eq!(solve_part_1(&battlefield, &units), expected[i]);
        }
    }

    #[test]
    fn test_part_2() {
        let input: [&str; 6] = [
            "#######
#.G...#
#...EG#
#.#.#G#
#..G#E#
#.....#
#######",
            "#######
#G..#E#
#E#E.E#
#G.##.#
#...#E#
#...E.#
#######",
            "#######
#E..EG#
#.#G.E#
#E.##E#
#G..#.#
#..E#.#
#######",
            "#######
#E.G#.#
#.#G..#
#G.#.G#
#G..#.#
#...E.#
#######",
            "#######
#.E...#
#.#..G#
#.###.#
#E#G#G#
#...#G#
#######",
            "#########
#G......#
#.E.#...#
#..##..G#
#...##..#
#...#...#
#.G...G.#
#.....G.#
#########",
        ];
        let expected: [i64; 6] = [4988, 29064, 31284, 3478, 6474, 1140];

        for i in 0..input.len() {
            let (battlefield, units) = parse_input(input[i].to_string());
            assert_eq!(solve_part_2(&battlefield, &units), expected[i]);
        }
    }
}
