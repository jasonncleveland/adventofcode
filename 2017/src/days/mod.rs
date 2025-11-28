mod day01;
mod day02;
mod day03;
mod day04;
mod day05;
mod day06;
mod day07;
mod day08;
mod day09;
mod day10;
mod day11;
mod day12;
mod day13;
mod day14;
mod day15;
mod day16;
mod day17;
mod day18;
mod day19;
mod day20;

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
        8 => Ok(day08::solve),
        9 => Ok(day09::solve),
        10 => Ok(day10::solve),
        11 => Ok(day11::solve),
        12 => Ok(day12::solve),
        13 => Ok(day13::solve),
        14 => Ok(day14::solve),
        15 => Ok(day15::solve),
        16 => Ok(day16::solve),
        17 => Ok(day17::solve),
        18 => Ok(day18::solve),
        19 => Ok(day19::solve),
        20 => Ok(day20::solve),
        other => Err(format!("Invalid day provided: {}", other)),
    }
}
