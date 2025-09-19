use std::time::Instant;

use aoc_helpers::point2d::Point2d;
use log::debug;
use regex::Regex;

pub fn solve(file_contents: String) -> (String, String) {
    let parse_timer = Instant::now();
    let input = parse_input(file_contents);
    debug!("File parse: ({:?})", parse_timer.elapsed());

    let part1_timer = Instant::now();
    let part1 = solve_part_1(&input);
    debug!("Part 1: {} ({:?})", part1, part1_timer.elapsed());

    let part2_timer = Instant::now();
    let part2 = solve_part_2(&input);
    debug!("Part 2: {} ({:?})", part2, part2_timer.elapsed());

    (part1.to_string(), part2.to_string())
}

fn parse_input(file_contents: String) -> Vec<Star> {
    let mut stars: Vec<Star> = Vec::new();

    if let Ok(re) = Regex::new(r"position=<(?<position>.*)>\s+velocity=<(?<velocity>.*)>") {
        for (_, [position, velocity]) in re.captures_iter(&file_contents).map(|c| c.extract()) {
            if let Some((px, py)) = position.split_once(",")
                && let Some((vdx, vdy)) = velocity.split_once(",")
                && let Ok(x) = px.trim().parse()
                && let Ok(y) = py.trim().parse()
                && let Ok(dx) = vdx.trim().parse()
                && let Ok(dy) = vdy.trim().parse()
            {
                stars.push(Star {
                    position: Point2d::new(x, y),
                    velocity: Point2d::new(dx, dy),
                });
            }
        }
    }

    stars
}

fn solve_part_1(input: &[Star]) -> String {
    let mut stars = input.to_owned();
    loop {
        process_tick(&mut stars);
        let is_valid = check_stars(&stars);
        if is_valid {
            break;
        }
    }
    generate_star_grid(&stars)
}

fn solve_part_2(input: &[Star]) -> i64 {
    let mut stars = input.to_owned();
    let mut iterations = 0;
    loop {
        iterations += 1;
        process_tick(&mut stars);
        let is_valid = check_stars(&stars);
        if is_valid {
            break;
        }
    }
    iterations
}

fn process_tick(stars: &mut [Star]) {
    for star in stars.iter_mut() {
        star.position.x += star.velocity.x;
        star.position.y += star.velocity.y;
    }
}

fn check_stars(stars: &[Star]) -> bool {
    let mut is_valid = true;
    for star in stars.iter() {
        let north_west = Point2d::new(star.position.x - 1, star.position.y - 1);
        let north = Point2d::new(star.position.x, star.position.y - 1);
        let north_east = Point2d::new(star.position.x + 1, star.position.y - 1);
        let west = Point2d::new(star.position.x - 1, star.position.y);
        let east = Point2d::new(star.position.x + 1, star.position.y);
        let south_west = Point2d::new(star.position.x - 1, star.position.y + 1);
        let south = Point2d::new(star.position.x, star.position.y + 1);
        let south_east = Point2d::new(star.position.x + 1, star.position.y + 1);

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
        let offset_position = Point2d::new(star.position.x + x_offset, star.position.y + y_offset);
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

#[derive(Clone, Debug)]
struct Star {
    position: Point2d,
    velocity: Point2d,
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let input: [&str; 1] = ["position=< 9,  1> velocity=< 0,  2>
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
position=<-3,  6> velocity=< 2, -1>"];
        let expected: [&str; 1] = ["
#...#..###
#...#...#.
#...#...#.
#####...#.
#...#...#.
#...#...#.
#...#...#.
#...#..###
"];

        for i in 0..input.len() {
            let input = parse_input(input[i].to_string());
            assert_eq!(solve_part_1(&input), expected[i]);
        }
    }

    #[test]
    fn test_part_2() {
        let input: [&str; 1] = ["position=< 9,  1> velocity=< 0,  2>
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
position=<-3,  6> velocity=< 2, -1>"];
        let expected: [i64; 1] = [3];

        for i in 0..input.len() {
            let input = parse_input(input[i].to_string());
            assert_eq!(solve_part_2(&input), expected[i]);
        }
    }
}
