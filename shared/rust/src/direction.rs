use std::fmt;

#[derive(Clone, Debug, Eq, PartialEq)]
pub enum Direction {
    Up,
    Down,
    Left,
    Right,
}

impl fmt::Display for Direction {
    #[inline]
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        match self {
            Self::Up => write!(f, "up"),
            Self::Down => write!(f, "down"),
            Self::Left => write!(f, "left"),
            Self::Right => write!(f, "right"),
        }
    }
}

impl Direction {
    /// Return the direction after performing a turn in the given direction
    #[inline]
    #[must_use]
    pub fn next(&self, turn: &Self) -> Self {
        match turn {
            Self::Left => match self {
                Self::Up => Self::Left,
                Self::Down => Self::Right,
                Self::Left => Self::Down,
                Self::Right => Self::Up,
            },
            Self::Right => match self {
                Self::Up => Self::Right,
                Self::Down => Self::Left,
                Self::Left => Self::Up,
                Self::Right => Self::Down,
            },
            _ => unreachable!(),
        }
    }

    /// Return the direction opposite the given direction
    #[inline]
    #[must_use]
    pub const fn opposite(&self) -> Self {
        match self {
            Self::Left => Self::Right,
            Self::Right => Self::Left,
            Self::Up => Self::Down,
            Self::Down => Self::Up,
        }
    }
}

#[must_use]
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
