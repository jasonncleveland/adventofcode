use std::collections::VecDeque;
use std::mem::swap;
use std::time::Instant;

use log::{debug, trace};

pub fn solve(file_contents: String) -> (String, String) {
    let parse_timer = Instant::now();
    let input = parse_input(file_contents);
    debug!("File parse: ({:?})", parse_timer.elapsed());

    let part1_timer = Instant::now();
    let part1 = solve_part_1(&input, 10007);
    debug!("Part 1: {} ({:?})", part1, part1_timer.elapsed());

    let part2_timer = Instant::now();
    let part2 = solve_part_2(&input, 119315717514047);
    debug!("Part 2: {} ({:?})", part2, part2_timer.elapsed());

    (part1.to_string(), part2.to_string())
}

fn parse_input(file_contents: String) -> Vec<Instruction> {
    let mut instructions: Vec<Instruction> = Vec::new();
    for line in file_contents.lines() {
        if line.starts_with("cut") {
            if let Some((_, value)) = line.split_once(' ')
                && let Ok(n) = value.parse::<i64>()
            {
                instructions.push(Instruction::Cut(n));
            }
        } else if line.starts_with("deal") {
            if line == "deal into new stack" {
                instructions.push(Instruction::DealIntoNewStack);
            } else if let Some(value) = line.split(' ').nth(3)
                && let Ok(n) = value.parse::<usize>()
            {
                instructions.push(Instruction::DealWithIncrement(n));
            }
        } else {
            panic!("invalid instruction");
        }
    }
    instructions
}

fn solve_part_1(input: &[Instruction], deck_size: i64) -> usize {
    let mut deck: VecDeque<i64> = VecDeque::from_iter(0..deck_size);

    process_instructions(input, &mut deck);

    if let Some(position) = deck.iter().position(|&x| x == 2019) {
        return position;
    }
    unreachable!();
}

fn solve_part_2(input: &[Instruction], deck_size: i128) -> i128 {
    let iterations = 101741582076661;
    let position = 2020;

    // This math is too complicated for me to come up with a solution myself
    // All the math has been copied from other solutions to this problem
    // https://github.com/twattanawaroon/adventofcode/blob/master/2019/q22b.py

    // Target card is in position ai + b mod deck size
    let mut a: i128 = 1;
    let mut b: i128 = 0;

    // Apply the shuffle instructions once
    for instruction in input {
        match instruction {
            Instruction::DealIntoNewStack => {
                // Position multiplied by -1
                a = (-a) % deck_size;
                b = (-b - 1) % deck_size;
            }
            Instruction::Cut(n) => {
                // Position is decreased by n
                b = (b - *n as i128) % deck_size;
            }
            Instruction::DealWithIncrement(n) => {
                // Position multiplied by n
                a = (a * *n as i128) % deck_size;
                b = (b * *n as i128) % deck_size;
            }
        }
    }

    // Apply the process iteration number of times
    let result = matrix_power([a, b, 0, 1], iterations, deck_size);

    let first = result[0];
    let second = result[1];

    // Solve for i in Ai+B = position (mod deck size)
    (inverse_prime(first, deck_size) * (position - second)) % deck_size
}

fn process_instructions(instructions: &[Instruction], deck: &mut VecDeque<i64>) {
    for instruction in instructions {
        trace!("processing instruction: {:?}", instruction);
        match instruction {
            Instruction::Cut(n) => {
                if *n < 0 {
                    trace!("cutting {:?} from bottom of deck", 0..(*n).unsigned_abs());
                    deck.rotate_right((*n).unsigned_abs() as usize);
                } else {
                    trace!("cutting {:?} from top of deck", 0..*n as usize);
                    deck.rotate_left((*n) as usize);
                }
            }
            Instruction::DealWithIncrement(n) => {
                trace!("deal {n} increments to new deck");
                let deck_size = deck.len();
                let mut new_deck = VecDeque::from_iter(vec![0; deck_size]);
                let mut index = 0;
                while let Some(card) = deck.pop_front() {
                    trace!("moving card {card} from current deck to index {index} in new deck");
                    new_deck[index % deck_size] = card;
                    index += *n;
                }
                swap(deck, &mut new_deck);
            }
            Instruction::DealIntoNewStack => {
                let mut new_deck = VecDeque::new();
                while let Some(card) = deck.pop_front() {
                    trace!("moving card {card} from top of current deck to top of new deck");
                    new_deck.push_front(card);
                }
                swap(deck, &mut new_deck);
            }
        }
    }
}

// Multiply two 1x4 matrices
pub fn matrix_multiplication(a: [i128; 4], b: [i128; 4], n: i128) -> [i128; 4] {
    [
        (a[0] * b[0] + a[1] * b[2]) % n,
        (a[0] * b[1] + a[1] * b[3]) % n,
        (a[2] * b[0] + a[3] * b[2]) % n,
        (a[2] * b[1] + a[3] * b[3]) % n,
    ]
}

// Multiply matrix to the exponent nth power
pub fn matrix_power(matrix: [i128; 4], mut exp: i128, n: i128) -> [i128; 4] {
    let mut mul = matrix;
    let mut ans = [1, 0, 0, 1];
    while exp > 0 {
        if exp % 2 == 1 {
            ans = matrix_multiplication(mul, ans, n);
        }
        exp /= 2;
        mul = matrix_multiplication(mul, mul, n);
    }
    ans
}

// Fermat's little theorem https://en.wikipedia.org/wiki/Fermat%27s_little_theorem
// Calculate the inverse of num, modulo p
pub fn inverse_prime(num: i128, p: i128) -> i128 {
    let mut exp = p - 2;
    let mut mul = num;
    let mut ans = 1;
    while exp > 0 {
        if exp % 2 == 1 {
            ans = (ans * mul) % p;
        }
        exp /= 2;
        mul = (mul * mul) % p;
    }
    ans
}

#[derive(Debug)]
enum Instruction {
    DealIntoNewStack,
    DealWithIncrement(usize),
    Cut(i64),
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_process_instructions() {
        let input: [&str; 8] = [
            "deal into new stack",
            "cut 3",
            "cut -4",
            "deal with increment 3",
            "deal with increment 7
deal into new stack
deal into new stack",
            "cut 6
deal with increment 7
deal into new stack",
            "deal with increment 7
deal with increment 9
cut -2",
            "deal into new stack
cut -2
deal with increment 7
cut 8
cut -4
deal with increment 7
cut 3
deal with increment 9
deal with increment 3
cut -1",
        ];
        let expected: [[i64; 10]; 8] = [
            [9, 8, 7, 6, 5, 4, 3, 2, 1, 0],
            [3, 4, 5, 6, 7, 8, 9, 0, 1, 2],
            [6, 7, 8, 9, 0, 1, 2, 3, 4, 5],
            [0, 7, 4, 1, 8, 5, 2, 9, 6, 3],
            [0, 3, 6, 9, 2, 5, 8, 1, 4, 7],
            [3, 0, 7, 4, 1, 8, 5, 2, 9, 6],
            [6, 3, 0, 7, 4, 1, 8, 5, 2, 9],
            [9, 2, 5, 8, 1, 4, 7, 0, 3, 6],
        ];

        for i in 0..input.len() {
            let instructions = parse_input(input[i].to_string());
            let mut deck = (0..10).collect();
            process_instructions(&instructions, &mut deck);
            assert_eq!(deck, expected[i]);
        }
    }
}
