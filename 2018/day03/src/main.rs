use std::collections::HashMap;
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

fn parse_input(file_contents: &str) -> Vec<Claim> {
    let mut claims: Vec<Claim> = Vec::new();
    for line in file_contents.lines() {
        let parts = line.split(" ").collect::<Vec<&str>>();
        let claim_id = parts[0].strip_prefix("#").unwrap().parse::<i64>().unwrap();
        let coordinate = parts[2].strip_suffix(":").unwrap().split(",").collect::<Vec<&str>>();
        let dimensions = parts[3].split("x").collect::<Vec<&str>>();
        claims.push(Claim {
            id: claim_id,
            origin: Point2D {
                x: coordinate[0].parse::<i64>().unwrap(),
                y: coordinate[1].parse::<i64>().unwrap(),
            },
            width: dimensions[0].parse::<i64>().unwrap(),
            height: dimensions[1].parse::<i64>().unwrap(),
        });
    }
    claims
}

fn part1(file_contents: &str) -> i64 {
    let claims = parse_input(file_contents);

    let mut tiles: HashMap<(i64, i64), i64> = HashMap::new();
    for claim in claims {
        for x in claim.origin.x..claim.origin.x + claim.width {
            for y in claim.origin.y..claim.origin.y + claim.height {
                tiles.entry((x, y))
                    .and_modify(|c| *c += 1)
                    .or_insert(1);
            }
        }
    }

    let mut total: i64 = 0;
    for (_, count) in tiles {
        if count > 1 {
            total += 1;
        }
    }
    total
}

fn part2(file_contents: &str) -> i64 {
    -1
}

#[derive(Debug)]
struct Point2D {
    x: i64,
    y: i64,
}

#[derive(Debug)]
struct Claim {
    id: i64,
    origin: Point2D,
    width: i64,
    height: i64,
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part1() {
        let input: [&str; 2] = [
            "#123 @ 3,2: 5x4",
            "#1 @ 1,3: 4x4\n#2 @ 3,1: 4x4\n#3 @ 5,5: 2x2",
        ];
        let expected: [i64; 2] = [
            0,
            4,
        ];

        for i in 0..input.len() {
            assert_eq!(part1(input[i]), expected[i]);
        }
    }

    #[test]
    fn test_part2() {
        let input: [&str; 0] = [
        ];
        let expected: [i64; 0] = [
        ];

        for i in 0..input.len() {
            assert_eq!(part2(input[i]), expected[i]);
        }
    }
}
