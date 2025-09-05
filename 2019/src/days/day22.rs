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
    let part2 = solve_part_2(&input);
    debug!("Part 2: {} ({:?})", part2, part2_timer.elapsed());

    (part1.to_string(), part2.to_string())
}

fn parse_input(file_contents: String) -> Vec<Instruction> {
    let mut instructions: Vec<Instruction> = Vec::new();
    for line in file_contents.lines() {
        if line.starts_with("cut") {
            if let Some((_, value)) = line.split_once(' ') && let Ok(n) = value.parse::<i64>() {
                instructions.push(Instruction::Cut(n));
            }
        } else if line.starts_with("deal") {
            if line == "deal into new stack" {
                instructions.push(Instruction::DealIntoNewStack);
            } else if let Some(value) = line.split(' ').nth(3) && let Ok(n) = value.parse::<usize>() {
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

fn solve_part_2(input: &[Instruction]) -> i64 {
    unimplemented!();
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
            },
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
            },
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
            [6, 7, 8, 9, 0 ,1, 2, 3, 4, 5],
            [0, 7, 4, 1, 8 ,5, 2, 9, 6, 3],
            [0, 3, 6, 9, 2 ,5, 8, 1, 4, 7],
            [3, 0, 7, 4, 1 ,8, 5, 2, 9, 6],
            [6, 3, 0, 7, 4 ,1, 8, 5, 2, 9],
            [9, 2, 5, 8, 1 ,4, 7, 0, 3, 6],
        ];

        for i in 0..input.len() {
            let instructions = parse_input(input[i].to_string());
            let mut deck = (0..10).collect();
            process_instructions(&instructions, &mut deck);
            assert_eq!(deck, expected[i]);
        }
    }
}
