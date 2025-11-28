use std::fmt;

#[derive(Clone)]
pub enum Operation {
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

impl Operation {
    pub fn from_str(operation: &str) -> Operation {
        match operation {
            "set" => Operation::Set,
            "add" => Operation::Add,
            "mul" => Operation::Multiply,
            "mod" => Operation::Modulo,
            "jgz" => Operation::JumpIfGreaterThanZero,
            "snd" => Operation::Send,
            "rcv" => Operation::Receive,
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
