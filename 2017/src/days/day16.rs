use std::collections::{HashMap, VecDeque};
use std::time::Instant;

use log::{debug, trace};

pub fn solve(file_contents: String) -> (String, String) {
    let parse_timer = Instant::now();
    let input = parse_input(file_contents);
    debug!("File parse: ({:?})", parse_timer.elapsed());

    let part1_timer = Instant::now();
    let part1 = solve_part_1(&input, 16);
    debug!("Part 1: {} ({:?})", part1, part1_timer.elapsed());

    let part2_timer = Instant::now();
    let part2 = solve_part_2(&input, 16);
    debug!("Part 2: {} ({:?})", part2, part2_timer.elapsed());

    (part1.to_string(), part2.to_string())
}

fn parse_input(file_contents: String) -> Vec<DanceMove> {
    let mut dance_moves: Vec<DanceMove> = Vec::new();
    for dance_move in file_contents.split(',') {
        let parts = dance_move.split('/').collect::<Vec<&str>>();
        if let Some(f) = parts.first()
            && let mut chars = f.chars()
            && let Some(t) = chars.next()
        {
            match t {
                's' => {
                    if let Ok(first) = chars.as_str().parse::<u8>() {
                        dance_moves.push(DanceMove {
                            dance_type: DanceType::Spin,
                            first_program: Some(first),
                            second_program: None,
                        });
                    }
                }
                'x' => {
                    if let Ok(first) = chars.as_str().parse::<u8>()
                        && let Some(s) = parts.last()
                        && let Ok(second) = s.parse::<u8>()
                    {
                        dance_moves.push(DanceMove {
                            dance_type: DanceType::Exchange,
                            first_program: Some(first),
                            second_program: Some(second),
                        });
                    }
                }
                'p' => {
                    if let Some(first) = chars.next()
                        && let Some(s) = parts.last()
                        && let Some(second) = s.chars().next()
                    {
                        dance_moves.push(DanceMove {
                            dance_type: DanceType::Partner,
                            first_program: Some(first as u8),
                            second_program: Some(second as u8),
                        });
                    }
                }
                _ => unreachable!(),
            };
        }
    }
    dance_moves
}

fn solve_part_1(dance_moves: &[DanceMove], program_count: u8) -> String {
    let mut programs: VecDeque<char> = VecDeque::with_capacity(16);
    for i in 0..program_count {
        programs.push_back((b'a' + i) as char);
    }
    perform_dance(dance_moves, &mut programs);
    programs.into_iter().collect::<String>()
}

fn solve_part_2(dance_moves: &[DanceMove], program_count: u8) -> String {
    let mut programs: VecDeque<char> = VecDeque::with_capacity(16);
    for i in 0..program_count {
        programs.push_back((b'a' + i) as char);
    }

    // Search for cycle by storing state after dance
    let mut seen_states: HashMap<VecDeque<char>, i64> = HashMap::new();
    let mut iteration = 1;
    loop {
        perform_dance(dance_moves, &mut programs);
        if seen_states.contains_key(&programs)
            && let Some(previous_iteration) = seen_states.get(&programs)
        {
            // Calculate the cycle length and the remaining cycles after skipping towards goal
            let cycle_length = iteration - previous_iteration;
            let remaining_iterations = (1_000_000_000 - iteration) % cycle_length;
            for _ in 0..remaining_iterations {
                perform_dance(dance_moves, &mut programs);
            }
            return programs.into_iter().collect::<String>();
        } else {
            seen_states.insert(programs.clone(), iteration);
        }
        iteration += 1;
    }
}

fn perform_dance(dance_moves: &[DanceMove], programs: &mut VecDeque<char>) {
    for dance_move in dance_moves {
        trace!("Dance move: {:?} {:?}", dance_move, programs);
        match dance_move.dance_type {
            DanceType::Spin => {
                if let Some(first) = dance_move.first_program {
                    trace!("Spinning {} programs", first);
                    programs.rotate_right(first as usize);
                }
            }
            DanceType::Exchange => {
                if let Some(first) = dance_move.first_program
                    && let Some(second) = dance_move.second_program
                {
                    trace!("Exchanging programs at index {} and {}", first, second);
                    let i = first as usize;
                    let j = second as usize;
                    programs.swap(i, j);
                }
            }
            DanceType::Partner => {
                if let Some(first) = dance_move.first_program
                    && let Some(second) = dance_move.second_program
                    && let Some(i) = programs.iter().position(|&c| c == first as char)
                    && let Some(j) = programs.iter().position(|&c| c == second as char)
                {
                    trace!(
                        "Exchanging programs {} ({}) and {} ({})",
                        first as char, i, second as char, j
                    );
                    programs.swap(i, j);
                }
            }
        }
    }
}

#[derive(Debug)]
enum DanceType {
    Spin,
    Exchange,
    Partner,
}

#[derive(Debug)]
struct DanceMove {
    dance_type: DanceType,
    first_program: Option<u8>,
    second_program: Option<u8>,
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let data: [(&str, &str); 1] = [("s1,x3/4,pe/b", "baedc")];

        for (input, expected) in data {
            let input = parse_input(input.to_string());
            assert_eq!(solve_part_1(&input, 5), expected);
        }
    }

    #[test]
    fn test_part_2() {
        let data: [(&str, &str); 1] = [("s1,x3/4,pe/b", "abcde")];

        for (input, expected) in data {
            let input = parse_input(input.to_string());
            assert_eq!(solve_part_2(&input, 5), expected);
        }
    }
}
