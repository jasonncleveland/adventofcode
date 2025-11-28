use std::fmt;

#[derive(Clone)]
pub enum Operation {
    Set,
    Subtract,
    Multiply,
    JumpIfNotEqualToZero,
}

impl fmt::Display for Operation {
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        write!(
            f,
            "{}",
            match self {
                Operation::Set => "set",
                Operation::Subtract => "sub",
                Operation::Multiply => "mul",
                Operation::JumpIfNotEqualToZero => "jnz",
            }
        )
    }
}

impl Operation {
    pub fn from_str(operation: &str) -> Operation {
        match operation {
            "set" => Operation::Set,
            "sub" => Operation::Subtract,
            "mul" => Operation::Multiply,
            "jnz" => Operation::JumpIfNotEqualToZero,
            other => unreachable!("Unknown operation: {}", other),
        }
    }
}

#[derive(Clone, Copy)]
pub struct InstructionArgument {
    pub register: Option<char>,
    pub value: Option<i64>,
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

impl InstructionArgument {
    pub fn from_str(arg: &str) -> InstructionArgument {
        if let Ok(value_a) = arg.parse::<i64>() {
            InstructionArgument {
                register: None,
                value: Some(value_a),
            }
        } else if let Some(register_a) = arg.chars().next() {
            InstructionArgument {
                register: Some(register_a),
                value: None,
            }
        } else {
            unreachable!("Instruction must be either a char or integer value")
        }
    }
}

#[derive(Clone)]
pub struct Instruction {
    pub operation: Operation,
    pub args: Vec<InstructionArgument>,
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
