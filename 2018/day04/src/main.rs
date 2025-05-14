use std::collections::HashMap;
use std::env;
use std::fs;
use std::time;
use regex::Regex;

fn main() {
    let args: Vec<String> = env::args().collect();
    if args.len() < 2 {
        panic!("Must pass filename as argument");
    }

    let input_timer = time::Instant::now();
    let file_name = &args[1];
    let file_contents = read_file(file_name);
    println!("File read: ({:?})", input_timer.elapsed());

    let part1_timer = time::Instant::now();
    let part1 = part1(&file_contents);
    println!("Part 1: {} ({:?})", part1, part1_timer.elapsed());

    let part2_timer = time::Instant::now();
    let part2 = part2(&file_contents);
    println!("Part 2: {} ({:?})", part2, part2_timer.elapsed());
}

fn read_file(file_name: &str) -> String {
    fs::read_to_string(file_name)
        .expect("Something went wrong reading the file")
}

fn part1(file_contents: &str) -> i64 {
    let re = Regex::new(r"^\[(?<year>\d{4})\-(?<month>\d{2})\-(?<day>\d{2}) (?<hour>\d{2}):(?<minute>\d{2})\] (?<log>(?:Guard #(?<guard>\d+) begins shift)|(?:falls asleep)|(?:wakes up))$").unwrap();
    let mut guard: &mut Guard = &mut Guard::new();
    let mut last_minute: i64 = -1;
    let mut guards: HashMap<i64, Guard> = HashMap::new();
    let mut lines : Vec<&str> = file_contents.lines().collect();
    lines.sort();
    for line in lines {
        let capture = re.captures(line).unwrap();
        let minute: i64 = capture.name("minute").unwrap().as_str().parse::<i64>().unwrap();
        let log: &str = capture.name("log").unwrap().as_str();
        if log.starts_with("Guard") {
            let guard_id = capture.name("guard").unwrap().as_str().parse::<i64>().unwrap();
            guards.entry(guard_id).or_insert(Guard {
                id: guard_id,
                minutes: HashMap::new(),
                minutes_asleep: 0,
            });
            guard = guards.get_mut(&guard_id).unwrap();
        } else if log.starts_with("falls asleep") {
            last_minute = minute;
        } else if log.starts_with("wakes up") {
            guard.minutes_asleep += minute - last_minute;
            for i in last_minute..minute {
                guard.minutes.entry(i).and_modify(|m| *m += 1).or_insert(1);
            }
        }
    }

    let mut most_minutes_asleep = i64::MIN;
    let mut total: i64 = -1;
    for (_, guard) in guards {
        if guard.minutes_asleep > most_minutes_asleep {
            most_minutes_asleep = guard.minutes_asleep;
            let mut most_times_asleep = i64::MIN;
            for (minute, count) in guard.minutes {
                if count > most_times_asleep {
                    most_times_asleep = count;
                    total = guard.id * minute;
                }
            }
        }
    }
    total
}

fn part2(file_contents: &str) -> i64 {
    -1
}

#[derive(Debug)]
struct Guard {
    id: i64,
    minutes: HashMap<i64, i64>,
    minutes_asleep: i64,
}

impl Guard {
    pub fn new() -> Self {
        Guard {
            id: -1,
            minutes: HashMap::new(),
            minutes_asleep: 0,
        }
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part1() {
        let input: &str=
            "\
[1518-11-01 00:00] Guard #10 begins shift
[1518-11-01 00:05] falls asleep
[1518-11-01 00:25] wakes up
[1518-11-01 00:30] falls asleep
[1518-11-01 00:55] wakes up
[1518-11-01 23:58] Guard #99 begins shift
[1518-11-02 00:40] falls asleep
[1518-11-02 00:50] wakes up
[1518-11-03 00:05] Guard #10 begins shift
[1518-11-03 00:24] falls asleep
[1518-11-03 00:29] wakes up
[1518-11-04 00:02] Guard #99 begins shift
[1518-11-04 00:36] falls asleep
[1518-11-04 00:46] wakes up
[1518-11-05 00:03] Guard #99 begins shift
[1518-11-05 00:45] falls asleep
[1518-11-05 00:55] wakes up";
        let expected: i64 = 240;

        assert_eq!(part1(input), expected);
    }

    #[test]
    fn test_part2() {
        let input: &str = "";
        let expected: i64 = 0;

        assert_eq!(part2(input), expected);
    }
}
