use std::collections::HashMap;
use std::fmt;
use std::time::Instant;

use aoc_helpers::point3d::Point3d;
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

fn parse_input(file_contents: String) -> Vec<Particle> {
    let mut particles: Vec<Particle> = Vec::new();
    for line in file_contents.lines() {
        let mut points = line.split(", ").map(|s| s.split_once('=')).map(|v| {
            if let Some((_, value)) = v
                && let mut coordinates = value[1..value.len() - 1].split(',')
                && let Some(value) = coordinates.next()
                && let Ok(x) = value.parse::<i64>()
                && let Some(value) = coordinates.next()
                && let Ok(y) = value.parse::<i64>()
                && let Some(value) = coordinates.next()
                && let Ok(z) = value.parse::<i64>()
            {
                return Point3d::new(x, y, z);
            }
            unreachable!();
        });
        if let Some(position) = points.next()
            && let Some(velocity) = points.next()
            && let Some(acceleration) = points.next()
        {
            particles.push(Particle {
                position: position.clone(),
                velocity: velocity.clone(),
                acceleration: acceleration.clone(),
            });
        }
    }
    particles
}

fn solve_part_1(input: &Vec<Particle>) -> i64 {
    let mut closest_particle = -1;
    let mut particles = input.to_owned();
    // Run the simulation for 500 rounds to reach an equilibrium
    for _ in 0..500 {
        let mut closest_distance = i64::MAX;
        for (particle_id, particle) in particles.iter_mut().enumerate() {
            particle.update();
            let manhattan =
                particle.position.x.abs() + particle.position.y.abs() + particle.position.z.abs();
            if manhattan < closest_distance {
                closest_particle = particle_id as i64;
                closest_distance = manhattan;
            }
        }
    }
    closest_particle
}

fn solve_part_2(input: &Vec<Particle>) -> usize {
    let mut particles = input.to_owned();
    // Run the simulation for 50 rounds to reach an equilibrium
    for _ in 0..50 {
        let mut locations: HashMap<Point3d, i64> = HashMap::new();
        for particle in particles.iter_mut() {
            particle.update();
            locations
                .entry(particle.position.clone())
                .and_modify(|v| *v += 1)
                .or_insert(1);
        }
        let particles_to_remove = locations
            .iter()
            .filter(|(_, v)| **v > 1)
            .map(|(p, _)| p)
            .collect::<Vec<_>>();
        particles.retain(|particle| !particles_to_remove.contains(&&particle.position));
    }
    particles.len()
}

#[derive(Clone, Debug, PartialEq)]
struct Particle {
    position: Point3d,
    velocity: Point3d,
    acceleration: Point3d,
}

impl fmt::Display for Particle {
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        write!(
            f,
            "pos=<x={}, y={}, z={}>, vel=<x={}, y={}, z={}>, acc=<x={}, y={}, z={}>",
            self.position.x,
            self.position.y,
            self.position.z,
            self.velocity.x,
            self.velocity.y,
            self.velocity.z,
            self.acceleration.x,
            self.acceleration.y,
            self.acceleration.z,
        )
    }
}

impl Particle {
    fn update(&mut self) {
        // Update velocity using acceleration
        self.velocity.x += self.acceleration.x;
        self.velocity.y += self.acceleration.y;
        self.velocity.z += self.acceleration.z;

        // Update position using velocity
        self.position.x += self.velocity.x;
        self.position.y += self.velocity.y;
        self.position.z += self.velocity.z;
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let data: [(&str, i64); 1] = [(
            "p=<3,0,0>, v=<2,0,0>, a=<-1,0,0>
p=<4,0,0>, v=<0,0,0>, a=<-2,0,0>",
            0,
        )];

        for (input, expected) in data {
            let input = parse_input(input.to_string());
            assert_eq!(solve_part_1(&input), expected);
        }
    }

    #[test]
    fn test_part_2() {
        let data: [(&str, usize); 1] = [(
            "p=<-6,0,0>, v=<3,0,0>, a=<0,0,0>
p=<-4,0,0>, v=<2,0,0>, a=<0,0,0>
p=<-2,0,0>, v=<1,0,0>, a=<0,0,0>
p=<3,0,0>, v=<-1,0,0>, a=<0,0,0>",
            1,
        )];

        for (input, expected) in data {
            let input = parse_input(input.to_string());
            assert_eq!(solve_part_2(&input), expected);
        }
    }
}
