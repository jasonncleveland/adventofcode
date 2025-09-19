use std::fmt;
use std::fmt::{Display, Formatter};

use log::trace;

#[derive(Clone, Debug)]
pub struct Instruction {
    pub opcode: String,
    pub a: usize,
    pub b: usize,
    pub c: usize,
}

impl Display for Instruction {
    fn fmt(&self, f: &mut Formatter<'_>) -> fmt::Result {
        write!(f, "{} {} {} {}", self.opcode, self.a, self.b, self.c)
    }
}

impl Instruction {
    pub fn new(opcode: String, a: usize, b: usize, c: usize) -> Instruction {
        Self { opcode, a, b, c }
    }
}

#[derive(Debug)]
pub struct Device {
    pub registers: Vec<usize>,
    instruction_pointer_register: usize,
    instructions: Vec<Instruction>,
}

impl Device {
    pub fn new(instructions: &[Instruction]) -> Device {
        Device {
            registers: vec![0; 6],
            // The first instruction contains the register to use as the instruction pointer
            instruction_pointer_register: instructions[0].a,
            instructions: instructions[1..].to_vec(),
        }
    }

    pub fn process_instruction(&mut self) -> bool {
        if self.registers[self.instruction_pointer_register] >= self.instructions.len() {
            trace!(
                "attempting to process instruction at index {} which is outside bounds [0..{}]",
                self.registers[self.instruction_pointer_register],
                self.instructions.len()
            );
            return false;
        }

        let instruction = &self.instructions[self.registers[self.instruction_pointer_register]];
        trace!(
            "executing instruction at {}: {}",
            self.registers[self.instruction_pointer_register], instruction
        );

        match instruction.opcode.as_str() {
            "#ip" => self.instruction_pointer_register = instruction.a,
            "addr" => {
                self.registers[instruction.c] =
                    self.registers[instruction.a] + self.registers[instruction.b]
            }
            "addi" => self.registers[instruction.c] = self.registers[instruction.a] + instruction.b,
            "mulr" => {
                self.registers[instruction.c] =
                    self.registers[instruction.a] * self.registers[instruction.b]
            }
            "muli" => self.registers[instruction.c] = self.registers[instruction.a] * instruction.b,
            "banr" => {
                self.registers[instruction.c] =
                    self.registers[instruction.a] & self.registers[instruction.b]
            }
            "bani" => self.registers[instruction.c] = self.registers[instruction.a] & instruction.b,
            "borr" => {
                self.registers[instruction.c] =
                    self.registers[instruction.a] | self.registers[instruction.b]
            }
            "bori" => self.registers[instruction.c] = self.registers[instruction.a] | instruction.b,
            "setr" => self.registers[instruction.c] = self.registers[instruction.a],
            "seti" => self.registers[instruction.c] = instruction.a,
            "gtir" => {
                self.registers[instruction.c] = match instruction.a > self.registers[instruction.b]
                {
                    true => 1,
                    false => 0,
                }
            }
            "gtri" => {
                self.registers[instruction.c] = match self.registers[instruction.a] > instruction.b
                {
                    true => 1,
                    false => 0,
                }
            }
            "gtrr" => {
                self.registers[instruction.c] =
                    match self.registers[instruction.a] > self.registers[instruction.b] {
                        true => 1,
                        false => 0,
                    }
            }
            "eqir" => {
                self.registers[instruction.c] = match instruction.a == self.registers[instruction.b]
                {
                    true => 1,
                    false => 0,
                }
            }
            "eqri" => {
                self.registers[instruction.c] = match self.registers[instruction.a] == instruction.b
                {
                    true => 1,
                    false => 0,
                }
            }
            "eqrr" => {
                self.registers[instruction.c] =
                    match self.registers[instruction.a] == self.registers[instruction.b] {
                        true => 1,
                        false => 0,
                    }
            }
            _ => panic!("Invalid instruction opcode"),
        }

        // Increment the instruction pointer by 1
        self.registers[self.instruction_pointer_register] += 1;
        true
    }

    pub fn process_instructions(&mut self) {
        // Run until the program halts
        while self.process_instruction() {}
    }
}

pub fn parse_device_instructions(file_contents: String) -> Vec<Instruction> {
    let mut instructions: Vec<Instruction> = Vec::new();

    // Parse instructions
    for line in file_contents.lines() {
        if let Some((opcode, inputs)) = line.split_once(' ') {
            if opcode == "#ip"
                && let Ok(a) = inputs.trim().parse::<usize>()
            {
                instructions.push(Instruction::new(opcode.to_string(), a, 0, 0));
            } else if let Some((left, rest)) = inputs.split_once(' ')
                && let Some((middle, right)) = rest.split_once(' ')
                && let Ok(a) = left.parse::<usize>()
                && let Ok(b) = middle.parse::<usize>()
                && let Ok(c) = right.parse::<usize>()
            {
                instructions.push(Instruction::new(opcode.to_string(), a, b, c));
            }
        }
    }

    instructions
}
