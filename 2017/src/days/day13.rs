use std::collections::HashMap;
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
    let part2 = solve_part_2(&input);
    debug!("Part 2: {} ({:?})", part2, part2_timer.elapsed());

    (part1.to_string(), part2.to_string())
}

fn parse_input(file_contents: String) -> HashMap<i64, i64> {
    let mut programs: HashMap<i64, i64> = HashMap::new();
    for line in file_contents.lines() {
        if let Some((left, right)) = line.split_once(": ")
            && let Ok(layer) = left.parse::<i64>()
            && let Ok(range) = right.parse::<i64>()
        {
            programs.insert(layer, range);
        }
    }
    programs
}

fn solve_part_1(input: &HashMap<i64, i64>) -> i64 {
    let mut total = 0;
    if let Some(&max_depth) = input.keys().max() {
        for layer in 0..=max_depth {
            if let Some(range) = input.get(&layer) {
                // The scanner cycles through its range so it will return to the top after range * 2 - 2 steps
                if layer % (range * 2 - 2) == 0 {
                    total += layer * range;
                }
            }
        }
    }
    total
}

fn solve_part_2(input: &HashMap<i64, i64>) -> i64 {
    let mut delay = 1;
    loop {
        let mut is_caught = false;
        for (layer, range) in input {
            // The scanner cycles through its range so it will return to the top after range * 2 - 2 steps
            if (delay + layer) % (range * 2 - 2) == 0 {
                is_caught = true;
                break;
            }
        }
        if !is_caught {
            return delay;
        }
        delay += 1;
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let data: [(&str, i64); 1] = [(
            "0: 3
1: 2
4: 4
6: 4",
            24,
        )];

        for (input, expected) in data {
            let input = parse_input(input.to_string());
            assert_eq!(solve_part_1(&input), expected);
        }
    }

    #[test]
    fn test_part_2() {
        let data: [(&str, i64); 1] = [(
            "0: 3
1: 2
4: 4
6: 4",
            10,
        )];

        for (input, expected) in data {
            let input = parse_input(input.to_string());
            assert_eq!(solve_part_2(&input), expected);
        }
    }
}
