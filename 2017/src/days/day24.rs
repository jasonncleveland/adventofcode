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

fn parse_input(file_contents: &str) -> Vec<(i64, i64)> {
    let mut components: Vec<(i64, i64)> = Vec::new();
    for line in file_contents.lines() {
        if let Some((left, right)) = line.split_once('/')
            && let Ok(first) = left.parse::<i64>()
            && let Ok(second) = right.parse::<i64>()
        {
            components.push((first, second));
        }
    }
    components
}

fn solve_part_1(input: &[(i64, i64)]) -> i64 {
    let mut max = i64::MIN;

    for component in input {
        if component.0 != 0 {
            continue;
        }
        let remaining = input
            .iter()
            .filter(|&c| c != component)
            .copied()
            .collect::<Vec<(i64, i64)>>();
        let result =
            build_strongest_bridge_rec(&remaining, (component.0 + component.1, component.1));
        if result > max {
            max = result;
        }
    }

    max
}

fn solve_part_2(input: &[(i64, i64)]) -> i64 {
    let mut max = (0, 0);

    for component in input {
        if component.0 != 0 {
            continue;
        }
        let remaining = input
            .iter()
            .filter(|&c| c != component)
            .copied()
            .collect::<Vec<(i64, i64)>>();
        let (len, strength) = build_longest_strongest_bridge_rec(
            &remaining,
            (0, component.0 + component.1, component.1),
        );
        if len > max.0 || len == max.0 && strength > max.1 {
            max = (len, strength);
        }
    }

    max.1
}

fn build_strongest_bridge_rec(components: &[(i64, i64)], bridge: (i64, i64)) -> i64 {
    let (total_strength, last) = bridge;
    let mut max_strength = i64::MIN;

    let mut found_next = false;
    for &(first, second) in components {
        if first != last && second != last {
            continue;
        }
        found_next = true;

        let remaining = components
            .iter()
            .filter(|&c| *c != (first, second))
            .copied()
            .collect::<Vec<(i64, i64)>>();
        if first == last {
            let result =
                build_strongest_bridge_rec(&remaining, (total_strength + first + second, second));
            if result > max_strength {
                max_strength = result;
            }
        } else if second == last {
            let result =
                build_strongest_bridge_rec(&remaining, (total_strength + first + second, first));
            if result > max_strength {
                max_strength = result;
            }
        } else {
            unreachable!();
        }
    }

    match found_next {
        true => max_strength,
        false => total_strength,
    }
}

fn build_longest_strongest_bridge_rec(
    components: &[(i64, i64)],
    bridge: (usize, i64, i64),
) -> (usize, i64) {
    let (total_length, total_strength, last) = bridge;
    let mut best_length = 0;
    let mut best_strength = 0;

    let mut found_next = false;
    for &(first, second) in components {
        if first != last && second != last {
            continue;
        }
        found_next = true;

        let remaining = components
            .iter()
            .filter(|&c| *c != (first, second))
            .copied()
            .collect::<Vec<(i64, i64)>>();
        if first == last {
            let (len, strength) = build_longest_strongest_bridge_rec(
                &remaining,
                (total_length + 2, total_strength + first + second, second),
            );
            if len > best_length || len == best_length && strength > best_strength {
                best_length = len;
                best_strength = strength;
            }
        } else if second == last {
            let (len, strength) = build_longest_strongest_bridge_rec(
                &remaining,
                (total_length + 2, total_strength + first + second, first),
            );
            if len > best_length || len == best_length && strength > best_strength {
                best_length = len;
                best_strength = strength;
            }
        } else {
            unreachable!();
        }
    }

    match found_next {
        true => (best_length, best_strength),
        false => (total_length, total_strength),
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let data: [(&str, i64); 1] = [(
            "0/2
2/2
2/3
3/4
3/5
0/1
10/1
9/10",
            31,
        )];

        for (input, expected) in data {
            let input = parse_input(input);
            assert_eq!(solve_part_1(&input), expected);
        }
    }

    #[test]
    fn test_part_2() {
        let data: [(&str, i64); 1] = [(
            "0/2
2/2
2/3
3/4
3/5
0/1
10/1
9/10",
            19,
        )];

        for (input, expected) in data {
            let input = parse_input(input);
            assert_eq!(solve_part_2(&input), expected);
        }
    }
}
