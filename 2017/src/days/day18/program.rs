use std::collections::HashMap;
use std::sync::Arc;
use std::sync::mpsc::{Receiver, Sender};
use std::time::Duration;

use log::{trace, warn};

use super::instruction::{Instruction, InstructionArgument, Operation};

pub struct Program<'i, 's, 'r> {
    label: i64,
    instructions: &'i Arc<Vec<Instruction>>,
    registers: HashMap<char, i64>,
    instruction_pointer: usize,
    send_count: i64,
    sender: &'s Sender<i64>,
    receiver: &'r Receiver<i64>,
}

impl<'i, 's, 'r> Program<'i, 's, 'r> {
    pub fn new(
        instructions: &'i Arc<Vec<Instruction>>,
        label: i64,
        sender: &'s Sender<i64>,
        receiver: &'r Receiver<i64>,
    ) -> Program<'i, 's, 'r> {
        let mut registers = HashMap::new();
        registers.insert('p', label);
        Program {
            label,
            instructions,
            registers,
            instruction_pointer: 0,
            send_count: 0,
            sender,
            receiver,
        }
    }

    pub fn get_send_count(&self) -> i64 {
        self.send_count
    }

    pub fn run(&mut self) {
        let mut instructions_executed = 0;
        loop {
            if let Some(instruction) = self.instructions.get(self.instruction_pointer) {
                trace!(
                    "Program {} executing instruction {} \t {} \t {} \t {:?}",
                    self.label,
                    instructions_executed,
                    self.instruction_pointer,
                    instruction,
                    self.registers
                );
                match instruction.operation {
                    Operation::Set => {
                        if let Some(register_a) = instruction.args[0].register {
                            let value = self.get_value(instruction.args[1]);
                            trace!(
                                "Program {} setting value in register {} to {}",
                                self.label, register_a, value
                            );
                            self.registers.insert(register_a, value);
                            // registers[program_index].entry(register_a).and_modify(|v| *v = value).or_insert(value);
                        } else {
                            panic!("should not be here in snd");
                        }
                        self.instruction_pointer += 1;
                    }
                    Operation::Add => {
                        if let Some(register_a) = instruction.args[0].register {
                            let value = self.get_value(instruction.args[1]);
                            trace!(
                                "Program {} adding value {} to register {}",
                                self.label, register_a, value
                            );
                            self.registers.entry(register_a).and_modify(|v| *v += value);
                        } else {
                            panic!("should not be here in add");
                        }
                        self.instruction_pointer += 1;
                    }
                    Operation::Multiply => {
                        if let Some(register_a) = instruction.args[0].register {
                            let value = self.get_value(instruction.args[1]);
                            trace!(
                                "Program {} multiplying value in register {} by {}",
                                self.label, register_a, value
                            );
                            self.registers.entry(register_a).and_modify(|v| *v *= value);
                        } else {
                            panic!("should not be here in mul");
                        }
                        self.instruction_pointer += 1;
                    }
                    Operation::Modulo => {
                        if let Some(register_a) = instruction.args[0].register {
                            let value = self.get_value(instruction.args[1]);
                            trace!(
                                "Program {} storing value in register {} mod {}",
                                self.label, register_a, value
                            );
                            self.registers.entry(register_a).and_modify(|v| *v %= value);
                        } else {
                            panic!("should not be here in mod");
                        }
                        self.instruction_pointer += 1;
                    }
                    Operation::Send => {
                        let value = self.get_value(instruction.args[0]);
                        trace!("Program {} sending value {}", self.label, value);
                        match self.sender.send(value) {
                            Ok(_) => {
                                self.send_count += 1;
                                self.instruction_pointer += 1;
                            }
                            Err(error) => warn!(
                                "Program {} failed to send value {} with error {}",
                                self.label, value, error
                            ),
                        }
                    }
                    Operation::Receive => {
                        if let Some(register_a) = instruction.args[0].register {
                            trace!(
                                "Program {} attempting to receive value to store in register {}",
                                self.label, register_a
                            );
                            // If the receiver does not have a value after 1 millisecond then assume we are deadlocked
                            match self.receiver.recv_timeout(Duration::from_millis(1)) {
                                Ok(value) => {
                                    trace!(
                                        "Program {} received input value {} and storing value in register {}",
                                        self.label, value, register_a
                                    );
                                    self.registers.insert(register_a, value);
                                    self.instruction_pointer += 1;
                                }
                                Err(error) => {
                                    trace!(
                                        "Program {} failed to receive input value with error {}",
                                        self.label, error
                                    );
                                    return;
                                }
                            }
                        } else {
                            panic!("should not be here in rcv");
                        }
                    }
                    Operation::JumpIfGreaterThanZero => {
                        let value_a = self.get_value(instruction.args[0]);
                        if value_a > 0 {
                            let value_b = self.get_value(instruction.args[1]);
                            trace!(
                                "Program {} jumping with offset value {}",
                                self.label, value_b
                            );
                            self.instruction_pointer =
                                (self.instruction_pointer as i64 + value_b) as usize;
                        } else {
                            trace!(
                                "Program {} cannot jump because value {} is less than or equal to 0",
                                self.label, value_a
                            );
                            self.instruction_pointer += 1;
                        }
                    }
                }
            } else {
                panic!(
                    "Program {} attempting to read instruction at index {} (out of bounds)",
                    self.label, self.instruction_pointer,
                );
            }
            instructions_executed += 1;
        }
    }

    fn get_value(&mut self, argument: InstructionArgument) -> i64 {
        match argument.register {
            Some(register) => self.registers[&register],
            None => match argument.value {
                Some(value) => value,
                None => unreachable!("should never get here without a register or value"),
            },
        }
    }
}
