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

    let calculate_power_level = generate_calculate_power_level_func(serial_number);
    let table: Vec<Vec<i64>> = generate_summed_area_table(301, calculate_power_level);

    let mut max_power_level = i64::MIN;
    let mut max_power_level_x = 0;
    let mut max_power_level_y = 0;
    for x in 1..=300 {
        for y in 1..=300 {
            if x + 2 > 300 || y + 2 > 300 {
                continue;
            }

            let power_level = calculate_summed_area(&table, (x, y), (x + 2, y + 2));
            if power_level > max_power_level {
                max_power_level = power_level;
                max_power_level_x = x;
                max_power_level_y = y;
            }
        }
    }

    format!("{},{}", max_power_level_x, max_power_level_y)
}

fn part2(file_contents: &str) -> String {
    let serial_number = parse_input(file_contents);

    let calculate_power_level = generate_calculate_power_level_func(serial_number);
    let table: Vec<Vec<i64>> = generate_summed_area_table(301, calculate_power_level);

    let mut max_power_level = i64::MIN;
    let mut max_power_level_x = 0;
    let mut max_power_level_y = 0;
    let mut max_power_level_size = 0;
    for s in 1..=300 {
        for x in 1..=300 {
            for y in 1..=300 {
                if x + s - 1 > 300 || y + s - 1 > 300 {
                    continue;
                }

                let power_level = calculate_summed_area(&table, (x, y), (x + s - 1, y + s - 1));
                if power_level > max_power_level {
                    max_power_level = power_level;
                    max_power_level_x = x;
                    max_power_level_y = y;
                    max_power_level_size = s;
                }
            }
        }
    }

    format!("{},{},{}", max_power_level_x, max_power_level_y, max_power_level_size)
}

fn generate_summed_area_table(width: usize, get_value: impl Fn(i64, i64) -> i64) -> Vec<Vec<i64>> {
    let mut summed_area_table: Vec<Vec<i64>> = vec![vec![0; width]; width];

    // Generate summed-area table https://en.wikipedia.org/wiki/Summed-area_table
    for row in 0..summed_area_table.len() {
        for column in 0..summed_area_table[row].len() {
            let mut previous = 0;
            if column > 0 {
                previous += summed_area_table[row][column - 1];
            }
            if row > 0 {
                previous += summed_area_table[row - 1][column];
            }
            if column > 0 && row > 0 {
                previous -= summed_area_table[row - 1][column - 1];
            }
            summed_area_table[row][column] = get_value(column as i64, row as i64) + previous;
        }
    }

    summed_area_table
}

fn generate_calculate_power_level_func(serial_number: i64) -> impl Fn(i64, i64) -> i64 {
    move |x, y| {
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
}

fn calculate_summed_area(summed_area_table: &[Vec<i64>], start: (usize, usize), end: (usize, usize)) -> i64 {
    // println!("{:?} {:?}", start, end);
    let mut a = 0;
    if start.1 > 0 && start.0 > 0 {
        a = summed_area_table[start.1 - 1][start.0 - 1];
    }
    let mut b = 0;
    if start.1 > 0 {
        b = summed_area_table[start.1 - 1][end.0];
    }
    let mut c = 0;
    if start.0 > 0{
        c = summed_area_table[end.1][start.0 - 1];
    }
    let d = summed_area_table[end.1][end.0];
    // println!("{} {} {} {}", a, b, c, d);

    a + d - b - c
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
            let calculate_power_level = generate_calculate_power_level_func(input[i].0);
            assert_eq!(calculate_power_level(input[i].1, input[i].2), expected[i]);
        }
    }

    fn generate_get_value(table: &Vec<Vec<i64>>) -> impl Fn(i64, i64) -> i64 {
        move |x, y| table[y as usize][x as usize]
    }

    #[test]
    fn test_generate_summed_area_table() {
        let input: [Vec<Vec<i64>>; 2] = [
            vec![
                vec![3, 1, 4, 1, 5, 9],
                vec![2, 6, 5, 3, 5, 8],
                vec![9, 7, 9, 3, 2, 3],
                vec![8, 4, 6, 2, 6, 4],
                vec![3, 3, 8, 3, 2, 7],
                vec![9, 5, 0, 2, 8, 8],
            ],
            vec![
                vec![3, 2, 1, 8],
                vec![9, 11, 15, 0],
                vec![8, 4, 7, 6],
                vec![12, 7, 8, 3],
            ],
        ];
        let expected: [Vec<Vec<i64>>; 2] = [
            vec![
                vec![3, 4, 8, 9, 14, 23],
                vec![5, 12, 21, 25, 35, 52],
                vec![14, 28, 46, 53, 65, 85],
                vec![22, 40, 64, 73, 91, 115],
                vec![25, 46, 78, 90, 110, 141],
                vec![34, 60, 92, 106, 134, 173],
            ],
            vec![
                vec![3, 5, 6, 14],
                vec![12, 25, 41, 49],
                vec![20, 37, 60, 74],
                vec![32, 56, 87, 104],
            ],
        ];

        for i in 0..input.len() {
            let get_value = generate_get_value(&input[i]);
            assert_eq!(generate_summed_area_table(input[i].len(), &get_value), expected[i]);
        }
    }

    #[test]
    fn test_calculate_summed_area() {
        let table: Vec<Vec<i64>> = vec![
            vec![3, 4, 8, 9, 14, 23],
            vec![5, 12, 21, 25, 35, 52],
            vec![14, 28, 46, 53, 65, 85],
            vec![22, 40, 64, 73, 91, 115],
            vec![25, 46, 78, 90, 110, 141],
            vec![34, 60, 92, 106, 134, 173],
        ];
        let input: [((usize, usize), (usize, usize)); 6] = [
            ((2, 3), (4, 4)),
            ((0, 0), (4,4)),
            ((0, 0), (1,2)),
            ((0, 0), (4,2)),
            ((0, 0), (1,4)),
            ((2, 3), (4,4)),
        ];
        let expected: [i64; 6] = [
            27,
            110,
            28,
            65,
            46,
            27,
        ];

        for i in 0..input.len() {
            assert_eq!(calculate_summed_area(&table, input[i].0, input[i].1), expected[i]);
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
        let input: [&str; 2] = [
            "18",
            "42",
        ];
        let expected: [&str; 2] = [
            "90,269,16",
            "232,251,12",
        ];

        for i in 0..input.len() {
            assert_eq!(part2(input[i]), expected[i]);
        }
    }
}
