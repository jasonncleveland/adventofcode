mod instruction;
mod program;

use std::collections::HashMap;
use std::sync::{Arc, mpsc};
use std::thread;
use std::time::Instant;

use log::{debug, trace};

use instruction::{Instruction, InstructionArgument, Operation};
use program::Program;

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

fn parse_input(file_contents: &str) -> Vec<Instruction> {
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

    let mut last_played_sound = 0;
    loop {
        if let Some(instruction) = instructions.get(instruction_pointer) {
            trace!("processing instruction: {}", instruction);
            trace!(
                "registers: {:?} last played sound {}",
                registers, last_played_sound
            );
            match instruction.operation {
                Operation::Send => {
                    let value = get_value(&mut registers, instruction.args[0]);
                    trace!("playing sound with frequency {}", value);
                    last_played_sound = value;
                    instruction_pointer += 1;
                }
                Operation::Set => {
                    if let Some(register_a) = instruction.args[0].register {
                        let value = get_value(&mut registers, instruction.args[1]);
                        trace!("setting value in register {} to {}", register_a, value);
                        registers.insert(register_a, value);
                    }
                    instruction_pointer += 1;
                }
                Operation::Add => {
                    if let Some(register_a) = instruction.args[0].register {
                        let value = get_value(&mut registers, instruction.args[1]);
                        trace!("adding value {} to register {}", register_a, value);
                        registers.entry(register_a).and_modify(|v| *v += value);
                    }
                    instruction_pointer += 1;
                }
                Operation::Multiply => {
                    if let Some(register_a) = instruction.args[0].register {
                        let value = get_value(&mut registers, instruction.args[1]);
                        trace!("multiplying value in register {} by {}", register_a, value);
                        registers.entry(register_a).and_modify(|v| *v *= value);
                    }
                    instruction_pointer += 1;
                }
                Operation::Modulo => {
                    if let Some(register_a) = instruction.args[0].register {
                        let value = get_value(&mut registers, instruction.args[1]);
                        trace!("storing value in register {} mod {}", register_a, value);
                        registers.entry(register_a).and_modify(|v| *v %= value);
                    }
                    instruction_pointer += 1;
                }
                Operation::Receive => {
                    let value = get_value(&mut registers, instruction.args[0]);
                    if value != 0 {
                        trace!(
                            "receiving frequency of last sound played {}",
                            last_played_sound
                        );
                        return last_played_sound;
                    } else {
                        trace!("cannot receive frequency because value {} is 0", value);
                    }
                    instruction_pointer += 1;
                }
                Operation::JumpIfGreaterThanZero => {
                    let value_a = get_value(&mut registers, instruction.args[0]);
                    if value_a > 0 {
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
            panic!(
                "Attempting to read instruction at index {} (out of bounds)",
                instruction_pointer
            );
        }
    }
}

fn solve_part_2(instructions: &[Instruction]) -> i64 {
    let (program_a_transmitter, program_a_receiver) = mpsc::channel::<i64>();
    let (program_b_transmitter, program_b_receiver) = mpsc::channel::<i64>();

    let instructions_ref = Arc::new(instructions.to_vec());
    let instructions_copy_a = Arc::clone(&instructions_ref);
    let instructions_copy_b = Arc::clone(&instructions_ref);

    let program_a_thread = thread::spawn(move || {
        trace!("Spawning thread for program a");
        let mut program_a = Program::new(
            &instructions_copy_a,
            0,
            &program_a_transmitter,
            &program_b_receiver,
        );
        program_a.run();
        trace!("Program a sent {} values", program_a.get_send_count());
        program_a.get_send_count()
    });
    let program_b_thread = thread::spawn(move || {
        trace!("Spawning thread for program b");
        let mut program_b = Program::new(
            &instructions_copy_b,
            1,
            &program_b_transmitter,
            &program_a_receiver,
        );
        program_b.run();
        trace!("Program b sent {} values", program_b.get_send_count());
        program_b.get_send_count()
    });

    if let Ok(result) = program_a_thread.join() {
        trace!("Program a thread joined: {}", result);
    }

    if let Ok(result) = program_b_thread.join() {
        trace!("Program b thread joined: {}", result);
        return result;
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

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let data: [(&str, i64); 1] = [(
            "set a 1
add a 2
mul a a
mod a 5
snd a
set a 0
rcv a
jgz a -1
set a 1
jgz a -2",
            4,
        )];

        for (input, expected) in data {
            let input = parse_input(input);
            assert_eq!(solve_part_1(&input), expected);
        }
    }

    #[test]
    fn test_part_2() {
        let data: [(&str, i64); 1] = [(
            "snd 1
snd 2
snd p
rcv a
rcv b
rcv c
rcv d",
            3,
        )];

        for (input, expected) in data {
            let input = parse_input(input);
            assert_eq!(solve_part_2(&input), expected);
        }
    }
}
