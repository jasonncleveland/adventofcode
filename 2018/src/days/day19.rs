use std::time::Instant;

use aoc_helpers::math::factors;
use log::{debug, trace};

use crate::shared::elfcode::{Device, Instruction, parse_device_instructions};

pub fn solve(file_contents: &str) -> (String, String) {
    let parse_timer = Instant::now();
    let input = parse_device_instructions(file_contents);
    debug!("File parse: ({:?})", parse_timer.elapsed());

    let part1_timer = Instant::now();
    let part1 = solve_part_1(&input);
    debug!("Part 1: {} ({:?})", part1, part1_timer.elapsed());

    let part2_timer = Instant::now();
    let part2 = solve_part_2();
    debug!("Part 2: {} ({:?})", part2, part2_timer.elapsed());

    (part1.to_string(), part2.to_string())
}

fn solve_part_1(instructions: &[Instruction]) -> usize {
    let mut device = Device::new(instructions);
    device.process_instructions();
    device.registers[0]
}

fn solve_part_2() -> i64 {
    // Notes breaking down program operations
    //  0: addi [1] 16 (1) - set instruction pointer to 16 (register 1 is always 0) (jump to instruction 17)
    //  1: seti 1 5 3 - set register 3 to 1
    //  2: seti 1 7 5 - set register 5 to 1
    //  3: mulr 3 5 4 - set register 4 to result of register 3 multiplied by register 5
    //  4: eqrr 4 2 4 - set register 4 to 1 if register 4 is equal to register 2, otherwise 0
    //  5: addr 4 [1] (1) - add register 4 to register 1 (register 1 is always 5) (skip next line if register 4 is equal to register 2)
    //  6: addi [1] 1 (1) - add 1 to register 1 (register 1 is always 6) (skip next line)
    //  7: addr 3 0 0 - add register 3 to register 0
    //  8: addi 5 1 5 - add 1 to register 5
    //  9: gtrr 5 2 4 - set register 4 to 1 if register 5 is greater than register 2, otherwise 0
    // 10: addr [1] 4 (1) - add register 4 to register 1 (register 1 is always 10) (skip next line if register 5 is greater than register 2)
    // 11: seti 2 1 (1) set register 1 to 2  (jump to instruction 3)
    // 12: addi 3 1 3 - add 1 to register 3
    // 13: gtrr 3 2 4 - set register 4 to 1 if register 3 > register 2, otherwise 0
    // 14: addr 4 [1] (1) - add register 4 to register 1 (register 1 is always 14) (skip next line if register 3 is greater than register 2)
    // 15: seti 1 3 (1) - set register 1 to 1 (jump to instruction 2)
    // 16: mulr [1] [1] (1) - multiply register 1 by register 1 (register 1 is always 16) (jump to instruction 257 (halt))
    // 17: addi 2 2 2 - add 2 to register 2
    // 18: mulr 2 2 2 - multiply register 2 by register 2 (square itself)
    // 19: mulr [1] 2 2 - multiply register 1 by register 2 (register 1 is always 19)
    // 20: muli 2 11 2 - multiply register 2 by 11
    // 21: addi 4 7 4 - add 7 to register 4
    // 22: mulr 4 [1] 4 - multiply register 4 by register 1 (register 1 is always 22)
    // 23: addi 4 13 4 - add 13 to register 4
    // 24: addr 2 4 2 - add register 2 to register 4
    // 25: addr [1] 0 (1) - add register 0 to register 1 (register 1 is always 25) (skip next line if register 0 is 1)
    // 26: seti 0 9 (1) - set register 1 to 0 (jump to instruction 1)
    // 27: setr [1] 0 4 - set register 4 to register 1 (register 1 is always 27)
    // 28: mulr 4 [1] 4 - multiply register 4 by register 1 (register 1 is always 28)
    // 29: addr [1] 4 4 - add register 1 to register 4 (register 1 is always 29)
    // 30: mulr [1] 4 4 - multiply register 4 by register 1 (register 1 is always 30)
    // 31: muli 4 14 4 - multiply register 4 by 14
    // 32: mulr 4 [1] 4 - multiply register 4 by register 1 (register 1 is always 32)
    // 33: addr 2 4 2 - add register 4 to register 2
    // 34: seti 0 2 0 - set register 0 to 0 (disable initialization code?)
    // 35: seti 0 0 (1) - set register 1 to 0 (incremented to 1 so next instruction is 1)  (jump to instruction 1)

    // This program generates a large number and then calculates the sum of its factors
    // This is an impossibly long program when done manually so we will generate the number and then
    // calculate the sum of the factors in a quicker way
    let mut registers: Vec<i64> = vec![0; 6];
    registers[0] = 1;
    // 17: addi 2 2 2
    registers[2] += 2;
    // 18: mulr 2 2 2
    registers[2] *= registers[2];
    // 19: mulr [1] 2 2
    registers[2] *= 19;
    // 20: muli 2 11 2
    registers[2] *= 11;
    // 21: addi 4 7 4
    registers[4] += 7;
    // 22: mulr 4 [1] 4
    registers[4] *= 22;
    // 23: addi 4 13 4
    registers[4] += 13;
    // 24: addr 2 4 2
    registers[2] += registers[4];
    // 27: setr [1] 0 4
    registers[4] = 27;
    // 28: mulr 4 [1] 4
    registers[4] *= 28;
    // 29: addr [1] 4 4
    registers[4] += 29;
    // 30: mulr [1] 4 4
    registers[4] *= 30;
    // 31: muli 4 14 4
    registers[4] *= 14;
    // 32: mulr 4 [1] 4
    registers[4] *= 32;
    // 33: addr 2 4 2
    registers[2] += registers[4];
    // 34: seti 0 2 0
    registers[0] = 0;
    trace!("registers after setup: {:?}", registers);

    trace!("target number: {}", registers[2]);

    let factors = factors(registers[2]);
    trace!("calculated factors of {}: {:?}", registers[2], factors);

    factors.iter().sum::<i64>()
}
