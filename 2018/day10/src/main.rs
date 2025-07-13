use std::env;
use std::fs;
use std::time;
use regex::Regex;

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

fn parse_input(file_contents: &str) -> Vec<Star> {
    let re = Regex::new(r"position=<(?<position>.*)>\s+velocity=<(?<velocity>.*)>").unwrap();

    let mut stars: Vec<Star> = Vec::new();
    for (_, [position, velocity]) in re.captures_iter(file_contents).map(|c| c.extract()) {
        let positions = position.split(",").collect::<Vec<&str>>();
        let velocities = velocity.split(",").collect::<Vec<&str>>();
        stars.push(Star {
            position: Position {
                x: positions[0].trim().parse().unwrap(),
                y: positions[1].trim().parse().unwrap(),
            },
            velocity: Velocity {
                dx: velocities[0].trim().parse().unwrap(),
                dy: velocities[1].trim().parse().unwrap(),
            },
        });
    }
    stars
}

fn part1(file_contents: &str) -> String {
    let mut stars = parse_input(file_contents);

    loop {
        process_tick(&mut stars);
        let is_valid = check_stars(&stars);
        if is_valid {
            break;
        }
    }
    generate_star_grid(&stars)
}

fn part2(file_contents: &str) -> i64 {
    -1
}

fn process_tick(stars: &mut [Star]) {
    for star in stars.iter_mut() {
        star.position.x += star.velocity.dx;
        star.position.y += star.velocity.dy;
    }
}

fn check_stars(stars: &[Star]) -> bool {

    let mut is_valid = true;
    for star in stars.iter() {
        let north_west = Position {
            x: star.position.x - 1,
            y: star.position.y - 1,
        };
        let north = Position {
            x: star.position.x,
            y: star.position.y - 1,
        };
        let north_east = Position {
            x: star.position.x + 1,
            y: star.position.y - 1,
        };
        let west = Position {
            x: star.position.x - 1,
            y: star.position.y,
        };
        let east = Position {
            x: star.position.x + 1,
            y: star.position.y,
        };
        let south_west = Position {
            x: star.position.x - 1,
            y: star.position.y + 1,
        };
        let south = Position {
            x: star.position.x,
            y: star.position.y + 1,
        };
        let south_east = Position {
            x: star.position.x + 1,
            y: star.position.y + 1,
        };

        let mut found_neighbour = false;
        for other in stars.iter() {
            found_neighbour = other.position == north_west
                || other.position == north
                || other.position == north_east
                || other.position == west
                || other.position == east
                || other.position == south_west
                || other.position == south
                || other.position == south_east;

            if found_neighbour {
                break;
            }
        }
        is_valid &= found_neighbour;

        if !is_valid {
            break;
        }
    }

    is_valid
}

fn generate_star_grid(stars: &Vec<Star>) -> String {
    let mut min_x = i64::MAX;
    let mut max_x = i64::MIN;
    let mut max_y = i64::MIN;
    let mut min_y = i64::MAX;
    for star in stars {
        if star.position.x < min_x {
            min_x = star.position.x;
        }
        if star.position.x > max_x {
            max_x = star.position.x;
        }
        if star.position.y < min_y {
            min_y = star.position.y;
        }
        if star.position.y > max_y {
            max_y = star.position.y;
        }
    }

    let width = max_x - min_x + 1;
    let height = max_y - min_y + 1;
    let x_offset = 0 - min_x;
    let y_offset = 0 - min_y;

    let mut galaxy: Vec<char> = vec!['.'; (width * height) as usize];

    for star in stars {
        let offset_position = Position {
            x: star.position.x + x_offset,
            y: star.position.y + y_offset,
        };
        let calculated_index = offset_position.y * width + offset_position.x;
        galaxy[calculated_index as usize] = '#'; // Use █ from better contrast
    }

    let mut result = String::from('\n');
    for y in 0..height {
        for x in 0..width {
            result.push(galaxy[(y * width + x) as usize]);
        }
        result.push('\n');
    }
    result
}

#[derive(Debug, PartialEq)]
struct Position {
    x: i64,
    y: i64,
}

#[derive(Debug)]
struct Velocity {
    dx: i64,
    dy: i64,
}

#[derive(Debug)]
struct Star {
    position: Position,
    velocity: Velocity,
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part1() {
        let input: [&str; 1] = [
            "position=< 9,  1> velocity=< 0,  2>
position=< 7,  0> velocity=<-1,  0>
position=< 3, -2> velocity=<-1,  1>
position=< 6, 10> velocity=<-2, -1>
position=< 2, -4> velocity=< 2,  2>
position=<-6, 10> velocity=< 2, -2>
position=< 1,  8> velocity=< 1, -1>
position=< 1,  7> velocity=< 1,  0>
position=<-3, 11> velocity=< 1, -2>
position=< 7,  6> velocity=<-1, -1>
position=<-2,  3> velocity=< 1,  0>
position=<-4,  3> velocity=< 2,  0>
position=<10, -3> velocity=<-1,  1>
position=< 5, 11> velocity=< 1, -2>
position=< 4,  7> velocity=< 0, -1>
position=< 8, -2> velocity=< 0,  1>
position=<15,  0> velocity=<-2,  0>
position=< 1,  6> velocity=< 1,  0>
position=< 8,  9> velocity=< 0, -1>
position=< 3,  3> velocity=<-1,  1>
position=< 0,  5> velocity=< 0, -1>
position=<-2,  2> velocity=< 2,  0>
position=< 5, -2> velocity=< 1,  2>
position=< 1,  4> velocity=< 2,  1>
position=<-2,  7> velocity=< 2, -2>
position=< 3,  6> velocity=<-1, -1>
position=< 5,  0> velocity=< 1,  0>
position=<-6,  0> velocity=< 2,  0>
position=< 5,  9> velocity=< 1, -2>
position=<14,  7> velocity=<-2,  0>
position=<-3,  6> velocity=< 2, -1>",
        ];
        let expected: [&str; 1] = [
            "
#...#..###
#...#...#.
#...#...#.
#####...#.
#...#...#.
#...#...#.
#...#...#.
#...#..###
",
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
