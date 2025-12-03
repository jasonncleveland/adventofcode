use std::collections::{HashMap, VecDeque};
use std::time::Instant;

use aoc_helpers::direction::Direction;
use aoc_helpers::point2d::Point2d;
use log::debug;

pub fn solve(file_contents: &str) -> (String, String) {
    let parse_timer = Instant::now();
    let (tracks, carts) = parse_input(file_contents);
    debug!("File parse: ({:?})", parse_timer.elapsed());

    let part1_timer = Instant::now();
    let part1 = solve_part_1(&tracks, &carts);
    debug!("Part 1: {} ({:?})", part1, part1_timer.elapsed());

    let part2_timer = Instant::now();
    let part2 = solve_part_2(&tracks, &carts);
    debug!("Part 2: {} ({:?})", part2, part2_timer.elapsed());

    (part1.to_string(), part2.to_string())
}

fn parse_input(file_contents: &str) -> (HashMap<Point2d, char>, Vec<Cart>) {
    let mut tracks: HashMap<Point2d, char> = HashMap::new();
    let mut carts: Vec<Cart> = Vec::new();
    let mut cart_id = 1;

    let lines = file_contents.lines();

    for (row_index, row) in lines.enumerate() {
        for (column_index, column) in row.chars().enumerate() {
            let position = Point2d::new(column_index as i64, row_index as i64);
            match column {
                '^' | 'v' => {
                    carts.push(Cart {
                        id: cart_id,
                        position,
                        direction: column,
                        next_turn: VecDeque::from([Turn::Left, Turn::Straight, Turn::Right]),
                    });
                    tracks.insert(position, '|');
                    cart_id += 1;
                }
                '<' | '>' => {
                    carts.push(Cart {
                        id: cart_id,
                        position,
                        direction: column,
                        next_turn: VecDeque::from([Turn::Left, Turn::Straight, Turn::Right]),
                    });
                    tracks.insert(position, '-');
                    cart_id += 1;
                }
                other => {
                    tracks.insert(position, other);
                }
            };
        }
    }

    (tracks, carts)
}

fn solve_part_1(tracks: &HashMap<Point2d, char>, carts: &Vec<Cart>) -> Point2d {
    let mut carts_copy = carts.to_owned();

    loop {
        // Sort the carts by position so that the top-left-most cart is first
        carts_copy.sort_by(|a, b| a.position.cmp(&b.position));
        if let Some(position) = move_carts(tracks, &mut carts_copy) {
            return position;
        }
    }
}

fn solve_part_2(tracks: &HashMap<Point2d, char>, carts: &Vec<Cart>) -> Point2d {
    let mut carts_copy = carts.to_owned();

    loop {
        // Sort the carts by position so that the top-left-most cart is first
        carts_copy.sort_by(|a, b| a.position.cmp(&b.position));

        // Move all carts and remove any that collide
        move_carts_with_removal(tracks, &mut carts_copy);

        // Stop when there is a single cart remaining
        if carts_copy.len() == 1
            && let Some(cart) = carts_copy.first()
        {
            return cart.position;
        }
    }
}

fn move_carts(tracks: &HashMap<Point2d, char>, carts: &mut [Cart]) -> Option<Point2d> {
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

fn move_carts_with_removal(tracks: &HashMap<Point2d, char>, carts: &mut Vec<Cart>) {
    // Copy the carts to allow easy mutation and removal
    let mut remaining_carts: VecDeque<Cart> = VecDeque::from_iter(carts.clone());

    while !remaining_carts.is_empty() {
        // Move the copied cart
        let mut cart = remaining_carts.pop_front().unwrap();
        move_cart(tracks, &mut cart);

        // Update the original cart
        let old_cart = carts.iter_mut().find(|c| c.id == cart.id).unwrap();
        old_cart.position = cart.position;
        old_cart.direction = cart.direction;
        old_cart.next_turn = cart.next_turn;

        let result = check_for_collisions(carts);
        if result.is_some() {
            // Remove collided carts
            remaining_carts.retain(|c| c.position != cart.position);
            carts.retain(|c| c.position != cart.position);
        }
    }
}

fn move_cart(tracks: &HashMap<Point2d, char>, cart: &mut Cart) -> Option<bool> {
    let next_position = get_next_position(cart);
    let next_track = tracks[&next_position];
    match next_track {
        '-' | '|' => {
            // Move forward to the next track segment (no special processing)
            cart.position = next_position;
        }
        '/' | '\\' => {
            // Need to turn 90 degrees based on incoming direction
            let next_direction = get_direction_after_corner(cart, next_track);
            cart.direction = next_direction;
            cart.position = next_position;
        }
        '+' => {
            // Handle intersection based on rules
            let next_direction = get_direction_after_intersection(cart, cart.next_turn.front()?);
            cart.direction = next_direction;
            cart.position = next_position;
            cart.next_turn.rotate_left(1);
        }
        _ => unreachable!("Unexpected track segment"),
    }
    None
}

fn check_for_collisions(carts: &[Cart]) -> Option<Point2d> {
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

fn get_next_position(cart: &Cart) -> Point2d {
    match cart.direction {
        '>' => cart.position.next(&Direction::Right),
        '<' => cart.position.next(&Direction::Left),
        '^' => cart.position.next(&Direction::Up),
        'v' => cart.position.next(&Direction::Down),
        _ => unreachable!("Unexpected direction"),
    }
}

fn get_direction_after_corner(cart: &Cart, corner: char) -> char {
    match corner {
        '/' => match cart.direction {
            '^' => '>',
            '>' => '^',
            'v' => '<',
            '<' => 'v',
            _ => unreachable!("Unexpected direction"),
        },
        '\\' => match cart.direction {
            '^' => '<',
            '>' => 'v',
            'v' => '>',
            '<' => '^',
            _ => unreachable!("Unexpected direction"),
        },
        _ => unreachable!("Unexpected corner"),
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
            _ => unreachable!("Unexpected direction"),
        },
        Turn::Right => match cart.direction {
            '^' => '>',
            '>' => 'v',
            'v' => '<',
            '<' => '^',
            _ => unreachable!("Unexpected direction"),
        },
    }
}

#[derive(Clone, Debug)]
struct Cart {
    id: i64,
    position: Point2d,
    direction: char,
    next_turn: VecDeque<Turn>,
}

#[derive(Clone, Debug)]
enum Turn {
    Left,
    Right,
    Straight,
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let input: [&str; 1] = ["/->-\\
|   |  /----\\
| /-+--+-\\  |
| | |  | v  |
\\-+-/  \\-+--/
  \\------/"];
        let expected: [Point2d; 1] = [Point2d::new(7, 3)];

        for i in 0..input.len() {
            let (tracks, carts) = parse_input(input[i]);
            assert_eq!(solve_part_1(&tracks, &carts), expected[i]);
        }
    }

    #[test]
    fn test_part_2() {
        let input: [&str; 1] = ["/>-<\\
|   |
| /<+-\\
| | | v
\\>+</ |
  |   ^
  \\<->/"];
        let expected: [Point2d; 1] = [Point2d::new(6, 4)];

        for i in 0..input.len() {
            let (tracks, carts) = parse_input(input[i]);
            assert_eq!(solve_part_2(&tracks, &carts), expected[i]);
        }
    }
}
