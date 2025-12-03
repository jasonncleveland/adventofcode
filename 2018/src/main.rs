mod days;
mod shared;

use std::env;
use std::time::Instant;

use aoc_helpers::io::read_file;
use log::{debug, info, trace, warn};

use days::get_solve_module;

fn main() {
    env_logger::init();

    let args: Vec<String> = env::args().collect();

    let mut selected_day: Option<u8> = None;
    if args.len() >= 2 {
        // Specific day given
        if let Ok(value) = args[1].parse::<u8>() {
            selected_day = Some(value);
        }
    }

    if let Some(day) = selected_day {
        run_single_day(2018, day);
    } else {
        run_all_days(2018);
    }
}

fn run_all_days(year: u16) {
    let all_days_timer = Instant::now();
    for day in 1..=25 {
        run_single_day(year, day);
    }
    info!("Total runtime: ({:?})", all_days_timer.elapsed());
}

fn run_single_day(year: u16, day: u8) {
    trace!("Attempting to run year {:04} day {:02}", year, day);

    if let Ok(solve) = get_solve_module(day) {
        let input_timer = Instant::now();

        let file_path = format!("../inputs/{}/day/{}/input", year, day);
        trace!("Attempting to read file at `{}`", file_path);

        let file_contents = read_file(file_path);
        debug!("File read: ({:?})", input_timer.elapsed());

        let solve_timer = Instant::now();
        let (part1, part2) = solve(&file_contents);
        info!(
            "Day {:02}: ({}, {}) ({:?})",
            day,
            part1,
            part2,
            solve_timer.elapsed()
        );
    } else {
        warn!("Could not find solution for day {:02}", day);
    }
}
