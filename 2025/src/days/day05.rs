use std::time::Instant;

use aoc_helpers::io::{parse_int_list, parse_range_list};
use aoc_helpers::range::Range;
use log::debug;

pub fn solve(file_contents: &str) -> (String, String) {
    let parse_timer = Instant::now();
    let input = parse_input(file_contents);
    debug!("File parse: ({:?})", parse_timer.elapsed());

    let part1_timer = Instant::now();
    let part1 = solve_part_1(&input);
    debug!("Part 1: {} ({:?})", part1, part1_timer.elapsed());

    let part2_timer = Instant::now();
    let part2 = solve_part_2(&input);
    debug!("Part 2: {} ({:?})", part2, part2_timer.elapsed());

    (part1.to_string(), part2.to_string())
}

fn parse_input(file_contents: &str) -> (Vec<Range>, Vec<i64>) {
    if let Some((first, second)) = file_contents.split_once("\n\n") {
        let ranges = parse_range_list(first, '\n');
        let numbers = parse_int_list(second, '\n');
        return (ranges, numbers);
    }
    unreachable!();
}

fn solve_part_1(input: &(Vec<Range>, Vec<i64>)) -> i64 {
    let mut total = 0;

    for &ingredient in &input.1 {
        for range in &input.0 {
            if ingredient >= range.start && ingredient <= range.end {
                total += 1;
                break;
            }
        }
    }

    total
}

fn solve_part_2(input: &(Vec<Range>, Vec<i64>)) -> i64 {
    let ranges = consolidate_ranges(&input.0);

    let mut total = 0;
    for range in ranges {
        total += range.end - range.start + 1;
    }
    total
}

/// Consolidate a list of ranges into the fewest number of ranges
///
/// ref: <https://rosettacode.org/wiki/Range_consolidation>
fn consolidate_ranges(ranges: &[Range]) -> Vec<Range> {
    // Sort the ranges by start
    let mut sorted = ranges.to_owned();
    sorted.sort_by(|a, b| a.start.cmp(&b.start));
    let mut ranges = sorted.into_iter();

    let mut consolidated = Vec::new();
    if let Some(mut combined_range) = ranges.next() {
        // Compare the ranges and attempt to combine until we find ranges that do not overlap
        for to_compare in ranges {
            // Attempt to combine the ranges and keep to compare with the next ranges
            // If the ranges do not overlap, push the previous range to the final list
            // and keep the new range to compare with next ranges
            combined_range = combined_range.consolidate(&to_compare).map_or_else(
                || {
                    consolidated.push(combined_range);
                    to_compare
                },
                |result| result,
            );
        }
        consolidated.push(combined_range);
    }
    consolidated
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let data: [(&str, i64); 1] = [(
            "3-5
10-14
16-20
12-18

1
5
8
11
17
32",
            3,
        )];

        for (input, expected) in data {
            let input = parse_input(input);
            assert_eq!(solve_part_1(&input), expected);
        }
    }

    #[test]
    fn test_part_2() {
        let data: [(&str, i64); 1] = [(
            "3-5
10-14
16-20
12-18

1
5
8
11
17
32",
            14,
        )];

        for (input, expected) in data {
            let input = parse_input(input);
            assert_eq!(solve_part_2(&input), expected);
        }
    }
}
