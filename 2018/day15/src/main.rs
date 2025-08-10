use std::collections::{HashMap, HashSet, VecDeque};
use std::env;
use std::fs;
use std::time;

fn main() {
    let args: Vec<String> = env::args().collect();
    if args.len() < 2 {
        panic!("Must pass filename as argument");
    }

    let input_timer = time::Instant::now();
    let file_name = &args[1];
    let file_contents = read_file(file_name);
    println!("File read: ({:?})", input_timer.elapsed());

    let part1_timer = time::Instant::now();
    let part1 = part1(&file_contents);
    println!("Part 1: {} ({:?})", part1, part1_timer.elapsed());

    let part2_timer = time::Instant::now();
    let part2 = part2(&file_contents);
    println!("Part 2: {} ({:?})", part2, part2_timer.elapsed());
}

fn read_file(file_name: &str) -> String {
    fs::read_to_string(file_name)
        .expect("Something went wrong reading the file")
}

fn parse_input(file_contents: &str) -> (HashMap<Coordinate, char>, HashMap<Coordinate, Unit>) {
    let mut battlefield: HashMap<Coordinate, char> = HashMap::new();
    let mut units: HashMap<Coordinate, Unit> = HashMap::new();

    let lines = file_contents.lines();

    for (row_index, row) in lines.enumerate() {
        for (column_index, column) in row.chars().enumerate() {
            let coordinate = (row_index as i8, column_index as i8);
            match column {
                'G' | 'E' => {
                    units.insert(coordinate, Unit {
                        position: coordinate,
                        faction: column,
                        hp: 200,
                        power: 3,
                    });
                    battlefield.insert(coordinate, column);
                },
                other => {
                    battlefield.insert(coordinate, other);
                },
            };
        }
    }

    (battlefield, units)
}

fn part1(file_contents: &str) -> i64 {
    let (mut battlefield, mut units) = parse_input(file_contents);

    simulate_battle(&mut battlefield, &mut units, 3)
}

fn part2(file_contents: &str) -> i64 {
    let (battlefield, units) = parse_input(file_contents);
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

fn print_battlefield(battlefield: &HashMap<Coordinate, char>) {
    let mut keys = battlefield.keys().collect::<Vec<&Coordinate>>();
    keys.sort();

    let mut row = 0;
    for key in keys {
        if key.0 != row {
            row = key.0;
            println!();
        }
        print!("{}", battlefield[key]);
    }
    println!();
}

fn print_units(units: &HashMap<Coordinate, Unit>) {
    let mut keys = units.keys().collect::<Vec<&Coordinate>>();
    keys.sort();

    for key in keys {
        println!("{:?}", units[key]);
    }
}

fn simulate_battle(battlefield: &mut HashMap<Coordinate, char>, units: &mut HashMap<Coordinate, Unit>, elf_power: i64) -> i64 {
    // Set the attack power for all elf units
    units.iter_mut().filter(|(_, u)| u.faction == 'E').for_each(|(_, u)| u.power = elf_power);

    let mut round = 1;
    loop {
        if let Some(result) = process_round(battlefield, units) && result {
            // If a unit is unable to find a move then the last completed round is the previous round
            if let Some(hp_remaining) = units.values().map(|u| u.hp).reduce(|a, b| a + b) {
                return (round - 1) * hp_remaining;
            }
        }
        round += 1;
    }
}

fn process_round(battlefield: &mut HashMap<Coordinate, char>, units: &mut HashMap<Coordinate, Unit>) -> Option<bool> {
    let units_copy = units.clone();
    let mut coordinates = units_copy.keys().collect::<Vec<&Coordinate>>();
    // Sort the units by position so that the top-left-most unit is first
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

fn get_neighbouring_coordinates(coordinate: Coordinate) -> Vec<Coordinate> {
    vec![
        (coordinate.0 - 1, coordinate.1), // up
        (coordinate.0, coordinate.1 - 1), // left
        (coordinate.0, coordinate.1 + 1), // right
        (coordinate.0 + 1, coordinate.1), // down
    ]
}

fn in_range_of_target(battlefield: &HashMap<Coordinate, char>, unit: &Unit) -> bool {
    let neighbours: Vec<Coordinate> = get_neighbouring_coordinates(unit.position);
    for coordinate in neighbours {
        if let Some(next_tile) = battlefield.get(&coordinate) && *next_tile == get_opposite_faction(unit.faction) {
            return true;
        }
    }
    false
}

fn find_target(battlefield: &HashMap<Coordinate, char>, unit: &Unit) -> Option<Coordinate> {
    let mut targets: HashMap<Coordinate, Vec<Coordinate>> = HashMap::new();

    let mut queue: VecDeque<(Coordinate, Coordinate, u8)> = VecDeque::new();
    let mut visited: HashSet<Coordinate> = HashSet::new();

    queue.push_back((unit.position, (0, 0), 0));
    visited.insert(unit.position);

    let mut max_path = u8::MAX;
    while let Some(current) = queue.pop_front() {
        if current.2 > max_path {
            // If we have already found the shortest path length to a target then cull any longer paths
            continue;
        }

        let neighbours: Vec<Coordinate> = get_neighbouring_coordinates(current.0);
        for coordinate in neighbours {
            if !visited.contains(&coordinate) && let Some(next_tile) = battlefield.get(&coordinate) {
                if *next_tile == get_opposite_faction(unit.faction) {
                    // If the current tile is an opponent, add the previous tile to the list of possible targets
                    match targets.contains_key(&current.0) {
                        true => {
                            targets.get_mut(&current.0)?.push(current.1);
                        },
                        false => {
                            targets.insert(current.0, vec![current.1]);
                        }
                    }
                    max_path = current.2;
                } else if *next_tile == '.' {
                    // Store the first move to help calculate the best move
                    let path_origin = match current.1 {
                        (0, 0) => coordinate,
                        origin => origin,
                    };
                    visited.insert(coordinate);
                    queue.push_back((coordinate, path_origin, current.2 + 1));
                }
            }
        }
    }

    // Find the closest target with ties broken by reading order
    let targets_clone = targets.clone();
    let mut possible_targets = targets_clone.keys().collect::<Vec<&Coordinate>>();
    possible_targets.sort();

    if let Some(first_target) = possible_targets.first() && let Some(possible_moves) = targets.get_mut(first_target) {
        // If there are multiple paths to the target then take the first one in reading order
        possible_moves.sort();
        if let Some(first_coordinate) = possible_moves.first() {
            return Some(*first_coordinate);
        }
    }
    None
}

fn get_prioritized_target(battlefield: &HashMap<Coordinate, char>, units: &HashMap<Coordinate, Unit>, unit: &Unit) -> Option<Coordinate> {
    let mut targets: Vec<&Unit> = Vec::new();

    let neighbours: Vec<Coordinate> = get_neighbouring_coordinates(unit.position);
    for coordinate in neighbours {
        if let Some(next_tile) = battlefield.get(&coordinate) && *next_tile == get_opposite_faction(unit.faction) {
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
    position: Coordinate,
    faction: char,
    hp: i64,
    power: i64,
}

type Coordinate = (i8, i8);

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part1() {
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
        let expected: [i64; 6] = [
            27730,
            36334,
            39514,
            27755,
            28944,
            18740,
        ];

        for i in 0..input.len() {
            assert_eq!(part1(input[i]), expected[i]);
        }
    }

    #[test]
    fn test_part2() {
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
        let expected: [i64; 6] = [
            4988,
            29064,
            31284,
            3478,
            6474,
            1140,
        ];

        for i in 0..input.len() {
            assert_eq!(part2(input[i]), expected[i]);
        }
    }
}
