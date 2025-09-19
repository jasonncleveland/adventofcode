use std::collections::{HashMap, HashSet, VecDeque};

use log::debug;

use std::time::Instant;

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

fn parse_input(file_contents: String) -> HashMap<String, Planet> {
    let mut result: HashMap<String, Planet> = HashMap::new();
    for line in file_contents.lines() {
        if let Some((orbits, planet_name)) = line.split_once(')') {
            if !result.contains_key(planet_name) {
                result.insert(
                    planet_name.to_string(),
                    Planet {
                        name: planet_name.to_string(),
                        orbits: None,
                        orbited_by: vec![],
                    },
                );
            }
            if let Some(planet) = result.get_mut(planet_name) {
                planet.orbits = Some(orbits.to_string());
            }
            if !result.contains_key(orbits) {
                result.insert(
                    orbits.to_string(),
                    Planet {
                        name: orbits.to_string(),
                        orbits: None,
                        orbited_by: vec![],
                    },
                );
            }
            if let Some(planet) = result.get_mut(orbits) {
                planet.orbited_by.push(planet_name.to_string());
            }
        }
    }
    result
}

fn solve_part_1(input: &HashMap<String, Planet>) -> i64 {
    let mut queue: VecDeque<(String, i64)> = VecDeque::new();

    queue.push_back(("COM".to_string(), 1));

    let mut total = 0;
    while let Some(current) = queue.pop_front() {
        if !input.contains_key(&current.0) {
            continue;
        }

        if let Some(planet) = input.get(&current.0) {
            for orbit in planet.orbited_by.iter() {
                total += current.1;
                queue.push_back((orbit.clone(), current.1 + 1));
            }
        }
    }
    total
}

fn solve_part_2(input: &HashMap<String, Planet>) -> i64 {
    let start = input["YOU"].clone();
    let end = "SAN".to_string();

    let mut queue: VecDeque<(Planet, i64)> = VecDeque::new();
    let mut visited: HashSet<String> = HashSet::new();

    if let Some(orbits) = start.orbits.as_ref() {
        queue.push_back((input[orbits].clone(), 0));
        visited.insert(orbits.clone());
    }

    while let Some(current) = queue.pop_front() {
        if current.0.orbited_by.contains(&end) {
            return current.1;
        }

        if let Some(planet) = input.get(&current.0.name) {
            // Check plant we are orbiting
            if let Some(orbits) = planet.orbits.as_ref()
                && !visited.contains(orbits)
            {
                visited.insert(orbits.clone());
                queue.push_back((input[orbits].clone(), current.1 + 1));
            }
            // Check planets that are orbiting
            for orbit in planet.orbited_by.iter() {
                if !visited.contains(orbit) {
                    visited.insert(orbit.clone());
                    queue.push_back((input[orbit].clone(), current.1 + 1));
                }
            }
        }
    }
    unreachable!();
}

#[derive(Clone, Debug)]
struct Planet {
    // TODO: Might not need name
    name: String,
    orbits: Option<String>,
    orbited_by: Vec<String>,
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let input: [&str; 1] = ["COM)B
B)C
C)D
D)E
E)F
B)G
G)H
D)I
E)J
J)K
K)L"];
        let expected: [i64; 1] = [42];

        for i in 0..input.len() {
            let parsed = parse_input(input[i].to_string());
            assert_eq!(solve_part_1(&parsed), expected[i]);
        }
    }

    #[test]
    fn test_part_2() {
        let input: [&str; 1] = ["COM)B
B)C
C)D
D)E
E)F
B)G
G)H
D)I
E)J
J)K
K)L
K)YOU
I)SAN"];
        let expected: [i64; 1] = [4];

        for i in 0..input.len() {
            let parsed = parse_input(input[i].to_string());
            assert_eq!(solve_part_2(&parsed), expected[i]);
        }
    }
}
