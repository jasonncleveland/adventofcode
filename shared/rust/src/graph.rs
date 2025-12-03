#[derive(Debug)]
pub struct Node<T: Clone> {
    pub edges: Vec<Edge<T>>,
}

impl<T: Clone> Node<T> {
    #[inline]
    #[must_use]
    pub const fn new(edges: Vec<Edge<T>>) -> Self {
        Self { edges }
    }
}

#[derive(Clone, Debug)]
pub struct Edge<T: Clone> {
    pub value: T,
    pub weight: i64,
}

impl<T: Clone> Edge<T> {
    #[inline]
    #[must_use]
    pub const fn new(value: T, weight: i64) -> Self {
        Self { value, weight }
    }
}
