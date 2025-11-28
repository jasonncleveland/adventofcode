use std::collections::HashMap;
use std::time::Instant;

use aoc_helpers::matrix::{flip_in_place, rotate_in_place};
use log::debug;

pub fn solve(file_contents: String) -> (String, String) {
    let parse_timer = Instant::now();
    let input = parse_input(file_contents);
    debug!("File parse: ({:?})", parse_timer.elapsed());

    let part1_timer = Instant::now();
    let part1 = solve_part_1(&input, 5);
    debug!("Part 1: {} ({:?})", part1, part1_timer.elapsed());

    let part2_timer = Instant::now();
    let part2 = solve_part_2(&input, 18);
    debug!("Part 2: {} ({:?})", part2, part2_timer.elapsed());

    (part1.to_string(), part2.to_string())
}

fn parse_input(file_contents: String) -> HashMap<Vec<Vec<char>>, Vec<Vec<char>>> {
    let mut rules: HashMap<Vec<Vec<char>>, Vec<Vec<char>>> = HashMap::new();
    for line in file_contents.lines() {
        if let Some((input, output)) = line.split_once(" => ") {
            // Convert patterns to vec to improve lookup speed
            let input_vec: Vec<Vec<char>> = input.split('/').map(|r| r.chars().collect()).collect();
            let output_vec: Vec<Vec<char>> =
                output.split('/').map(|r| r.chars().collect()).collect();
            rules.insert(input_vec, output_vec);
        }
    }
    rules
}

fn solve_part_1(input: &HashMap<Vec<Vec<char>>, Vec<Vec<char>>>, iterations: i64) -> i64 {
    let start_pattern = vec![
        vec!['.', '#', '.'],
        vec!['.', '.', '#'],
        vec!['#', '#', '#'],
    ];

    let mut current_pattern = start_pattern.clone();
    for _ in 0..iterations {
        current_pattern = execute_iteration(input, &current_pattern);
    }

    let mut total = 0;
    for row in current_pattern {
        for column in row {
            if column == '#' {
                total += 1;
            }
        }
    }
    total
}

fn solve_part_2(input: &HashMap<Vec<Vec<char>>, Vec<Vec<char>>>, iterations: i64) -> i64 {
    let start_pattern = vec![
        vec!['.', '#', '.'],
        vec!['.', '.', '#'],
        vec!['#', '#', '#'],
    ];

    let mut current_pattern = start_pattern.clone();
    for _ in 0..iterations {
        current_pattern = execute_iteration(input, &current_pattern);
    }

    let mut total = 0;
    for row in current_pattern {
        for column in row {
            if column == '#' {
                total += 1;
            }
        }
    }
    total
}

fn execute_iteration(
    rules: &HashMap<Vec<Vec<char>>, Vec<Vec<char>>>,
    pattern: &[Vec<char>],
) -> Vec<Vec<char>> {
    if pattern.len().is_multiple_of(2) {
        get_squares(rules, pattern, 2)
    } else if pattern.len().is_multiple_of(3) {
        get_squares(rules, pattern, 3)
    } else {
        panic!(
            "Invalid pattern size {}. Pattern size must be multiple of 2 or 3",
            pattern.len()
        );
    }
}

fn get_squares(
    rules: &HashMap<Vec<Vec<char>>, Vec<Vec<char>>>,
    pattern: &[Vec<char>],
    square_width: usize,
) -> Vec<Vec<char>> {
    let transformed_square_width = match square_width {
        2 => 3,
        3 => 4,
        _ => unreachable!(),
    };

    // Find squares and compute enhanced patterns
    let mut transformations: Vec<Vec<&Vec<Vec<char>>>> = Vec::new();
    let mut found_squares = 0;
    for start_row in (0..pattern.len()).step_by(square_width) {
        found_squares += 1;
        let mut transformations_rows: Vec<&Vec<Vec<char>>> = Vec::new();
        for start_column in (0..pattern.len()).step_by(square_width) {
            // Generate square
            let mut square: Vec<Vec<char>> = Vec::with_capacity(square_width);
            for row in pattern.iter().skip(start_row).take(square_width) {
                square.push(row[start_column..start_column + square_width].to_vec());
            }
            // Calculate and store the enhanced pattern using the found square
            transformations_rows.push(find_enhancement(rules, &mut square));
        }
        transformations.push(transformations_rows);
    }

    // Combine the enhanced patterns into the new pattern
    let mut next_pattern: Vec<Vec<char>> = Vec::with_capacity(found_squares);
    for (offset, transformation_rows) in transformations.iter().enumerate() {
        for _ in 0..transformed_square_width {
            next_pattern.push(Vec::with_capacity(found_squares));
        }
        for transformation in transformation_rows {
            for (j, row) in transformation.iter().enumerate() {
                next_pattern[offset * transformed_square_width + j].extend(row);
            }
        }
    }
    next_pattern
}

fn find_enhancement<'a>(
    rules: &'a HashMap<Vec<Vec<char>>, Vec<Vec<char>>>,
    square: &mut [Vec<char>],
) -> &'a Vec<Vec<char>> {
    // Search for enhancement rule by rotating and flipping the given square
    // There are 8 possible orientations the square could be in, rotated 90 degrees 4 times and each rotation flipped
    loop {
        // Try square as given
        if let Some(output) = rules.get(square) {
            return output;
        }

        // Flip horizontally
        flip_in_place(square);
        if let Some(output) = rules.get(square) {
            return output;
        }
        // Flip back for next rotation
        // Memory allocation is so expensive that it's faster to rotate in place twice vs allocating
        flip_in_place(square);

        // Rotate 90 degrees right for next iteration
        rotate_in_place(square);
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let data: [(&str, i64); 1] = [(
            "../.# => ##./#../...
.#./..#/### => #..#/..../..../#..#",
            12,
        )];

        for (input, expected) in data {
            let input = parse_input(input.to_string());
            assert_eq!(solve_part_1(&input, 2), expected);
        }
    }
}
