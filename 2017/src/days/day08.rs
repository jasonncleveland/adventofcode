use std::collections::HashMap;
use std::time::Instant;

use log::{debug, trace};

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

fn parse_input(file_contents: String) -> Vec<Instruction> {
    let mut instructions: Vec<Instruction> = Vec::new();
    for line in file_contents.lines() {
        if let Some((o, c)) = line.split_once(" if ")
            && let Some(condition) = Action::parse(c)
            && let Some(operation) = Action::parse(o)
        {
            instructions.push(Instruction {
                condition: condition.to_owned(),
                operation: operation.to_owned(),
            })
        }
    }
    instructions
}

fn solve_part_1(instructions: &[Instruction]) -> i64 {
    let (max_value, _) = process_instructions(instructions);
    max_value
}

fn solve_part_2(instructions: &[Instruction]) -> i64 {
    let (_, max_value) = process_instructions(instructions);
    max_value
}

fn process_instructions(instructions: &[Instruction]) -> (i64, i64) {
    let mut registers: HashMap<&str, i64> = HashMap::new();

    let mut total_max_value: i64 = i64::MIN;
    for instruction in instructions {
        // Evaluate condition
        let condition_register = registers.entry(&instruction.condition.field).or_insert(0);
        let condition_result = match instruction.condition.action.as_str() {
            "==" => {
                trace!(
                    "evaluating {} == {} = {}",
                    condition_register,
                    instruction.condition.value,
                    *condition_register == instruction.condition.value
                );
                *condition_register == instruction.condition.value
            }
            "!=" => {
                trace!(
                    "evaluating {} != {} = {}",
                    condition_register,
                    instruction.condition.value,
                    *condition_register != instruction.condition.value
                );
                *condition_register != instruction.condition.value
            }
            ">" => {
                trace!(
                    "evaluating {} > {} = {}",
                    condition_register,
                    instruction.condition.value,
                    *condition_register > instruction.condition.value
                );
                *condition_register > instruction.condition.value
            }
            ">=" => {
                trace!(
                    "evaluating {} >= {} = {}",
                    condition_register,
                    instruction.condition.value,
                    *condition_register >= instruction.condition.value
                );
                *condition_register >= instruction.condition.value
            }
            "<" => {
                trace!(
                    "evaluating {} < {} = {}",
                    condition_register,
                    instruction.condition.value,
                    *condition_register < instruction.condition.value
                );
                *condition_register < instruction.condition.value
            }
            "<=" => {
                trace!(
                    "evaluating {} <= {} = {}",
                    condition_register,
                    instruction.condition.value,
                    *condition_register <= instruction.condition.value
                );
                *condition_register <= instruction.condition.value
            }
            other => unreachable!("invalid condition action given: {}", other),
        };

        // Evaluate operation if condition is true
        if condition_result {
            let operation_register = registers.entry(&instruction.operation.field).or_insert(0);
            match instruction.operation.action.as_str() {
                "inc" => {
                    trace!(
                        "incrementing value {} by {} = {}",
                        *operation_register,
                        instruction.operation.value,
                        *operation_register + instruction.operation.value
                    );
                    *operation_register += instruction.operation.value;
                }
                "dec" => {
                    trace!(
                        "decrementing value {} by {} = {}",
                        *operation_register,
                        instruction.operation.value,
                        *operation_register - instruction.operation.value
                    );
                    *operation_register -= instruction.operation.value;
                }
                other => unreachable!("invalid operation action given: {}", other),
            }
            if *operation_register > total_max_value {
                total_max_value = *operation_register;
            }
        }
    }

    if let Some(&end_max_value) = registers.values().max() {
        return (end_max_value, total_max_value);
    }
    unreachable!();
}

#[derive(Debug)]
struct Instruction {
    condition: Action,
    operation: Action,
}

#[derive(Clone, Debug)]
struct Action {
    field: String,
    action: String,
    value: i64,
}

impl Action {
    fn parse(expression: &str) -> Option<Action> {
        let mut parts = expression.split_whitespace();
        if let Some(field) = parts.next()
            && let Some(action) = parts.next()
            && let Some(value) = parts.next()
            && let Ok(number) = value.parse::<i64>()
        {
            return Some(Action {
                field: field.to_owned(),
                action: action.to_owned(),
                value: number,
            });
        }
        None
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let data: [(&str, i64); 1] = [(
            "b inc 5 if a > 1
a inc 1 if b < 5
c dec -10 if a >= 1
c inc -20 if c == 10",
            1,
        )];

        for (input, expected) in data {
            let input = parse_input(input.to_string());
            assert_eq!(solve_part_1(&input), expected);
        }
    }

    #[test]
    fn test_part_2() {
        let data: [(&str, i64); 1] = [(
            "b inc 5 if a > 1
a inc 1 if b < 5
c dec -10 if a >= 1
c inc -20 if c == 10",
            10,
        )];

        for (input, expected) in data {
            let input = parse_input(input.to_string());
            assert_eq!(solve_part_2(&input), expected);
        }
    }
}
