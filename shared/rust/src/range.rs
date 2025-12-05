use std::fmt;

#[derive(Clone, Copy, Debug, Eq, PartialEq)]
pub struct Range {
    pub start: i64,
    pub end: i64,
}

impl fmt::Display for Range {
    #[inline]
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        write!(f, "[{}, {}]", self.start, self.end)
    }
}

impl Range {
    #[inline]
    #[must_use]
    pub const fn new(start: i64, end: i64) -> Self {
        Self { start, end }
    }

    /// Attempt to combine two ranges of numbers
    ///
    /// # Examples
    /// ```
    /// use aoc_helpers::range::Range;
    ///
    /// // Successful merge
    /// let first = Range::new(1, 4);
    /// let second = Range::new(3, 7);
    /// let result = first.consolidate(&second);
    /// assert_eq!(result, Some(Range::new(1, 7)));
    /// let result = second.consolidate(&first);
    /// assert_eq!(result, Some(Range::new(1, 7)));
    ///
    /// // Failed merge
    /// let first = Range::new(1, 4);
    /// let second = Range::new(6, 8);
    /// let result = first.consolidate(&second);
    /// assert_eq!(result, None);
    /// ```
    #[inline]
    #[must_use]
    pub fn consolidate(&self, other: &Self) -> Option<Self> {
        if self.start <= other.start {
            if self.end < other.start {
                None
            } else if other.end <= self.end {
                Some(*self)
            } else {
                Some(Self::new(self.start, other.end))
            }
        } else {
            other.consolidate(self)
        }
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_consolidate() {
        let data: [(Range, Range, Option<Range>); 5] = [
            (Range::new(-1, 4), Range::new(2, 7), Some(Range::new(-1, 7))),
            (Range::new(2, 7), Range::new(-1, 4), Some(Range::new(-1, 7))),
            (Range::new(2, 8), Range::new(2, 8), Some(Range::new(2, 8))),
            (Range::new(2, 8), Range::new(4, 6), Some(Range::new(2, 8))),
            (Range::new(-1, 2), Range::new(5, 7), None),
        ];

        for (first, second, expected) in data {
            assert_eq!(first.consolidate(&second), expected);
        }
    }
}
