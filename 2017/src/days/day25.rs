use std::collections::HashMap;
use std::time::Instant;

use aoc_helpers::direction::Direction;
use log::debug;

pub fn solve(file_contents: String) -> (String, String) {
    let parse_timer = Instant::now();
    let input = parse_input(file_contents);
    debug!("File parse: ({:?})", parse_timer.elapsed());

    let part1_timer = Instant::now();
    let part1 = solve_part_1(&input);
    debug!("Part 1: {} ({:?})", part1, part1_timer.elapsed());

    (part1.to_string(), "Merry Christmas!".to_string())
}

fn parse_input(file_contents: String) -> TuringMachine {
    let mut parts = file_contents.split("\n\n");

    // Parse metadata
    if let Some(metadata) = parts.next()
        && let Some((state_line, checksum_line)) = metadata.split_once('\n')
        && let Some(state_string) = state_line.split_whitespace().last()
        && let Some(start_state) = state_string.chars().next()
        && let Some(checksum_string) = checksum_line.split_whitespace().nth(5)
        && let Ok(checksum_step) = checksum_string.parse::<usize>()
    {
        // Parse states
        let mut states: HashMap<char, [TuringMachineState; 2]> = HashMap::new();
        for state in parts {
            let mut state_parts = state.split('\n');
            if let Some(in_state) = state_parts.next()
                && let Some(in_state_char) = in_state.chars().nth(9)
                // Current value 0
                && let Some(zero_write) = state_parts.nth(1)
                && let Some(zero_write_char) = zero_write.chars().nth(22)
                && let Some(zero_write_value) = zero_write_char.to_digit(10)
                && let Some(zero_slot) = state_parts.next()
                && let Some(zero_next) = state_parts.next()
                && let Some(zero_next_state) = zero_next.chars().nth(26)
                // Current value 1
                && let Some(one_write) = state_parts.nth(1)
                && let Some(one_write_char) = one_write.chars().nth(22)
                && let Some(one_write_value) = one_write_char.to_digit(10)
                && let Some(one_slot) = state_parts.next()
                && let Some(one_next) = state_parts.next()
                && let Some(one_next_state) = one_next.chars().nth(26)
            {
                let zero_direction = match zero_slot.len() {
                    32 => Direction::Left,
                    33 => Direction::Right,
                    _ => unreachable!(),
                };
                let one_direction = match one_slot.len() {
                    32 => Direction::Left,
                    33 => Direction::Right,
                    _ => unreachable!(),
                };

                states.insert(
                    in_state_char,
                    [
                        TuringMachineState {
                            write_value: zero_write_value as usize,
                            direction: zero_direction,
                            next_state: zero_next_state,
                        },
                        TuringMachineState {
                            write_value: one_write_value as usize,
                            direction: one_direction,
                            next_state: one_next_state,
                        },
                    ],
                );
            }
        }
        return TuringMachine {
            start_state,
            checksum_step,
            states,
        };
    }
    unreachable!()
}

fn solve_part_1(input: &TuringMachine) -> usize {
    let mut current_state = input.start_state;
    let mut tape: Vec<usize> = vec![0; 20_000];
    let mut tape_index = 10_000;
    for _ in 0..input.checksum_step {
        if let Some(state) = input.states.get(&current_state) {
            let tape_value = tape[tape_index];
            if let Some(instruction) = state.get(tape_value) {
                tape[tape_index] = instruction.write_value;
                match instruction.direction {
                    Direction::Left => tape_index -= 1,
                    Direction::Right => tape_index += 1,
                    _ => unreachable!(),
                }
                current_state = instruction.next_state;
            }
        }
    }
    tape.iter().sum()
}

#[derive(Debug)]
struct TuringMachine {
    start_state: char,
    checksum_step: usize,
    states: HashMap<char, [TuringMachineState; 2]>,
}

#[derive(Debug)]
struct TuringMachineState {
    write_value: usize,
    direction: Direction,
    next_state: char,
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let data: [(&str, usize); 1] = [(
            "Begin in state A.
Perform a diagnostic checksum after 6 steps.

In state A:
  If the current value is 0:
    - Write the value 1.
    - Move one slot to the right.
    - Continue with state B.
  If the current value is 1:
    - Write the value 0.
    - Move one slot to the left.
    - Continue with state B.

In state B:
  If the current value is 0:
    - Write the value 1.
    - Move one slot to the left.
    - Continue with state A.
  If the current value is 1:
    - Write the value 1.
    - Move one slot to the right.
    - Continue with state A.",
            3,
        )];

        for (input, expected) in data {
            let input = parse_input(input.to_string());
            assert_eq!(solve_part_1(&input), expected);
        }
    }
}
