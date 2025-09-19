use std::fmt;

#[derive(Clone, Copy, Debug, Eq, Hash, PartialEq)]
pub struct Point4d {
    pub x: i64,
    pub y: i64,
    pub z: i64,
    pub w: i64,
}

impl fmt::Display for Point4d {
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        write!(f, "({}, {}, {}, {})", self.x, self.y, self.z, self.w)
    }
}

impl Point4d {
    pub fn new(x: i64, y: i64, z: i64, w: i64) -> Self {
        Self { x, y, z, w }
    }

    /// Calculate the manhattan distance between two points
    /// d(a, b) = |a₁ - b₁| + |a₂ - b₂| + |a₃ - b₃| + |a₄ - b₄|
    pub fn manhattan(&self, other: &Point4d) -> i64 {
        (self.x - other.x).abs()
            + (self.y - other.y).abs()
            + (self.z - other.z).abs()
            + (self.w - other.w).abs()
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_manhattan() {
        let input: [(Point4d, Point4d); 8] = [
            (Point4d::new(0, 0, 0, 0), Point4d::new(0, 0, 0, 0)),
            (Point4d::new(0, 0, 0, 0), Point4d::new(3, 0, 0, 0)),
            (Point4d::new(0, 0, 0, 0), Point4d::new(0, 3, 0, 0)),
            (Point4d::new(0, 0, 0, 0), Point4d::new(0, 0, 3, 0)),
            (Point4d::new(0, 0, 0, 0), Point4d::new(0, 0, 0, 3)),
            (Point4d::new(0, 0, 0, 0), Point4d::new(0, 0, 0, 6)),
            (Point4d::new(0, 0, 0, 0), Point4d::new(9, 0, 0, 0)),
            (Point4d::new(0, 0, 0, 0), Point4d::new(12, 0, 0, 0)),
        ];
        let expected: [i64; 8] = [0, 3, 3, 3, 3, 6, 9, 12];

        for i in 0..input.len() {
            assert_eq!(input[i].0.manhattan(&input[i].1), expected[i]);
        }
    }
}
