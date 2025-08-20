use std::collections::VecDeque;
use std::fmt;

use log::{trace};

#[derive(Clone, Debug, PartialEq)]
pub struct IntCodeComputer {
    pub memory: Vec<i64>,
    pub input: VecDeque<i64>,
    pub output: VecDeque<i64>,
    instruction_pointer: i64,
}

#[derive(Debug)]
pub enum IntCodeError {
    InvalidFileAccessMode,
    InvalidInstructionOpCode,
    NoInputGiven,
}

impl fmt::Display for IntCodeError {
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        match self {
            IntCodeError::InvalidFileAccessMode => write!(f, "Invalid file access mode"),
            IntCodeError::InvalidInstructionOpCode => write!(f, "Invalid instruction opcode"),
            IntCodeError::NoInputGiven => write!(f, "No input given"),
        }
    }
}

impl IntCodeComputer {
    pub fn new(memory: &[i64]) -> IntCodeComputer {
        IntCodeComputer {
            instruction_pointer: 0,
            memory: memory.to_owned(),
            input: VecDeque::new(),
            output: VecDeque::new(),
        }
    }

    fn get_parameter_value(&self, offset: i64, mode: i64) -> Result<i64, IntCodeError> {
        trace!("retrieving parameter {} in mode {}", offset, mode);
        let parameter = self.memory[(self.instruction_pointer + offset) as usize];
        match mode {
            // Position mode
            0 => Ok(self.memory[parameter as usize]),
            // Immediate mode
            1 => Ok(parameter),
            _ => Err(IntCodeError::InvalidFileAccessMode),
        }
    }

