use std::collections::{HashSet, VecDeque};
use std::time::Instant;

use aoc_helpers::point2d::Point2d;
use log::debug;

pub fn solve(file_contents: String) -> (String, String) {
    let parse_timer = Instant::now();
    let (points, boundaries) = parse_input(file_contents);
    debug!("File parse: ({:?})", parse_timer.elapsed());

    let part1_timer = Instant::now();
    let part1 = solve_part_1(&points, &boundaries);
    debug!("Part 1: {} ({:?})", part1, part1_timer.elapsed());

    let part2_timer = Instant::now();
    let part2 = solve_part_2(&points, &boundaries, 10000);
    debug!("Part 2: {} ({:?})", part2, part2_timer.elapsed());

    (part1.to_string(), part2.to_string())
}

fn parse_input(file_contents: String) -> (Vec<Point2d>, Boundaries) {
    let mut points: Vec<Point2d> = Vec::new();
    let mut boundaries: Boundaries = Boundaries {
        min_x: i64::MAX,
        max_x: i64::MIN,
        min_y: i64::MAX,
        max_y: i64::MIN,
    };

    for line in file_contents.lines() {
        let parts = line.split(", ").collect::<Vec<&str>>();
        if let Ok(x) = parts[0].parse::<i64>() && let Ok(y) = parts[1].parse::<i64>() {
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
            points.push(Point2d::new(x, y));
        }
    }
    (points, boundaries)
}

fn solve_part_1(points: &Vec<Point2d>, boundaries: &Boundaries) -> i64 {
    let mut max_area = i64::MIN;
    for point in points.iter() {
        if let Ok(area) = find_area_rec(points, boundaries, &mut HashSet::new(), point, point.x, point.y)
            && area > max_area {
            max_area = area;
        }
    }
    max_area
}

fn solve_part_2(points: &Vec<Point2d>, boundaries: &Boundaries, max_distance: i64) -> i64 {
    for point in points.iter() {
        if find_total_distance(points, point) < max_distance {
            return find_region_area(points, boundaries, point, max_distance);
        }
    }
    -1
}

fn is_out_of_bounds(boundaries: &Boundaries, x: i64, y: i64) -> bool {
    x < boundaries.min_x || x > boundaries.max_x || y < boundaries.min_y || y > boundaries.max_y
}

fn find_area_rec(points: &Vec<Point2d>, boundaries: &Boundaries, visited: &mut HashSet<(i64, i64)>, source: &Point2d, x: i64, y: i64) -> Result<i64, i64> {
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
    let closest_index = find_closest_coordinate(points, &Point2d::new(x, y));
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

fn find_closest_coordinate(points: &[Point2d], origin: &Point2d) -> usize {
    let mut closest_count = 0;
    let mut min_distance = i64::MAX;
    let mut closest_point_index = points.len();
    for (i, point) in points.iter().enumerate() {
        let distance = origin.manhattan(point);
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

fn find_region_area(points: &Vec<Point2d>, boundaries: &Boundaries, source: &Point2d, max_distance: i64) -> i64 {
    let mut queue: VecDeque<(i64, i64)> = VecDeque::new();
    let mut visited: HashSet<(i64, i64)> = HashSet::new();

    queue.push_back((source.x, source.y));
    visited.insert((source.x, source.y));

    let mut neighbours = 0;

    while let Some((x, y)) = queue.pop_front() {
        neighbours += 1;

        // Up
        if !is_out_of_bounds(boundaries, x, y - 1) && !visited.contains(&(x, y - 1)) {
            visited.insert((x, y - 1));
            let distance = find_total_distance(points, &Point2d::new(x, y - 1));
            if distance < max_distance {
                queue.push_back((x, y - 1));
            }
        }

        // Down
        if !is_out_of_bounds(boundaries, x, y + 1) && !visited.contains(&(x, y + 1)) {
            visited.insert((x, y + 1));
            let distance = find_total_distance(points, &Point2d::new(x, y + 1));
            if distance < max_distance {
                queue.push_back((x, y + 1));
            }
        }

        // Left
        if !is_out_of_bounds(boundaries, x - 1, y) && !visited.contains(&(x - 1, y)) {
            visited.insert((x - 1, y));
            let distance = find_total_distance(points, &Point2d::new(x - 1, y));
            if distance < max_distance {
                queue.push_back((x - 1, y));
            }
        }

        // Right
        if !is_out_of_bounds(boundaries, x + 1, y) && !visited.contains(&(x + 1, y)) {
            visited.insert((x + 1, y));
            let distance = find_total_distance(points, &Point2d::new(x + 1, y));
            if distance < max_distance {
                queue.push_back((x + 1, y));
            }
        }
    }

    neighbours
}

fn find_total_distance(points: &Vec<Point2d>, origin: &Point2d) -> i64 {
    let mut distance = 0i64;
    for point in points {
        distance += origin.manhattan(point);
    }

    distance
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
    fn test_part_1() {
        let input: [&str; 1] = [
            "1, 1
1, 6
8, 3
3, 4
5, 5
8, 9",
        ];
        let expected: [i64; 1] = [
            17,
        ];

        for i in 0..input.len() {
            let (points, boundaries) = parse_input(input[i].to_string());
            assert_eq!(solve_part_1(&points, &boundaries), expected[i]);
        }
    }

    #[test]
    fn test_part_2() {
        let input: [&str; 1] = [
            "1, 1
1, 6
8, 3
3, 4
5, 5
8, 9",
        ];
        let expected: [i64; 1] = [
            16,
        ];

        for i in 0..input.len() {
            let (points, boundaries) = parse_input(input[i].to_string());
            assert_eq!(solve_part_2(&points, &boundaries, 32), expected[i]);
        }
    }
}
