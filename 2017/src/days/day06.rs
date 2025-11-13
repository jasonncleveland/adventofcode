use std::collections::{HashMap, HashSet};
use std::time::Instant;

use aoc_helpers::io::parse_int_list;
use log::{debug, trace};

pub fn solve(file_contents: String) -> (String, String) {
    let parse_timer = Instant::now();
    let input = parse_int_list(file_contents, '\t');
    debug!("File parse: ({:?})", parse_timer.elapsed());

    let part1_timer = Instant::now();
    let part1 = solve_part_1(&input);
    debug!("Part 1: {} ({:?})", part1, part1_timer.elapsed());

    let part2_timer = Instant::now();
    let part2 = solve_part_2(&input);
    debug!("Part 2: {} ({:?})", part2, part2_timer.elapsed());

    (part1.to_string(), part2.to_string())
}

fn solve_part_1(input: &[i64]) -> i64 {
    let mut memory = input.to_owned();
    let mut states: HashSet<Vec<i64>> = HashSet::new();
    let mut cycles = 0;

    loop {
        // Find bank with most blocks
        let index = find_max_memory_bank(&memory);

        // Rebalance blocks
        balance_memory_banks(&mut memory, index);

        cycles += 1;

        // Check state
        if states.contains(&memory) {
            trace!("Found duplicate state: {:?} after {} cycles", memory, cycles);
            return cycles;
        }
        states.insert(memory.clone());
    }
}

fn solve_part_2(input: &[i64]) -> i64 {
    let mut memory = input.to_owned();
    let mut states: HashMap<Vec<i64>, i64> = HashMap::new();
    let mut cycles = 0;

    loop {
        // Find bank with most blocks
        let index = find_max_memory_bank(&memory);

        // Rebalance blocks
        balance_memory_banks(&mut memory, index);

        cycles += 1;

        // Check state
        if let Some(last_seen) = states.get(&memory) {
            trace!("Found duplicate state: {:?} after {} cycles. Last seen on cycle {}", memory, cycles, last_seen);
            return cycles - last_seen;
        }
        states.insert(memory.clone(), cycles);
    }
}

fn find_max_memory_bank(memory: &[i64]) -> usize {
    let mut index = 0;
    let mut max_blocks = i64::MIN;
    for (i, &value) in memory.iter().enumerate() {
        if value > max_blocks {
            max_blocks = value;
            index = i;
        }
    }
    index
}

fn balance_memory_banks(memory: &mut [i64], mut index: usize) {
    let mut blocks = memory[index];
    memory[index] = 0;
    while blocks > 0 {
        index += 1;
        index %= memory.len();
        memory[index] += 1;
        blocks -= 1;
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let data: [(&str, i64); 1] = [("2\t4\t1\t2", 5)];

        for (input, expected) in data {
            let input = parse_int_list(input.to_string(), '\t');
            assert_eq!(solve_part_1(&input), expected);
        }
    }

    #[test]
    fn test_part_2() {
        let data: [(&str, i64); 1] = [("2\t4\t1\t2", 4)];

        for (input, expected) in data {
            let input = parse_int_list(input.to_string(), '\t');
            assert_eq!(solve_part_2(&input), expected);
        }
    }
}
