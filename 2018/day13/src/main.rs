use std::collections::{HashMap, VecDeque};
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

fn parse_input(file_contents: &str) -> (HashMap<(usize, usize), char>, Vec<Cart>) {
    let mut tracks: HashMap<(usize, usize), char> = HashMap::new();
    let mut carts: Vec<Cart> = Vec::new();
    let mut cart_id = 1;

    let lines = file_contents.lines();

    for (row_index, row) in lines.enumerate() {
        for (column_index, column) in row.chars().enumerate() {
            match column {
                '^' | 'v' => {
                    carts.push(Cart {
                        id: cart_id,
                        position: (row_index, column_index),
                        direction: column,
                        next_turn: VecDeque::from([Turn::Left, Turn::Straight, Turn::Right]),
                    });
                    tracks.insert((row_index, column_index), '|');
                    cart_id += 1;
                },
                '<' | '>' => {
                    carts.push(Cart {
                        id: cart_id,
                        position: (row_index, column_index),
                        direction: column,
                        next_turn: VecDeque::from([Turn::Left, Turn::Straight, Turn::Right]),
                    });
                    tracks.insert((row_index, column_index), '-');
                    cart_id += 1;
                },
                other => {
                    tracks.insert((row_index, column_index), other);
                },
            };
        }
    }

    (tracks, carts)
}

fn part1(file_contents: &str) -> String {
    let (tracks, mut carts) = parse_input(file_contents);

    loop {
        // Sort the carts by position so that the top-left-most cart is first
        carts.sort_by(|a, b| a.position.cmp(&b.position));
        let result = move_carts(&tracks, &mut carts);
        if result.is_some() {
            let position = result.unwrap();
            return format!("{},{}", position.1, position.0);
        }
    }
}

fn part2(file_contents: &str) -> i64 {
    -1
}

fn move_carts(tracks: &HashMap<(usize, usize), char>, carts: &mut [Cart]) -> Option<(usize, usize)> {
    for i in 0..carts.len() {
        move_cart(tracks, &mut carts[i]);
        // Check for collisions after every move
        let result = check_for_collisions(carts);
        if result.is_some() {
            return result;
        }
    }
    None
}

fn move_cart(tracks: &HashMap<(usize, usize), char>, cart: &mut Cart) -> Option<bool> {
    let next_position = get_next_position(cart);
    let next_track = tracks[&next_position];
    match next_track {
        '-' | '|' => {
            // Move forward to the next track segment (no special processing)
            cart.position = next_position;
        },
        '/' | '\\' => {
            // Need to turn 90 degrees based on incoming direction
            let next_direction = get_direction_after_corner(cart, next_track);
            cart.direction = next_direction;
            cart.position = next_position;
        },
        '+' => {
            // Handle intersection based on rules
            let next_direction = get_direction_after_intersection(cart, cart.next_turn.front()?);
            cart.direction = next_direction;
            cart.position = next_position;
            cart.next_turn.rotate_left(1);
        },
        _ => panic!("Unexpected track segment")
    }
    None
}

fn check_for_collisions(carts: &[Cart]) -> Option<(usize, usize)> {
    for first_cart in carts {
        for second_cart in carts {
            if first_cart.id == second_cart.id {
                continue;
            }

            if first_cart.position == second_cart.position {
                return Some(first_cart.position);
            }
        }
    }
    None
}

fn get_next_position(cart: &Cart) -> (usize, usize) {
    match cart.direction {
        '>' => (cart.position.0, cart.position.1 + 1),
        '<' => (cart.position.0, cart.position.1 - 1),
        '^' => (cart.position.0 - 1, cart.position.1),
        'v' => (cart.position.0 + 1, cart.position.1),
        _ => panic!("Unexpected direction"),
    }
}

fn get_direction_after_corner(cart: &Cart, corner: char) -> char {
    match corner {
        '/' => match cart.direction {
            '^' => '>',
            '>' => '^',
            'v' => '<',
            '<' => 'v',
            _ => panic!("Unexpected direction"),
        },
        '\\' => match cart.direction {
            '^' => '<',
            '>' => 'v',
            'v' => '>',
            '<' => '^',
            _ => panic!("Unexpected direction"),
        },
        _ => panic!("Unexpected corner"),
    }
}

fn get_direction_after_intersection(cart: &Cart, turn: &Turn) -> char {
    match turn {
        Turn::Straight => cart.direction,
        Turn::Left => match cart.direction {
            '^' => '<',
            '>' => '^',
            'v' => '>',
            '<' => 'v',
            _ => panic!("Unexpected direction"),
        },
        Turn::Right => match cart.direction {
            '^' => '>',
            '>' => 'v',
            'v' => '<',
            '<' => '^',
            _ => panic!("Unexpected direction"),
        },
    }
}

#[derive(Debug)]
struct Cart {
    id: i64,
    position: (usize, usize),
    direction: char,
    next_turn: VecDeque<Turn>,
}

#[derive(Debug)]
enum Turn {
    Left,
    Right,
    Straight,
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part1() {
        let input: [&str; 1] = [
            "/->-\\
|   |  /----\\
| /-+--+-\\  |
| | |  | v  |
\\-+-/  \\-+--/
  \\------/",
        ];
        let expected: [&str; 1] = [
            "7,3",
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
