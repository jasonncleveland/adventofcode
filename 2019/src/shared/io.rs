use std::fs::read_to_string;

pub fn read_file(file_name: String) -> String {
    read_to_string(file_name).expect("Something went wrong reading the file")
}

pub fn parse_int_list(file_contents: String) -> Vec<i64> {
    let mut result: Vec<i64> = Vec::new();
    for line in file_contents.split(',') {
        if let Ok(value) = line.parse::<i64>() {
            result.push(value);
        }
    }
    result
}
