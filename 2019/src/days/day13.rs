use std::io;
use std::time::Instant;
use std::{env, fmt};

use aoc_helpers::io::parse_int_list;
use aoc_helpers::point2d::Point2d;
use log::{debug, trace};

use crate::shared::intcode::{IntCodeComputer, IntCodeDisplay, IntCodeStatus};

pub fn solve(file_contents: &str) -> (String, String) {
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

fn solve_part_1(input: &[i64]) -> usize {
    let mut arcade = IntCodeComputer::new(input);
    let mut display = DisplayWithScore::new();

    run(&mut display, &mut arcade);

    // Count number of blocks
    display
        .screen
        .pixels
        .values()
        .filter(|t| **t == '@')
        .count()
}

fn solve_part_2(input: &[i64]) -> i64 {
    let mut arcade = IntCodeComputer::new(input);
    let mut display = DisplayWithScore::new();

    // Insert 2 quarters to play
    arcade.memory[0] = 2;

    // Hold the joystick still for the first move to prevent the AI from being dumb
    arcade.input.push_back(0);

    run(&mut display, &mut arcade);

    // Return to total score after the program halts (all blocks destroyed or tile hits bottom)
    display.score
}

fn run(display: &mut DisplayWithScore, arcade: &mut IntCodeComputer) {
    // Set the environment variable AOC_MANUAL_INPUT=true to have manual input
    let mut manual_input: bool = false;
    if let Ok(env_var) = env::var("AOC_MANUAL_INPUT")
        && let Ok(value) = env_var.parse::<bool>()
    {
        debug!("accepting manual input: {} {:?}", env_var, value);
        manual_input = value;
    }

    while let Ok(status) = arcade.run_interactive(3) {
        match status {
            IntCodeStatus::OutputWaiting => {
                if let Some(x) = arcade.output.pop_front()
                    && let Some(y) = arcade.output.pop_front()
                    && let Some(tile_id) = arcade.output.pop_front()
                {
                    if x == -1 && y == 0 {
                        trace!("Printing score: {}", tile_id);
                        display.score = tile_id;
                    } else {
                        trace!(
                            "printing tile {} at {}",
                            get_tile(tile_id),
                            Point2d::new(x, y)
                        );
                        display
                            .screen
                            .pixels
                            .insert(Point2d::new(x, y), get_tile(tile_id));
                    }
                }
            }
            IntCodeStatus::InputRequired => {
                match manual_input {
                    false => {
                        // AI input
                        // The "AI" aims to keep the paddle directly below the ball
                        if let Some((ball_position, _)) =
                            display.screen.pixels.iter().find(|(_, v)| **v == '*')
                            && let Some((paddle_position, _)) =
                                display.screen.pixels.iter().find(|(_, v)| **v == '=')
                        {
                            let optimal_move = match paddle_position.partial_cmp(ball_position) {
                                Some(std::cmp::Ordering::Less) => 1,
                                Some(std::cmp::Ordering::Equal) => 0,
                                Some(std::cmp::Ordering::Greater) => -1,
                                None => unreachable!(),
                            };
                            trace!("Moving paddle from {} to {}", paddle_position, optimal_move);
                            arcade.input.push_back(optimal_move);
                            continue;
                        }
                    }
                    true => {
                        // Manual input
                        // Use a, s, d to control horizontal movement of the paddle
                        println!("{}", display);
                        println!("Left (a) | Pause (s) | Right (d)");
                        println!("press key to move..");
                        loop {
                            let mut input = String::new();
                            if let Ok(_) = io::stdin().read_line(&mut input)
                                && let Some(char) = input.chars().next()
                            {
                                match char {
                                    'a' => {
                                        // Move paddle left
                                        arcade.input.push_back(-1);
                                        break;
                                    }
                                    's' => {
                                        // Hold paddle at current position
                                        arcade.input.push_back(0);
                                        break;
                                    }
                                    'd' => {
                                        // Move paddle right
                                        arcade.input.push_back(1);
                                        break;
                                    }
                                    _ => {
                                        continue;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            IntCodeStatus::ProgramHalted => break,
        }
    }
}

struct DisplayWithScore {
    screen: IntCodeDisplay,
    score: i64,
}

impl fmt::Display for DisplayWithScore {
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        write!(f, "Score: {}\n{}", self.score, self.screen)
    }
}

impl DisplayWithScore {
    fn new() -> Self {
        DisplayWithScore {
            screen: IntCodeDisplay::new(),
            score: 0,
        }
    }
}

fn get_tile(tile_id: i64) -> char {
    match tile_id {
        0 => '.',
        1 => '#',
        2 => '@',
        3 => '=',
        4 => '*',
        _ => panic!("Unknown tile id: {}", tile_id),
    }
}
