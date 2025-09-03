use std::collections::{HashMap, HashSet, VecDeque};
use std::time::Instant;

use log::{debug, trace};

use crate::shared::graph::{Edge, Node};
use crate::shared::io::parse_char_grid;
use crate::shared::point2d::Point2d;
use crate::shared::priority_queue::{PriorityQueue, PriorityQueueItem};

pub fn solve(file_contents: String) -> (String, String) {
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

fn convert_to_graph(grid: &HashMap<Point2d, char>) -> HashMap<char, Node> {
    let mut graph: HashMap<char, Node> = HashMap::new();
    let nodes: Vec<(&Point2d, &char)> = grid.iter().filter(|(_, v)| **v != '#' && **v != '.').collect();
    for (origin, &name) in nodes {
        trace!("searching for nodes reachable from {} {}", name, origin);
        let mut reachable_nodes: Vec<Edge> = Vec::new();

        let mut queue: VecDeque<(Point2d, i64)> = VecDeque::new();
        let mut visited: HashSet<Point2d> = HashSet::new();

        queue.push_back((origin.to_owned(), 0));
        visited.insert(origin.to_owned());

        while let Some((coordinate, steps)) = queue.pop_front() {
            for neighbour in coordinate.neighbours() {
                if !visited.contains(&neighbour) {
                    // in bounds and not seen
                    visited.insert(neighbour);
                    match grid.get(&neighbour) {
                        None => continue,
                        Some('#') => continue,
                        // Ignore start position(s) when generating node pairs
                        Some('.') | Some('@') | Some('1') | Some('2') | Some('3') | Some('4') => {
                            queue.push_back((neighbour, steps + 1));
                        },
                        Some(target) => {
                            trace!("Found node {} at {} with distance {} from {}", target, neighbour, steps + 1, name);
                            reachable_nodes.push(Edge::new(*target, steps + 1));
                        }
                    }
                }
            }
        }

        graph.insert(name, Node::new(reachable_nodes));
    }
    graph
}

fn find_reachable_nodes(graph: &HashMap<char, Node>, start: char, keys: &Vec<char>) -> Vec<Edge> {
    trace!("finding nodes reachable from {} with keys {:?}", start, keys);

    let mut queue: VecDeque<(char, i64)> = VecDeque::new();
    let mut visited: HashSet<char> = HashSet::new();

    queue.push_back((start.to_owned(), 0));
    visited.insert(start.to_owned());

    let mut reachable: Vec<Edge> = Vec::new();
    while let Some((name, steps)) = queue.pop_front() {
        trace!("current: {} ({})", name, steps);

        trace!("edges: {:?}", graph.get(&name));
        if let Some(node) = graph.get(&name) {
            for edge in &node.edges {
                if visited.contains(&edge.name) {
                    continue;
                }
                visited.insert(edge.name.to_owned());

                trace!("found edge: {:?}", edge);
                if edge.name.is_ascii_lowercase() {
                    trace!("found key {}", edge.name);
                    if keys.contains(&edge.name) {
                        trace!("we already collected this key");
                        // Continue to next nodes
                        queue.push_back((edge.name.to_owned(), steps + edge.weight));
                    } else {
                        trace!("we have not collected this key");
                        // Add to reachable
                        reachable.push(Edge::new(edge.name, steps + edge.weight));
                    }
                }
                if edge.name.is_ascii_uppercase() {
                    trace!("found door {}", edge.name);
                    if keys.contains(&edge.name.to_ascii_lowercase()) {
                        trace!("we already unlocked this door");
                        // Continue to next nodes
                        queue.push_back((edge.name.to_owned(), steps + edge.weight));
                    } else {
                        trace!("we cannot unlock this door");
                        // Add to reachable
                        reachable.push(Edge::new(edge.name, steps + edge.weight));
                    }
                }
            }
        }
    }
    trace!("reachable nodes from {}: {:?}", start, reachable);
    reachable
}

fn solve_part_1(input: &HashMap<Point2d, char>) -> i64 {
    let graph = convert_to_graph(input);

    let key_count = graph.keys().filter(|c| c.is_ascii_lowercase()).count();

    #[derive(Eq, PartialEq)]
    struct Data {
        name: char,
        keys: Vec<char>,
    }

    let mut queue: PriorityQueue<PriorityQueueItem<Data>> = PriorityQueue::new();
    let mut min_steps: HashMap<(char, Vec<char>), i64> = HashMap::new();

    queue.push(PriorityQueueItem::new(0, Data { name: '@', keys: Vec::with_capacity(key_count) }));

    while let Some(PriorityQueueItem { weight: steps, data: Data { name, keys }}) = queue.pop() {
        trace!("checking node {} with keys {:?} ({}/{}) after {} steps", name, keys, keys.len(), key_count, steps);

        if keys.len() == key_count {
            trace!("collected all {} keys after {} steps", key_count, steps);
            return steps;
        }

        // There can be multiple ways to get to a given node. We need to ensure we've found the shortest
        let steps_key = (name, keys.clone());
        if let Some(&distance) = min_steps.get(&steps_key) && distance <= steps {
            trace!("we have seen this exact state {:?} so it can be skipped", steps_key);
            continue;
        }
        min_steps.insert(steps_key, steps);

        for edge in find_reachable_nodes(&graph, name, &keys) {
            let mut keys_copy = keys.clone();
            if edge.name.is_ascii_lowercase() {
                trace!("found key {}", edge.name);
                if !keys_copy.contains(&edge.name) {
                    trace!("collecting key {} after {} steps", edge.name, steps + edge.weight);
                    keys_copy.push(edge.name);
                    keys_copy.sort();
                }
                trace!("moving from node {} to {} with weight of {}", name, edge.name, edge.weight);
                queue.push(PriorityQueueItem::new(steps + edge.weight, Data { name: edge.name, keys: keys_copy }));
            } else if edge.name.is_ascii_uppercase() {
                trace!("found door {}", edge.name);
                if keys_copy.contains(&edge.name.to_ascii_lowercase()) {
                    trace!("unlocking door {} after {} steps", edge.name, steps + edge.weight);
                    trace!("moving from node {} to {} with weight of {}", name, edge.name, edge.weight);
                    queue.push(PriorityQueueItem::new(steps + edge.weight, Data { name: edge.name, keys: keys_copy }));
                }
            } else {
                panic!("invalid node {}", edge.name);
            }
        }
    }
    unreachable!();
}

fn solve_part_2(grid: &HashMap<Point2d, char>) -> i64 {
    let mut updated_grid = grid.clone();
    let mut robot_starting_positions = Vec::with_capacity(4);

    if let Some((start, _)) = grid.iter().find(|(_,v)| **v == '@') {
        //         ...      1#2
        // Replace .@. with ###
        //         ...      3#4
        updated_grid.insert(Point2d::new(start.x, start.y), '#');
        updated_grid.insert(Point2d::new(start.x - 1, start.y), '#');
        updated_grid.insert(Point2d::new(start.x + 1, start.y), '#');
        updated_grid.insert(Point2d::new(start.x, start.y - 1), '#');
        updated_grid.insert(Point2d::new(start.x, start.y + 1), '#');
        updated_grid.insert(Point2d::new(start.x - 1, start.y - 1), '1');
        updated_grid.insert(Point2d::new(start.x + 1, start.y - 1), '2');
        updated_grid.insert(Point2d::new(start.x - 1, start.y + 1), '3');
        updated_grid.insert(Point2d::new(start.x + 1, start.y + 1), '4');

        robot_starting_positions = vec!['1', '2', '3', '4'];
    }

    let graph = convert_to_graph(&updated_grid);

    let key_count = graph.keys().filter(|c| c.is_ascii_lowercase()).count();

    #[derive(Eq, PartialEq)]
    struct Data {
        robot_positions: Vec<char>,
        keys: Vec<char>,
    }

    let mut queue: PriorityQueue<PriorityQueueItem<Data>> = PriorityQueue::new();
    let mut min_steps: HashMap<(Vec<char>, Vec<char>), i64> = HashMap::new();

    queue.push(PriorityQueueItem::new(0, Data { robot_positions: robot_starting_positions.to_owned(), keys: Vec::with_capacity(key_count) }));

    while let Some(PriorityQueueItem { weight: steps, data: Data { robot_positions, keys }}) = queue.pop() {
        trace!("checking robot positions {:?} with keys {:?} ({}/{}) after {} steps", robot_positions, keys, keys.len(), key_count, steps);

        if keys.len() == key_count {
            trace!("collected all {} keys after {} steps", key_count, steps);
            return steps;
        }

        // There can be multiple ways to get to a given node. We need to ensure we've found the shortest
        let steps_key = (robot_positions.clone(), keys.clone());
        if let Some(&distance) = min_steps.get(&steps_key) && distance <= steps {
            trace!("we have seen this exact state {:?} so it can be skipped", steps_key);
            continue;
        }
        min_steps.insert(steps_key, steps);

        for (i, &robot_position) in robot_positions.iter().enumerate() {
            trace!("attempting to move robot {} at node {}", i, robot_position);

            for edge in find_reachable_nodes(&graph, robot_position, &keys) {
                trace!("checking movement from {} to {} ({})", robot_position, edge.name, edge.weight);
                let mut copied_robot_positions = robot_positions.clone();
                copied_robot_positions[i] = edge.name;

                let mut keys_copy = keys.clone();
                if edge.name.is_ascii_lowercase() {
                    trace!("found key {}", edge.name);
                    if !keys_copy.contains(&edge.name) {
                        trace!("collecting key {} after {} steps", edge.name, steps + edge.weight);
                        keys_copy.push(edge.name);
                        keys_copy.sort();
                    }
                    trace!("moving from node {} to {} with weight of {}", robot_position, edge.name, edge.weight);
                    queue.push(PriorityQueueItem::new(steps + edge.weight, Data { robot_positions: copied_robot_positions.to_owned(), keys: keys_copy }));
                } else if edge.name.is_ascii_uppercase() {
                    trace!("found door {}", edge.name);
                    if keys_copy.contains(&edge.name.to_ascii_lowercase()) {
                        trace!("unlocking door {} after {} steps", edge.name, steps + edge.weight);
                        trace!("moving from node {} to {} with weight of {}", robot_position, edge.name, edge.weight);
                        queue.push(PriorityQueueItem::new(steps + edge.weight, Data { robot_positions: copied_robot_positions.to_owned(), keys: keys_copy }));
                    }
                } else {
                    panic!("invalid node {}", edge.name);
                }
            }
        }
    }
    unreachable!();
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
            let input = parse_char_grid(input[i].to_string());
            assert_eq!(solve_part_1(&input), expected[i]);
        }
    }

    #[test]
    fn test_part_2() {
        let input: [&str; 4] = [
            "#######
#a.#Cd#
##...##
##.@.##
##...##
#cB#Ab#
#######",
            "###############
#d.ABC.#.....a#
######...######
######.@.######
######...######
#b.....#.....c#
###############",
            "#############
#DcBa.#.GhKl#
#.###...#I###
#e#d#.@.#j#k#
###C#...###J#
#fEbA.#.FgHi#
#############",
            "#############
#g#f.D#..h#l#
#F###e#E###.#
#dCba...BcIJ#
#####.@.#####
#nK.L...G...#
#M###N#H###.#
#o#m..#i#jk.#
#############"
        ];
        let expected: [i64; 4] = [
            8,
            24,
            32,
            72,
        ];

        for i in 0..input.len() {
            let input = parse_char_grid(input[i].to_string());
            assert_eq!(solve_part_2(&input), expected[i]);
        }
    }
}
