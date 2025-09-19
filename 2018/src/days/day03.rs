use std::collections::{HashMap, HashSet};
use std::time::Instant;

use aoc_helpers::point2d::Point2d;
use log::debug;

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

fn parse_input(file_contents: String) -> HashMap<Point2d, Vec<i64>> {
    let mut tiles: HashMap<Point2d, Vec<i64>> = HashMap::new();

    for line in file_contents.lines() {
        let parts = line.split(" ").collect::<Vec<&str>>();

        if let Some(c) = parts[0].strip_prefix("#")
            && let Ok(claim_id) = c.parse::<i64>()
            && let Some(coordinates) = parts[2].strip_suffix(":")
            && let Some((xs, ys)) = coordinates.split_once(',')
            && let Ok(x) = xs.parse::<i64>()
            && let Ok(y) = ys.parse::<i64>()
            && let Some((w, h)) = parts[3].split_once('x')
            && let Ok(width) = w.parse::<i64>()
            && let Ok(height) = h.parse::<i64>()
        {
            for x in x..x + width {
                for y in y..y + height {
                    tiles
                        .entry(Point2d::new(x, y))
                        .and_modify(|c| c.push(claim_id))
                        .or_insert(vec![claim_id]);
                }
            }
        }
    }

    tiles
}

fn solve_part_1(tiles: &HashMap<Point2d, Vec<i64>>) -> i64 {
    let mut total: i64 = 0;
    for count in tiles.values() {
        if count.len() > 1 {
            total += 1;
        }
    }
    total
}

fn solve_part_2(tiles: &HashMap<Point2d, Vec<i64>>) -> i64 {
    let mut invalid: HashSet<i64> = HashSet::new();
    let mut valid: HashSet<i64> = HashSet::new();

    for count in tiles.values() {
        if count.len() == 1
            && let Some(&claim_id) = count.first()
        {
            if !invalid.contains(&claim_id) {
                valid.insert(claim_id);
            }
        } else {
            for &claim_id in count {
                invalid.insert(claim_id);
                valid.remove(&claim_id);
            }
        }
    }

    if valid.len() == 1
        && let Some(&claim_id) = valid.iter().next()
    {
        return claim_id;
    }

    unreachable!()
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let input: [&str; 2] = [
            "#123 @ 3,2: 5x4",
            "#1 @ 1,3: 4x4\n#2 @ 3,1: 4x4\n#3 @ 5,5: 2x2",
        ];
        let expected: [i64; 2] = [0, 4];

        for i in 0..input.len() {
            let input = parse_input(input[i].to_string());
            assert_eq!(solve_part_1(&input), expected[i]);
        }
    }

    #[test]
    fn test_part_2() {
        let input: [&str; 1] = ["#1 @ 1,3: 4x4\n#2 @ 3,1: 4x4\n#3 @ 5,5: 2x2"];
        let expected: [i64; 1] = [3];

        for i in 0..input.len() {
            let input = parse_input(input[i].to_string());
            assert_eq!(solve_part_2(&input), expected[i]);
        }
    }
}
