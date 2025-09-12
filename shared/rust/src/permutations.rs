pub fn generate_permutations(input: &Vec<i64>) -> Vec<Vec<i64>> {
    if input.len() == 1 {
        return vec![vec![input[0]]];
    }

    let mut permutations: Vec<Vec<i64>> = Vec::new();
    for item in input {
        let remaining = input.iter().filter(|&i| i != item).copied().collect::<Vec<i64>>();
        for sub_result in generate_permutations(&remaining) {
            let mut permutation: Vec<i64> = vec![*item];
            permutation.extend(sub_result);
            permutations.push(permutation);
        }
    }
    permutations
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_permutations() {
        let input = vec![0, 1, 2, 3];
        let expected = vec![
            vec![0, 1, 2, 3],
            vec![0, 1, 3, 2],
            vec![0, 2, 1, 3],
            vec![0, 2, 3, 1],
            vec![0, 3, 1, 2],
            vec![0, 3, 2, 1],
            //
            vec![1, 0, 2, 3],
            vec![1, 0, 3, 2],
            vec![1, 2, 0, 3],
            vec![1, 2, 3, 0],
            vec![1, 3, 0, 2],
            vec![1, 3, 2, 0],
            //
            vec![2, 0, 1, 3],
            vec![2, 0, 3, 1],
            vec![2, 1, 0, 3],
            vec![2, 1, 3, 0],
            vec![2, 3, 0, 1],
            vec![2, 3, 1, 0],
            //
            vec![3, 0, 1, 2],
            vec![3, 0, 2, 1],
            vec![3, 1, 0, 2],
            vec![3, 1, 2, 0],
            vec![3, 2, 0, 1],
            vec![3, 2, 1, 0],
        ];

        assert_eq!(generate_permutations(&input), expected);
    }
}