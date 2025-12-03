use std::fmt;

use super::direction::Direction;

#[derive(Clone, Copy, Debug, Eq, Hash, Ord, PartialEq, PartialOrd)]
pub struct Point2d {
    // Ord trait sorts fields in order so y is first to sort by y then x
    pub y: i64,
    pub x: i64,
}

impl fmt::Display for Point2d {
    #[inline]
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        write!(f, "({}, {})", self.x, self.y)
    }
}

impl Point2d {
    #[inline]
    #[must_use]
    pub const fn new(x: i64, y: i64) -> Self {
        Self { y, x }
    }

    /// Calculate the manhattan distance between two points
    /// d(a, b) = |a₁ - b₁| + |a₂ - b₂|
    #[inline]
    #[must_use]
    pub const fn manhattan(&self, other: &Self) -> i64 {
        (self.x - other.x).abs() + (self.y - other.y).abs()
    }

    /// Calculate the Euclidean distance between two points
    /// d(a, b) = √(a₁ - b₁)² + (a₂ - b₂)²
    #[inline]
    #[must_use]
    pub fn euclidean(&self, other: &Self) -> f64 {
        let dx = self.x - other.x;
        let dx_squared = dx.pow(2) as f64;
        let dy = self.y - other.y;
        let dy_squared = dy.pow(2) as f64;
        (dx_squared + dy_squared).sqrt()
    }

    /// Calculate the angle between two points
    #[inline]
    #[must_use]
    pub fn angle(&self, other: &Self) -> f64 {
        let dx = self.x as f64 - other.x as f64;
        let dy = self.y as f64 - other.y as f64;
        dx.atan2(dy)
    }

    #[inline]
    #[must_use]
    pub const fn next(&self, direction: &Direction) -> Self {
        match direction {
            Direction::Up => Self::new(self.x, self.y - 1),
            Direction::Down => Self::new(self.x, self.y + 1),
            Direction::Left => Self::new(self.x - 1, self.y),
            Direction::Right => Self::new(self.x + 1, self.y),
        }
    }

    #[inline]
    #[must_use]
    pub fn neighbours(&self) -> Vec<Self> {
        vec![
            // up
            Self::new(self.x, self.y - 1),
            // left
            Self::new(self.x - 1, self.y),
            // right
            Self::new(self.x + 1, self.y),
            // down
            Self::new(self.x, self.y + 1),
        ]
    }

    #[inline]
    #[must_use]
    pub fn neighbours8(&self) -> Vec<Self> {
        vec![
            // up left
            Self::new(self.x - 1, self.y - 1),
            // up
            Self::new(self.x, self.y - 1),
            // up right
            Self::new(self.x + 1, self.y - 1),
            // left
            Self::new(self.x - 1, self.y),
            // right
            Self::new(self.x + 1, self.y),
            // down left
            Self::new(self.x - 1, self.y + 1),
            // down
            Self::new(self.x, self.y + 1),
            // down right
            Self::new(self.x + 1, self.y + 1),
        ]
    }
}

#[cfg(test)]
mod tests {
    use super::*;
    use std::f64::consts::PI;

    #[test]
    fn test_manhattan() {
        let input: [(Point2d, Point2d); 3] = [
            (Point2d::new(0, 0), Point2d::new(1, 1)),
            (Point2d::new(0, 0), Point2d::new(3, 3)),
            (Point2d::new(0, 0), Point2d::new(6, 5)),
        ];
        let expected: [i64; 3] = [2, 6, 11];

        for i in 0..input.len() {
            assert_eq!(input[i].0.manhattan(&input[i].1), expected[i]);
        }
    }

    #[test]
    fn test_euclidean() {
        let input: [(Point2d, Point2d); 3] = [
            (Point2d::new(0, 0), Point2d::new(0, 5)),
            (Point2d::new(0, 0), Point2d::new(3, 0)),
            (Point2d::new(0, 0), Point2d::new(6, 5)),
        ];
        let expected: [f64; 3] = [5.0, 3.0, 7.810249675906654];

        for i in 0..input.len() {
            assert_eq!(input[i].0.euclidean(&input[i].1), expected[i]);
        }
    }

    #[test]
    fn test_angle() {
        let input: [(Point2d, Point2d); 5] = [
            // 0 degrees
            (Point2d::new(0, 0), Point2d::new(0, 0)),
            // 45 degrees
            (Point2d::new(1, 1), Point2d::new(0, 0)),
            // 90 degrees
            (Point2d::new(1, 0), Point2d::new(0, 0)),
            // 180 degrees
            (Point2d::new(0, 0), Point2d::new(0, 1)),
            // -90 / 270 degrees
            (Point2d::new(-1, 0), Point2d::new(0, 0)),
        ];
        let expected: [f64; 5] = [
            // 0 degrees
            0.0,
            // 45 degrees
            PI / 4.0,
            // 90 degrees
            PI / 2.0,
            // 180 degrees
            PI,
            // -90 / 270 degrees
            -PI / 2.0,
        ];

        for i in 0..input.len() {
            let result = input[i].0.angle(&input[i].1);
            assert_eq!(result, expected[i]);
        }
    }

    #[test]
    fn test_neighbours() {
        let input = Point2d::new(0, 0);
        let expected: [Point2d; 4] = [
            Point2d::new(0, -1),
            Point2d::new(-1, 0),
            Point2d::new(1, 0),
            Point2d::new(0, 1),
        ];

        assert_eq!(input.neighbours(), expected);
    }

    #[test]
    fn test_neighbours8() {
        let input = Point2d::new(0, 0);
        let expected: [Point2d; 8] = [
            Point2d::new(-1, -1),
            Point2d::new(0, -1),
            Point2d::new(1, -1),
            Point2d::new(-1, 0),
            Point2d::new(1, 0),
            Point2d::new(-1, 1),
            Point2d::new(0, 1),
            Point2d::new(1, 1),
        ];

        assert_eq!(input.neighbours8(), expected);
    }
}
