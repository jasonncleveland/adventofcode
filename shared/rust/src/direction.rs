use std::fmt;

#[derive(Clone, Debug, PartialEq)]
pub enum Direction {
    Up,
    Down,
    Left,
    Right,
}

impl fmt::Display for Direction {
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        match self {
            Direction::Up => write!(f, "up"),
            Direction::Down => write!(f, "down"),
            Direction::Left => write!(f, "left"),
            Direction::Right => write!(f, "right"),
        }
    }
}

impl Direction {
    /// Return the direction after performing a turn in the given direction
    pub fn next(&self, turn: &Direction) -> Direction {
        match turn {
            Direction::Left => match self {
                Direction::Up => Direction::Left,
                Direction::Down => Direction::Right,
                Direction::Left => Direction::Down,
                Direction::Right => Direction::Up,
            },
            Direction::Right => match self {
                Direction::Up => Direction::Right,
                Direction::Down => Direction::Left,
                Direction::Left => Direction::Up,
                Direction::Right => Direction::Down,
            },
            _ => unreachable!(),
        }
    }

    /// Return the direction opposite the given direction
    pub fn opposite(&self) -> Direction {
        match self {
            Direction::Left => Direction::Right,
            Direction::Right => Direction::Left,
            Direction::Up => Direction::Down,
            Direction::Down => Direction::Up,
        }
    }
}

pub fn get_directions() -> Vec<Direction> {
    vec![
        Direction::Up,
        Direction::Right,
        Direction::Down,
        Direction::Left,
    ]
}

mod tests {
    #[test]
    fn test_next() {
        use super::*;

        let data: [(Direction, Direction, Direction); 8] = [
            (Direction::Up, Direction::Left, Direction::Left),
            (Direction::Up, Direction::Right, Direction::Right),
            (Direction::Left, Direction::Left, Direction::Down),
            (Direction::Left, Direction::Right, Direction::Up),
            (Direction::Right, Direction::Left, Direction::Up),
            (Direction::Right, Direction::Right, Direction::Down),
            (Direction::Down, Direction::Left, Direction::Right),
            (Direction::Down, Direction::Right, Direction::Left),
        ];

        for (current, turn, expected) in data {
            assert_eq!(current.next(&turn), expected);
        }
    }

    #[test]
    fn test_opposite() {
        use super::*;

        let data: [(Direction, Direction); 4] = [
            (Direction::Up, Direction::Down),
            (Direction::Down, Direction::Up),
            (Direction::Left, Direction::Right),
            (Direction::Right, Direction::Left),
        ];

        for (current, expected) in data {
            assert_eq!(current.opposite(), expected);
        }
    }
}
