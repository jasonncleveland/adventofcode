use std::collections::HashMap;
use std::time::Instant;

use chrono::{NaiveDateTime, Timelike};
use log::debug;

pub fn solve(file_contents: String) -> (String, String) {
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

fn parse_input(file_contents: String) -> Vec<GuardRecord> {
    let mut records: Vec<GuardRecord> = Vec::new();

    for line in file_contents.lines() {
        let mut guard_id: i16 = -1;
        let mut record_type: GuardRecordType = GuardRecordType::Start;

        let datetime_str = &line[..18];
        let log_str = &line[19..];
        if let Ok(datetime) = NaiveDateTime::parse_from_str(datetime_str, "[%Y-%m-%d %H:%M]") {
            if log_str.starts_with("Guard") {
                let log_parts = log_str.split(" ").collect::<Vec<&str>>();
                if let Ok(id) = log_parts[1][1..].parse::<i16>() {
                    guard_id = id;
                }
                record_type = GuardRecordType::Start;
            } else if log_str.starts_with("falls asleep") {
                record_type = GuardRecordType::Sleep;
            } else if log_str.starts_with("wakes up") {
                record_type = GuardRecordType::Wake;
            }
            records.push(GuardRecord {
                datetime,
                guard_id,
                record_type,
            });
        }
    }

    records.sort_by(|a, b| a.datetime.cmp(&b.datetime));

    records
}

fn solve_part_1(records: &[GuardRecord]) -> i64 {
    let mut guard: &mut Guard = &mut Guard::new();
    let mut last_minute: u32 = 0;
    let mut guards: HashMap<i16, Guard> = HashMap::new();

    for record in records {
        match record.record_type {
            GuardRecordType::Start => {
                guard = guards
                    .entry(record.guard_id)
                    .or_insert(Guard {
                        id: record.guard_id,
                        minutes: HashMap::new(),
                        minutes_asleep: 0,
                    });
            },
            GuardRecordType::Sleep => {
                last_minute = record.datetime.minute();
            },
            GuardRecordType::Wake => {
                guard.minutes_asleep += record.datetime.minute() - last_minute;
                for i in last_minute..record.datetime.minute() {
                    guard.minutes.entry(i).and_modify(|m| *m += 1).or_insert(1);
                }
            },
        }
    }

    let mut most_minutes_asleep = u32::MIN;
    let mut total: i64 = -1;
    for (_, guard) in guards {
        if guard.minutes_asleep > most_minutes_asleep {
            most_minutes_asleep = guard.minutes_asleep;
            let mut most_times_asleep = i64::MIN;
            for (minute, count) in guard.minutes {
                if count > most_times_asleep {
                    most_times_asleep = count;
                    total = guard.id as i64 * minute as i64;
                }
            }
        }
    }
    total
}

fn solve_part_2(records: &[GuardRecord]) -> i64 {
    let mut guard: &mut Guard = &mut Guard::new();
    let mut last_minute: u32 = 0;
    let mut guards: HashMap<i16, Guard> = HashMap::new();

    for record in records {
        match record.record_type {
            GuardRecordType::Start => {
                guard = guards.entry(record.guard_id).or_insert(Guard {
                    id: record.guard_id,
                    minutes: HashMap::new(),
                    minutes_asleep: 0,
                });
            },
            GuardRecordType::Sleep => {
                last_minute = record.datetime.minute();
            },
            GuardRecordType::Wake => {
                guard.minutes_asleep += record.datetime.minute() - last_minute;
                for i in last_minute..record.datetime.minute() {
                    guard.minutes.entry(i).and_modify(|m| *m += 1).or_insert(1);
                }
            },
        }
    }

    let mut most_times_asleep = i64::MIN;
    let mut total: i64 = -1;
    for (_, guard) in guards {
        for (minute, count) in guard.minutes {
            if count > most_times_asleep {
                most_times_asleep = count;
                total = guard.id as i64 * minute as i64;
            }
        }
    }
    total
}

#[derive(Debug)]
enum GuardRecordType {
    Start,
    Sleep,
    Wake,
}

#[derive(Debug)]
struct GuardRecord {
    datetime: NaiveDateTime,
    guard_id: i16,
    record_type: GuardRecordType,
}

#[derive(Debug)]
struct Guard {
    id: i16,
    minutes: HashMap<u32, i64>,
    minutes_asleep: u32,
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
    fn test_part_1() {
        let input: [&str; 1] = [
            "[1518-11-01 00:00] Guard #10 begins shift
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
[1518-11-05 00:55] wakes up",
        ];
        let expected: [i64; 1] = [
            240,
        ];

        for i in 0..input.len() {
            let input = parse_input(input[i].to_string());
            assert_eq!(solve_part_1(&input), expected[i]);
        }
    }

    #[test]
    fn test_part_2() {
        let input: [&str; 1] = [
            "[1518-11-01 00:00] Guard #10 begins shift
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
[1518-11-05 00:55] wakes up",
        ];
        let expected: [i64; 1] = [
            4455,
        ];

        for i in 0..input.len() {
            let input = parse_input(input[i].to_string());
            assert_eq!(solve_part_2(&input), expected[i]);
        }
    }
}
