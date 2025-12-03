use std::time::Instant;

use log::{debug, trace};

pub fn solve(file_contents: String) -> (String, String) {
    let parse_timer = Instant::now();
    let input = parse_input(file_contents);
    debug!("File parse: ({:?})", parse_timer.elapsed());

    let part1_timer = Instant::now();
    let part1 = solve_part_1(&input);
    debug!("Part 1: {} ({:?})", part1, part1_timer.elapsed());

    let part2_timer = Instant::now();
    let part2 = solve_part_2(&input);
    debug!("Part 2: {} ({:?})", part2, part2_timer.elapsed());

    (part1.to_string(), part2.to_string())
}

fn parse_input(file_contents: String) -> Vec<Vec<i64>> {
    let mut turns: Vec<Vec<i64>> = Vec::new();
    for line in file_contents.lines() {
        turns.push(line.chars().map(|c| c as i64 - '0' as i64).collect());
    }
    turns
}

fn solve_part_1(input: &Vec<Vec<i64>>) -> i64 {
    let mut total = 0;

    trace!("input: {:?}", input);
    for bank in input {
        trace!("Bank: {:?}", bank);
        let mut first_highest = 0;
        let mut second_highest = 0;
        for (i, &battery) in bank.iter().enumerate() {
            trace!("Battery: {:?}", battery);
            if battery > first_highest && i != bank.len() - 1 {
                first_highest = battery;
                second_highest = 0;
                continue;
            }
            if battery > second_highest {
                second_highest = battery;
            }
        }
        trace!("highest battery: {} {}", first_highest, second_highest);
        total += first_highest * 10 + second_highest;
    }

    total
}

fn solve_part_2(input: &Vec<Vec<i64>>) -> i64 {
    let mut total = 0;

    trace!("input: {:?}", input);
    for bank in input {
        trace!("Bank: {:?}", bank);
        let mut first_highest = 0;
        let mut second_highest = 0;
        let mut third_highest = 0;
        let mut fourth_highest = 0;
        let mut fifth_highest = 0;
        let mut sixth_highest = 0;
        let mut seventh_highest = 0;
        let mut eigth_highest = 0;
        let mut ninth_highest = 0;
        let mut tenth_highest = 0;
        let mut eleventh_highest = 0;
        let mut twelth_highest = 0;
        for (i, &battery) in bank.iter().enumerate() {
            trace!("Battery: {:?}", battery);
            if battery > first_highest && i < bank.len() - 11 {
                first_highest = battery;
                second_highest = 0;
                third_highest = 0;
                fourth_highest = 0;
                fifth_highest = 0;
                sixth_highest = 0;
                seventh_highest = 0;
                eigth_highest = 0;
                ninth_highest = 0;
                tenth_highest = 0;
                eleventh_highest = 0;
                twelth_highest = 0;
                continue;
            }
            if battery > second_highest && i < bank.len() - 10 {
                second_highest = battery;
                third_highest = 0;
                fourth_highest = 0;
                fifth_highest = 0;
                sixth_highest = 0;
                seventh_highest = 0;
                eigth_highest = 0;
                ninth_highest = 0;
                tenth_highest = 0;
                eleventh_highest = 0;
                twelth_highest = 0;
                continue;
            }
            if battery > third_highest && i < bank.len() - 9 {
                third_highest = battery;
                fourth_highest = 0;
                fifth_highest = 0;
                sixth_highest = 0;
                seventh_highest = 0;
                eigth_highest = 0;
                ninth_highest = 0;
                tenth_highest = 0;
                eleventh_highest = 0;
                twelth_highest = 0;
                continue;
            }
            if battery > fourth_highest && i < bank.len() - 8 {
                fourth_highest = battery;
                fifth_highest = 0;
                sixth_highest = 0;
                seventh_highest = 0;
                eigth_highest = 0;
                ninth_highest = 0;
                tenth_highest = 0;
                eleventh_highest = 0;
                twelth_highest = 0;
                continue;
            }
            if battery > fifth_highest && i < bank.len() - 7 {
                fifth_highest = battery;
                sixth_highest = 0;
                seventh_highest = 0;
                eigth_highest = 0;
                ninth_highest = 0;
                tenth_highest = 0;
                eleventh_highest = 0;
                twelth_highest = 0;
                continue;
            }
            if battery > sixth_highest && i < bank.len() - 6 {
                sixth_highest = battery;
                seventh_highest = 0;
                eigth_highest = 0;
                ninth_highest = 0;
                tenth_highest = 0;
                eleventh_highest = 0;
                twelth_highest = 0;
                continue;
            }
            if battery > seventh_highest && i < bank.len() - 5 {
                seventh_highest = battery;
                eigth_highest = 0;
                ninth_highest = 0;
                tenth_highest = 0;
                eleventh_highest = 0;
                twelth_highest = 0;
                continue;
            }
            if battery > eigth_highest && i < bank.len() - 4 {
                eigth_highest = battery;
                ninth_highest = 0;
                tenth_highest = 0;
                eleventh_highest = 0;
                twelth_highest = 0;
                continue;
            }
            if battery > ninth_highest && i < bank.len() - 3 {
                ninth_highest = battery;
                tenth_highest = 0;
                eleventh_highest = 0;
                twelth_highest = 0;
                continue;
            }
            if battery > tenth_highest && i < bank.len() - 2 {
                tenth_highest = battery;
                eleventh_highest = 0;
                twelth_highest = 0;
                continue;
            }
            if battery > eleventh_highest && i < bank.len() - 1 {
                eleventh_highest = battery;
                twelth_highest = 0;
                continue;
            }
            if battery > twelth_highest {
                twelth_highest = battery;
            }
        }
        trace!("highest battery: {} {}", first_highest, second_highest);
        total += first_highest * 100000000000;
        total += second_highest * 10000000000;
        total += third_highest * 1000000000;
        total += fourth_highest * 100000000;
        total += fifth_highest * 10000000;
        total += sixth_highest * 1000000;
        total += seventh_highest * 100000;
        total += eigth_highest * 10000;
        total += ninth_highest * 1000;
        total += tenth_highest * 100;
        total += eleventh_highest * 10;
        total += twelth_highest;
    }

    total
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let data: [(&str, i64); 1] = [(
            "987654321111111
811111111111119
234234234234278
818181911112111",
            357,
        )];

        for (input, expected) in data {
            let input = parse_input(input.to_string());
            assert_eq!(solve_part_1(&input), expected);
        }
    }

    #[test]
    fn test_part_2() {
        let data: [(&str, i64); 1] = [(
            "987654321111111
811111111111119
234234234234278
818181911112111",
            3121910778619,
        )];

        for (input, expected) in data {
            let input = parse_input(input.to_string());
            assert_eq!(solve_part_2(&input), expected);
        }
    }
}
