use std::fmt;

#[derive(Clone, Copy, Debug, Eq, Hash, PartialEq)]
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

    /// Calculate the Euclidean distance between two points
    /// d(a, b) = √(a₁ - b₁)² + (a₂ - b₂)² + (a₃ - b₃)²
    #[inline]
    #[must_use]
    pub fn euclidean(&self, other: &Self) -> f64 {
        let dx = self.x - other.x;
        let sqdx = dx.pow(2) as f64;
        let dy = self.y - other.y;
        let sqdy = dy.pow(2) as f64;
        let dz = self.z - other.z;
        let sqdz = dz.pow(2) as f64;
        (sqdx + sqdy + sqdz).sqrt()
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

    #[test]
    fn test_euclidean() {
        let input: [(Point3d, Point3d); 4] = [
            (Point3d::new(0, 0, 0), Point3d::new(0, 5, 0)),
            (Point3d::new(0, 0, 0), Point3d::new(3, 0, 0)),
            (Point3d::new(0, 0, 0), Point3d::new(0, 0, 8)),
            (Point3d::new(0, 0, 0), Point3d::new(6, 5, 4)),
        ];
        let expected: [f64; 4] = [5.0, 3.0, 8.0, 8.774_964_387_392_123];

        for i in 0..input.len() {
            assert_eq!(input[i].0.euclidean(&input[i].1), expected[i]);
        }
    }
}
