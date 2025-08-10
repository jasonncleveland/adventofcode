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

fn parse_input(file_contents: &str) -> Vec<Sample> {
    let mut samples: Vec<Sample> = Vec::new();
    let (left, _) = file_contents.split_once("\n\n\n\n").unwrap();
    for line in left.split("\n\n") {
        let sample = line.split('\n').collect::<Vec<&str>>();
        let mut before = sample[0].chars();
        let mut instruction = sample[1].split(' ').map(|i| i.parse::<u32>().unwrap()).collect::<Vec<u32>>();
        let mut after = sample[2].chars();
        samples.push(Sample {
            instruction: Instruction {
                opcode: instruction[0] as usize,
                a: instruction[1] as usize,
                b: instruction[2] as usize,
                c: instruction[3] as usize,
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
    samples
}

fn part1(file_contents: &str) -> i64 {
    let mut samples = parse_input(file_contents);
    let mut total = 0;
    for sample in samples {
        let mut match_count = 0;
        for opcode in 0..=15 {
            let mut registers = sample.before.clone();
            let mut instruction = sample.instruction.clone();
            instruction.opcode = opcode;
            process_instruction(&mut registers, &instruction);
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

fn part2(file_contents: &str) -> i64 {
    -1
}

fn process_instruction(registers: &mut [usize; 4], instruction: &Instruction) {
    match instruction.opcode {
        // addr
        0 => {
            // println!("Executing addr instruction {} + {} = {}", registers[instruction.a], registers[instruction.b], registers[instruction.a] + registers[instruction.b]);
            registers[instruction.c] = registers[instruction.a] + registers[instruction.b];
        },
        // addi
        1 => {
            // println!("Executing addi instruction {} + {} = {}", registers[instruction.a], instruction.b, registers[instruction.a] + instruction.b);
            registers[instruction.c] = registers[instruction.a] + instruction.b;
        },
        // mulr
        2 => {
            // println!("Executing mulr instruction {} * {} = {}", registers[instruction.a], registers[instruction.b], registers[instruction.a] * registers[instruction.b]);
            registers[instruction.c] = registers[instruction.a] * registers[instruction.b];
        },
        // muli
        3 => {
            // println!("Executing muli instruction {} * {} = {}", registers[instruction.a], instruction.b, registers[instruction.a] * instruction.b);
            registers[instruction.c] = registers[instruction.a] * instruction.b;
        },
        // banr
        4 => {
            // println!("Executing banr instruction {} * {} = {}", registers[instruction.a], registers[instruction.b], registers[instruction.a] & registers[instruction.b]);
            registers[instruction.c] = registers[instruction.a] & registers[instruction.b];
        },
        // bani
        5 => {
            // println!("Executing bani instruction {} * {} = {}", registers[instruction.a], instruction.b, registers[instruction.a] & instruction.b);
            registers[instruction.c] = registers[instruction.a] & instruction.b;
        },
        // borr
        6 => {
            // println!("Executing borr instruction {} * {} = {}", registers[instruction.a], registers[instruction.b], registers[instruction.a] | registers[instruction.b]);
            registers[instruction.c] = registers[instruction.a] | registers[instruction.b];
        },
        // bori
        7 => {
            // println!("Executing bori instruction {} * {} = {}", registers[instruction.a], instruction.b, registers[instruction.a] | instruction.b);
            registers[instruction.c] = registers[instruction.a] | instruction.b;
        },
        // setr
        8 => {
            // println!("Executing setr instruction {}", registers[instruction.a]);
            registers[instruction.c] = registers[instruction.a];
        },
        // seti
        9 => {
            // println!("Executing seti instruction {}", instruction.a);
            registers[instruction.c] = instruction.a;
        },
        // gtir
        10 => {
            // println!("Executing gtir instruction {} > {}", instruction.a, registers[instruction.b]);
            registers[instruction.c] = match instruction.a > registers[instruction.b] {
                true => 1,
                false => 0,
            }
        },
        // gtri
        11 => {
            // println!("Executing gtri instruction {} > {}", registers[instruction.a], instruction.b);
            registers[instruction.c] = match registers[instruction.a] > instruction.b {
                true => 1,
                false => 0,
            }
        },
        // gtrr
        12 => {
            // println!("Executing gtrr instruction {} > {}", registers[instruction.a], registers[instruction.b]);
            registers[instruction.c] = match registers[instruction.a] > registers[instruction.b] {
                true => 1,
                false => 0,
            }
        },
        // eqir
        13 => {
            // println!("Executing eqir instruction {} == {}", instruction.a, registers[instruction.b]);
            registers[instruction.c] = match instruction.a == registers[instruction.b] {
                true => 1,
                false => 0,
            }
        },
        // eqri
        14 => {
            // println!("Executing eqri instruction {} == {}", registers[instruction.a], instruction.b);
            registers[instruction.c] = match registers[instruction.a] == instruction.b {
                true => 1,
                false => 0,
            }
        },
        // eqrr
        15 => {
            // println!("Executing eqrr instruction {} == {}", registers[instruction.a], registers[instruction.b]);
            registers[instruction.c] = match registers[instruction.a] == registers[instruction.b] {
                true => 1,
                false => 0,
            }
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
    fn test_part1() {
        let input: [&str; 1] = [
            "",
        ];
        let expected: [i64; 1] = [
            0,
        ];

        for i in 0..input.len() {
            assert_eq!(part1(input[i]), expected[i]);
        }
    }

    #[test]
    fn test_part2() {
        let input: [&str; 1] = [
            "",
        ];
        let expected: [i64; 1] = [
            0,
        ];

        for i in 0..input.len() {
            assert_eq!(part2(input[i]), expected[i]);
        }
    }
}
