#[inline]
#[must_use]
pub fn greatest_common_divisor(a: i64, b: i64) -> i64 {
    if b == 0 {
        return a;
    }

    match b > a {
        true => greatest_common_divisor(a, b % a),
        false => greatest_common_divisor(b, a % b),
    }
}

#[inline]
#[must_use]
pub fn least_common_multiple(first: i64, second: i64) -> i64 {
    first.abs() * (second.abs() / greatest_common_divisor(first, second))
}

#[inline]
#[must_use]
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

/// Gets the number of digits in the given number.
///
/// # Examples
/// ```
/// get_digits_count(12345) -> 5
/// get_digits_count(123456789) -> 9
/// ```
#[inline]
#[must_use]
pub fn get_digits_count(number: i64) -> u32 {
    // The number of digits in a number can be found by num log10 + 1
    // The +1 is needed because the log is a float and this is equivalent to num.ceil()
    // u128 has a max of 39 digits so we can use u32
    (number as f64).log10() as u32 + 1
}

/// Returns the digit at the given index.
/// Index is 1 based from the left-most digit.
///
/// # Errors
///
/// Will return `Err` if `index` is less than 0
/// or greater than the number of digits in the given number
///
/// # Examples
/// ```
/// get_digit(12345, 2) -> Ok(2)
/// get_digit(12345, 5) -> Ok(5)
/// get_digit(12345, 0) -> Err("Index out of range")
/// get_digit(12345, 6) -> Err("Index out of range")
/// ```
#[inline]
pub fn get_digit(number: i64, index: u32) -> Result<i64, String> {
    let digits = get_digits_count(number);
    if index < 1 || index > digits {
        return Err("Index out of range".to_string());
    }

    let ten: i64 = 10;
    // Remove digits to the left using the remainder of the next power of 10
    let remainder = number % ten.pow(digits - index + 1);
    // Remove digits to the right using division of the current power of 10
    let digit = remainder / ten.pow(digits - index);
    Ok(digit)
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

    #[test]
    fn test_get_digits_count() {
        let data: [(i64, u32); 3] = [(7, 1), (24680, 5), (123456789, 9)];

        for (input, expected) in data {
            assert_eq!(get_digits_count(input), expected);
        }
    }

    #[test]
    fn test_get_digit() {
        let data: [(i64, u32, Result<i64, String>); 5] = [
            (7, 1, Ok(7)),
            (24680, 3, Ok(6)),
            (12345678987654321, 12, Ok(6)),
            (123, 0, Err("Index out of range".to_string())),
            (123, 5, Err("Index out of range".to_string())),
        ];

        for (number, index, expected) in data {
            assert_eq!(get_digit(number, index), expected);
        }
    }
}
