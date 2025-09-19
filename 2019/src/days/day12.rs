use std::cmp::Ordering;
use std::fmt;
use std::time::Instant;

use aoc_helpers::math::least_common_multiple;
use aoc_helpers::point3d::Point3d;
use log::{debug, trace};

pub fn solve(file_contents: String) -> (String, String) {
    let parse_timer = Instant::now();
    let input = parse_input(file_contents);
    debug!("File parse: ({:?})", parse_timer.elapsed());

    let part1_timer = Instant::now();
    let part1 = solve_part_1(&input, 1000);
    debug!("Part 1: {} ({:?})", part1, part1_timer.elapsed());

    let part2_timer = Instant::now();
    let part2 = solve_part_2(&input);
    debug!("Part 2: {} ({:?})", part2, part2_timer.elapsed());

    (part1.to_string(), part2.to_string())
}

fn parse_input(file_contents: String) -> Vec<Moon> {
    let mut points: Vec<Moon> = Vec::new();
    for line in file_contents.lines() {
        let positions = &line[1..line.len() - 1]
            .split(", ")
            .map(|s| s.split_once('='))
            .collect::<Vec<_>>();
        if let Some(Some((_, value))) = positions.first()
            && let Ok(x) = value.parse::<i64>()
            && let Some(Some((_, value))) = positions.get(1)
            && let Ok(y) = value.parse::<i64>()
            && let Some(Some((_, value))) = positions.get(2)
            && let Ok(z) = value.parse::<i64>()
        {
            points.push(Moon {
                position: Point3d::new(x, y, z),
                velocity: Point3d::new(0, 0, 0),
            });
        }
    }
    points
}

fn solve_part_1(input: &[Moon], steps: i64) -> i64 {
    let mut moons = input.to_owned();

    for _ in 0..steps {
        simulate_moons(&mut moons);
    }

    calculate_energy(&moons)
}

fn solve_part_2(input: &[Moon]) -> i64 {
    let mut x_moons = input
        .iter()
        .flat_map(|moon| [moon.position.x, moon.velocity.x])
        .collect::<Vec<i64>>();
    let x_step = get_repetition_step(&mut x_moons);
    trace!("x repeats after {} steps", x_step);

    let mut y_moons = input
        .iter()
        .flat_map(|moon| [moon.position.y, moon.velocity.y])
        .collect::<Vec<i64>>();
    let y_step = get_repetition_step(&mut y_moons);
    trace!("y repeats after {} steps", y_step);

    let mut z_moons = input
        .iter()
        .flat_map(|moon| [moon.position.z, moon.velocity.z])
        .collect::<Vec<i64>>();
    let z_step = get_repetition_step(&mut z_moons);
    trace!("z repeats after {} steps", z_step);

    least_common_multiple(least_common_multiple(x_step, y_step), z_step)
}

fn get_repetition_step(moons: &mut Vec<i64>) -> i64 {
    let initial_state = moons.clone();
    let mut step = 1;
    loop {
        simulate_single_axis(moons);
        if *moons == initial_state {
            return step;
        }
        step += 1;
    }
}

fn simulate_single_axis(moons: &mut [i64]) {
    // Apply gravity
    for s in (0..moons.len()).step_by(2) {
        for e in (0..moons.len()).step_by(2) {
            if s == e {
                continue;
            }

            match moons[s].cmp(&moons[e]) {
                Ordering::Less => {
                    moons[s + 1] += 1;
                }
                Ordering::Greater => {
                    moons[s + 1] -= 1;
                }
                Ordering::Equal => {}
            }
        }
    }

    // Apply velocity
    for m in (0..moons.len()).step_by(2) {
        moons[m] += moons[m + 1];
    }
}

fn simulate_moons(moons: &mut [Moon]) {
    // Apply gravity
    for s in 0..moons.len() {
        for e in 0..moons.len() {
            if s == e {
                continue;
            }

            match moons[s].position.x.cmp(&moons[e].position.x) {
                Ordering::Less => {
                    moons[s].velocity.x += 1;
                }
                Ordering::Greater => {
                    moons[s].velocity.x -= 1;
                }
                Ordering::Equal => {}
            }

            match moons[s].position.y.cmp(&moons[e].position.y) {
                Ordering::Less => {
                    moons[s].velocity.y += 1;
                }
                Ordering::Greater => {
                    moons[s].velocity.y -= 1;
                }
                Ordering::Equal => {}
            }

            match moons[s].position.z.cmp(&moons[e].position.z) {
                Ordering::Less => {
                    moons[s].velocity.z += 1;
                }
                Ordering::Greater => {
                    moons[s].velocity.z -= 1;
                }
                Ordering::Equal => {}
            }
        }
    }

    // Apply velocity
    for moon in moons.iter_mut() {
        moon.position.x += moon.velocity.x;
        moon.position.y += moon.velocity.y;
        moon.position.z += moon.velocity.z;
    }
}

fn calculate_energy(moons: &Vec<Moon>) -> i64 {
    let mut total_energy: i64 = 0;
    for moon in moons {
        let potential_energy =
            moon.position.x.abs() + moon.position.y.abs() + moon.position.z.abs();
        let kinetic_energy = moon.velocity.x.abs() + moon.velocity.y.abs() + moon.velocity.z.abs();
        total_energy += potential_energy * kinetic_energy;
    }
    total_energy
}

#[derive(Clone, Debug, PartialEq)]
struct Moon {
    position: Point3d,
    velocity: Point3d,
}

impl fmt::Display for Moon {
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        write!(
            f,
            "pos=<x={}, y={}, z={}>, vel=<x={}, y={}, z={}>",
            self.position.x,
            self.position.y,
            self.position.z,
            self.velocity.x,
            self.velocity.y,
            self.velocity.z
        )
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let input: [(&str, i64); 2] = [
            (
                "<x=-1, y=0, z=2>
<x=2, y=-10, z=-7>
<x=4, y=-8, z=8>
<x=3, y=5, z=-1>",
                10,
            ),
            (
                "<x=-8, y=-10, z=0>
<x=5, y=5, z=10>
<x=2, y=-7, z=3>
<x=9, y=-8, z=-3>",
                100,
            ),
        ];
        let expected: [i64; 2] = [179, 1940];

        for i in 0..input.len() {
            let parsed = parse_input(input[i].0.to_string());
            assert_eq!(solve_part_1(&parsed, input[i].1), expected[i]);
        }
    }

    #[test]
    fn test_part_2() {
        let input: [&str; 2] = [
            "<x=-1, y=0, z=2>
<x=2, y=-10, z=-7>
<x=4, y=-8, z=8>
<x=3, y=5, z=-1>",
            "<x=-8, y=-10, z=0>
<x=5, y=5, z=10>
<x=2, y=-7, z=3>
<x=9, y=-8, z=-3>",
        ];
        let expected: [i64; 2] = [2772, 4686774924];

        for i in 0..input.len() {
            let parsed = parse_input(input[i].to_string());
            assert_eq!(solve_part_2(&parsed), expected[i]);
        }
    }
}
