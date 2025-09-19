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

pub fn factors(number: i64) -> Vec<i64> {
    let mut factors: Vec<i64> = Vec::new();

    let square_root = number.isqrt();

    for i in 1..=square_root {
        if number % i == 0 {
            // Add the found factor and its pair
            factors.push(i);
            if i * i != number {
                factors.push(number / i);
            }
        }
    }

    factors.sort();
    factors
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_greatest_common_divisor() {
        let input: [(i64, i64); 4] = [(8, 12), (54, 24), (42, 56), (48, 18)];
        let expected: [i64; 4] = [4, 6, 14, 6];

        for i in 0..input.len() {
            assert_eq!(greatest_common_divisor(input[i].0, input[i].1), expected[i]);
        }
    }

    #[test]
    fn test_least_common_multiple() {
        let input: [(i64, i64); 2] = [(4, 6), (21, 6)];
        let expected: [i64; 2] = [12, 42];

        for i in 0..input.len() {
            assert_eq!(least_common_multiple(input[i].0, input[i].1), expected[i]);
        }
    }

    #[test]
    fn test_factors() {
        let input: [i64; 4] = [3, 18, 36, 48];
        let expected: [Vec<i64>; 4] = [
            vec![1, 3],
            vec![1, 2, 3, 6, 9, 18],
            vec![1, 2, 3, 4, 6, 9, 12, 18, 36],
            vec![1, 2, 3, 4, 6, 8, 12, 16, 24, 48],
        ];

        for i in 0..input.len() {
            assert_eq!(factors(input[i]), expected[i]);
        }
    }
}
