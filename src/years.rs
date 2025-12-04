use aoc_helpers::solve::SolveFunction;

pub const YEARS: [u16; 4] = [2017, 2018, 2019, 2025];

pub const fn get_max_days(year: u16) -> Option<u8> {
    match year {
        2017 => Some(advent_of_code_2017::days::MAX_DAY),
        2018 => Some(advent_of_code_2018::days::MAX_DAY),
        2019 => Some(advent_of_code_2019::days::MAX_DAY),
        2025 => Some(advent_of_code_2025::days::MAX_DAY),
        _ => None,
    }
}

pub fn get_solve_module(year: u16, day: u8) -> Result<SolveFunction, String> {
    match year {
        2017 => advent_of_code_2017::days::get_solve_module(day),
        2018 => advent_of_code_2018::days::get_solve_module(day),
        2019 => advent_of_code_2019::days::get_solve_module(day),
        2025 => advent_of_code_2025::days::get_solve_module(day),
        other => Err(format!("Invalid year provided: {other}")),
    }
}
