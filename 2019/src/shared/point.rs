use std::fmt;

#[derive(Clone, Copy, Debug, PartialEq, PartialOrd)]
pub struct Point2d {
    pub x: i64,
    pub y: i64,
}

impl fmt::Display for Point2d {
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        write!(f, "({}, {})", self.x, self.y)
    }
}

impl Point2d {
    pub fn new(x: i64, y: i64) -> Point2d {
        Point2d { x, y }
    }

    /// Calculate the manhattan distance between two points
    /// d(a, b) = |a₁ - b₁| + |a₂ - b₂|
    pub fn manhattan(&self, other: &Point2d) -> i64 {
        (self.x - other.x).abs() + (self.y - other.y).abs()
    }

    /// Calculate the Euclidean distance between two points
    /// d(a, b) = √(a₁ - b₁)² + (a₂ - b₂)²
    pub fn euclidean(&self, other: &Point2d) -> f64 {
        let dx = self.x - other.x;
        let sqdx = dx.pow(2) as f64;
        let dy = self.y - other.y;
        let sqdy = dy.pow(2) as f64;
        (sqdx + sqdy).sqrt()
    }

    /// Calculate the angle between two points
    pub fn angle(&self, other: &Point2d) -> f64 {
        let dx = self.x as f64 - other.x as f64;
        let dy = self.y as f64 - other.y as f64;
        dx.atan2(dy)
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
        let expected: [i64; 3] = [
            2,
            6,
            11,
        ];

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
        let expected: [f64; 3] = [
            5.0,
            3.0,
            7.810249675906654,
        ];

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
}