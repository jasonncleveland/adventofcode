use std::cmp::max;

#[derive(Debug)]
pub enum HexDirection {
    NorthWest,
    North,
    NorthEast,
    SouthWest,
    South,
    SouthEast,
}

/// Hex coordinate is stored using cube coordinates
///
/// ref: https://www.redblobgames.com/grids/hexagons/#neighbors-cube
#[derive(Clone, Debug, PartialEq)]
pub struct HexCoordinate {
    q: i64,
    s: i64,
    r: i64,
}

impl HexCoordinate {
    pub fn new(q: i64, s: i64, r: i64) -> HexCoordinate {
        HexCoordinate { q, s, r }
    }

    pub fn move_hex(&mut self, direction: &HexDirection) {
        match direction {
            HexDirection::NorthWest => {
                self.q -= 1;
                self.s += 1;
            }
            HexDirection::North => {
                self.s += 1;
                self.r -= 1;
            }
            HexDirection::NorthEast => {
                self.q += 1;
                self.r -= 1;
            }
            HexDirection::SouthWest => {
                self.q -= 1;
                self.r += 1;
            }
            HexDirection::South => {
                self.s -= 1;
                self.r += 1;
            }
            HexDirection::SouthEast => {
                self.q += 1;
                self.s -= 1;
            }
        }
    }

    /// Calculate distance between two hexes using cube coordinates
    ///
    /// d(a, b) = max(|q₁ - q₂|, |s₁ - s₂|, |r₁ - r₂|)
    ///
    /// ref: https://www.redblobgames.com/grids/hexagons/#distances
    pub fn distance(&self, other: &HexCoordinate) -> i64 {
        max(
            max((self.q - other.q).abs(), (self.s - other.s).abs()),
            (self.r - other.r).abs(),
        )
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_move_hex() {
        let data: [(HexCoordinate, HexDirection, HexCoordinate); 6] = [
            (
                HexCoordinate::new(0, 0, 0),
                HexDirection::NorthWest,
                HexCoordinate::new(-1, 1, 0),
            ),
            (
                HexCoordinate::new(0, 0, 0),
                HexDirection::North,
                HexCoordinate::new(0, 1, -1),
            ),
            (
                HexCoordinate::new(0, 0, 0),
                HexDirection::NorthEast,
                HexCoordinate::new(1, 0, -1),
            ),
            (
                HexCoordinate::new(0, 0, 0),
                HexDirection::SouthWest,
                HexCoordinate::new(-1, 0, 1),
            ),
            (
                HexCoordinate::new(0, 0, 0),
                HexDirection::South,
                HexCoordinate::new(0, -1, 1),
            ),
            (
                HexCoordinate::new(0, 0, 0),
                HexDirection::SouthEast,
                HexCoordinate::new(1, -1, 0),
            ),
        ];

        for (input, direction, expected) in data {
            let mut coordinate = input.clone();
            coordinate.move_hex(&direction);
            assert_eq!(coordinate, expected);
        }
    }

    #[test]
    fn test_distance() {
        let data: [(HexCoordinate, HexCoordinate, i64); 3] = [
            (HexCoordinate::new(0, 0, 0), HexCoordinate::new(0, 0, 0), 0),
            (
                HexCoordinate::new(0, 0, 0),
                HexCoordinate::new(-2, 3, -1),
                3,
            ),
            (
                HexCoordinate::new(0, 0, 0),
                HexCoordinate::new(-3, 8, -5),
                8,
            ),
        ];

        for (origin, destination, expected) in data {
            assert_eq!(origin.distance(&destination), expected);
        }
    }
}
