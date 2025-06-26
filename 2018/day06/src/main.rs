use std::collections::{HashSet, VecDeque};
use std::env;
use std::fs;
use std::time;

fn main() {
    let args: Vec<String> = env::args().collect();
    if args.len() < 2 {
        panic!("Must pass filename as argument");
    }

    let mut max_distance = 10000i64;
    if args.len() == 3 {
        max_distance = args[2].parse::<i64>().unwrap();
    }

    let input_timer = time::Instant::now();
    let file_name = &args[1];
    let file_contents = read_file(file_name);
    println!("File read: ({:?})", input_timer.elapsed());

    let part1_timer = time::Instant::now();
    let part1 = part1(&file_contents);
    println!("Part 1: {} ({:?})", part1, part1_timer.elapsed());

    let part2_timer = time::Instant::now();
    let part2 = part2(&file_contents, max_distance);
    println!("Part 2: {} ({:?})", part2, part2_timer.elapsed());
}

fn read_file(file_name: &str) -> String {
    fs::read_to_string(file_name)
        .expect("Something went wrong reading the file")
}

fn parse_input(file_contents: &str) -> (Vec<Point2D>, Boundaries) {
    let mut points: Vec<Point2D> = Vec::new();
    let mut boundaries: Boundaries = Boundaries {
        min_x: i64::MAX,
        max_x: i64::MIN,
        min_y: i64::MAX,
        max_y: i64::MIN,
    };

    for line in file_contents.lines() {
        let parts = line.split(", ").collect::<Vec<&str>>();
        let x = parts[0].parse::<i64>().unwrap();
        let y = parts[1].parse::<i64>().unwrap();
        if x < boundaries.min_x {
            boundaries.min_x = x;
        }
        if x > boundaries.max_x {
            boundaries.max_x = x;
        }
        if y < boundaries.min_y {
            boundaries.min_y = y;
        }
        if y > boundaries.max_y {
            boundaries.max_y = y;
        }
        points.push(Point2D { x, y });
    }
    (points, boundaries)
}

fn part1(file_contents: &str) -> i64 {
    let (points, boundaries) = parse_input(file_contents);

    let mut max_area = i64::MIN;
    for point in points.iter() {
        let result = find_area_rec(&points, &boundaries, &mut HashSet::new(), point, point.x, point.y);
        if result.is_ok() {
            let area = result.unwrap();
            if area > max_area {
                max_area = area;
            }
        }
    }
    max_area
}

fn part2(file_contents: &str, max_distance: i64) -> i64 {
    let (points, boundaries) = parse_input(file_contents);

    for point in points.iter() {
        if find_total_distance(&points, point) < max_distance {
            return find_region_area(&points, &boundaries, point, max_distance);
        }
    }
    -1
}

fn calc_manhattan_distance(first: &Point2D, second: &Point2D) -> i64 {
    (first.x - second.x).abs() + (first.y - second.y).abs()
}

fn is_out_of_bounds(boundaries: &Boundaries, x: i64, y: i64) -> bool {
    x < boundaries.min_x || x > boundaries.max_x || y < boundaries.min_y || y > boundaries.max_y
}

fn find_area_rec(points: &Vec<Point2D>, boundaries: &Boundaries, visited: &mut HashSet<(i64, i64)>, source: &Point2D, x: i64, y: i64) -> Result<i64, i64> {
    if visited.contains(&(x, y)) {
        return Ok(0);
    }

    if is_out_of_bounds(boundaries, x, y) {
        return Err(-1);
    }
    visited.insert((x, y));

    // Check neighbours
    let mut neighbours = 0;

    // Check closest
    let closest_index = find_closest_coordinate(points, &Point2D { x, y });
    if closest_index == usize::MAX || *source != points[closest_index] {
        return Ok(0);
    } else {
        neighbours += 1;
    }

    // Up
    let up = find_area_rec(points, boundaries, visited, source, x, y - 1);
    if up.is_err() {
        return Err(-1);
    } else {
        neighbours += up?;
    }

    // Down
    let down = find_area_rec(points, boundaries, visited, source, x, y + 1);
    if down.is_err() {
        return Err(-1);
    } else {
        neighbours += down?;
    }

    // Left
    let left = find_area_rec(points, boundaries, visited, source, x - 1, y);
    if left.is_err() {
        return Err(-1);
    } else {
        neighbours += left?;
    }

    // Right
    let right = find_area_rec(points, boundaries, visited, source, x + 1, y);
    if right.is_err() {
        return Err(-1);
    } else {
        neighbours += right?;
    }

    Ok(neighbours)
}

