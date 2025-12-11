use std::collections::HashMap;
use std::time::Instant;

use log::debug;

pub fn solve(file_contents: &str) -> (String, String) {
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

fn parse_input(file_contents: &str) -> HashMap<&str, Vec<&str>> {
    let mut machines: HashMap<&str, Vec<_>> = HashMap::new();

    for line in file_contents.lines() {
        if let Some((left, right)) = line.split_once(": ") {
            machines.insert(left, right.split_whitespace().collect::<Vec<_>>());
        }
    }

    machines
}

fn solve_part_1(input: &HashMap<&str, Vec<&str>>) -> i64 {
    find_path_rec(input, "you")
}

fn solve_part_2(input: &HashMap<&str, Vec<&str>>) -> i64 {
    let mut cache: HashMap<(&str, bool, bool), i64> = HashMap::new();
    find_path_with_required_nodes_rec(input, "svr", false, false, &mut cache)
}

fn find_path_rec(input: &HashMap<&str, Vec<&str>>, node: &str) -> i64 {
    if node == "out" {
        return 1;
    }

    let mut total = 0;

    if let Some(nodes) = input.get(node) {
        for next in nodes {
            total += find_path_rec(input, next);
        }
    }

    total
}

fn find_path_with_required_nodes_rec<'a>(
    input: &HashMap<&str, Vec<&'a str>>,
    node: &'a str,
    visited_dac: bool,
    visited_fft: bool,
    cache: &mut HashMap<(&'a str, bool, bool), i64>,
) -> i64 {
    if let Some(&cached) = cache.get(&(node, visited_dac, visited_fft)) {
        return cached;
    }

    if node == "out" && visited_dac && visited_fft {
        return 1;
    }

    let mut total = 0;
    if let Some(nodes) = input.get(node) {
        for &next in nodes {
            total += find_path_with_required_nodes_rec(
                input,
                next,
                visited_dac || next == "dac",
                visited_fft || next == "fft",
                cache,
            );
        }
    }

    cache.insert((node, visited_dac, visited_fft), total);
    total
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let data: [(&str, i64); 1] = [(
            "aaa: you hhh
you: bbb ccc
bbb: ddd eee
ccc: ddd eee fff
ddd: ggg
eee: out
fff: out
ggg: out
hhh: ccc fff iii
iii: out
",
            5,
        )];

        for (input, expected) in data {
            let input = parse_input(input);
            assert_eq!(solve_part_1(&input), expected);
        }
    }

    #[test]
    fn test_part_2() {
        let data: [(&str, i64); 1] = [(
            "svr: aaa bbb
aaa: fft
fft: ccc
bbb: tty
tty: ccc
ccc: ddd eee
ddd: hub
hub: fff
eee: dac
dac: fff
fff: ggg hhh
ggg: out
hhh: out
",
            2,
        )];

        for (input, expected) in data {
            let input = parse_input(input);
            assert_eq!(solve_part_2(&input), expected);
        }
    }
}
