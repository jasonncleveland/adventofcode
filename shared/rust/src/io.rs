use std::collections::HashMap;
use std::fs::read_to_string;

use super::point2d::Point2d;

pub fn read_file(file_name: String) -> String {
    // Some input has a Byte Order Mark at the start of the input.
    // We need to remove the BOM before parsing
    // Strip trailing newline to help parsing
    let file_contents = read_to_string(file_name).expect("Something went wrong reading the file");
    file_contents
        .trim_start_matches("\u{feff}")
        .trim_end_matches("\n")
        .to_string()
}

pub fn parse_int(file_contents: String) -> i64 {
    file_contents
        .parse::<i64>()
        .expect("Input string is not a single number")
}

pub fn parse_int_list(file_contents: String, separator: char) -> Vec<i64> {
    let mut result: Vec<i64> = Vec::new();
    for line in file_contents.split(separator) {
        if let Ok(value) = line.parse::<i64>() {
            result.push(value);
        }
    }
    result
}

pub fn parse_char_list(file_contents: String) -> Vec<char> {
    file_contents.chars().collect()
}

pub fn parse_char_grid(file_contents: String) -> HashMap<Point2d, char> {
    let mut result: HashMap<Point2d, char> = HashMap::new();
    for (y, line) in file_contents.lines().enumerate() {
        for (x, c) in line.chars().enumerate() {
            result.insert(Point2d::new(x as i64, y as i64), c);
        }
    }
    result
}

pub fn parse_char_vec(file_contents: String) -> Vec<Vec<char>> {
    file_contents
        .lines()
        .map(|line| line.chars().collect())
        .collect()
}
