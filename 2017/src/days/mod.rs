mod day01;
mod day02;

type SolveFunction = fn(String) -> (String, String);

pub fn get_solve_module(day: u8) -> Result<SolveFunction, String> {
    match day {
        1 => Ok(day01::solve),
        2 => Ok(day02::solve),
        other => Err(format!("Invalid day provided: {}", other)),
    }
}
