use log::{trace};

#[derive(Clone, Debug, PartialEq)]
pub struct IntCodeComputer {
    pub memory: Vec<i64>,
    pub input: Vec<i64>,
    pub output: Vec<i64>,
    instruction_pointer: i64,
}

pub fn initialize_computer(memory: &[i64]) -> IntCodeComputer {
    IntCodeComputer {
        instruction_pointer: 0,
        memory: memory.to_owned(),
        input: Vec::new(),
        output: Vec::new(),
    }
}

impl IntCodeComputer {
    fn get_parameter_value(&self, offset: i64, mode: i64) -> i64 {
        trace!("retrieving parameter {} in mode {}", offset, mode);
        let parameter = self.memory[(self.instruction_pointer + offset) as usize];
        match mode {
            // Position mode
            0 => self.memory[parameter as usize],
            // Immediate mode
            1 => parameter,
            _ => panic!("invalid mode"),
        }
    }

    fn process_instruction(&mut self) -> bool {
        trace!("Processing instruction: [{}] ({})", self.instruction_pointer, self.memory[self.instruction_pointer as usize]);

        let instruction = self.memory[self.instruction_pointer as usize];
        let opcode = instruction % 100;
        let first_parameter_mode = (instruction % 1000) / 100;
        let second_parameter_mode = (instruction % 10000) / 1000;
        let third_parameter_mode = (instruction % 100000) / 10000;

        trace!("Processing opcode {} in modes {}/{}/{}", opcode, first_parameter_mode, second_parameter_mode, third_parameter_mode);
        match opcode {
            1 => {
                let first_value = self.get_parameter_value(1, first_parameter_mode);
                let second_value = self.get_parameter_value(2, second_parameter_mode);
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
                true
            },
            2 => {
                let first_value = self.get_parameter_value(1, first_parameter_mode);
                let second_value = self.get_parameter_value(2, second_parameter_mode);
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
                true
            },
            3 => {
                if let Some(input) = self.input.pop() {
                    // Write parameters will never be in immediate mode
                    let first_address = self.memory[self.instruction_pointer as usize + 1];
                    trace!(
                        "Storing ({}) -> [{}]",
                        input,
                        first_address,
                    );
                    self.memory[first_address as usize] = input;
                    self.instruction_pointer += 2;
                    return true;
                }
                panic!("no input value given to store instruction");
            },
            4 => {
                let first_value = self.get_parameter_value(1, first_parameter_mode);
                trace!(
                    "Outputting ({})",
                    first_value,
                );
                self.output.push(first_value);
                self.instruction_pointer += 2;
                true
            },
            5 => {
                let first_value = self.get_parameter_value(1, first_parameter_mode);
                trace!(
                    "Checking if ({}) is non-zero ({})",
                    first_value,
                    first_value != 0,
                );
                if first_value != 0 {
                    let second_value = self.get_parameter_value(2, second_parameter_mode);
                    trace!(
                        "Jumping to [{}]",
                        second_value,
                    );
                    self.instruction_pointer = second_value;
                } else {
                    self.instruction_pointer += 3;
                }
                true
            },
            6 => {
                let first_value = self.get_parameter_value(1, first_parameter_mode);
                trace!(
                    "Checking if ({}) is zero ({})",
                    first_value,
                    first_value == 0,
                );
                if first_value == 0 {
                    let second_value = self.get_parameter_value(2, second_parameter_mode);
                    trace!(
                        "Jumping to [{}]",
                        second_value,
                    );
                    self.instruction_pointer = second_value;
                } else {
                    self.instruction_pointer += 3;
                }
                true
            },
            7 => {
                let first_value = self.get_parameter_value(1, first_parameter_mode);
                let second_value = self.get_parameter_value(2, second_parameter_mode);
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
                true
            },
            8 => {
                let first_value = self.get_parameter_value(1, first_parameter_mode);
                let second_value = self.get_parameter_value(2, second_parameter_mode);
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
                true
            },
            99 => false,
            other => panic!("Invalid instruction opcode: {}", other),
        }
    }

