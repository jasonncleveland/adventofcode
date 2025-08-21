mod shared;
mod days;

use std::env;
use std::time::Instant;

use log::{debug, info, trace};

use days::{
    day01,
    day02,
    day03,
    day04,
    day05,
    day06,
    day07,
    day08,
    day09,
    day10,
};
use shared::io::read_file;

const HIGHEST_DAY_IMPLEMENTED: u8 = 10;

fn main() {
    env_logger::init();

    let args: Vec<String> = env::args().collect();

    if args.len() < 2 {
        panic!("Must pass filename as argument");
    }
    let file_name = &args[1];
    trace!("Reading file: {}", file_name);

    let mut selected_day: Option<u8> = None;
    if args.len() >= 3 {
        // Specific day given
        if let Ok(value) = args[2].parse::<u8>() {
            selected_day = Some(value);
        }
    }

    if let Some(day) = selected_day {
        run_single_day(day, file_name);
    } else {
        run_all_days(file_name);
    }
}

fn run_all_days(file_name: &str) {
    let all_days_timer = Instant::now();
    for day in 1..=HIGHEST_DAY_IMPLEMENTED {
        run_single_day(day, file_name);
    }
    info!("Total runtime: ({:?})", all_days_timer.elapsed());
}

fn run_single_day(day: u8, file_name: &str) {
    debug!("Running day {:02} with file {}", day, file_name);

    let input_timer = Instant::now();
    let file_contents = read_file(format!("day{:02}/{}", day, file_name));
    debug!("File read: ({:?})", input_timer.elapsed());

    let day_timer = Instant::now();
    let solve = get_day_module(day);
    let (part1, part2) = solve(file_contents);
    info!("Day {:02}: ({}, {}) ({:?})", day, part1, part2, day_timer.elapsed());
}

fn get_day_module(day: u8)  -> fn(String) -> (String, String) {
    match day {
        1 => day01::solve,
        2 => day02::solve,
        3 => day03::solve,
        4 => day04::solve,
        5 => day05::solve,
        6 => day06::solve,
        7 => day07::solve,
        8 => day08::solve,
        9 => day09::solve,
        10 => day10::solve,
        other => panic!("Invalid day provided: {}. Day must be less than {}", other, HIGHEST_DAY_IMPLEMENTED),
    }
}
