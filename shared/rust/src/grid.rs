use std::collections::HashMap;

use super::point2d::Point2d;

#[inline]
#[must_use]
pub fn get_dimensions(grid: &HashMap<Point2d, char>) -> (i64, i64) {
    // Get boundaries
    let mut min_x = i64::MAX;
    let mut min_y = i64::MAX;
    let mut max_x = i64::MIN;
    let mut max_y = i64::MIN;

    for point in grid.keys() {
        if point.x < min_x {
            min_x = point.x;
        }
        if point.y < min_y {
            min_y = point.y;
        }
        if point.x > max_x {
            max_x = point.x;
        }
        if point.y > max_y {
            max_y = point.y;
        }
    }

    (max_x - min_x + 1, max_y - min_y + 1)
}
