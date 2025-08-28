use std::collections::{HashMap, VecDeque};
use std::fmt;

use log::{error, trace};

use super::point2d::Point2d;

#[derive(Clone, Debug, PartialEq)]
pub struct IntCodeComputer {
    pub memory: Vec<i64>,
    pub input: VecDeque<i64>,
    pub output: VecDeque<i64>,
    instruction_pointer: i64,
    relative_base_pointer: i64,
}

pub enum IntCodeStatus {
    ProgramHalted,
    InputRequired,
    OutputWaiting,
}

impl fmt::Display for IntCodeStatus {
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        match self {
            IntCodeStatus::ProgramHalted => write!(f, "Computer has halted execution"),
            IntCodeStatus::InputRequired => write!(f, "Computer waiting for input to be provided"),
            IntCodeStatus::OutputWaiting => write!(f, "Computer has output waiting"),
        }
    }
}

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
        let mut allocated_memory = vec![0; 10_000];
        allocated_memory[..memory.len()].copy_from_slice(memory);
        IntCodeComputer {
            instruction_pointer: 0,
            relative_base_pointer: 0,
            memory: allocated_memory,
            input: VecDeque::new(),
            output: VecDeque::new(),
        }
    }

    fn get_parameter_address(&self, offset: i64, mode: i64) -> Result<usize, IntCodeError> {
        trace!("calculating parameter address {} in mode {}", offset, mode);
        let parameter = self.memory[(self.instruction_pointer + offset) as usize];
        match mode {
            // Position mode
            0 => Ok(parameter as usize),
            // Relative mode
            2 => Ok((self.relative_base_pointer + parameter) as usize),
            other => {
                error!("Unknown file access mode: {}", other);
                Err(IntCodeError::InvalidFileAccessMode)
            },
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
            // Relative mode
            2 => Ok(self.memory[(self.relative_base_pointer + parameter) as usize]),
            other => {
                error!("Unknown file access mode: {}", other);
                Err(IntCodeError::InvalidFileAccessMode)
            },
        }
    }

    fn process_instruction(&mut self) -> Result<bool, IntCodeError> {
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
                let third_address = self.get_parameter_address(3, third_parameter_mode)?;
                trace!(
                    "Adding ({}) + ({}) = [{}] ({})",
                    first_value,
                    second_value,
                    third_address,
                    first_value + second_value,
                );
                self.memory[third_address] = first_value + second_value;
                self.instruction_pointer += 4;
                Ok(true)
            },
            2 => {
                let first_value = self.get_parameter_value(1, first_parameter_mode)?;
                let second_value = self.get_parameter_value(2, second_parameter_mode)?;
                let third_address = self.get_parameter_address(3, third_parameter_mode)?;
                trace!(
                    "Multiplying ({}) * ({}) = [{}] ({})",
                    first_value,
                    second_value,
                    third_address,
                    first_value * second_value,
                );
                self.memory[third_address] = first_value * second_value;
                self.instruction_pointer += 4;
                Ok(true)
            },
            3 => {
                if let Some(input) = self.input.pop_front() {
                    let first_address = self.get_parameter_address(1, first_parameter_mode)?;
                    trace!(
                        "Storing ({}) -> [{}]",
                        input,
                        first_address,
                    );
                    self.memory[first_address] = input;
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
                let third_address = self.get_parameter_address(3, third_parameter_mode)?;
                trace!(
                    "Checking if ({}) is less than ({}) ({})",
                    first_value,
                    second_value,
                    first_value < second_value,
                );

                if first_value < second_value {
                    self.memory[third_address] = 1;
                } else {
                    self.memory[third_address] = 0;
                }
                self.instruction_pointer += 4;
                Ok(true)
            },
            8 => {
                let first_value = self.get_parameter_value(1, first_parameter_mode)?;
                let second_value = self.get_parameter_value(2, second_parameter_mode)?;
                let third_address = self.get_parameter_address(3, third_parameter_mode)?;
                trace!(
                    "Checking if ({}) is equal to ({}) ({})",
                    first_value,
                    second_value,
                    first_value == second_value,
                );

                if first_value == second_value {
                    self.memory[third_address] = 1;
                } else {
                    self.memory[third_address] = 0;
                }
                self.instruction_pointer += 4;
                Ok(true)
            },
            9 => {
                let first_value = self.get_parameter_value(1, first_parameter_mode)?;
                trace!(
                    "Adjusting relative base pointer ({}) + ({}) = ({})",
                    self.relative_base_pointer,
                    first_value,
                    self.relative_base_pointer + first_value,
                );
                self.relative_base_pointer += first_value;
                self.instruction_pointer += 2;
                Ok(true)
            },
            99 => Ok(false),
            other => {
                error!("Unknown opcode: {}", other);
                Err(IntCodeError::InvalidInstructionOpCode)
            },
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

    pub fn run_interactive(&mut self, output_count: usize) -> Result<IntCodeStatus, IntCodeError> {
        loop {
            match self.process_instruction() {
                Ok(true) => {
                    if self.output.len() == output_count {
                        return Ok(IntCodeStatus::OutputWaiting);
                    }
                },
                Ok(false) => {
                    return Ok(IntCodeStatus::ProgramHalted);
                },
                Err(IntCodeError::NoInputGiven) => {
                    return Ok(IntCodeStatus::InputRequired);
                },
                Err(error) => panic!("Unexpected error: {}", error),
            }
        }
    }
}

