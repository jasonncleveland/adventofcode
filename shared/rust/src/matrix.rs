/// Flips an array along the horizontal axis.
///
/// .#.    .#.
/// ..# -> #..
/// ###    ###
#[inline]
#[must_use]
pub fn flip_horizontal<T: Copy>(array: &[Vec<T>]) -> Vec<Vec<T>> {
    let mut output = array.to_owned();
    for row in 0..array.len() {
        for column in 0..array.len() {
            output[row][column] = array[row][array.len() - 1 - column];
        }
    }
    output
}

/// Flips an array along the vertical axis.
///
/// .#.    ###
/// ..# -> ..#
/// ###    .#.
#[inline]
#[must_use]
pub fn flip_vertical<T: Copy>(array: &[Vec<T>]) -> Vec<Vec<T>> {
    let mut output = array.to_owned();
    for column in 0..array.len() {
        let mut i = 0;
        let mut j = array.len() - 1;
        while i < j {
            output[i][column] = array[j][column];
            output[j][column] = array[i][column];
            i += 1;
            j -= 1;
        }
    }
    output
}

/// Flips an array along the horizontal axis.
///
/// .#.    .#.
/// ..# -> #..
/// ###    ###
#[inline]
#[must_use]
pub fn flip<T: Copy>(array: &[Vec<T>]) -> Vec<Vec<T>> {
    let mut output = array.to_owned();
    for row in 0..array.len() {
        for column in 0..array.len() {
            output[row][column] = array[row][array.len() - 1 - column];
        }
    }
    output
}

/// Flips an array along the horizontal axis without allocating additional memory.
///
/// .#.    .#.
/// ..# -> #..
/// ###    ###
#[inline]
pub fn flip_in_place<T: Copy>(array: &mut [Vec<T>]) {
    for row in array.iter_mut() {
        let mut i = 0;
        let mut j = row.len() - 1;
        while i < j {
            row.swap(i, j);
            i += 1;
            j -= 1;
        }
    }
}

/// Rotates an array 90 degrees.
/// Rotating consists of transposing then flipping the array.
/// Array must be an NxN square matrix.
///
/// .#.    #..
/// ..# -> #.#
/// ###    ##.
#[inline]
#[must_use]
pub fn rotate<T: Copy>(array: &[Vec<T>]) -> Vec<Vec<T>> {
    // Transpose
    let mut transposed = array.to_owned();
    for (r, row) in array.iter().enumerate() {
        for (c, &value) in row.iter().enumerate() {
            transposed[c][r] = value;
        }
    }
    // Swap
    flip(&transposed)
}

/// Rotates an array 90 degrees without allocating additional memory.
/// Rotating consists of transposing then flipping the array.
/// Array must be an NxN square matrix.
///
/// .#.    #..
/// ..# -> #.#
/// ###    ##.
#[inline]
pub fn rotate_in_place<T: Copy>(array: &mut [Vec<T>]) {
    for row in 0..array.len() - 1 {
        for column in row + 1..array.len() {
            let copy = array[row][column];
            array[row][column] = array[column][row];
            array[column][row] = copy;
        }
    }
    // Swap
    flip_in_place(array);
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_flip_horizontal() {
        let input = vec![vec![1, 2, 3], vec![4, 5, 6], vec![7, 8, 9]];
        let expected = vec![vec![3, 2, 1], vec![6, 5, 4], vec![9, 8, 7]];

        assert_eq!(flip_horizontal(&input), expected);
    }

    #[test]
    fn test_flip_vertical() {
        let input = vec![
            vec!['a', 'b', 'c'],
            vec!['d', 'e', 'f'],
            vec!['g', 'h', 'i'],
        ];
        let expected = vec![
            vec!['g', 'h', 'i'],
            vec!['d', 'e', 'f'],
            vec!['a', 'b', 'c'],
        ];

        assert_eq!(flip_vertical(&input), expected);
    }

    #[test]
    fn test_flip() {
        let input = vec![vec![1, 2, 3], vec![4, 5, 6], vec![7, 8, 9]];
        let expected = vec![vec![3, 2, 1], vec![6, 5, 4], vec![9, 8, 7]];

        assert_eq!(flip(&input), expected);
    }

    #[test]
    fn test_flip_in_place() {
        let mut input = vec![vec![1, 2, 3], vec![4, 5, 6], vec![7, 8, 9]];
        let expected = vec![vec![3, 2, 1], vec![6, 5, 4], vec![9, 8, 7]];

        flip_in_place(&mut input);
        assert_eq!(input, expected);
    }

    #[test]
    fn test_rotate() {
        let input = vec![
            vec!['a', 'b', 'c'],
            vec!['d', 'e', 'f'],
            vec!['g', 'h', 'i'],
        ];
        let expected = vec![
            vec!['g', 'd', 'a'],
            vec!['h', 'e', 'b'],
            vec!['i', 'f', 'c'],
        ];

        assert_eq!(rotate(&input), expected);
    }

    #[test]
    fn test_rotate_in_place() {
        let mut input = vec![
            vec!['a', 'b', 'c'],
            vec!['d', 'e', 'f'],
            vec!['g', 'h', 'i'],
        ];
        let expected = vec![
            vec!['g', 'd', 'a'],
            vec!['h', 'e', 'b'],
            vec!['i', 'f', 'c'],
        ];

        rotate_in_place(&mut input);
        assert_eq!(input, expected);
    }
}
