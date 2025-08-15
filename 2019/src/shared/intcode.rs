#[derive(Debug)]
pub struct IntCodeComputer {
    pub memory: Vec<usize>,
    instruction_pointer: usize,
}

pub fn initialize_computer(memory: &[usize]) -> IntCodeComputer {
    IntCodeComputer {
        instruction_pointer: 0,
        memory: memory.to_owned(),
    }
}

impl IntCodeComputer {
    fn process_instruction(&mut self) -> bool {
        match self.memory[self.instruction_pointer] {
            1 => {
                let first_address = self.memory[self.instruction_pointer + 1];
                let second_address = self.memory[self.instruction_pointer + 2];
                let third_address = self.memory[self.instruction_pointer + 3];
                println!(
                    "Adding [{}] ({}) + [{}] ({}) = [{}] ({})",
                    first_address,
                    self.memory[first_address],
                    second_address,
                    self.memory[second_address],
                    third_address,
                    self.memory[first_address] + self.memory[second_address],
                );
                self.memory[third_address] = self.memory[first_address] + self.memory[second_address];
                self.instruction_pointer += 4;
                true
            },
            2 => {
                let first_address = self.memory[self.instruction_pointer + 1];
                let second_address = self.memory[self.instruction_pointer + 2];
                let third_address = self.memory[self.instruction_pointer + 3];
                println!(
                    "Multiplying [{}] ({}) + [{}] ({}) = [{}] ({})",
                    first_address,
                    self.memory[first_address],
                    second_address,
                    self.memory[second_address],
                    third_address,
                    self.memory[first_address] + self.memory[second_address],
                );
                self.memory[third_address] = self.memory[first_address] * self.memory[second_address];
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
            memory: vec![1, 5, 6, 7, 99, 7, 9, 0],
        };

        let result = computer.process_instruction();
        assert!(result);
        assert_eq!(computer.instruction_pointer, 4);
        assert_eq!(computer.memory, vec![1, 5, 6, 7, 99, 7, 9, 16]);
    }

    #[test]
    fn test_multiply() {
        let mut computer = IntCodeComputer {
            instruction_pointer: 0,
            memory: vec![2, 5, 6, 7, 99, 7, 9, 0],
        };

        let result = computer.process_instruction();
        assert!(result);
        assert_eq!(computer.instruction_pointer, 4);
        assert_eq!(computer.memory, vec![2, 5, 6, 7, 99, 7, 9, 63]);
    }

    #[test]
    fn test_halt() {
        let mut computer = IntCodeComputer {
            instruction_pointer: 0,
            memory: vec![99],
        };

        let result = computer.process_instruction();
        assert!(!result);
        assert_eq!(computer.instruction_pointer, 0);
        assert_eq!(computer.memory, vec![99]);
    }

    #[test]
    fn test_run() {
        let input: [Vec<usize>; 4] = [
            vec![1, 0, 0, 0, 99],
            vec![2, 3, 0, 3, 99],
            vec![2, 4, 4, 5, 99, 0],
            vec![1, 1, 1, 4, 99, 5, 6, 0, 99],
        ];
        let expected: [Vec<usize>; 4] = [
            vec![2, 0, 0, 0, 99],
            vec![2, 3, 0, 6, 99],
            vec![2, 4, 4, 5, 99, 9801],
            vec![30, 1, 1, 4, 2, 5, 6, 0, 99],
        ];

        for i in 0..input.len() {
            let mut computer = IntCodeComputer {
                instruction_pointer: 0,
                memory: input[i].clone(),
            };
            computer.run();
            assert_eq!(computer.memory, expected[i]);
        }
    }
}
