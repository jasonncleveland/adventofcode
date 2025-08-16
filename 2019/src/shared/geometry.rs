#[derive(Clone, Copy, Debug, PartialEq)]
pub struct Point2d {
    pub x: i64,
    pub y: i64,
}

impl Point2d {
    /// Calculate the manhattan distance between two points
    pub fn manhattan(&self, other: &Point2d) -> i64 {
        (self.x - other.x).abs() + (self.y - other.y).abs()
    }
}

#[derive(Debug)]
pub struct LineSegment2d {
    pub start: Point2d,
    pub end: Point2d,
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_manhattan() {
        let input: [(Point2d, Point2d); 3] = [
            (Point2d { x: 0, y: 0 }, Point2d { x: 1, y: 1 }),
            (Point2d { x: 0, y: 0 }, Point2d { x: 3, y: 3 }),
            (Point2d { x: 0, y: 0 }, Point2d { x: 6, y: 5 }),
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
}
