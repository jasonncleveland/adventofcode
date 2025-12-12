use log::debug;
use std::time::Instant;

pub fn solve(file_contents: &str) -> (String, String) {
    let parse_timer = Instant::now();
    let input = parse_input(file_contents);
    debug!("File parse: ({:?})", parse_timer.elapsed());

    let part1_timer = Instant::now();
    let part1 = solve_part_1(&input);
    debug!("Part 1: {} ({:?})", part1, part1_timer.elapsed());

    (part1.to_string(), "Merry Christmas!".to_string())
}

fn parse_input(file_contents: &str) -> Items {
    let input = file_contents.split("\n\n").collect::<Vec<&str>>();

    if let Some(last) = input.last() {
        let mut regions: Vec<(usize, usize, Vec<usize>)> = Vec::new();
        for line in last.lines() {
            if let Some((size, region)) = line.split_once(": ") {
                let region = region
                    .split_whitespace()
                    .map(|r| r.parse().unwrap())
                    .collect::<Vec<usize>>();
                if let Some((w, h)) = size.split_once('x')
                    && let Ok(width) = w.parse::<usize>()
                    && let Ok(height) = h.parse::<usize>()
                {
                    regions.push((width, height, region));
                }
            }
        }

        let gifts: Vec<Vec<Vec<char>>> = input[..input.len() - 1]
            .iter()
            .map(|n| {
                n.lines()
                    .skip(1)
                    .map(|l| l.chars().collect())
                    .collect::<Vec<Vec<char>>>()
            })
            .collect();

        return Items { gifts, regions };
    }

    unreachable!();
}

fn solve_part_1(input: &Items) -> i64 {
    let mut total = 0;

    for (width, height, gift_indexes) in &input.regions {
        let mut gifts: Vec<Vec<Vec<char>>> = Vec::new();
        for (gift_index, &quantity) in gift_indexes.iter().enumerate() {
            if quantity > 0
                && let Some(gift) = input.gifts.get(gift_index)
            {
                for _ in 0..quantity {
                    gifts.push(gift.clone());
                }
            }
        }

        let count = gifts.iter().fold(0, |t, g| {
            t + g.iter().fold(0, |t, r| {
                t + r.iter().fold(0, |t, &c| t + usize::from(c == '#'))
            })
        });

        // Who needs a complex algorithm when you can just check if the number of gift tiles fits in
        // the number of available tiles ¯\_(ツ)_/¯
        if count <= width * height {
            total += 1;
        }
    }

    total
}

#[derive(Debug)]
struct Items {
    gifts: Vec<Vec<Vec<char>>>,
    regions: Vec<(usize, usize, Vec<usize>)>,
}
