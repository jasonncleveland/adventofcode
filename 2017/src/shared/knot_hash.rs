use std::collections::VecDeque;

pub fn calculate_knot_hash(input: String) -> String {
    let mut numbers: VecDeque<i64> = (0..256).collect();
    let mut current_position = 0;
    let mut skip_size = 0;

    let mut lengths: VecDeque<usize> = input.chars().map(|c| c as usize).collect();
    lengths.extend([17, 31, 73, 47, 23]);

    for _ in 0..64 {
        for length in &lengths {
            reverse_numbers(&mut numbers, current_position, current_position + length);
            current_position += length + skip_size;
            skip_size += 1;
        }
    }

    let mut hash = String::new();
    for i in 0..16 {
        let mut reduced = 0;
        for j in 0..16 {
            reduced ^= numbers[i * 16 + j];
        }
        hash.push_str(&format!("{:02x}", reduced));
    }
    hash
}

pub fn reverse_numbers(numbers: &mut VecDeque<i64>, start: usize, end: usize) {
    let mut i = start;
    let mut j = end - 1;
    while i < j {
        numbers.swap(i % numbers.len(), j % numbers.len());
        i += 1;
        j -= 1;
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_calculate_knot_hash() {
        let data: [(&str, &str); 4] = [
            ("", "a2582a3a0e66e6e86e3812dcb672a272"),
            ("AoC 2017", "33efeb34ea91902bb2f59c9920caa6cd"),
            ("1,2,3", "3efbe78a8d82f29979031a4aa0b16a9d"),
            ("1,2,4", "63960835bcdc130f0b66d7ff4f6a5a8e"),
        ];

        for (input, expected) in data {
            assert_eq!(calculate_knot_hash(input.to_string()), expected);
        }
    }
}
