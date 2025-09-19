use std::collections::VecDeque;
use std::time::Instant;

use aoc_helpers::io::parse_int_list;
use aoc_helpers::permutations::generate_permutations;
use log::{debug, trace};

use crate::shared::intcode::{IntCodeComputer, IntCodeStatus};

pub fn solve(file_contents: String) -> (String, String) {
    let parse_timer = Instant::now();
    let input = parse_int_list(file_contents, ',');
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
    let permutations = generate_permutations(&vec![0, 1, 2, 3, 4]);

    let mut max_thruster_signal = i64::MIN;
    for phase_settings in permutations {
        trace!("testing phase settings: {:?}", phase_settings);
        let mut input_signal = 0;
        for phase_setting in phase_settings {
            trace!(
                "giving input signal {} and phase setting: {} to amplifier",
                phase_setting, input_signal
            );
            let mut amplifier = IntCodeComputer::new(input);
            amplifier.input.push_back(phase_setting);
            amplifier.input.push_back(input_signal);
            amplifier.run();
            if let Some(output_signal) = amplifier.output.back() {
                trace!("received amplifier result: {}", output_signal);
                input_signal = *output_signal;
                if *output_signal > max_thruster_signal {
                    max_thruster_signal = *output_signal;
                }
            }
        }
    }
    max_thruster_signal
}

fn solve_part_2(input: &[i64]) -> i64 {
    let permutations = generate_permutations(&vec![5, 6, 7, 8, 9]);

    let mut max_thruster_signal = i64::MIN;
    for phase_settings in permutations {
        trace!("testing phase settings: {:?}", phase_settings);

        // Initialize amplifiers
        let mut amplifiers = VecDeque::from(vec![
            IntCodeComputer::new(input),
            IntCodeComputer::new(input),
            IntCodeComputer::new(input),
            IntCodeComputer::new(input),
            IntCodeComputer::new(input),
        ]);
        for (i, phase_setting) in phase_settings.iter().enumerate() {
            amplifiers[i].input.push_back(*phase_setting);
        }

        let mut input_signal = 0;
        loop {
            if let Some(mut amplifier) = amplifiers.pop_front() {
                amplifier.input.push_back(input_signal);
                while let Ok(status) = amplifier.run_interactive(1) {
                    match status {
                        IntCodeStatus::OutputWaiting => continue,
                        IntCodeStatus::InputRequired => {
                            // Store output when input required
                            if let Some(output_signal) = amplifier.output.pop_back() {
                                trace!("received amplifier result: {}", output_signal);
                                input_signal = output_signal;
                            }
                            amplifiers.push_back(amplifier);
                            break;
                        }
                        IntCodeStatus::ProgramHalted => {
                            // Store output when program halts
                            if let Some(output_signal) = amplifier.output.pop_back() {
                                trace!("received amplifier result: {}", output_signal);
                                input_signal = output_signal;
                            }
                            break;
                        }
                    }
                }
            } else {
                // All amplifiers have finished so we can compare final signal
                if input_signal > max_thruster_signal {
                    max_thruster_signal = input_signal;
                }
                break;
            }
        }
    }
    max_thruster_signal
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let input: [&str; 3] = [
            "3,15,3,16,1002,16,10,16,1,16,15,15,4,15,99,0,0",
            "3,23,3,24,1002,24,10,24,1002,23,-1,23,101,5,23,23,1,24,23,23,4,23,99,0,0",
            "3,31,3,32,1002,32,10,32,1001,31,-2,31,1007,31,0,33,1002,33,7,33,1,33,31,31,1,32,31,31,4,31,99,0,0,0",
        ];
        let expected: [i64; 3] = [43210, 54321, 65210];

        for i in 0..input.len() {
            let parsed = parse_int_list(input[i].to_string(), ',');
            assert_eq!(solve_part_1(&parsed), expected[i]);
        }
    }

    #[test]
    fn test_part_2() {
        let input: [&str; 2] = [
            "3,26,1001,26,-4,26,3,27,1002,27,2,27,1,27,26,27,4,27,1001,28,-1,28,1005,28,6,99,0,0,5",
            "3,52,1001,52,-5,52,3,53,1,52,56,54,1007,54,5,55,1005,55,26,1001,54,-5,54,1105,1,12,1,53,54,53,1008,54,0,55,1001,55,1,55,2,53,55,53,4,53,1001,56,-1,56,1005,56,6,99,0,0,0,0,10",
        ];
        let expected: [i64; 2] = [139629729, 18216];

        for i in 0..input.len() {
            let parsed = parse_int_list(input[i].to_string(), ',');
            assert_eq!(solve_part_2(&parsed), expected[i]);
        }
    }
}
