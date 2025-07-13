use std::env;
use std::fs;
use std::time;

fn main() {
    let args: Vec<String> = env::args().collect();
    if args.len() < 2 {
        panic!("Must pass filename as argument");
    }

    let input_timer = time::Instant::now();
    let file_name = &args[1];
    let file_contents = read_file(file_name);
    println!("File read: ({:?})", input_timer.elapsed());

    let part1_timer = time::Instant::now();
    let part1 = part1(&file_contents);
    println!("Part 1: {} ({:?})", part1, part1_timer.elapsed());

    let part2_timer = time::Instant::now();
    let part2 = part2(&file_contents);
    println!("Part 2: {} ({:?})", part2, part2_timer.elapsed());
}

fn read_file(file_name: &str) -> String {
    fs::read_to_string(file_name)
        .expect("Something went wrong reading the file")
}

fn parse_input(file_contents: &str) -> i64 {
    file_contents.parse::<i64>().unwrap()
}

fn part1(file_contents: &str) -> String {
    let serial_number = parse_input(file_contents);

    let mut max_power_level = i64::MIN;
    let mut max_power_level_x = 0;
    let mut max_power_level_y = 0;
    for x in 1..=300 {
        for y in 1..=300 {
            if x + 2 > 300 || y + 2 > 300 {
                continue;
            }

            let power_level = calculate_fuel_cell_power_level(serial_number, x, y);
            if power_level > max_power_level {
                max_power_level = power_level;
                max_power_level_x = x;
                max_power_level_y = y;
            }
        }
    }

    format!("{},{}", max_power_level_x, max_power_level_y)
}

fn part2(file_contents: &str) -> i64 {
    -1
}

fn calculate_fuel_cell_power_level(serial_number: i64, x: i64, y: i64) -> i64 {
    let mut power_level = 0;

    for ix in 0..=2 {
        for iy in 0..=2 {
            let local_power_level = calculate_power_level(serial_number, x + ix, y + iy);
            power_level += local_power_level;
        }
    }

    power_level
}

fn calculate_power_level(serial_number: i64, x: i64, y: i64) -> i64 {
    // Find the fuel cell's rack ID, which is its X coordinate plus 10
    let rack_id = x + 10;
    // Begin with a power level of the rack ID times the Y coordinate
    let mut power_level = rack_id * y;
    // Increase the power level by the value of the grid serial number
    power_level += serial_number;
    // Set the power level to itself multiplied by the rack ID
    power_level *= rack_id;
    // Keep only the hundreds digit of the power level
    power_level = (power_level / 100) % 10;
    // Subtract 5 from the power level
    power_level -= 5;
    power_level
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_calculate_power_level() {
        let input: [(i64, i64, i64); 4] = [
            (8, 3, 5),
            (57, 122, 79),
            (39, 217, 196),
            (71, 101, 153),
        ];
        let expected: [i64; 4] = [
            4,
            -5,
            0,
            4,
        ];

        for i in 0..input.len() {
            assert_eq!(calculate_power_level(input[i].0, input[i].1, input[i].2), expected[i]);
        }
    }

    #[test]
    fn test_part1() {
        let input: [&str; 2] = [
            "18",
            "42",
        ];
        let expected: [&str; 2] = [
            "33,45",
            "21,61",
        ];

        for i in 0..input.len() {
            assert_eq!(part1(input[i]), expected[i]);
        }
    }

    #[test]
    fn test_part2() {
        let input: [&str; 1] = [
            "",
        ];
        let expected: [i64; 1] = [
            0,
        ];

        for i in 0..input.len() {
            assert_eq!(part2(input[i]), expected[i]);
        }
    }
}