    pub fn run(&mut self) {
        while self.process_instruction() {}
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
            input: vec![],
            output: vec![],
        };

        // Process instruction using position mode
        let result = computer.process_instruction();
        assert!(result);
        assert_eq!(computer.instruction_pointer, 4);
        assert_eq!(computer.memory, vec![1, 9, 10, 11, 1101, 27, 12, 12, 99, 7, 9, 16, 0]);

        // Process instruction using immediate mode
        let result = computer.process_instruction();
        assert!(result);
        assert_eq!(computer.instruction_pointer, 8);
        assert_eq!(computer.memory, vec![1, 9, 10, 11, 1101, 27, 12, 12, 99, 7, 9, 16, 39]);
    }

    #[test]
    fn test_multiply() {
        let mut computer = IntCodeComputer {
            instruction_pointer: 0,
            memory: vec![2, 9, 10, 11, 1102, 18, 3, 12, 99, 7, 9, 0, 0],
            input: vec![],
            output: vec![],
        };

        // Process instruction using position mode
        let result = computer.process_instruction();
        assert!(result);
        assert_eq!(computer.instruction_pointer, 4);
        assert_eq!(computer.memory, vec![2, 9, 10, 11, 1102, 18, 3, 12, 99, 7, 9, 63, 0]);

        // Process instruction using immediate mode
        let result = computer.process_instruction();
        assert!(result);
        assert_eq!(computer.instruction_pointer, 8);
        assert_eq!(computer.memory, vec![2, 9, 10, 11, 1102, 18, 3, 12, 99, 7, 9, 63, 54]);
    }

    #[test]
    fn test_store() {
        let mut computer = IntCodeComputer {
            instruction_pointer: 0,
            memory: vec![3, 4, 99, 0, 0],
            input: vec![7],
            output: vec![],
        };

        let result = computer.process_instruction();
        assert!(result);
        assert_eq!(computer.instruction_pointer, 2);
        assert_eq!(computer.memory, vec![3, 4, 99, 0, 7]);
    }

    #[test]
    fn test_output() {
        let mut computer = IntCodeComputer {
            instruction_pointer: 0,
            memory: vec![4, 6, 104, 5, 99, 0, 7],
            input: vec![7],
            output: vec![],
        };


        // Process instruction using position mode
        let result = computer.process_instruction();
        assert!(result);
        assert_eq!(computer.instruction_pointer, 2);
        assert_eq!(computer.output, vec![7]);

        // Process instruction using immediate mode
        let result = computer.process_instruction();
        assert!(result);
        assert_eq!(computer.instruction_pointer, 4);
        assert_eq!(computer.output, vec![7, 5]);
    }

    #[test]
    fn test_jump_if_true() {
        let mut computer = IntCodeComputer {
            instruction_pointer: 0,
            memory: vec![5, 4, 5, 99, 1, 6, 1105, 1, 9, 99],
            input: vec![],
            output: vec![],
        };

        // Process instruction using position mode
        let result = computer.process_instruction();
        assert!(result);
        assert_eq!(computer.instruction_pointer, 6);

        // Process instruction using immediate mode
        let result = computer.process_instruction();
        assert!(result);
        assert_eq!(computer.instruction_pointer, 9);
    }

    #[test]
    fn test_jump_if_false() {
        let mut computer = IntCodeComputer {
            instruction_pointer: 0,
            memory: vec![6, 4, 5, 99, 0, 6, 1106, 0, 9, 99],
            input: vec![],
            output: vec![],
        };

        // Process instruction using position mode
        let result = computer.process_instruction();
        assert!(result);
        assert_eq!(computer.instruction_pointer, 6);

        // Process instruction using immediate mode
        let result = computer.process_instruction();
        assert!(result);
        assert_eq!(computer.instruction_pointer, 9);
    }

    #[test]
    fn test_less_than() {
        let mut computer = IntCodeComputer {
            instruction_pointer: 0,
            memory: vec![7, 9, 10, 11, 1107, 37, 42, 12, 99, 6, 3, 0, 0],
            input: vec![],
            output: vec![],
        };

        // Process instruction using position mode
        let result = computer.process_instruction();
        assert!(result);
        assert_eq!(computer.instruction_pointer, 4);
        assert_eq!(computer.memory, vec![7, 9, 10, 11, 1107, 37, 42, 12, 99, 6, 3, 0, 0]);

        // Process instruction using immediate mode
        let result = computer.process_instruction();
        assert!(result);
        assert_eq!(computer.instruction_pointer, 8);
        assert_eq!(computer.memory, vec![7, 9, 10, 11, 1107, 37, 42, 12, 99, 6, 3, 0, 1]);
    }

    #[test]
    fn test_equals() {
        let mut computer = IntCodeComputer {
            instruction_pointer: 0,
            memory: vec![8, 9, 10, 11, 1108, 37, 42, 12, 99, 3, 3, 0, 0],
            input: vec![],
            output: vec![],
        };

        // Process instruction using position mode
        let result = computer.process_instruction();
        assert!(result);
        assert_eq!(computer.instruction_pointer, 4);
        assert_eq!(computer.memory, vec![8, 9, 10, 11, 1108, 37, 42, 12, 99, 3, 3, 1, 0]);

        // Process instruction using immediate mode
        let result = computer.process_instruction();
        assert!(result);
        assert_eq!(computer.instruction_pointer, 8);
        assert_eq!(computer.memory, vec![8, 9, 10, 11, 1108, 37, 42, 12, 99, 3, 3, 1, 0]);
    }

    #[test]
    fn test_halt() {
        let mut computer = IntCodeComputer {
            instruction_pointer: 0,
            memory: vec![99],
            input: vec![],
            output: vec![],
        };

        let result = computer.process_instruction();
        assert!(!result);
        assert_eq!(computer.instruction_pointer, 0);
        assert_eq!(computer.memory, vec![99]);
    }

    #[test]
    fn test_run() {
        let input: [IntCodeComputer; 10] = [
            IntCodeComputer {
                memory: vec![1, 0, 0, 0, 99],
                input: vec![],
                output: vec![],
                instruction_pointer: 0,
            },
            IntCodeComputer {
                memory: vec![2, 3, 0, 3, 99],
                input: vec![],
                output: vec![],
                instruction_pointer: 0,
            },
            IntCodeComputer {
                memory: vec![2, 4, 4, 5, 99, 0],
                input: vec![],
                output: vec![],
                instruction_pointer: 0,
            },
            IntCodeComputer {
                memory: vec![1, 1, 1, 4, 99, 5, 6, 0, 99],
                input: vec![],
                output: vec![],
                instruction_pointer: 0,
            },
            IntCodeComputer {
                memory: vec![3, 0, 4, 0, 99],
                input: vec![42],
                output: vec![],
                instruction_pointer: 0,
            },
            IntCodeComputer {
                memory: vec![1002, 4, 3, 4, 33],
                input: vec![],
                output: vec![],
                instruction_pointer: 0,
            },
            IntCodeComputer {
                memory: vec![1101, 100, -1, 4, 0],
                input: vec![],
                output: vec![],
                instruction_pointer: 0,
            },
            IntCodeComputer {
                memory: vec![3, 21, 1008, 21, 8, 20, 1005, 20, 22, 107, 8, 21, 20, 1006, 20, 31, 1106, 0, 36, 98, 0, 0, 1002, 21, 125, 20, 4, 20, 1105, 1, 46, 104, 999, 1105, 1, 46, 1101, 1000, 1, 20, 4, 20, 1105, 1, 46, 98, 99],
                input: vec![7],
                output: vec![],
                instruction_pointer: 0,
            },
            IntCodeComputer {
                memory: vec![3, 21, 1008, 21, 8, 20, 1005, 20, 22, 107, 8, 21, 20, 1006, 20, 31, 1106, 0, 36, 98, 0, 0, 1002, 21, 125, 20, 4, 20, 1105, 1, 46, 104, 999, 1105, 1, 46, 1101, 1000, 1, 20, 4, 20, 1105, 1, 46, 98, 99],
                input: vec![8],
                output: vec![],
                instruction_pointer: 0,
            },
            IntCodeComputer {
                memory: vec![3, 21, 1008, 21, 8, 20, 1005, 20, 22, 107, 8, 21, 20, 1006, 20, 31, 1106, 0, 36, 98, 0, 0, 1002, 21, 125, 20, 4, 20, 1105, 1, 46, 104, 999, 1105, 1, 46, 1101, 1000, 1, 20, 4, 20, 1105, 1, 46, 98, 99],
                input: vec![9],
                output: vec![],
                instruction_pointer: 0,
            },
        ];
        let expected: [IntCodeComputer; 10] = [
            IntCodeComputer {
                memory: vec![2, 0, 0, 0, 99],
                input: vec![],
                output: vec![],
                instruction_pointer: 4,
            },
            IntCodeComputer {
                memory: vec![2, 3, 0, 6, 99],
                input: vec![],
                output: vec![],
                instruction_pointer: 4,
            },
            IntCodeComputer {
                memory: vec![2, 4, 4, 5, 99, 9801],
                input: vec![],
                output: vec![],
                instruction_pointer: 4,
            },
            IntCodeComputer {
                memory: vec![30, 1, 1, 4, 2, 5, 6, 0, 99],
                input: vec![],
                output: vec![],
                instruction_pointer: 8,
            },
            IntCodeComputer {
                memory: vec![42, 0, 4, 0, 99],
                input: vec![],
                output: vec![42],
                instruction_pointer: 4,
            },
            IntCodeComputer {
                memory: vec![1002, 4, 3, 4, 99],
                input: vec![],
                output: vec![],
                instruction_pointer: 4,
            },
            IntCodeComputer {
                memory: vec![1101, 100, -1, 4, 99],
                input: vec![],
                output: vec![],
                instruction_pointer: 4,
            },
            IntCodeComputer {
                memory: vec![3, 21, 1008, 21, 8, 20, 1005, 20, 22, 107, 8, 21, 20, 1006, 20, 31, 1106, 0, 36, 98, 0, 7, 1002, 21, 125, 20, 4, 20, 1105, 1, 46, 104, 999, 1105, 1, 46, 1101, 1000, 1, 20, 4, 20, 1105, 1, 46, 98, 99],
                input: vec![],
                output: vec![999],
                instruction_pointer: 46,
            },
            IntCodeComputer {
                memory: vec![3, 21, 1008, 21, 8, 20, 1005, 20, 22, 107, 8, 21, 20, 1006, 20, 31, 1106, 0, 36, 98, 1000, 8, 1002, 21, 125, 20, 4, 20, 1105, 1, 46, 104, 999, 1105, 1, 46, 1101, 1000, 1, 20, 4, 20, 1105, 1, 46, 98, 99],
                input: vec![],
                output: vec![1000],
                instruction_pointer: 46,
            },
            IntCodeComputer {
                memory: vec![3, 21, 1008, 21, 8, 20, 1005, 20, 22, 107, 8, 21, 20, 1006, 20, 31, 1106, 0, 36, 98, 1001, 9, 1002, 21, 125, 20, 4, 20, 1105, 1, 46, 104, 999, 1105, 1, 46, 1101, 1000, 1, 20, 4, 20, 1105, 1, 46, 98, 99],
                input: vec![],
                output: vec![1001],
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
