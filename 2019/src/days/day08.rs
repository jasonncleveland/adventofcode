use std::time::Instant;

use log::debug;

pub fn solve(file_contents: String) -> (String, String) {
    let width = 25;
    let height = 6;

    let parse_timer = Instant::now();
    let input = parse_input(file_contents, width, height);
    debug!("File parse: ({:?})", parse_timer.elapsed());


    let part1_timer = Instant::now();
    let part1 = solve_part_1(&input);
    debug!("Part 1: {} ({:?})", part1, part1_timer.elapsed());

    let part2_timer = Instant::now();
    let part2 = solve_part_2(&input, width, height);
    debug!("Part 2: {} ({:?})", part2, part2_timer.elapsed());

    (part1.to_string(), part2.to_string())
}

fn parse_input(file_contents: String, width: usize, height: usize) -> Vec<Layer> {
    let mut layers: Vec<Layer> = Vec::new();
    let mut rows: Vec<Vec<u32>> = Vec::new();
    let mut row: Vec<u32> = Vec::new();

    let mut rows_taken = 0;
    let mut columns_taken = 0;

    for char in file_contents.chars() {
        if let Some(digit) = char.to_digit(10) {
            row.push(digit);

            columns_taken += 1;
            if columns_taken == width {
                rows.push(row.clone());
                row = Vec::new();
                columns_taken = 0;
                rows_taken += 1;
            }

            if rows_taken == height {
                layers.push(Layer { pixels: rows.clone() });
                rows = Vec::new();
                rows_taken = 0;
            }
        }
    }
    layers
}

fn solve_part_1(input: &Vec<Layer>) -> i64 {
    let mut min_zeros = i32::MAX;
    let mut ones = 0;
    let mut twos = 0;

    for layer in input {
        let mut zero_count = 0;
        let mut one_count = 0;
        let mut two_count = 0;
        for row in &layer.pixels {
            for column in row {
                match *column {
                    0 => zero_count += 1,
                    1 => one_count += 1,
                    2 => two_count += 1,
                    _ => continue,
                }
            }
        }

        if zero_count < min_zeros {
            min_zeros = zero_count;
            ones = one_count;
            twos = two_count;
        }
    }

    ones * twos
}

fn solve_part_2(input: &[Layer], width: usize, height: usize) -> String {
    let mut screen = vec![vec![0; width]; height];

    for layer in input.iter().rev() {
        for (row_index, row) in layer.pixels.iter().enumerate() {
            for (column_index, column) in row.iter().enumerate() {
                match column {
                    0 | 1 => screen[row_index][column_index] = *column,
                    _ => continue,
                }
            }
        }
    }

    let mut output = String::with_capacity(width * height);
    for row in screen {
        output.push('\n');
        for column in row {
            output.push(match column {
                0 => '.',
                1 => '#',
                _ => unreachable!()
            });
        }
    }

    output
}

#[derive(Debug)]
struct Layer {
    pixels: Vec<Vec<u32>>,
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let input: [&str; 1] = [
            "123456789012",
        ];
        let expected: [i64; 1] = [
            1,
        ];

        let width = 3;
        let height = 2;
        for i in 0..input.len() {
            let parsed = parse_input(input[i].to_string(), width, height);
            assert_eq!(solve_part_1(&parsed), expected[i]);
        }
    }

    #[test]
    fn test_part_2() {
        let input: [&str; 1] = [
            "0222112222120000",
        ];
        let expected: [&str; 1] = [
            "\n.#\n#.",
        ];

        let width = 2;
        let height = 2;
        for i in 0..input.len() {
            let parsed = parse_input(input[i].to_string(), width, height);
            assert_eq!(solve_part_2(&parsed, width, height), expected[i]);
        }
    }
}
