#[derive(Debug)]
pub struct Node {
    pub edges: Vec<Edge>,
}

impl Node {
    pub fn new(edges: Vec<Edge>) -> Self {
        Self { edges }
    }
}

#[derive(Clone, Debug)]
pub struct Edge {
    pub name: char,
    pub weight: i64,
}

impl Edge {
    pub fn new(name: char, weight: i64) -> Self {
        Self { name, weight }
    }
}
