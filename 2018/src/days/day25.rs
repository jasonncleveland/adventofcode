use std::collections::{HashMap, HashSet, VecDeque};
use std::time::Instant;

use aoc_helpers::graph::{Edge, Node};
use aoc_helpers::point4d::Point4d;
use log::{debug, trace};

pub fn solve(file_contents: String) -> (String, String) {
    let parse_timer = Instant::now();
    let input = parse_input(file_contents);
    debug!("File parse: ({:?})", parse_timer.elapsed());

    let part1_timer = Instant::now();
    let part1 = solve_part_1(&input);
    debug!("Part 1: {} ({:?})", part1, part1_timer.elapsed());

    (part1.to_string(), "Merry Christmas!".to_string())
}

fn parse_input(file_contents: String) -> Vec<Point4d> {
    let mut coordinates: Vec<Point4d> = Vec::new();
    for line in file_contents.lines() {
        let mut numbers: Vec<i64> = Vec::with_capacity(4);
        for value in line.split(',') {
            if let Ok(number) = value.parse::<i64>() {
                numbers.push(number);
            }
        }
        coordinates.push(Point4d::new(numbers[0], numbers[1], numbers[2], numbers[3]));
    }
    coordinates
}

fn solve_part_1(input: &[Point4d]) -> i64 {
    let mut graph: HashMap<Point4d, Node<Point4d>> = HashMap::new();
    for coordinate in input {
        let mut edges: Vec<Edge<Point4d>> = Vec::new();
        trace!("Checking coordinate: {}", coordinate);
        for other in input {
            if coordinate == other {
                continue;
            }

            let manhattan_distance = coordinate.manhattan(other);
            trace!("Checking distance from {} to: {} {}", coordinate, other, manhattan_distance);
            if manhattan_distance <= 3 {
                trace!("{} is connected to {}", coordinate, other);
                edges.push(Edge::new(*other, manhattan_distance));
            }
        }
        graph.insert(*coordinate, Node::new(edges));
    }

    let mut total = 0;
    let mut visited: HashSet<Point4d> = HashSet::new();
    for (origin, node) in &graph {
        trace!("Origin: {} Edges: {:?}", origin, node.edges);
        if visited.contains(origin) {
            continue;
        }
        visited.insert(*origin);
        total += 1;

        let mut queue: VecDeque<Point4d> = VecDeque::new();
        queue.push_back(*origin);

        while let Some(current) = queue.pop_front() {
            trace!("Checking {}", current);

            if let Some(node) = graph.get(&current) {
                for edge in &node.edges {
                    trace!("Found connected node {}", edge.value);
                    if visited.contains(&edge.value) {
                        continue;
                    }
                    visited.insert(edge.value);
                    queue.push_back(edge.value);
                }
            }
        }
    }
    total
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let input: [&str; 4] = [
            "0,0,0,0
3,0,0,0
0,3,0,0
0,0,3,0
0,0,0,3
0,0,0,6
9,0,0,0
12,0,0,0",
            "-1,2,2,0
0,0,2,-2
0,0,0,-2
-1,2,0,0
-2,-2,-2,2
3,0,2,-1
-1,3,2,2
-1,0,-1,0
0,2,1,-2
3,0,0,0",
            "1,-1,0,1
2,0,-1,0
3,2,-1,0
0,0,3,1
0,0,-1,-1
2,3,-2,0
-2,2,0,0
2,-2,0,-1
1,-1,0,-1
3,2,0,2",
            "1,-1,-1,-2
-2,-2,0,1
0,2,1,3
-2,3,-2,1
0,2,3,-2
-1,-1,1,-2
0,-2,-1,0
-2,2,3,-1
1,2,2,0
-1,-2,0,-2",
        ];
        let expected: [i64; 4] = [
            2,
            4,
            3,
            8,
        ];

        for i in 0..input.len() {
            let input = parse_input(input[i].to_string());
            assert_eq!(solve_part_1(&input), expected[i]);
        }
    }
}
