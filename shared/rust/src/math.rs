pub fn greatest_common_divisor(a: i64, b: i64) -> i64 {
    if b == 0 {
        return a;
    }

    match b > a {
        true => greatest_common_divisor(a, b % a),
        false => greatest_common_divisor(b, a % b),
    }
}

pub fn least_common_multiple(first: i64, second: i64) -> i64 {
    first.abs() * (second.abs() / greatest_common_divisor(first, second))
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_greatest_common_divisor() {
        let input: [(i64, i64); 4] = [
            (8, 12),
            (54, 24),
            (42, 56),
            (48, 18),
        ];
        let expected: [i64; 4] = [
            4,
            6,
            14,
            6,
        ];

        for i in 0..input.len() {
            assert_eq!(greatest_common_divisor(input[i].0, input[i].1), expected[i]);
        }
    }

    #[test]
    fn test_least_common_multiple() {
        let input: [(i64, i64); 2] = [
            (4, 6),
            (21, 6),
        ];
        let expected: [i64; 2] = [
            12,
            42,
        ];

        for i in 0..input.len() {
            assert_eq!(least_common_multiple(input[i].0, input[i].1), expected[i]);
        }
    }
}
