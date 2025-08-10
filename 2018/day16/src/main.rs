use std::collections::HashMap;
use std::env;
use std::fs;
use std::time;

fn main() {
    let args: Vec<String> = env::args().collect();
    if args.len() < 2 {
        panic!("Must pass filename as argument");
    }

    let input_timer = time::Instant::now();
    let file_name = &args[1];
    let file_contents = read_file(file_name);
    println!("File read: ({:?})", input_timer.elapsed());

    let part1_timer = time::Instant::now();
    let part1 = part1(&file_contents);
    println!("Part 1: {} ({:?})", part1, part1_timer.elapsed());

    let part2_timer = time::Instant::now();
    let part2 = part2(&file_contents);
    println!("Part 2: {} ({:?})", part2, part2_timer.elapsed());
}

fn read_file(file_name: &str) -> String {
    fs::read_to_string(file_name)
        .expect("Something went wrong reading the file")
}

fn parse_input(file_contents: &str) -> (Vec<Sample>, Vec<Instruction>) {
    let mut samples: Vec<Sample> = Vec::new();
    let mut instructions: Vec<Instruction> = Vec::new();

    let (left, right) = file_contents.split_once("\n\n\n\n").unwrap();

    // Parse samples
    for line in left.split("\n\n") {
        let sample = line.split('\n').collect::<Vec<&str>>();
        let mut before = sample[0].chars();
        let instruction = sample[1].split(' ').map(|i| i.parse::<usize>().unwrap()).collect::<Vec<usize>>();
        let mut after = sample[2].chars();
        samples.push(Sample {
            instruction: Instruction {
                opcode: instruction[0],
                a: instruction[1],
                b: instruction[2],
                c: instruction[3],
            },
            before: [
                before.nth(9).unwrap().to_digit(10).unwrap() as usize,
                before.nth(2).unwrap().to_digit(10).unwrap() as usize,
                before.nth(2).unwrap().to_digit(10).unwrap() as usize,
                before.nth(2).unwrap().to_digit(10).unwrap() as usize,
            ],
            after: [
                after.nth(9).unwrap().to_digit(10).unwrap() as usize,
                after.nth(2).unwrap().to_digit(10).unwrap() as usize,
                after.nth(2).unwrap().to_digit(10).unwrap() as usize,
                after.nth(2).unwrap().to_digit(10).unwrap() as usize,
            ],
        });
    }

    // Parse instructions
    for line in right.lines() {
        let instruction = line.split(' ').map(|i| i.parse::<usize>().unwrap()).collect::<Vec<usize>>();
        instructions.push(Instruction {
            opcode: instruction[0],
            a: instruction[1],
            b: instruction[2],
            c: instruction[3],
        })
    }

    (samples, instructions)
}

fn part1(file_contents: &str) -> i64 {
    let (samples, _) = parse_input(file_contents);

    let opcodes: Vec<&str> = vec![
        "addr",
        "addi",
        "mulr",
        "muli",
        "banr",
        "bani",
        "borr",
        "bori",
        "setr",
        "seti",
        "gtir",
        "gtri",
        "gtrr",
        "eqir",
        "eqri",
        "eqrr",
    ];

    let mut total = 0;
    for sample in samples {
        let mut match_count = 0;
        for opcode_name in opcodes.clone() {
            let mut registers = sample.before;
            process_instruction(&mut registers, &sample.instruction, opcode_name);
            if registers == sample.after {
                match_count += 1;
            }
        }
        if match_count >= 3 {
            total += 1;
        }
    }
    total
}

fn part2(file_contents: &str) -> usize {
    let (samples, instructions) = parse_input(file_contents);

    let mut opcodes: Vec<&str> = vec![
        "addr",
        "addi",
        "mulr",
        "muli",
        "banr",
        "bani",
        "borr",
        "bori",
        "setr",
        "seti",
        "gtir",
        "gtri",
        "gtrr",
        "eqir",
        "eqri",
        "eqrr",
    ];
    let mut opcode_map: HashMap<usize, &str> = HashMap::new();

    // Create map of opcodes to instruction names
    for sample in samples {
        let mut match_count = 0;
        let mut possible_opcodes: Vec<&str> = Vec::new();
        for opcode_name in opcodes.clone() {
            let mut registers = sample.before;
            process_instruction(&mut registers, &sample.instruction, opcode_name);
            if registers == sample.after {
                possible_opcodes.push(opcode_name);
                match_count += 1;
            }
        }
        if match_count == 1 {
            if let Some(name) = possible_opcodes.first() {
                opcode_map.insert(sample.instruction.opcode, *name);
                if let Some(index) = opcodes.iter().position(|op| op == name) {
                    opcodes.remove(index);
                }
            }
        }
    }

    let mut registers = [0, 0, 0, 0];

    // Process instructions
    for instruction in instructions {
        if let Some(opcode_name) = opcode_map.get(&instruction.opcode) {
            process_instruction(&mut registers, &instruction, opcode_name);
        }
    }
    registers[0]
}

