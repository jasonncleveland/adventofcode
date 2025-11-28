mod instruction;

use std::collections::HashMap;
use std::time::Instant;

use log::{debug, trace};

use instruction::{Instruction, InstructionArgument, Operation};

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
        let mut parts = line.split(" ");
        if let Some(operation) = parts.next()
            && let Some(first_argument) = parts.next()
        {
            let mut instruction = Instruction {
                operation: Operation::from_str(operation),
                args: Vec::new(),
            };
            instruction
                .args
                .push(InstructionArgument::from_str(first_argument));

            if let Some(second_argument) = parts.next() {
                instruction
                    .args
                    .push(InstructionArgument::from_str(second_argument));
            }
            instructions.push(instruction);
        }
    }
    instructions
}

fn solve_part_1(instructions: &[Instruction]) -> i64 {
    let mut registers: HashMap<char, i64> = HashMap::new();
    let mut instruction_pointer = 0;

    let mut total = 0;
    loop {
        if let Some(instruction) = instructions.get(instruction_pointer) {
            trace!("processing instruction: {}", instruction);
            match instruction.operation {
                Operation::Set => {
                    if let Some(register_a) = instruction.args[0].register {
                        let value = get_value(&mut registers, instruction.args[1]);
                        trace!("setting value in register {} to {}", register_a, value);
                        registers.insert(register_a, value);
                    }
                    instruction_pointer += 1;
                }
                Operation::Subtract => {
                    if let Some(register_a) = instruction.args[0].register {
                        let value = get_value(&mut registers, instruction.args[1]);
                        trace!("adding value {} to register {}", register_a, value);
                        registers.entry(register_a).and_modify(|v| *v -= value);
                    }
                    instruction_pointer += 1;
                }
                Operation::Multiply => {
                    if let Some(register_a) = instruction.args[0].register {
                        let value = get_value(&mut registers, instruction.args[1]);
                        trace!("multiplying value in register {} by {}", register_a, value);
                        registers.entry(register_a).and_modify(|v| *v *= value);
                    }
                    total += 1;
                    instruction_pointer += 1;
                }
                Operation::JumpIfNotEqualToZero => {
                    let value_a = get_value(&mut registers, instruction.args[0]);
                    if value_a != 0 {
                        let value_b = get_value(&mut registers, instruction.args[1]);
                        trace!("jumping with offset value {}", value_b);
                        instruction_pointer = (instruction_pointer as i64 + value_b) as usize;
                    } else {
                        trace!("cannot jump because value {} is 0", value_a);
                        instruction_pointer += 1;
                    }
                }
            }
        } else {
            trace!(
                "Attempting to read instruction at index {} (out of bounds)",
                instruction_pointer
            );
            // Return the total number of times the mul instruction was executed
            return total;
        }
    }
}

fn solve_part_2(instructions: &[Instruction]) -> i64 {
    // Input program counts the number of non-prime numbers between b and c in increments of 17
    if let Some(set_instruction) = instructions.first()
        && let Some(last_arg) = set_instruction.args.last()
        && let Some(input) = last_arg.value
    {
        // Setup
        let b = input * 100 + 100_000;
        let c = b + 17_000;
        let mut h = 0;

        // Loop from b to c in increments of 17
        'a: for b in (b..=c).step_by(17) {
            // Loop until the square root of a number to check for factors
            for d in 2..b.isqrt() {
                // Check if a number is a factor
                if b % d == 0 {
                    h += 1;
                    continue 'a;
                }
            }
        }
        return h;
    }
    unreachable!();
}

fn get_value(registers: &mut HashMap<char, i64>, argument: InstructionArgument) -> i64 {
    match argument.register {
        Some(register) => *registers.entry(register).or_insert(0),
        None => match argument.value {
            Some(value) => value,
            None => unreachable!("should never get here without a register or value"),
        },
    }
}