fn find_closest_coordinate(points: &[Point2D], origin: &Point2D) -> usize {
    let mut closest_count = 0;
    let mut min_distance = i64::MAX;
    let mut closest_point_index = points.len();
    for (i, point) in points.iter().enumerate() {
        let distance = calc_manhattan_distance(origin, point);
        if distance == min_distance {
            closest_count += 1;
        }
        if distance < min_distance {
            min_distance = distance;
            closest_point_index = i;
            closest_count = 1;
        }
    }

    // Return the index of the closest point if there is only 1
    if closest_count == 1 {
        closest_point_index
    } else {
        usize::MAX
    }
}

fn find_region_area(points: &Vec<Point2D>, boundaries: &Boundaries, source: &Point2D, max_distance: i64) -> i64 {
    let mut queue: VecDeque<(i64, i64)> = VecDeque::new();
    let mut visited: HashSet<(i64, i64)> = HashSet::new();

    queue.push_back((source.x, source.y));
    visited.insert((source.x, source.y));

    let mut neighbours = 0;

    while !queue.is_empty() {
        let (x, y) = queue.pop_front().unwrap();

        neighbours += 1;

        // Up
        if !is_out_of_bounds(boundaries, x, y - 1) && !visited.contains(&(x, y - 1)) {
            visited.insert((x, y - 1));
            let distance = find_total_distance(points, &Point2D { x, y: y - 1 });
            if distance < max_distance {
                queue.push_back((x, y - 1));
            }
        }

        // Down
        if !is_out_of_bounds(boundaries, x, y + 1) && !visited.contains(&(x, y + 1)) {
            visited.insert((x, y + 1));
            let distance = find_total_distance(points, &Point2D { x, y: y + 1 });
            if distance < max_distance {
                queue.push_back((x, y + 1));
            }
        }

        // Left
        if !is_out_of_bounds(boundaries, x - 1, y) && !visited.contains(&(x - 1, y)) {
            visited.insert((x - 1, y));
            let distance = find_total_distance(points, &Point2D { x: x - 1, y });
            if distance < max_distance {
                queue.push_back((x - 1, y));
            }
        }

        // Right
        if !is_out_of_bounds(boundaries, x + 1, y) && !visited.contains(&(x + 1, y)) {
            visited.insert((x + 1, y));
            let distance = find_total_distance(points, &Point2D { x: x + 1, y });
            if distance < max_distance {
                queue.push_back((x + 1, y));
            }
        }
    }

    neighbours
}

fn find_total_distance(points: &Vec<Point2D>, origin: &Point2D) -> i64 {
    let mut distance = 0i64;
    for point in points {
        distance += calc_manhattan_distance(origin, point);
    }

    distance
}

#[derive(Debug)]
#[derive(PartialEq)]
struct Point2D {
    x: i64,
    y: i64,
}

#[derive(Debug)]
struct Boundaries {
    min_x: i64,
    max_x: i64,
    min_y: i64,
    max_y: i64,
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part1() {
        let input: [&str; 1] = [
            "1, 1\n1, 6\n8, 3\n3, 4\n5, 5\n8, 9",
        ];
        let expected: [i64; 1] = [
            17,
        ];

        for i in 0..input.len() {
            assert_eq!(part1(input[i]), expected[i]);
        }
    }

    #[test]
    fn test_part2() {
        let input: [&str; 1] = [
            "1, 1\n1, 6\n8, 3\n3, 4\n5, 5\n8, 9",
        ];
        let expected: [i64; 1] = [
            16,
        ];

        for i in 0..input.len() {
            assert_eq!(part2(input[i], 32), expected[i]);
        }
    }
}
