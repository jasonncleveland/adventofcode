use std::collections::{HashMap, VecDeque};
use std::time::Instant;

use log::debug;

pub fn solve(file_contents: &str) -> (String, String) {
    let parse_timer = Instant::now();
    let (player_count, last_marble_value) = parse_input(file_contents);
    debug!("File parse: ({:?})", parse_timer.elapsed());

    let part1_timer = Instant::now();
    let part1 = solve_part_1(player_count, last_marble_value);
    debug!("Part 1: {} ({:?})", part1, part1_timer.elapsed());

    let part2_timer = Instant::now();
    let part2 = solve_part_2(player_count, last_marble_value);
    debug!("Part 2: {} ({:?})", part2, part2_timer.elapsed());

    (part1.to_string(), part2.to_string())
}

fn parse_input(file_contents: &str) -> (i64, i64) {
    let words = file_contents.split_whitespace().collect::<Vec<&str>>();
    if let Ok(player_count) = words[0].parse::<i64>()
        && let Ok(last_marble_value) = words[6].parse::<i64>()
    {
        return (player_count, last_marble_value);
    }
    unreachable!()
}

fn solve_part_1(player_count: i64, last_marble_value: i64) -> i64 {
    simulate_game(player_count, last_marble_value)
}

fn solve_part_2(player_count: i64, last_marble_value: i64) -> i64 {
    simulate_game(player_count, last_marble_value * 100)
}

fn simulate_game(player_count: i64, max_marble: i64) -> i64 {
    let mut player_scores: HashMap<i64, i64> = HashMap::new();
    for i in 0..player_count {
        player_scores.insert(i, 0);
    }

    let mut marbles: VecDeque<i64> = VecDeque::new();
    marbles.push_back(0);

    let mut current_player = 0i64;

    for marble in 1..=max_marble {
        if current_player == player_count {
            current_player = 0;
        }
        if marble % 23 == 0 {
            marbles.rotate_right(7);
            let removed_marble = marbles.pop_back().unwrap();
            player_scores
                .entry(current_player)
                .and_modify(|s| *s += marble + removed_marble);
            marbles.rotate_left(1);
        } else {
            marbles.rotate_left(1);
            marbles.push_back(marble);
        }
        current_player += 1;
    }

    let (_, score) = player_scores.iter().max_by_key(|(_, v)| *v).unwrap();
    *score
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let input: [&str; 6] = [
            "9 players; last marble is worth 25 points",
            "10 players; last marble is worth 1618 points",
            "13 players; last marble is worth 7999 points",
            "17 players; last marble is worth 1104 points",
            "21 players; last marble is worth 6111 points",
            "30 players; last marble is worth 5807 points",
        ];
        let expected: [i64; 6] = [32, 8317, 146373, 2764, 54718, 37305];

        for i in 0..input.len() {
            let (player_count, last_marble_value) = parse_input(input[i]);
            assert_eq!(solve_part_1(player_count, last_marble_value), expected[i]);
        }
    }

    #[test]
    fn test_part_2() {
        let input: [&str; 6] = [
            "9 players; last marble is worth 25 points",
            "10 players; last marble is worth 1618 points",
            "13 players; last marble is worth 7999 points",
            "17 players; last marble is worth 1104 points",
            "21 players; last marble is worth 6111 points",
            "30 players; last marble is worth 5807 points",
        ];
        let expected: [i64; 6] = [22563, 74765078, 1406506154, 20548882, 507583214, 320997431];

        for i in 0..input.len() {
            let (player_count, last_marble_value) = parse_input(input[i]);
            assert_eq!(solve_part_2(player_count, last_marble_value), expected[i]);
        }
    }
}
