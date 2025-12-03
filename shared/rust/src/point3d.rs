use std::fmt;

#[derive(Clone, Debug, Eq, Hash, PartialEq)]
pub struct Point3d {
    pub x: i64,
    pub y: i64,
    pub z: i64,
}

impl fmt::Display for Point3d {
    #[inline]
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        write!(f, "({}, {}, {})", self.x, self.y, self.z)
    }
}

impl Point3d {
    #[inline]
    #[must_use]
    pub const fn new(x: i64, y: i64, z: i64) -> Self {
        Self { x, y, z }
    }

    /// Calculate the manhattan distance between two points
    /// d(a, b) = |a₁ - b₁| + |a₂ - b₂| + |a₃ - b₃|
    #[inline]
    #[must_use]
    pub const fn manhattan(&self, other: &Self) -> i64 {
        (self.x - other.x).abs() + (self.y - other.y).abs() + (self.z - other.z).abs()
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_manhattan() {
        let input: [(Point3d, Point3d); 9] = [
            (Point3d::new(0, 0, 0), Point3d::new(0, 0, 0)),
            (Point3d::new(0, 0, 0), Point3d::new(1, 0, 0)),
            (Point3d::new(0, 0, 0), Point3d::new(4, 0, 0)),
            (Point3d::new(0, 0, 0), Point3d::new(0, 2, 0)),
            (Point3d::new(0, 0, 0), Point3d::new(0, 5, 0)),
            (Point3d::new(0, 0, 0), Point3d::new(0, 0, 3)),
            (Point3d::new(0, 0, 0), Point3d::new(1, 1, 1)),
            (Point3d::new(0, 0, 0), Point3d::new(1, 1, 2)),
            (Point3d::new(0, 0, 0), Point3d::new(1, 3, 1)),
        ];
        let expected: [i64; 9] = [0, 1, 4, 2, 5, 3, 3, 4, 5];

        for i in 0..input.len() {
            assert_eq!(input[i].0.manhattan(&input[i].1), expected[i]);
        }
    }
}
