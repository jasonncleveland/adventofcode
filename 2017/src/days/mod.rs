mod day01;
mod day02;
mod day03;
mod day04;
mod day05;
mod day06;
mod day07;

type SolveFunction = fn(String) -> (String, String);

pub fn get_solve_module(day: u8) -> Result<SolveFunction, String> {
    match day {
        1 => Ok(day01::solve),
        2 => Ok(day02::solve),
        3 => Ok(day03::solve),
        4 => Ok(day04::solve),
        5 => Ok(day05::solve),
        6 => Ok(day06::solve),
        7 => Ok(day07::solve),
        other => Err(format!("Invalid day provided: {}", other)),
    }
}