fn process_instruction(registers: &mut [usize; 4], instruction: &Instruction, opcode: &str) {
    match opcode {
        "addr" => registers[instruction.c] = registers[instruction.a] + registers[instruction.b],
        "addi" => registers[instruction.c] = registers[instruction.a] + instruction.b,
        "mulr" => registers[instruction.c] = registers[instruction.a] * registers[instruction.b],
        "muli" => registers[instruction.c] = registers[instruction.a] * instruction.b,
        "banr" => registers[instruction.c] = registers[instruction.a] & registers[instruction.b],
        "bani" => registers[instruction.c] = registers[instruction.a] & instruction.b,
        "borr" => registers[instruction.c] = registers[instruction.a] | registers[instruction.b],
        "bori" => registers[instruction.c] = registers[instruction.a] | instruction.b,
        "setr" => registers[instruction.c] = registers[instruction.a],
        "seti" => registers[instruction.c] = instruction.a,
        "gtir" => registers[instruction.c] = match instruction.a > registers[instruction.b] {
            true => 1,
            false => 0,
        },
        "gtri" => registers[instruction.c] = match registers[instruction.a] > instruction.b {
            true => 1,
            false => 0,
        },
        "gtrr" => registers[instruction.c] = match registers[instruction.a] > registers[instruction.b] {
            true => 1,
            false => 0,
        },
        "eqir" => registers[instruction.c] = match instruction.a == registers[instruction.b] {
            true => 1,
            false => 0,
        },
        "eqri" => registers[instruction.c] = match registers[instruction.a] == instruction.b {
            true => 1,
            false => 0,
        },
        "eqrr" => registers[instruction.c] = match registers[instruction.a] == registers[instruction.b] {
            true => 1,
            false => 0,
        },
        _ => panic!("Invalid instruction opcode")
    }
}

#[derive(Debug)]
struct Sample {
    instruction: Instruction,
    before: [usize; 4],
    after: [usize; 4],
}

#[derive(Clone, Debug)]
struct Instruction {
    opcode: usize,
    a: usize,
    b: usize,
    c: usize,
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_process_instruction() {
        let input: [([usize; 4], Instruction, &str); 22] = [
            ([6, 3, 0, 0], Instruction { opcode: 0, a: 0, b: 1, c: 3 }, "addr"),
            ([3, 0, 0, 0], Instruction { opcode: 1, a: 0, b: 5, c: 3 }, "addi"),
            ([2, 3, 0, 0], Instruction { opcode: 2, a: 0, b: 1, c: 3 }, "mulr"),
            ([3, 0, 0, 0], Instruction { opcode: 3, a: 0, b: 2, c: 3 }, "muli"),
            ([7, 5, 0, 0], Instruction { opcode: 4, a: 0, b: 1, c: 3 }, "banr"),
            ([9, 0, 0, 0], Instruction { opcode: 5, a: 0, b: 1, c: 3 }, "bani"),
            ([2, 5, 0, 0], Instruction { opcode: 6, a: 0, b: 1, c: 3 }, "borr"),
            ([6, 0, 0, 0], Instruction { opcode: 7, a: 0, b: 3, c: 3 }, "bori"),
            ([0, 0, 7, 0], Instruction { opcode: 8, a: 2, b: 0, c: 3 }, "setr"),
            ([0, 0, 0, 0], Instruction { opcode: 9, a: 9, b: 0, c: 3 }, "seti"),
            ([0, 0, 4, 0], Instruction { opcode: 10, a: 7, b: 2, c: 3 }, "gtir"), // true
            ([0, 0, 9, 0], Instruction { opcode: 10, a: 5, b: 2, c: 3 }, "gtir"), // false
            ([0, 6, 0, 0], Instruction { opcode: 11, a: 1, b: 3, c: 3 }, "gtri"), // true
            ([0, 1, 0, 0], Instruction { opcode: 11, a: 1, b: 9, c: 3 }, "gtri"), // false
            ([9, 2, 0, 0], Instruction { opcode: 12, a: 0, b: 1, c: 3 }, "gtrr"), // true
            ([2, 9, 0, 0], Instruction { opcode: 12, a: 0, b: 1, c: 3 }, "gtrr"), // false
            ([0, 0, 9, 0], Instruction { opcode: 13, a: 9, b: 2, c: 3 }, "eqir"), // true
            ([0, 0, 2, 0], Instruction { opcode: 13, a: 6, b: 2, c: 3 }, "eqir"), // false
            ([0, 4, 0, 0], Instruction { opcode: 14, a: 1, b: 4, c: 3 }, "eqri"), // true
            ([0, 5, 0, 0], Instruction { opcode: 14, a: 1, b: 3, c: 3 }, "eqri"), // false
            ([3, 3, 0, 0], Instruction { opcode: 15, a: 0, b: 1, c: 3 }, "eqrr"), // true
            ([2, 6, 0, 0], Instruction { opcode: 15, a: 0, b: 1, c: 3 }, "eqrr"), // false
        ];
        let expected: [[usize; 4]; 22] = [
            [6, 3, 0, 9], // addr
            [3, 0, 0, 8], // addi
            [2, 3, 0, 6], // mulr
            [3, 0, 0, 6], // muli
            [7, 5, 0, 5], // banr
            [9, 0, 0, 1], // bani
            [2, 5, 0, 7], // borr
            [6, 0, 0, 7], // bori
            [0, 0, 7, 7], // setr
            [0, 0, 0, 9], // seti
            [0, 0, 4, 1], // gtir true
            [0, 0, 9, 0], // gtir false
            [0, 6, 0, 1], // gtri true
            [0, 1, 0, 0], // gtri false
            [9, 2, 0, 1], // gtrr true
            [2, 9, 0, 0], // gtrr false
            [0, 0, 9, 1], // eqir true
            [0, 0, 2, 0], // eqir false
            [0, 4, 0, 1], // eqri true
            [0, 5, 0, 0], // eqri false
            [3, 3, 0, 1], // eqrr true
            [2, 6, 0, 0], // eqrr false
        ];

        for i in 0..input.len() {
            let mut registers = input[i].0.clone();
            process_instruction(&mut registers, &input[i].1, input[i].2);
            assert_eq!(registers, expected[i], "Test case {} failed", input[i].2);
        }
    }
}
