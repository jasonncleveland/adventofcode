use std::time::Instant;

use log::debug;

use crate::shared::elfcode::{parse_device_instructions, Device, Instruction};

pub fn solve(file_contents: String) -> (String, String) {
    let parse_timer = Instant::now();
    let input = parse_device_instructions(file_contents);
    debug!("File parse: ({:?})", parse_timer.elapsed());

    let part1_timer = Instant::now();
    let part1 = solve_part_1(&input);
    debug!("Part 1: {} ({:?})", part1, part1_timer.elapsed());

    let part2_timer = Instant::now();
    let part2 = solve_part_2(&input);
    debug!("Part 2: {} ({:?})", part2, part2_timer.elapsed());

    (part1.to_string(), part2.to_string())
}

fn solve_part_1(instructions: &[Instruction]) -> usize {
    // Notes breaking down program operations
    // #ip 2 - use register 2 as the instruction pointer
    // 00 seti 123 0 5 - set register 5 to 123
    // 01 bani {5} 456 5 - set register 5 to the bitwise and of register 5 (123) and 456
    // 02 eqri {5} 72 5 - set register 5 to 1 if register 5 is equal to 72, otherwise 0
    // 03 addr {5} 2 (2) - add register 5 to register 2
    // 04 seti 0 0 (2) - set register 2 to 0 (jump to instruction 1)
    // 05 seti 0 4 5 - set register 5 to 0
    // 06 bori {5} 65536 4 - set register 4 to the bitwise or of register 5 and 65536
    // 07 seti 15466939 9 5 - set register 5 to 15466939
    // 08 bani 4 255 3 - set register 3 to the bitwise and of register 4 and 255
    // 09 addr {5} 3 5 - add register 3 to register 5
    // 10 bani {5} 16777215 5 - set register 5 to the bitwise and of register 5 and 16777215
    // 11 muli {5} 65899 5 - multiply register 5 by 65899
    // 12 bani {5} 16777215 5 - set register 5 to the bitwise and of register 5 and 16777215
    // 13 gtir 256 4 3 - set register 3 to 1 if 256 is greater than register 4
    // 14 addr 3 2 (2) - add register 3 to register 2
    // 15 addi 2 1 (2) - add 1 to register 2 (skip next instruction)
    // 16 seti 27 8 (2) - set register 2 to 27 (jump to instruction 28)
    // 17 seti 0 7 3 - set register 3 to 0
    // 18 addi 3 1 1 - set register 1 to the value of register 3 plus 1
    // 19 muli 1 256 1 - multiply register 1 by 256
    // 20 gtrr 1 4 1 - set register 1 to 1 if register 1 is greater than register 4, otherwise 0
    // 21 addr 1 2 (2) - add register 1 to register 2
    // 22 addi 2 1 (2) - add 1 to register 2 (skip next line)
    // 23 seti 25 2 (2) - set register 2 to 25 (jump to instruction 26)
    // 24 addi 3 1 3 - add 1 to register 3
    // 25 seti 17 7 (2) - set register 2 to 17 (jump to instruction 18)
    // 26 setr 3 7 4 - set register 4 to the value of register 3
    // 27 seti 7 3 (2) - set register 2 to 7 (jump to instruction 8)
    // 28 eqrr {5} [0] 3 - set register 3 to 1 if registers 5 and 0 are equal, otherwise 0
    // 29 addr 3 2 (2) - add register 3 to register 2
    // 30 seti 5 9 (2) - set register 2 to 5 (jump to instruction 6)

    let mut device = Device::new(instructions);
    loop {
        device.process_instruction();
        // Instruction 28 is the only instruction that uses register 0
        // eqrr 5 0 3 - halt if register 0 is equal to register 5
        if device.registers[2] == 28 {
            debug!("the value in register 5 is {}", device.registers[5]);
            return device.registers[5];
        }
    }
}

fn solve_part_2(instructions: &[Instruction]) -> usize {
    0
}
