use std::collections::HashMap;
use std::time::Instant;

use log::debug;

pub fn solve(file_contents: String) -> (String, String) {
    let parse_timer = Instant::now();
    let (state, mutations) = parse_input(file_contents);
    debug!("File parse: ({:?})", parse_timer.elapsed());

    let part1_timer = Instant::now();
    let part1 = solve_part_1(&state, &mutations);
    debug!("Part 1: {} ({:?})", part1, part1_timer.elapsed());

    let part2_timer = Instant::now();
    let part2 = solve_part_2(&state, &mutations);
    debug!("Part 2: {} ({:?})", part2, part2_timer.elapsed());

    (part1.to_string(), part2.to_string())
}

fn parse_input(file_contents: String) -> (Vec<char>, HashMap<String, char>) {
    let mut state: Vec<char> = Vec::new();
    let mut mutations: HashMap<String, char> = HashMap::new();

    if let Some((state_contents, mutations_content)) = file_contents.split_once("\n\n") {
        if let Some((_, s)) = state_contents.split_once(": ") {
            state = s.chars().collect();
        }

        for line in mutations_content.lines() {
            if let Some((input, o)) = line.split_once(" => ")
                && let Some(output) = o.chars().next()
            {
                mutations.insert(input.to_string(), output);
            }
        }
    }

    (state, mutations)
}

fn solve_part_1(state: &[char], mutations: &HashMap<String, char>) -> i64 {
    let mut state_copy = state.to_owned();

    let offset = simulate_generations(&mut state_copy, mutations, 20);

    calculate_score(&state_copy, offset)
}

fn solve_part_2(state: &[char], mutations: &HashMap<String, char>) -> i64 {
    let mut current_state = state.to_owned();

    let mut total_offset = 0;

    let mut is_score_converging = false;
    let mut last_score = 0;
    let mut last_score_delta = 0;
    let mut generations = 0;

    // Perform generations until we have three consecutive generations with the same score delta
    loop {
        total_offset += simulate_generations(&mut current_state, mutations, 1);
        let current_score = calculate_score(&current_state, total_offset);
        let current_score_delta = current_score - last_score;
        if current_score_delta == last_score_delta {
            match is_score_converging {
                true => break,
                false => is_score_converging = true,
            }
        }
        last_score_delta = current_score_delta;
        last_score = current_score;
        generations += 1;
    }

    // Multiply the score delta by the remaining generations to add to the current score
    last_score + (last_score_delta * (50_000_000_000 - generations))
}

fn simulate_generations(
    state: &mut Vec<char>,
    mutations: &HashMap<String, char>,
    iterations: i64,
) -> i64 {
    // Keep track of the left side offset to know how many zeros were added for padding
    let mut offset: i64 = 0;

    for _ in 0..iterations {
        // Add 2 empty pots on the left to help calculations
        state.insert(0, '.');
        state.insert(0, '.');
        offset += 2;
        // Add 3 empty pots on the right to help calculations
        state.push('.');
        state.push('.');
        state.push('.');

        let mut current_state = state.clone();
        for pot_index in 2..current_state.len() - 2 {
            let row: Vec<char> = vec![
                state[pot_index - 2],
                state[pot_index - 1],
                state[pot_index],
                state[pot_index + 1],
                state[pot_index + 2],
            ];

            if let Some(&next_value) = mutations.get(&String::from_iter(&row)) {
                current_state[pot_index] = next_value;
            } else {
                current_state[pot_index] = '.';
            }
        }
        *state = current_state;
    }

    offset
}

fn calculate_score(state: &[char], offset: i64) -> i64 {
    let mut total: i64 = 0;
    for (i, pot) in state.iter().enumerate() {
        if *pot == '#' {
            total += i as i64 - offset;
        }
    }
    total
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let input: [&str; 1] = ["initial state: #..#.#..##......###...###

...## => #
..#.. => #
.#... => #
.#.#. => #
.#.## => #
.##.. => #
.#### => #
#.#.# => #
#.### => #
##.#. => #
##.## => #
###.. => #
###.# => #
####. => #"];
        let expected: [i64; 1] = [325];

        for i in 0..input.len() {
            let (state, mutations) = parse_input(input[i].to_string());
            assert_eq!(solve_part_1(&state, &mutations), expected[i]);
        }
    }

    #[test]
    fn test_part_2() {
        let input: [&str; 1] = ["initial state: #..#.#..##......###...###

...## => #
..#.. => #
.#... => #
.#.#. => #
.#.## => #
.##.. => #
.#### => #
#.#.# => #
#.### => #
##.#. => #
##.## => #
###.. => #
###.# => #
####. => #"];
        let expected: [i64; 1] = [999999999374];

        for i in 0..input.len() {
            let (state, mutations) = parse_input(input[i].to_string());
            assert_eq!(solve_part_2(&state, &mutations), expected[i]);
        }
    }
}
