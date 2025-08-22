use std::collections::HashMap;
use std::{env, fmt};
use std::io;
use std::time::Instant;

use log::{debug, trace};

use crate::shared::intcode::{IntCodeComputer, IntCodeError};
use crate::shared::io::parse_int_list;
use crate::shared::point2d::Point2d;

pub fn solve(file_contents: String) -> (String, String) {
    let parse_timer = Instant::now();
    let input = parse_int_list(file_contents);
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
    let mut screen = Screen::new();

    run(&mut screen, &mut arcade);

    // Count number of blocks
    screen.pixels.values().filter(|t| **t == Tile::Block).count()
}

fn solve_part_2(input: &[i64]) -> i64 {
    let mut arcade = IntCodeComputer::new(input);
    let mut screen = Screen::new();

    // Insert 2 quarters to play
    arcade.memory[0] = 2;

    // Hold the joystick still for the first move to prevent the AI from being dumb
    arcade.input.push_back(0);

    run(&mut screen, &mut arcade);

    // Return to total score after the program halts (all blocks destroyed or tile hits bottom)
    screen.score
}

fn run(screen: &mut Screen, arcade: &mut IntCodeComputer) {
    // Set the environment variable AOC_MANUAL_INPUT=true to have manual input
    let mut manual_input: bool = false;
    if let Ok(env_var) = env::var("AOC_MANUAL_INPUT") && let Ok(value) = env_var.parse::<bool>() {
        debug!("accepting manual input: {} {:?}", env_var, value);
        manual_input = value;
    }

    loop {
        match arcade.process_instruction() {
            Ok(result) => match result {
                true => {
                    if arcade.output.len() == 3
                        && let Some(x) = arcade.output.pop_front()
                        && let Some(y) = arcade.output.pop_front()
                        && let Some(tile_id) = arcade.output.pop_front() {
                        if x == -1 && y == 0 {
                            trace!("Printing score: {}", tile_id);
                            screen.score = tile_id;
                        } else {
                            trace!("printing tile {} at {}", get_tile(tile_id), Point2d::new(x, y));
                            screen.pixels.insert(Point2d::new(x, y), get_tile(tile_id));
                        }
                    }
                },
                false => {
                    // Stop when program halts
                    trace!("program halted: {:?}", arcade.output);
                    break;
                },
            },
            Err(IntCodeError::NoInputGiven) => {
                match manual_input {
                    false => {
                        // AI input
                        // The "AI" aims to keep the paddle directly below the ball
                        if let Some((ball_position, _)) = screen.pixels.iter().find(|(_, v)| **v == Tile::Ball)
                            && let Some((paddle_position, _)) = screen.pixels.iter().find(|(_, v)| **v == Tile::Paddle) {
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
                        println!("{}", print_screen(screen));
                        println!("Left (a) | Pause (s) | Right (d)");
                        println!("press key to move..");
                        loop {
                            let mut input = String::new();
                            if let Ok(_) = io::stdin().read_line(&mut input) && let Some(char) = input.chars().next() {
                                match char {
                                    'a' => {
                                        // Move paddle left
                                        arcade.input.push_back(-1);
                                        break;
                                    },
                                    's' => {
                                        // Hold paddle at current position
                                        arcade.input.push_back(0);
                                        break;
                                    },
                                    'd' => {
                                        // Move paddle right
                                        arcade.input.push_back(1);
                                        break;
                                    },
                                    _ => {
                                        continue;
                                    },
                                }
                            }
                        }
                    },
                }
            },
            Err(error) => panic!("Unexpected error: {}", error),
        }
    }
}

fn print_screen(screen: &Screen) -> String {
    // Get boundaries
    let mut min_x = i64::MAX;
    let mut min_y = i64::MAX;
    let mut max_x = i64::MIN;
    let mut max_y = i64::MIN;

    for point in screen.pixels.keys() {
        if point.x < min_x {
            min_x = point.x;
        }
        if point.y < min_y {
            min_y = point.y;
        }
        if point.x > max_x {
            max_x = point.x;
        }
        if point.y > max_y {
            max_y = point.y;
        }
    }

    // Get tile values
    let mut output = format!("Score: {}", screen.score);
    for y in min_y..=max_y {
        output.push('\n');
        for x in min_x..=max_x {
            let value = match screen.pixels.get(&Point2d::new(x, y)) {
                Some(Tile::Empty) => "\x1b[37m.\x1b[0m",
                Some(Tile::Wall) => "\x1b[90m#\x1b[0m",
                Some(Tile::Block) => "\x1b[33m@\x1b[0m",
                Some(Tile::Paddle) => "\x1b[96m=\x1b[0m",
                Some(Tile::Ball) => "\x1b[91m*\x1b[0m",
                None => unreachable!(),
            };
            output.push_str(value);
        }
    }

    output
}

struct Screen {
    pixels: HashMap<Point2d, Tile>,
    score: i64,
}

impl Screen {
    fn new() -> Self {
        Screen { pixels: HashMap::new(), score: 0 }
    }
}

#[derive(Clone, Debug, PartialEq)]
enum Tile {
    Empty,
    Wall,
    Block,
    Paddle,
    Ball,
}

impl fmt::Display for Tile {
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        match self {
            Tile::Empty => write!(f, "."),
            Tile::Wall => write!(f, "#"),
            Tile::Block => write!(f, "+"),
            Tile::Paddle => write!(f, "_"),
            Tile::Ball => write!(f, "*"),
        }
    }
}

fn get_tile(tile_id: i64) -> Tile {
    match tile_id {
        0 => Tile::Empty,
        1 => Tile::Wall,
        2 => Tile::Block,
        3 => Tile::Paddle,
        4 => Tile::Ball,
        _ => panic!("Unknown tile id: {}", tile_id),
    }
}
