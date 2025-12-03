mod years;

use std::env;
use std::time::Instant;

use aoc_helpers::io::read_file;
use log::{debug, info, trace, warn};

use years::get_solve_module;

fn main() {
    env_logger::init();

    let args: Vec<String> = env::args().collect();

    let mut selected_year: Option<u16> = None;
    let mut selected_day: Option<u8> = None;
    if args.len() >= 2
        && let Some(arg) = args.get(1)
    {
        // Specific year given
        if let Ok(value) = arg.parse::<u16>() {
            selected_year = Some(value);
        }
    }
    if args.len() >= 3
        && let Some(arg) = args.get(2)
    {
        // Specific day given
        if let Ok(value) = arg.parse::<u8>() {
            selected_day = Some(value);
        }
    }

    if let Some(year) = selected_year {
        if let Some(day) = selected_day {
            run_single_day(year, day);
        } else {
            run_all_days(year);
        }
    } else {
        run_all_years();
    }
}

fn run_all_years() {
    trace!("Attempting to run all years");
    let all_years_timer = Instant::now();
    for year in 2015..=2025 {
        run_all_days(year);
    }
    info!("Total runtime: ({:?})", all_years_timer.elapsed());
}

fn run_all_days(year: u16) {
    trace!("Attempting to run all days for year {year:04}");
    let all_days_timer = Instant::now();
    for day in 1..=25 {
        run_single_day(year, day);
    }
    info!("Total runtime: ({:?})", all_days_timer.elapsed());
}

fn run_single_day(year: u16, day: u8) {
    trace!("Attempting to run year {year:04} day {day:02}");

    if let Ok(solve) = get_solve_module(year, day) {
        let input_timer = Instant::now();

        let file_path = format!("inputs/{year}/day/{day}/input");
        trace!("Attempting to read file at `{file_path}`");

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
        warn!("Could not find solution for year {year:04} day {day:02}");
    }
}