    pub fn process_instruction(&mut self) -> Result<bool, IntCodeError> {
        trace!("Processing instruction: [{}] ({})", self.instruction_pointer, self.memory[self.instruction_pointer as usize]);

        let instruction = self.memory[self.instruction_pointer as usize];
        let opcode = instruction % 100;
        let first_parameter_mode = (instruction % 1000) / 100;
        let second_parameter_mode = (instruction % 10000) / 1000;
        let third_parameter_mode = (instruction % 100000) / 10000;

        trace!("Processing opcode {} in modes {}/{}/{}", opcode, first_parameter_mode, second_parameter_mode, third_parameter_mode);
        match opcode {
            1 => {
                let first_value = self.get_parameter_value(1, first_parameter_mode)?;
                let second_value = self.get_parameter_value(2, second_parameter_mode)?;
                // Write parameters will never be in immediate mode
                let third_address = self.memory[self.instruction_pointer as usize + 3];
                trace!(
                    "Adding ({}) + ({}) = [{}] ({})",
                    first_value,
                    second_value,
                    third_address,
                    first_value + second_value,
                );
                self.memory[third_address as usize] = first_value + second_value;
                self.instruction_pointer += 4;
                Ok(true)
            },
            2 => {
                let first_value = self.get_parameter_value(1, first_parameter_mode)?;
                let second_value = self.get_parameter_value(2, second_parameter_mode)?;
                // Write parameters will never be in immediate mode
                let third_address = self.memory[self.instruction_pointer as usize + 3];
                trace!(
                    "Multiplying ({}) * ({}) = [{}] ({})",
                    first_value,
                    second_value,
                    third_address,
                    first_value * second_value,
                );
                self.memory[third_address as usize] = first_value * second_value;
                self.instruction_pointer += 4;
                Ok(true)
            },
            3 => {
                if let Some(input) = self.input.pop_front() {
                    // Write parameters will never be in immediate mode
                    let first_address = self.memory[self.instruction_pointer as usize + 1];
                    trace!(
                        "Storing ({}) -> [{}]",
                        input,
                        first_address,
                    );
                    self.memory[first_address as usize] = input;
                    self.instruction_pointer += 2;
                    return Ok(true);
                }
                Err(IntCodeError::NoInputGiven)
            },
            4 => {
                let first_value = self.get_parameter_value(1, first_parameter_mode)?;
                trace!(
                    "Outputting ({})",
                    first_value,
                );
                self.output.push_back(first_value);
                self.instruction_pointer += 2;
                Ok(true)
            },
            5 => {
                let first_value = self.get_parameter_value(1, first_parameter_mode)?;
                trace!(
                    "Checking if ({}) is non-zero ({})",
                    first_value,
                    first_value != 0,
                );
                if first_value != 0 {
                    let second_value = self.get_parameter_value(2, second_parameter_mode)?;
                    trace!(
                        "Jumping to [{}]",
                        second_value,
                    );
                    self.instruction_pointer = second_value;
                } else {
                    self.instruction_pointer += 3;
                }
                Ok(true)
            },
            6 => {
                let first_value = self.get_parameter_value(1, first_parameter_mode)?;
                trace!(
                    "Checking if ({}) is zero ({})",
                    first_value,
                    first_value == 0,
                );
                if first_value == 0 {
                    let second_value = self.get_parameter_value(2, second_parameter_mode)?;
                    trace!(
                        "Jumping to [{}]",
                        second_value,
                    );
                    self.instruction_pointer = second_value;
                } else {
                    self.instruction_pointer += 3;
                }
                Ok(true)
            },
            7 => {
                let first_value = self.get_parameter_value(1, first_parameter_mode)?;
                let second_value = self.get_parameter_value(2, second_parameter_mode)?;
                // Write parameters will never be in immediate mode
                let third_address = self.memory[self.instruction_pointer as usize + 3];
                trace!(
                    "Checking if ({}) is less than ({}) ({})",
                    first_value,
                    second_value,
                    first_value < second_value,
                );

                if first_value < second_value {
                    self.memory[third_address as usize] = 1;
                } else {
                    self.memory[third_address as usize] = 0;
                }
                self.instruction_pointer += 4;
                Ok(true)
            },
            8 => {
                let first_value = self.get_parameter_value(1, first_parameter_mode)?;
                let second_value = self.get_parameter_value(2, second_parameter_mode)?;
                // Write parameters will never be in immediate mode
                let third_address = self.memory[self.instruction_pointer as usize + 3];
                trace!(
                    "Checking if ({}) is equal to ({}) ({})",
                    first_value,
                    second_value,
                    first_value == second_value,
                );

                if first_value == second_value {
                    self.memory[third_address as usize] = 1;
                } else {
                    self.memory[third_address as usize] = 0;
                }
                self.instruction_pointer += 4;
                Ok(true)
            },
            99 => Ok(false),
            _ => Err(IntCodeError::InvalidInstructionOpCode),
        }
    }

