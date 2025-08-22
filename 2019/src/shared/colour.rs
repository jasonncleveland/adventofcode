use std::fmt;

#[derive(Clone, Debug, PartialEq)]
pub enum Colour {
    White,
    Black,
}

impl fmt::Display for Colour {
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        match self {
            Colour::White => write!(f, "white"),
            Colour::Black => write!(f, "black"),
        }
    }
}
