use std::collections::{HashMap, HashSet};
use std::time::Instant;

use aoc_helpers::io::parse_int_list;
use aoc_helpers::point3d::Point3d;
use log::debug;

pub fn solve(file_contents: &str) -> (String, String) {
    let parse_timer = Instant::now();
    let input = parse_input(file_contents);
    debug!("File parse: ({:?})", parse_timer.elapsed());

    let part1_timer = Instant::now();
    let part1 = solve_part_1(&input, 1000);
    debug!("Part 1: {} ({:?})", part1, part1_timer.elapsed());

    let part2_timer = Instant::now();
    let part2 = solve_part_2(&input);
    debug!("Part 2: {} ({:?})", part2, part2_timer.elapsed());

    (part1.to_string(), part2.to_string())
}

fn parse_input(file_contents: &str) -> Vec<Connection> {
    let mut coordinates: Vec<Point3d> = Vec::new();

    for line in file_contents.lines() {
        let numbers = parse_int_list(line, ',');
        if let Some(&x) = numbers.first()
            && let Some(&y) = numbers.get(1)
            && let Some(&z) = numbers.get(2)
        {
            coordinates.push(Point3d::new(x, y, z));
        }
    }

    let mut connections: Vec<Connection> = Vec::new();

    // Calculate the distances of connections between all junction boxes
    for source_index in 0..coordinates.len() {
        // Start the target index at the next entry after the source to avoid duplicate entries
        for target_index in source_index + 1..coordinates.len() {
            if let Some(source) = coordinates.get(source_index)
                && let Some(target) = coordinates.get(target_index)
            {
                let distance = source.euclidean(target);
                connections.push(Connection {
                    distance,
                    source: *source,
                    target: *target,
                });
            }
        }
    }
    // Sort the connections in descending order by distance
    connections.sort_by(|a, b| a.distance.total_cmp(&b.distance));

    connections
}

fn solve_part_1(connections: &Vec<Connection>, iterations: usize) -> i64 {
    let mut circuits: HashMap<Point3d, i64> = HashMap::new();
    let mut unique_circuit_id = 1;
    for connection in connections {
        if circuits
            .insert(connection.source, unique_circuit_id)
            .is_some()
        {
            unique_circuit_id += 1;
        }
        if circuits
            .insert(connection.target, unique_circuit_id)
            .is_some()
        {
            unique_circuit_id += 1;
        }
    }

    // Iterate over X number of connections
    for Connection { source, target, .. } in connections.iter().take(iterations) {
        // Combine the junction boxes into a single circuit unless they are already connected
        if let Some(&source_id) = circuits.get(source)
            && let Some(&target_id) = circuits.get(target)
            && source_id != target_id
        {
            // Assign the source id to all junction boxes in the target circuit
            circuits
                .iter_mut()
                .filter(|(_, v)| **v == target_id)
                .for_each(|(_, v)| *v = source_id);
        }
    }

    // Find the circuit sizes
    let mut circuit_sizes: HashMap<i64, i64> = HashMap::new();
    for (_, v) in circuits {
        circuit_sizes.entry(v).and_modify(|v| *v += 1).or_insert(1);
    }

    let mut sizes: Vec<i64> = circuit_sizes.into_values().collect();
    sizes.sort_by(|a, b| b.cmp(a));

    // Return the product of the 3 largest circuit sizes
    sizes.iter().take(3).product()
}

fn solve_part_2(connections: &Vec<Connection>) -> i64 {
    let mut circuits: HashMap<Point3d, i64> = HashMap::new();
    let mut unique_circuit_id = 1;
    for connection in connections {
        if circuits
            .insert(connection.source, unique_circuit_id)
            .is_some()
        {
            unique_circuit_id += 1;
        }
        if circuits
            .insert(connection.target, unique_circuit_id)
            .is_some()
        {
            unique_circuit_id += 1;
        }
    }

    // Iterate over all connections
    for Connection { source, target, .. } in connections {
        // Combine the junction boxes into a single circuit unless they are already connected
        if let Some(&source_id) = circuits.get(source)
            && let Some(&target_id) = circuits.get(target)
            && source_id != target_id
        {
            // Assign the source id to all junction boxes in the target circuit
            circuits
                .iter_mut()
                .filter(|(_, v)| **v == target_id)
                .for_each(|(_, v)| *v = source_id);
        }

        // Break when all junction boxes are connected
        if circuits.values().collect::<HashSet<_>>().len() == 1 {
            return source.x * target.x;
        }
    }
    unreachable!();
}

#[derive(Debug)]
struct Connection {
    source: Point3d,
    target: Point3d,
    distance: f64,
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let data: [(&str, i64); 1] = [(
            "162,817,812
57,618,57
906,360,560
592,479,940
352,342,300
466,668,158
542,29,236
431,825,988
739,650,466
52,470,668
216,146,977
819,987,18
117,168,530
805,96,715
346,949,466
970,615,88
941,993,340
862,61,35
984,92,344
425,690,689",
            40,
        )];

        for (input, expected) in data {
            let input = parse_input(input);
            assert_eq!(solve_part_1(&input, 10), expected);
        }
    }

    #[test]
    fn test_part_2() {
        let data: [(&str, i64); 1] = [(
            "162,817,812
57,618,57
906,360,560
592,479,940
352,342,300
466,668,158
542,29,236
431,825,988
739,650,466
52,470,668
216,146,977
819,987,18
117,168,530
805,96,715
346,949,466
970,615,88
941,993,340
862,61,35
984,92,344
425,690,689",
            25272,
        )];

        for (input, expected) in data {
            let input = parse_input(input);
            assert_eq!(solve_part_2(&input), expected);
        }
    }
}
