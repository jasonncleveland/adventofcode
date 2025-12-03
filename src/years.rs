use aoc_helpers::solve::SolveFunction;

pub fn get_solve_module(year: u16, day: u8) -> Result<SolveFunction, String> {
    match year {
        2017 => advent_of_code_2017::days::get_solve_module(day),
        2018 => advent_of_code_2018::days::get_solve_module(day),
        2019 => advent_of_code_2019::days::get_solve_module(day),
        2025 => advent_of_code_2025::days::get_solve_module(day),
        other => Err(format!("Invalid day provided: {other}")),
    }
}
