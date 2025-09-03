use std::cmp::Ordering;
use std::collections::BinaryHeap;

pub type PriorityQueue<T> = BinaryHeap<T>;

#[derive(Debug, Eq, PartialEq)]
pub struct PriorityQueueItem<T: Eq> {
    pub weight: i64,
    pub data: T,
}

impl<T: Eq> Ord for PriorityQueueItem<T> {
    fn cmp(&self, other: &Self) -> Ordering {
        other.weight.cmp(&self.weight)
    }
}

impl<T: Eq> PartialOrd for PriorityQueueItem<T> {
    fn partial_cmp(&self, other: &Self) -> Option<Ordering> {
        Some(self.cmp(other))
    }
}

impl<T: Eq> PriorityQueueItem<T> {
    pub fn new(steps: i64, data: T) -> Self {
        Self { weight: steps, data }
    }
}