pub struct IntCodeDisplay {
    pub pixels: HashMap<Point2d, char>,
}

impl fmt::Display for IntCodeDisplay {
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        // Get boundaries
        let mut min_x = i64::MAX;
        let mut min_y = i64::MAX;
        let mut max_x = i64::MIN;
        let mut max_y = i64::MIN;

        for point in self.pixels.keys() {
            if point.x < min_x {
                min_x = point.x;
            }
            if point.y < min_y {
                min_y = point.y;
            }
            if point.x > max_x {
                max_x = point.x;
            }
            if point.y > max_y {
                max_y = point.y;
            }
        }

        // Get values
        let mut output = String::new();
        for y in min_y..=max_y {
            output.push('\n');
            for x in min_x..=max_x {
                let value = match self.pixels.get(&Point2d::new(x, y)) {
                    Some(value) => *value,
                    None => '.',
                };
                output.push(value);
            }
        }
        write!(f, "{}", output)
    }
}

impl IntCodeDisplay {
    pub fn new() -> Self {
        IntCodeDisplay { pixels: HashMap::new() }
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    // Helper functions for tests
    impl IntCodeComputer {
        pub fn with_instruction_pointer(&mut self, value: i64) -> IntCodeComputer {
            self.instruction_pointer = value;
            self.clone()
        }

        pub fn with_relative_base_pointer(&mut self, value: i64) -> IntCodeComputer {
            self.relative_base_pointer = value;
            self.clone()
        }

        pub fn with_input(&mut self, value: i64) -> IntCodeComputer {
            self.input.push_back(value);
            self.clone()
        }

        pub fn with_output(&mut self, value: i64) -> IntCodeComputer {
            self.output.push_back(value);
            self.clone()
        }

        pub fn with_outputs(&mut self, outputs: Vec<i64>) -> IntCodeComputer {
            self.output = outputs.into();
            self.clone()
        }
    }

    #[test]
    fn test_add() {
        let mut computer = IntCodeComputer::new(&[1, 9, 10, 11, 1101, 27, 12, 12, 99, 7, 9, 0, 0]);

        // Process instruction using position mode
        let result = computer.process_instruction();
        assert!(result.is_ok());
        assert_eq!(computer, IntCodeComputer::new(&[1, 9, 10, 11, 1101, 27, 12, 12, 99, 7, 9, 16, 0])
            .with_instruction_pointer(4));

        // Process instruction using immediate mode
        let result = computer.process_instruction();
        assert!(result.is_ok());
        assert_eq!(computer, IntCodeComputer::new(&[1, 9, 10, 11, 1101, 27, 12, 12, 99, 7, 9, 16, 39])
            .with_instruction_pointer(8));
    }

    #[test]
    fn test_multiply() {
        let mut computer = IntCodeComputer::new(&[2, 9, 10, 11, 1102, 18, 3, 12, 99, 7, 9, 0, 0]);

        // Process instruction using position mode
        let result = computer.process_instruction();
        assert!(result.is_ok());
        assert_eq!(computer, IntCodeComputer::new(&[2, 9, 10, 11, 1102, 18, 3, 12, 99, 7, 9, 63, 0])
            .with_instruction_pointer(4));

        // Process instruction using immediate mode
        let result = computer.process_instruction();
        assert!(result.is_ok());
        assert_eq!(computer, IntCodeComputer::new(&[2, 9, 10, 11, 1102, 18, 3, 12, 99, 7, 9, 63, 54])
            .with_instruction_pointer(8));
    }

    #[test]
    fn test_store() {
        let mut computer = IntCodeComputer::new(&[3, 4, 99, 0, 0])
            .with_input(7);

        let result = computer.process_instruction();
        assert!(result.is_ok());
        assert_eq!(computer, IntCodeComputer::new(&[3, 4, 99, 0, 7])
            .with_instruction_pointer(2));
    }

    #[test]
    fn test_output() {
        let mut computer = IntCodeComputer::new(&[4, 6, 104, 5, 99, 0, 7]);

        // Process instruction using position mode
        let result = computer.process_instruction();
        assert!(result.is_ok());
        assert_eq!(computer, IntCodeComputer::new(&[4, 6, 104, 5, 99, 0, 7])
            .with_instruction_pointer(2)
            .with_outputs(vec![7]));

        // Process instruction using immediate mode
        let result = computer.process_instruction();
        assert!(result.is_ok());
        assert_eq!(computer, IntCodeComputer::new(&[4, 6, 104, 5, 99, 0, 7])
            .with_instruction_pointer(4)
            .with_outputs(vec![7, 5]));
    }

    #[test]
    fn test_jump_if_true() {
        let mut computer = IntCodeComputer::new(&[5, 4, 5, 99, 1, 6, 1105, 1, 9, 99]);

        // Process instruction using position mode
        let result = computer.process_instruction();
        assert!(result.is_ok());
        assert_eq!(computer, IntCodeComputer::new(&[5, 4, 5, 99, 1, 6, 1105, 1, 9, 99])
            .with_instruction_pointer(6));

        // Process instruction using immediate mode
        let result = computer.process_instruction();
        assert!(result.is_ok());
        assert_eq!(computer, IntCodeComputer::new(&[5, 4, 5, 99, 1, 6, 1105, 1, 9, 99])
            .with_instruction_pointer(9));
    }

    #[test]
    fn test_jump_if_false() {
        let mut computer = IntCodeComputer::new(&[6, 4, 5, 99, 0, 6, 1106, 0, 9, 99]);

        // Process instruction using position mode
        let result = computer.process_instruction();
        assert!(result.is_ok());
        assert_eq!(computer, IntCodeComputer::new(&[6, 4, 5, 99, 0, 6, 1106, 0, 9, 99])
            .with_instruction_pointer(6));

        // Process instruction using immediate mode
        let result = computer.process_instruction();
        assert!(result.is_ok());
        assert_eq!(computer, IntCodeComputer::new(&[6, 4, 5, 99, 0, 6, 1106, 0, 9, 99])
            .with_instruction_pointer(9));
    }

    #[test]
    fn test_less_than() {
        let mut computer = IntCodeComputer::new(&[7, 9, 10, 11, 1107, 37, 42, 12, 99, 6, 3, 0, 0]);

        // Process instruction using position mode
        let result = computer.process_instruction();
        assert!(result.is_ok());
        assert_eq!(computer, IntCodeComputer::new(&[7, 9, 10, 11, 1107, 37, 42, 12, 99, 6, 3, 0, 0])
            .with_instruction_pointer(4));

        // Process instruction using immediate mode
        let result = computer.process_instruction();
        assert!(result.is_ok());
        assert_eq!(computer, IntCodeComputer::new(&[7, 9, 10, 11, 1107, 37, 42, 12, 99, 6, 3, 0, 1])
            .with_instruction_pointer(8));
    }

    #[test]
    fn test_equals() {
        let mut computer = IntCodeComputer::new(&[8, 9, 10, 11, 1108, 37, 42, 12, 99, 3, 3, 0, 0]);

        // Process instruction using position mode
        let result = computer.process_instruction();
        assert!(result.is_ok());
        assert_eq!(computer, IntCodeComputer::new(&[8, 9, 10, 11, 1108, 37, 42, 12, 99, 3, 3, 1, 0])
            .with_instruction_pointer(4));

        // Process instruction using immediate mode
        let result = computer.process_instruction();
        assert!(result.is_ok());
        assert_eq!(computer, IntCodeComputer::new(&[8, 9, 10, 11, 1108, 37, 42, 12, 99, 3, 3, 1, 0])
            .with_instruction_pointer(8));
    }

    #[test]
    fn test_relative_base() {
        let mut computer = IntCodeComputer::new(&[9, 4, 109, 5, -5, 5])
            .with_relative_base_pointer(42);

        // Process instruction using position mode
        let result = computer.process_instruction();
        assert!(result.is_ok());
        assert_eq!(computer, IntCodeComputer::new(&[9, 4, 109, 5, -5, 5])
            .with_instruction_pointer(2)
            .with_relative_base_pointer(37));

        // Process instruction using immediate mode
        let result = computer.process_instruction();
        assert!(result.is_ok());
        assert_eq!(computer, IntCodeComputer::new(&[9, 4, 109, 5, -5, 5])
            .with_instruction_pointer(4)
            .with_relative_base_pointer(42));
    }

    #[test]
    fn test_halt() {
        let mut computer = IntCodeComputer::new(&[99]);

        let result = computer.process_instruction();
        assert!(result.is_ok());
        assert_eq!(computer, IntCodeComputer::new(&[99]));
    }

    #[test]
    fn test_run() {
        let input: [IntCodeComputer; 13] = [
            IntCodeComputer::new(&[1, 0, 0, 0, 99]),
            IntCodeComputer::new(&[2, 3, 0, 3, 99]),
            IntCodeComputer::new(&[2, 4, 4, 5, 99, 0]),
            IntCodeComputer::new(&[1, 1, 1, 4, 99, 5, 6, 0, 99]),
            IntCodeComputer::new(&[3, 0, 4, 0, 99])
                .with_input(42),
            IntCodeComputer::new(&[1002, 4, 3, 4, 33]),
            IntCodeComputer::new(&[1101, 100, -1, 4, 0]),
            IntCodeComputer::new(&[3, 21, 1008, 21, 8, 20, 1005, 20, 22, 107, 8, 21, 20, 1006, 20, 31, 1106,
                                       0, 36, 98, 0, 0, 1002, 21, 125, 20, 4, 20, 1105, 1, 46, 104, 999, 1105, 1, 46,
                                       1101, 1000, 1, 20, 4, 20, 1105, 1, 46, 98, 99])
                .with_input(7),
            IntCodeComputer::new(&[3, 21, 1008, 21, 8, 20, 1005, 20, 22, 107, 8, 21, 20, 1006, 20, 31, 1106,
                                       0, 36, 98, 0, 0, 1002, 21, 125, 20, 4, 20, 1105, 1, 46, 104, 999, 1105, 1, 46,
                                       1101, 1000, 1, 20, 4, 20, 1105, 1, 46, 98, 99])
                .with_input(8),
            IntCodeComputer::new(&[3, 21, 1008, 21, 8, 20, 1005, 20, 22, 107, 8, 21, 20, 1006, 20, 31, 1106,
                                       0, 36, 98, 0, 0, 1002, 21, 125, 20, 4, 20, 1105, 1, 46, 104, 999, 1105, 1, 46,
                                       1101, 1000, 1, 20, 4, 20, 1105, 1, 46, 98, 99])
                .with_input(9),
            IntCodeComputer::new(&[109, 1, 204, -1, 1001, 100, 1, 100, 1008, 100, 16, 101, 1006, 101, 0, 99]),
            IntCodeComputer::new(&[1102, 34915192, 34915192, 7, 4, 7, 99, 0]),
            IntCodeComputer::new(&[104, 1125899906842624, 99]),
        ];
        let expected: [IntCodeComputer; 13] = [
            IntCodeComputer::new(&[2, 0, 0, 0, 99])
                .with_instruction_pointer(4),
            IntCodeComputer::new(&[2, 3, 0, 6, 99])
                .with_instruction_pointer(4),
            IntCodeComputer::new(&[2, 4, 4, 5, 99, 9801])
                .with_instruction_pointer(4),
            IntCodeComputer::new(&[30, 1, 1, 4, 2, 5, 6, 0, 99])
                .with_instruction_pointer(8),
            IntCodeComputer::new(&[42, 0, 4, 0, 99])
                .with_instruction_pointer(4)
                .with_output(42),
            IntCodeComputer::new(&[1002, 4, 3, 4, 99])
                .with_instruction_pointer(4),
            IntCodeComputer::new(&[1101, 100, -1, 4, 99])
                .with_instruction_pointer(4),
            IntCodeComputer::new(&[3, 21, 1008, 21, 8, 20, 1005, 20, 22, 107, 8, 21, 20, 1006, 20, 31, 1106,
                                       0, 36, 98, 0, 7, 1002, 21, 125, 20, 4, 20, 1105, 1, 46, 104, 999, 1105, 1, 46,
                                       1101, 1000, 1, 20, 4, 20, 1105, 1, 46, 98, 99])
                .with_instruction_pointer(46)
                .with_output(999),
            IntCodeComputer::new(&[3, 21, 1008, 21, 8, 20, 1005, 20, 22, 107, 8, 21, 20, 1006, 20, 31, 1106,
                                       0, 36, 98, 1000, 8, 1002, 21, 125, 20, 4, 20, 1105, 1, 46, 104, 999, 1105, 1, 46,
                                       1101, 1000, 1, 20, 4, 20, 1105, 1, 46, 98, 99])
                .with_instruction_pointer(46)
                .with_output(1000),
            IntCodeComputer::new(&[3, 21, 1008, 21, 8, 20, 1005, 20, 22, 107, 8, 21, 20, 1006, 20, 31, 1106,
                                       0, 36, 98, 1001, 9, 1002, 21, 125, 20, 4, 20, 1105, 1, 46, 104, 999, 1105, 1, 46,
                                       1101, 1000, 1, 20, 4, 20, 1105, 1, 46, 98, 99])
                .with_instruction_pointer(46)
                .with_output(1001),
            IntCodeComputer::new(&[
                109, 1, 204, -1, 1001, 100, 1, 100, 1008, 100, 16, 101, 1006, 101, 0, 99, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 16, 1])
                .with_instruction_pointer(15)
                .with_relative_base_pointer(16)
                .with_outputs(vec![109, 1, 204, -1, 1001, 100, 1, 100, 1008, 100, 16, 101, 1006, 101, 0, 99]),
            IntCodeComputer::new(&[1102, 34915192, 34915192, 7, 4, 7, 99, 1219070632396864])
                .with_instruction_pointer(6)
                .with_output(1219070632396864),
            IntCodeComputer::new(&[104, 1125899906842624, 99])
                .with_instruction_pointer(2)
                .with_output(1125899906842624),
        ];

        for i in 0..input.len() {
            let mut computer = input[i].clone();
            computer.run();
            assert_eq!(computer, expected[i]);
        }
    }
}
