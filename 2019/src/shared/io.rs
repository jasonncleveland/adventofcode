use std::fs::read_to_string;

pub fn read_file(file_name: String) -> String {
    read_to_string(file_name).expect("Something went wrong reading the file")
}
