use std::collections::HashMap;
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

fn parse_input(file_contents: &str) -> Vec<(char, char)> {
    let mut edges: Vec<(char, char)> = Vec::new();
    for line in file_contents.lines() {
        let chars = line.chars().collect::<Vec<char>>();
        edges.push((chars[5], chars[36]));
    }
    edges
}

fn part1(file_contents: &str) -> String {
    let chars = parse_input(file_contents);

    let mut incoming: HashMap<char, Vec<char>> = HashMap::new();

    // Setup graph
    for char in chars {
        incoming.entry(char.0).or_default();
        incoming.entry(char.1).or_default().push(char.0);
    }

    sort_steps(&mut incoming)
}

fn part2(file_contents: &str) -> i64 {
    -1
}

fn sort_steps(incoming: &mut HashMap<char, Vec<char>>) -> String {
    let mut result: String = String::new();

    loop {
        // Janky topological sort attempt
        let incoming_clone = incoming.clone();
        let mut empty_nodes = get_empty_nodes(&incoming_clone);
        empty_nodes.sort();
        if empty_nodes.is_empty() {
            break;
        }

        let empty_node = empty_nodes.first().unwrap();
        result.push(**empty_node);
        remove_node(incoming, empty_node);
        remove_contains_node(incoming, empty_node);
    }

    result
}

fn get_empty_nodes(incoming: &HashMap<char, Vec<char>>) -> Vec<&char> {
    incoming
        .iter()
        .filter(|(_, v)| v.is_empty())
        .map(|(k, _)| k)
        .collect()
}

fn remove_node(graph: &mut HashMap<char, Vec<char>>, node: &char) {
    graph.remove(node);
}

fn remove_contains_node(graph: &mut HashMap<char, Vec<char>>, node: &char) {
    graph.iter_mut()
        .filter(|(_, v)| v.contains(node))
        .for_each(|(_, v)| { v.swap_remove(v.iter().position(|c| c == node ).unwrap()); });
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part1() {
        let input: [&str; 1] = [
            "Step C must be finished before step A can begin.
Step C must be finished before step F can begin.
Step A must be finished before step B can begin.
Step A must be finished before step D can begin.
Step B must be finished before step E can begin.
Step D must be finished before step E can begin.
Step F must be finished before step E can begin.",
        ];
        let expected: [&str; 1] = [
            "CABDFE",
        ];

        for i in 0..input.len() {
            assert_eq!(part1(input[i]), expected[i]);
        }
    }

    #[test]
    fn test_part2() {
        let input: [&str; 1] = [
            "",
        ];
        let expected: [i64; 1] = [
            0,
        ];

        for i in 0..input.len() {
            assert_eq!(part2(input[i]), expected[i]);
        }
    }
}
