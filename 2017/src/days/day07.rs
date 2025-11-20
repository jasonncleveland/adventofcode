use std::collections::{HashMap, HashSet};
use std::time::Instant;

use log::debug;

pub fn solve(file_contents: String) -> (String, String) {
    let parse_timer = Instant::now();
    let input = parse_input(file_contents);
    debug!("File parse: ({:?})", parse_timer.elapsed());

    let part1_timer = Instant::now();
    let part1 = solve_part_1(&input);
    debug!("Part 1: {} ({:?})", part1, part1_timer.elapsed());

    let part2_timer = Instant::now();
    let part2 = solve_part_2(&input, part1.to_owned());
    debug!("Part 2: {} ({:?})", part2, part2_timer.elapsed());

    (part1.to_string(), part2.to_string())
}

fn parse_input(file_contents: String) -> HashMap<String, Program> {
    let mut programs: HashMap<String, Program> = HashMap::new();
    for line in file_contents.lines() {
        let splits = line.split(" -> ").collect::<Vec<&str>>();
        if let Some(p) = splits.first()
            && let Some((name, w)) = p.split_once(' ')
            && let Ok(weight) = w.replace("(", "").replace(")", "").parse::<i64>()
        {
            if splits.len() > 1
                && let Some(c) = splits.last()
            {
                let mut children: Vec<String> = Vec::new();
                for child_name in c.split(", ") {
                    programs
                        .entry(child_name.to_string())
                        .and_modify(|p| p.parent = Some(name.to_string()))
                        .or_insert(Program {
                            name: child_name.to_string(),
                            weight: 0,
                            parent: Some(name.to_string()),
                            children: Vec::new(),
                        });
                    children.push(child_name.to_string());
                }
                programs
                    .entry(name.to_string())
                    .and_modify(|p| {
                        p.weight = weight;
                        p.children = children.to_owned();
                    })
                    .or_insert(Program {
                        name: name.to_string(),
                        weight,
                        parent: None,
                        children: children.to_owned(),
                    });
            } else {
                programs
                    .entry(name.to_string())
                    .and_modify(|p| p.weight = weight)
                    .or_insert(Program {
                        name: name.to_string(),
                        weight,
                        parent: None,
                        children: Vec::new(),
                    });
            }
        }
    }
    programs
}

fn solve_part_1(input: &HashMap<String, Program>) -> String {
    if let Some((_, program)) = input.iter().find(|(_, v)| v.parent.is_none()) {
        return program.name.to_owned();
    }
    String::new()
}

fn solve_part_2(input: &HashMap<String, Program>, root: String) -> i64 {
    let mut corrected_weight = 0;
    recursive_traverse(input, root, &mut corrected_weight);
    corrected_weight
}

fn recursive_traverse(
    programs: &HashMap<String, Program>,
    name: String,
    corrected_weight: &mut i64,
) -> i64 {
    if let Some(program) = programs.get(&name) {
        if program.children.is_empty() {
            return program.weight;
        }

        let mut child_weights: Vec<(i64, String)> = Vec::new();
        for child in &program.children {
            child_weights.push((
                recursive_traverse(programs, child.to_owned(), corrected_weight),
                child.to_owned(),
            ));
            if *corrected_weight != 0 {
                return 0;
            }
        }
        let mut total_child_weight = 0;
        let mut unique_weights: HashSet<i64> = HashSet::new();
        let mut weights: HashMap<i64, Vec<String>> = HashMap::new();
        for (child_weight, child_name) in child_weights {
            total_child_weight += child_weight;
            unique_weights.insert(child_weight);
            weights
                .entry(child_weight)
                .and_modify(|v| v.push(child_name.to_owned()))
                .or_insert(vec![child_name.to_owned()]);
        }
        if unique_weights.len() > 1
            && let Some((correct_weight, _)) = weights.iter().find(|(_, w)| w.len() == 2)
            && let Some((incorrect_weight, child_names)) =
                weights.iter().find(|(_, w)| w.len() == 1)
            && let Some(incorrect_program_name) = child_names.first()
            && let Some(incorrect_program) = programs.get(incorrect_program_name)
        {
            let delta = correct_weight - incorrect_weight;
            *corrected_weight = incorrect_program.weight + delta;
            return incorrect_program.weight + delta;
        }
        return program.weight + total_child_weight;
    }
    unreachable!();
}

#[derive(Clone, Debug)]
struct Program {
    name: String,
    weight: i64,
    parent: Option<String>,
    children: Vec<String>,
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let data: [(&str, &str); 1] = [(
            "pbga (66)
xhth (57)
ebii (61)
havc (66)
ktlj (57)
fwft (72) -> ktlj, cntj, xhth
qoyq (66)
padx (45) -> pbga, havc, qoyq
tknk (41) -> ugml, padx, fwft
jptl (61)
ugml (68) -> gyxo, ebii, jptl
gyxo (61)
cntj (57)",
            "tknk",
        )];

        for (input, expected) in data {
            let input = parse_input(input.to_string());
            assert_eq!(solve_part_1(&input), expected);
        }
    }

    #[test]
    fn test_part_2() {
        let data: [(&str, i64); 1] = [(
            "pbga (66)
xhth (57)
ebii (61)
havc (66)
ktlj (57)
fwft (72) -> ktlj, cntj, xhth
qoyq (66)
padx (45) -> pbga, havc, qoyq
tknk (41) -> ugml, padx, fwft
jptl (61)
ugml (68) -> gyxo, ebii, jptl
gyxo (61)
cntj (57)",
            60,
        )];

        for (input, expected) in data {
            let input = parse_input(input.to_string());
            assert_eq!(solve_part_2(&input, "tknk".to_string()), expected);
        }
    }
}
