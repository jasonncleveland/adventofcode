use std::collections::{HashMap, VecDeque};
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

fn parse_input(file_contents: &str) -> (i64, i64) {
    let words = file_contents.split_whitespace().collect::<Vec<&str>>();
    let player_count = words[0].parse::<i64>().unwrap();
    let last_marble_value = words[6].parse::<i64>().unwrap();
    (player_count, last_marble_value)
}

fn part1(file_contents: &str) -> i64 {
    let (player_count, last_marble_value) = parse_input(file_contents);

    simulate_game(player_count, last_marble_value)
}

fn part2(file_contents: &str) -> i64 {
    -1
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
    fn test_part1() {
        let input: [&str; 6] = [
            "9 players; last marble is worth 25 points",
            "10 players; last marble is worth 1618 points",
            "13 players; last marble is worth 7999 points",
            "17 players; last marble is worth 1104 points",
            "21 players; last marble is worth 6111 points",
            "30 players; last marble is worth 5807 points",
        ];
        let expected: [i64; 6] = [
            32,
            8317,
            146373,
            2764,
            54718,
            37305,
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
