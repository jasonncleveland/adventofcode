use std::collections::HashMap;
use std::env;
use std::fs;
use std::time;

fn main() {
    let args: Vec<String> = env::args().collect();
    if args.len() < 2 {
        panic!("Must pass filename as argument");
    }

    let mut workers = 5usize;
    if args.len() >= 3 {
        workers = args[2].parse::<usize>().unwrap();
    }
    let mut delay = 60u8;
    if args.len() >= 4 {
        delay = args[3].parse::<u8>().unwrap();
    }

    let input_timer = time::Instant::now();
    let file_name = &args[1];
    let file_contents = read_file(file_name);
    println!("File read: ({:?})", input_timer.elapsed());

    let part1_timer = time::Instant::now();
    let part1 = part1(&file_contents);
    println!("Part 1: {} ({:?})", part1, part1_timer.elapsed());

    let part2_timer = time::Instant::now();
    let part2 = part2(&file_contents, workers, delay);
    println!("Part 2: {} ({:?})", part2, part2_timer.elapsed());
}

fn read_file(file_name: &str) -> String {
    fs::read_to_string(file_name)
        .expect("Something went wrong reading the file")
}

fn parse_input(file_contents: &str) -> HashMap<char, Vec<char>> {
    let mut incoming: HashMap<char, Vec<char>> = HashMap::new();
    for line in file_contents.lines() {
        let chars = line.chars().collect::<Vec<char>>();
        incoming.entry(chars[5]).or_default();
        incoming.entry(chars[36]).or_default().push(chars[5]);
    }
    incoming
}

fn part1(file_contents: &str) -> String {
    let mut incoming: HashMap<char, Vec<char>> = parse_input(file_contents);

    sort_steps(&mut incoming)
}

fn part2(file_contents: &str, workers: usize, delay: u8) -> i64 {
    let mut incoming: HashMap<char, Vec<char>> = parse_input(file_contents);

    sort_steps_multi(&mut incoming, workers, delay)
}

fn sort_steps(incoming: &mut HashMap<char, Vec<char>>) -> String {
    let mut result: String = String::new();

    loop {
        // Janky topological sort attempt
        let next_step = get_next_step(incoming);
        if next_step == 'x' {
            break;
        }
        result.push(next_step);
        remove_contains_node(incoming, &next_step);
    }

    result
}

fn sort_steps_multi(incoming: &mut HashMap<char, Vec<char>>, workers: usize, delay: u8) -> i64 {
    let mut result: String = String::new();

    let mut pool: Vec<Process> = Vec::with_capacity(workers);
    for _ in 0..workers {
        pool.push(Process {
            ticks_remaining: 0,
            process: 'x'
        });
    }

    let mut tick = 0i64;
    let mut jobs = incoming.keys().len();
    loop {
        if jobs == 0 {
            break;
        }

        // Assign steps
        for worker in pool.iter_mut() {
            // Attempt to assign step to worker if they have no current process
            if worker.ticks_remaining == 0 {
                let next_available_step = get_next_step(incoming);
                if next_available_step != 'x' {
                    let duration = next_available_step as u8 - b'A' + 1 + delay;
                    worker.ticks_remaining = duration;
                    worker.process = next_available_step;
                }
            }
        }

        // Progress steps
        for worker in pool.iter_mut() {
            // Process one tick of the step
            if worker.process != 'x' {
                worker.ticks_remaining -= 1;
                if worker.ticks_remaining == 0 {
                    // Mark node as completed
                    remove_contains_node(incoming, &worker.process);
                    result.push(worker.process);
                    worker.process = 'x';
                    jobs -= 1;
                }
            }
        }

        tick += 1;
    }

    tick
}

fn get_next_step(incoming: &mut HashMap<char, Vec<char>>) -> char {
    let incoming_clone = incoming.clone();
    let mut empty_nodes = get_empty_nodes(&incoming_clone);
    // Default to alphabetical order if there are multiple nodes
    empty_nodes.sort();
    if empty_nodes.is_empty() {
        return 'x';
    }

    let result = empty_nodes.first();
    if result.is_none() {
        return 'x';
    }
    let char = result.unwrap();
    remove_node(incoming, char);
    **char
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

#[derive(Debug)]
struct Process {
    ticks_remaining: u8,
    process: char,
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
            "Step C must be finished before step A can begin.
Step C must be finished before step F can begin.
Step A must be finished before step B can begin.
Step A must be finished before step D can begin.
Step B must be finished before step E can begin.
Step D must be finished before step E can begin.
Step F must be finished before step E can begin.",
        ];
        let expected: [i64; 1] = [
            15,
        ];

        for i in 0..input.len() {
            assert_eq!(part2(input[i], 2, 0), expected[i]);
        }
    }
}
