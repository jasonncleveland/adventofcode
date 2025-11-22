use std::collections::{HashMap, VecDeque};
use std::fmt;
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
        let mut parts = line.split(" ");
        if let Some(i) = parts.next()
            && let Some(f) = parts.next()
        {
            let mut instruction = Instruction {
                operation: match i {
                    "set" => Operation::Set,
                    "add" => Operation::Add,
                    "mul" => Operation::Multiply,
                    "mod" => Operation::Modulo,
                    "jgz" => Operation::JumpIfGreaterThanZero,
                    "snd" => Operation::Send,
                    "rcv" => Operation::Receive,
                    other => unreachable!("Unknown operation: {}", other),
                },
                args: Vec::new(),
            };
            if let Ok(value_a) = f.parse::<i64>() {
                instruction.args.push(InstructionArgument {
                    register: None,
                    value: Some(value_a),
                });
            } else if let Some(register_a) = f.chars().next() {
                instruction.args.push(InstructionArgument {
                    register: Some(register_a),
                    value: None,
                });
            }
            if let Some(s) = parts.next() {
                if let Ok(value_b) = s.parse::<i64>() {
                    instruction.args.push(InstructionArgument {
                        register: None,
                        value: Some(value_b),
                    });
                } else if let Some(register_b) = s.chars().next() {
                    instruction.args.push(InstructionArgument {
                        register: Some(register_b),
                        value: None,
                    });
                }
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
    let mut instruction_pointers: [usize; 2] = [0, 0];
    let mut input_queues: [VecDeque<i64>; 2] = [VecDeque::new(), VecDeque::new()];
    let mut registers: [HashMap<char, i64>; 2] = [HashMap::new(), HashMap::new()];
    let mut send_counts: [i64; 2] = [0, 0];
    let mut is_waiting: [bool; 2] = [false, false];

    registers[0].insert('p', 0);
    registers[1].insert('p', 1);

    loop {
        for program_index in 0..2 {
            let mut instructions_executed = 0;
            loop {
                if let Some(instruction) = instructions.get(instruction_pointers[program_index]) {
                    trace!(
                        "Program {} executing instruction {} {}",
                        program_index, instruction_pointers[program_index], instruction
                    );
                    match instruction.operation {
                        Operation::Set => {
                            if let Some(register_a) = instruction.args[0].register {
                                let value =
                                    get_value(&mut registers[program_index], instruction.args[1]);
                                trace!(
                                    "Program {} setting value in register {} to {}",
                                    program_index, register_a, value
                                );
                                registers[program_index].insert(register_a, value);
                            }
                            instruction_pointers[program_index] += 1;
                        }
                        Operation::Add => {
                            if let Some(register_a) = instruction.args[0].register {
                                let value =
                                    get_value(&mut registers[program_index], instruction.args[1]);
                                trace!(
                                    "Program {} adding value {} to register {}",
                                    program_index, register_a, value
                                );
                                registers[program_index]
                                    .entry(register_a)
                                    .and_modify(|v| *v += value);
                            }
                            instruction_pointers[program_index] += 1;
                        }
                        Operation::Multiply => {
                            if let Some(register_a) = instruction.args[0].register {
                                let value =
                                    get_value(&mut registers[program_index], instruction.args[1]);
                                trace!(
                                    "Program {} multiplying value in register {} by {}",
                                    program_index, register_a, value
                                );
                                registers[program_index]
                                    .entry(register_a)
                                    .and_modify(|v| *v *= value);
                            }
                            instruction_pointers[program_index] += 1;
                        }
                        Operation::Modulo => {
                            if let Some(register_a) = instruction.args[0].register {
                                let value =
                                    get_value(&mut registers[program_index], instruction.args[1]);
                                trace!(
                                    "Program {} storing value in register {} mod {}",
                                    program_index, register_a, value
                                );
                                registers[program_index]
                                    .entry(register_a)
                                    .and_modify(|v| *v %= value);
                            }
                            instruction_pointers[program_index] += 1;
                        }
                        Operation::Send => {
                            let value =
                                get_value(&mut registers[program_index], instruction.args[0]);
                            trace!("Program {} sending value {}", program_index, value);
                            input_queues[1 - program_index].push_back(value);
                            send_counts[program_index] += 1;
                            instruction_pointers[program_index] += 1;
                        }
                        Operation::Receive => {
                            if let Some(register_a) = instruction.args[0].register {
                                trace!(
                                    "Program {} attempting to receive value to store in register {}",
                                    program_index, register_a
                                );
                                if let Some(value) = input_queues[program_index].pop_front() {
                                    trace!("Program {} received value {}", program_index, value);
                                    registers[program_index].insert(register_a, value);
                                    instruction_pointers[program_index] += 1;
                                } else {
                                    trace!(
                                        "Program {} no input found {}",
                                        program_index, instruction_pointers[program_index]
                                    );
                                    break;
                                }
                            }
                        }
                        Operation::JumpIfGreaterThanZero => {
                            let value_a =
                                get_value(&mut registers[program_index], instruction.args[0]);
                            if value_a > 0 {
                                let value_b =
                                    get_value(&mut registers[program_index], instruction.args[1]);
                                trace!(
                                    "Program {} jumping with offset value {}",
                                    program_index, value_b
                                );
                                let next_address =
                                    instruction_pointers[program_index] as i64 + value_b;
                                instruction_pointers[program_index] = next_address as usize;
                            } else {
                                trace!(
                                    "Program {} cannot jump because value {} is less than or equal to 0",
                                    program_index, value_a
                                );
                                instruction_pointers[program_index] += 1;
                            }
                        }
                    }
                } else {
                    panic!(
                        "Attempting to read instruction at index {} (out of bounds)",
                        instruction_pointers[program_index]
                    );
                }
                instructions_executed += 1;
            }
            // If no instructions were executed before waiting for input then mark the program as waiting
            is_waiting[program_index] = instructions_executed == 0;
        }
        if is_waiting.iter().all(|b| *b) {
            // If both programs are waiting then we have a deadlock and need to stop
            return send_counts[1];
        }
    }
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

#[derive(Clone)]
enum Operation {
    Set,
    Add,
    Multiply,
    Modulo,
    JumpIfGreaterThanZero,
    Send,
    Receive,
}

impl fmt::Display for Operation {
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        write!(
            f,
            "{}",
            match self {
                Operation::Set => "set",
                Operation::Add => "add",
                Operation::Multiply => "mul",
                Operation::Modulo => "mod",
                Operation::JumpIfGreaterThanZero => "jgz",
                Operation::Send => "snd",
                Operation::Receive => "rcv",
            }
        )
    }
}

#[derive(Clone)]
struct Instruction {
    operation: Operation,
    args: Vec<InstructionArgument>,
}

impl fmt::Display for Instruction {
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        write!(
            f,
            "{} {} {}",
            self.operation,
            self.args[0],
            match self.args.get(1) {
                Some(param) => param.to_string(),
                None => "".to_string(),
            }
        )
    }
}

#[derive(Clone, Copy)]
struct InstructionArgument {
    register: Option<char>,
    value: Option<i64>,
}

impl fmt::Display for InstructionArgument {
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        write!(
            f,
            "{}",
            match self.register {
                Some(register) => register.to_string(),
                None => match self.value {
                    Some(value) => value.to_string(),
                    None => "".to_string(),
                },
            }
        )
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
            let input = parse_input(input.to_string());
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
            let input = parse_input(input.to_string());
            assert_eq!(solve_part_2(&input), expected);
        }
    }
}
