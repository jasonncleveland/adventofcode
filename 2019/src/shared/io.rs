use std::collections::HashMap;
use std::fs::read_to_string;

use super::point2d::Point2d;

pub fn read_file(file_name: String) -> String {
    read_to_string(file_name).expect("Something went wrong reading the file")
}

pub fn parse_int_list(file_contents: String) -> Vec<i64> {
    let mut result: Vec<i64> = Vec::new();
    for line in file_contents.trim_start_matches("\u{feff}").split(',') {
        if let Ok(value) = line.parse::<i64>() {
            result.push(value);
        }
    }
    result
}

pub fn parse_char_grid(file_contents: String, start_character: char) -> (HashMap<Point2d, char>, Point2d) {
    let mut result: HashMap<Point2d, char> = HashMap::new();
    let mut start: Point2d = Point2d::new(0, 0);
    for (y, line) in file_contents.lines().enumerate() {
        for (x, c) in line.trim_start_matches("\u{feff}").chars().enumerate() {
            if c == start_character {
                start = Point2d::new(x as i64, y as i64);
                result.insert(Point2d::new(x as i64, y as i64), '.');
            } else {
                result.insert(Point2d::new(x as i64, y as i64), c);
            }
        }
    }
    (result, start)
}
