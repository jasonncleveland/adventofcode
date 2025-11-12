type SolveFunction = fn(String) -> (String, String);

pub fn get_solve_module(day: u8) -> Result<SolveFunction, String> {
    match day {
        other => Err(format!("Invalid day provided: {}", other)),
    }
}
