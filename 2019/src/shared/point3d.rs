use std::fmt;

#[derive(Clone, Debug, Eq, Hash, PartialEq)]
pub struct Point3d {
    pub x: i64,
    pub y: i64,
    pub z: i64,
}

impl fmt::Display for Point3d {
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        write!(f, "({}, {}, {})", self.x, self.y, self.z)
    }
}

impl Point3d {
    pub fn new(x: i64, y: i64, z: i64) -> Self {
        Self { x, y, z }
    }
}