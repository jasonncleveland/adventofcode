use std::collections::HashMap;
use std::fmt;
use std::time::Instant;

use log::{debug, trace};

use crate::shared::intcode::IntCodeComputer;
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
    let mut screen: HashMap<Point2d, Tile> = HashMap::new();

    run(&mut screen, &mut arcade);

    // Count number of blocks
    screen.values().filter(|t| **t == Tile::Block).count()
}

fn solve_part_2(input: &[i64]) -> i64 {
    -1
}

fn run(screen: &mut HashMap<Point2d, Tile>, arcade: &mut IntCodeComputer) {
    loop {
        match arcade.process_instruction() {
            Ok(result) => match result {
                true => {
                    if arcade.output.len() == 3
                        && let Some(x) = arcade.output.pop_front()
                        && let Some(y) = arcade.output.pop_front()
                        && let Some(tile_id) = arcade.output.pop_front() {
                        trace!("printing tile {} at {}", get_tile(tile_id), Point2d::new(x, y));
                        screen.insert(Point2d::new(x, y), get_tile(tile_id));
                    }
                },
                false => {
                    // Stop when program halts
                    trace!("program halted: {:?}", arcade.output);
                    break;
                },
            },
            Err(error) => panic!("Unexpected error: {}", error),
        }
    }
}

fn print_screen(tiles: &HashMap<Point2d, Tile>) -> String {
    // Get boundaries
    let mut min_x = i64::MAX;
    let mut min_y = i64::MAX;
    let mut max_x = i64::MIN;
    let mut max_y = i64::MIN;

    for point in tiles.keys() {
        if let Some(c) = tiles.get(point) && *c != Tile::Empty {
            continue;
        }

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
    let mut output = String::new();
    for y in min_y..=max_y {
        output.push('\n');
        for x in min_x..=max_x {
            let value = match tiles.get(&Point2d::new(x, y)) {
                Some(Tile::Empty) => '.',
                Some(Tile::Wall) => '#',
                Some(Tile::Block) => '+',
                Some(Tile::Paddle) => '_',
                Some(Tile::Ball) => '*',
                None => '.',
            };
            output.push(value);
        }
    }

    output
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