    pub fn run(&mut self) {
        loop {
            let result = self.process_instruction();
            match result {
                Ok(true) => continue,
                Ok(false) => break,
                Err(e) => panic!("{}", e),
            }
        }
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_add() {
        let mut computer = IntCodeComputer {
            instruction_pointer: 0,
            memory: vec![1, 9, 10, 11, 1101, 27, 12, 12, 99, 7, 9, 0, 0],
            input: VecDeque::from(vec![]),
            output: VecDeque::from(vec![]),
        };

        // Process instruction using position mode
        let result = computer.process_instruction();
        assert!(result.is_ok());
        assert_eq!(computer.instruction_pointer, 4);
        assert_eq!(computer.memory, vec![1, 9, 10, 11, 1101, 27, 12, 12, 99, 7, 9, 16, 0]);

        // Process instruction using immediate mode
        let result = computer.process_instruction();
        assert!(result.is_ok());
        assert_eq!(computer.instruction_pointer, 8);
        assert_eq!(computer.memory, vec![1, 9, 10, 11, 1101, 27, 12, 12, 99, 7, 9, 16, 39]);
    }

    #[test]
    fn test_multiply() {
        let mut computer = IntCodeComputer {
            instruction_pointer: 0,
            memory: vec![2, 9, 10, 11, 1102, 18, 3, 12, 99, 7, 9, 0, 0],
            input: VecDeque::from(vec![]),
            output: VecDeque::from(vec![]),
        };

        // Process instruction using position mode
        let result = computer.process_instruction();
        assert!(result.is_ok());
        assert_eq!(computer.instruction_pointer, 4);
        assert_eq!(computer.memory, vec![2, 9, 10, 11, 1102, 18, 3, 12, 99, 7, 9, 63, 0]);

        // Process instruction using immediate mode
        let result = computer.process_instruction();
        assert!(result.is_ok());
        assert_eq!(computer.instruction_pointer, 8);
        assert_eq!(computer.memory, vec![2, 9, 10, 11, 1102, 18, 3, 12, 99, 7, 9, 63, 54]);
    }

    #[test]
    fn test_store() {
        let mut computer = IntCodeComputer {
            instruction_pointer: 0,
            memory: vec![3, 4, 99, 0, 0],
            input: VecDeque::from(vec![7]),
            output: VecDeque::from(vec![]),
        };

        let result = computer.process_instruction();
        assert!(result.is_ok());
        assert_eq!(computer.instruction_pointer, 2);
        assert_eq!(computer.memory, vec![3, 4, 99, 0, 7]);
    }

    #[test]
    fn test_output() {
        let mut computer = IntCodeComputer {
            instruction_pointer: 0,
            memory: vec![4, 6, 104, 5, 99, 0, 7],
            input: VecDeque::from(vec![7]),
            output: VecDeque::from(vec![]),
        };


        // Process instruction using position mode
        let result = computer.process_instruction();
        assert!(result.is_ok());
        assert_eq!(computer.instruction_pointer, 2);
        assert_eq!(computer.output, vec![7]);

        // Process instruction using immediate mode
        let result = computer.process_instruction();
        assert!(result.is_ok());
        assert_eq!(computer.instruction_pointer, 4);
        assert_eq!(computer.output, vec![7, 5]);
    }

    #[test]
    fn test_jump_if_true() {
        let mut computer = IntCodeComputer {
            instruction_pointer: 0,
            memory: vec![5, 4, 5, 99, 1, 6, 1105, 1, 9, 99],
            input: VecDeque::from(vec![]),
            output: VecDeque::from(vec![]),
        };

        // Process instruction using position mode
        let result = computer.process_instruction();
        assert!(result.is_ok());
        assert_eq!(computer.instruction_pointer, 6);

        // Process instruction using immediate mode
        let result = computer.process_instruction();
        assert!(result.is_ok());
        assert_eq!(computer.instruction_pointer, 9);
    }

    #[test]
    fn test_jump_if_false() {
        let mut computer = IntCodeComputer {
            instruction_pointer: 0,
            memory: vec![6, 4, 5, 99, 0, 6, 1106, 0, 9, 99],
            input: VecDeque::from(vec![]),
            output: VecDeque::from(vec![]),
        };

        // Process instruction using position mode
        let result = computer.process_instruction();
        assert!(result.is_ok());
        assert_eq!(computer.instruction_pointer, 6);

        // Process instruction using immediate mode
        let result = computer.process_instruction();
        assert!(result.is_ok());
        assert_eq!(computer.instruction_pointer, 9);
    }

    #[test]
    fn test_less_than() {
        let mut computer = IntCodeComputer {
            instruction_pointer: 0,
            memory: vec![7, 9, 10, 11, 1107, 37, 42, 12, 99, 6, 3, 0, 0],
            input: VecDeque::from(vec![]),
            output: VecDeque::from(vec![]),
        };

        // Process instruction using position mode
        let result = computer.process_instruction();
        assert!(result.is_ok());
        assert_eq!(computer.instruction_pointer, 4);
        assert_eq!(computer.memory, vec![7, 9, 10, 11, 1107, 37, 42, 12, 99, 6, 3, 0, 0]);

        // Process instruction using immediate mode
        let result = computer.process_instruction();
        assert!(result.is_ok());
        assert_eq!(computer.instruction_pointer, 8);
        assert_eq!(computer.memory, vec![7, 9, 10, 11, 1107, 37, 42, 12, 99, 6, 3, 0, 1]);
    }

    #[test]
    fn test_equals() {
        let mut computer = IntCodeComputer {
            instruction_pointer: 0,
            memory: vec![8, 9, 10, 11, 1108, 37, 42, 12, 99, 3, 3, 0, 0],
            input: VecDeque::from(vec![]),
            output: VecDeque::from(vec![]),
        };

        // Process instruction using position mode
        let result = computer.process_instruction();
        assert!(result.is_ok());
        assert_eq!(computer.instruction_pointer, 4);
        assert_eq!(computer.memory, vec![8, 9, 10, 11, 1108, 37, 42, 12, 99, 3, 3, 1, 0]);

        // Process instruction using immediate mode
        let result = computer.process_instruction();
        assert!(result.is_ok());
        assert_eq!(computer.instruction_pointer, 8);
        assert_eq!(computer.memory, vec![8, 9, 10, 11, 1108, 37, 42, 12, 99, 3, 3, 1, 0]);
    }

    #[test]
    fn test_halt() {
        let mut computer = IntCodeComputer {
            instruction_pointer: 0,
            memory: vec![99],
            input: VecDeque::from(vec![]),
            output: VecDeque::from(vec![]),
        };

        let result = computer.process_instruction();
        assert!(result.is_ok());
        assert_eq!(computer.instruction_pointer, 0);
        assert_eq!(computer.memory, vec![99]);
    }

    #[test]
    fn test_run() {
        let input: [IntCodeComputer; 10] = [
            IntCodeComputer {
                memory: vec![1, 0, 0, 0, 99],
                input: VecDeque::from(vec![]),
                output: VecDeque::from(vec![]),
                instruction_pointer: 0,
            },
            IntCodeComputer {
                memory: vec![2, 3, 0, 3, 99],
                input: VecDeque::from(vec![]),
                output: VecDeque::from(vec![]),
                instruction_pointer: 0,
            },
            IntCodeComputer {
                memory: vec![2, 4, 4, 5, 99, 0],
                input: VecDeque::from(vec![]),
                output: VecDeque::from(vec![]),
                instruction_pointer: 0,
            },
            IntCodeComputer {
                memory: vec![1, 1, 1, 4, 99, 5, 6, 0, 99],
                input: VecDeque::from(vec![]),
                output: VecDeque::from(vec![]),
                instruction_pointer: 0,
            },
            IntCodeComputer {
                memory: vec![3, 0, 4, 0, 99],
                input: VecDeque::from(vec![42]),
                output: VecDeque::from(vec![]),
                instruction_pointer: 0,
            },
            IntCodeComputer {
                memory: vec![1002, 4, 3, 4, 33],
                input: VecDeque::from(vec![]),
                output: VecDeque::from(vec![]),
                instruction_pointer: 0,
            },
            IntCodeComputer {
                memory: vec![1101, 100, -1, 4, 0],
                input: VecDeque::from(vec![]),
                output: VecDeque::from(vec![]),
                instruction_pointer: 0,
            },
            IntCodeComputer {
                memory: vec![3, 21, 1008, 21, 8, 20, 1005, 20, 22, 107, 8, 21, 20, 1006, 20, 31, 1106, 0, 36, 98, 0, 0, 1002, 21, 125, 20, 4, 20, 1105, 1, 46, 104, 999, 1105, 1, 46, 1101, 1000, 1, 20, 4, 20, 1105, 1, 46, 98, 99],
                input: VecDeque::from(vec![7]),
                output: VecDeque::from(vec![]),
                instruction_pointer: 0,
            },
            IntCodeComputer {
                memory: vec![3, 21, 1008, 21, 8, 20, 1005, 20, 22, 107, 8, 21, 20, 1006, 20, 31, 1106, 0, 36, 98, 0, 0, 1002, 21, 125, 20, 4, 20, 1105, 1, 46, 104, 999, 1105, 1, 46, 1101, 1000, 1, 20, 4, 20, 1105, 1, 46, 98, 99],
                input: VecDeque::from(vec![8]),
                output: VecDeque::from(vec![]),
                instruction_pointer: 0,
            },
            IntCodeComputer {
                memory: vec![3, 21, 1008, 21, 8, 20, 1005, 20, 22, 107, 8, 21, 20, 1006, 20, 31, 1106, 0, 36, 98, 0, 0, 1002, 21, 125, 20, 4, 20, 1105, 1, 46, 104, 999, 1105, 1, 46, 1101, 1000, 1, 20, 4, 20, 1105, 1, 46, 98, 99],
                input: VecDeque::from(vec![9]),
                output: VecDeque::from(vec![]),
                instruction_pointer: 0,
            },
        ];
        let expected: [IntCodeComputer; 10] = [
            IntCodeComputer {
                memory: vec![2, 0, 0, 0, 99],
                input: VecDeque::from(vec![]),
                output: VecDeque::from(vec![]),
                instruction_pointer: 4,
            },
            IntCodeComputer {
                memory: vec![2, 3, 0, 6, 99],
                input: VecDeque::from(vec![]),
                output: VecDeque::from(vec![]),
                instruction_pointer: 4,
            },
            IntCodeComputer {
                memory: vec![2, 4, 4, 5, 99, 9801],
                input: VecDeque::from(vec![]),
                output: VecDeque::from(vec![]),
                instruction_pointer: 4,
            },
            IntCodeComputer {
                memory: vec![30, 1, 1, 4, 2, 5, 6, 0, 99],
                input: VecDeque::from(vec![]),
                output: VecDeque::from(vec![]),
                instruction_pointer: 8,
            },
            IntCodeComputer {
                memory: vec![42, 0, 4, 0, 99],
                input: VecDeque::from(vec![]),
                output: VecDeque::from(vec![42]),
                instruction_pointer: 4,
            },
            IntCodeComputer {
                memory: vec![1002, 4, 3, 4, 99],
                input: VecDeque::from(vec![]),
                output: VecDeque::from(vec![]),
                instruction_pointer: 4,
            },
            IntCodeComputer {
                memory: vec![1101, 100, -1, 4, 99],
                input: VecDeque::from(vec![]),
                output: VecDeque::from(vec![]),
                instruction_pointer: 4,
            },
            IntCodeComputer {
                memory: vec![3, 21, 1008, 21, 8, 20, 1005, 20, 22, 107, 8, 21, 20, 1006, 20, 31, 1106, 0, 36, 98, 0, 7, 1002, 21, 125, 20, 4, 20, 1105, 1, 46, 104, 999, 1105, 1, 46, 1101, 1000, 1, 20, 4, 20, 1105, 1, 46, 98, 99],
                input: VecDeque::from(vec![]),
                output: VecDeque::from(vec![999]),
                instruction_pointer: 46,
            },
            IntCodeComputer {
                memory: vec![3, 21, 1008, 21, 8, 20, 1005, 20, 22, 107, 8, 21, 20, 1006, 20, 31, 1106, 0, 36, 98, 1000, 8, 1002, 21, 125, 20, 4, 20, 1105, 1, 46, 104, 999, 1105, 1, 46, 1101, 1000, 1, 20, 4, 20, 1105, 1, 46, 98, 99],
                input: VecDeque::from(vec![]),
                output: VecDeque::from(vec![1000]),
                instruction_pointer: 46,
            },
            IntCodeComputer {
                memory: vec![3, 21, 1008, 21, 8, 20, 1005, 20, 22, 107, 8, 21, 20, 1006, 20, 31, 1106, 0, 36, 98, 1001, 9, 1002, 21, 125, 20, 4, 20, 1105, 1, 46, 104, 999, 1105, 1, 46, 1101, 1000, 1, 20, 4, 20, 1105, 1, 46, 98, 99],
                input: VecDeque::from(vec![]),
                output: VecDeque::from(vec![1001]),
                instruction_pointer: 46,
            },
        ];

        for i in 0..input.len() {
            let mut computer = input[i].clone();
            computer.run();
            assert_eq!(computer, expected[i]);
        }
    }
}
