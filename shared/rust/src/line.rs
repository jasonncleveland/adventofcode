use std::fmt;

use super::point2d::Point2d;

#[derive(Debug)]
pub struct LineSegment2d {
    pub start: Point2d,
    pub end: Point2d,
}

impl fmt::Display for LineSegment2d {
    #[inline]
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        write!(f, "{} -> {}", self.start, self.end)
    }
}

impl LineSegment2d {
    #[inline]
    #[must_use]
    pub const fn new(start: Point2d, end: Point2d) -> Self {
        Self { start, end }
    }
}
