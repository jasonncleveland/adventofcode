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

fn parse_input(file_contents: &str) -> (Vec<char>, HashMap<String, char>) {
    let input_parts = file_contents.split("\n\n").collect::<Vec<&str>>();

    let state: Vec<char> = input_parts[0].split(": ").collect::<Vec<&str>>()[1].chars().collect();

    let mut mutations: HashMap<String, char> = HashMap::new();
    for line in input_parts[1].split("\n") {
        let mutation = line.split(" => ").collect::<Vec<&str>>();
        mutations.insert(mutation[0].to_string(), mutation[1].chars().next().unwrap());
    }

    (state, mutations)
}

fn part1(file_contents: &str) -> i64 {
    let (mut state, mutations) = parse_input(file_contents);

    let mut offset: i64 = 0;
    for _ in 0..20 {
        // Add 2 empty pots on the left to help calculations
        state.insert(0, '.');
        state.insert(0, '.');
        offset += 2;
        // Add 3 empty pots on the right to help calculations
        state.push('.');
        state.push('.');
        state.push('.');

        let mut current_state = state.clone();
        for pot_index in 2..current_state.len()-2 {
            let row: Vec<char> = vec![
                state[pot_index - 2],
                state[pot_index - 1],
                state[pot_index],
                state[pot_index + 1],
                state[pot_index + 2],
            ];

            let next_value = mutations.get(&String::from_iter(&row));
            if next_value.is_some() {
                current_state[pot_index] = *next_value.unwrap();
            } else {
                current_state[pot_index] = '.';
            }
        }
        state = current_state;
    }

    let mut total: i64 = 0;
    for (i, pot) in state.iter().enumerate() {
        if *pot == '#' {
            total += i as i64 - offset;
        }
    }
    total
}

fn part2(file_contents: &str) -> i64 {
    -1
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part1() {
        let input: [&str; 1] = [
            "initial state: #..#.#..##......###...###

...## => #
..#.. => #
.#... => #
.#.#. => #
.#.## => #
.##.. => #
.#### => #
#.#.# => #
#.### => #
##.#. => #
##.## => #
###.. => #
###.# => #
####. => #",
        ];
        let expected: [i64; 1] = [
            325,
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
