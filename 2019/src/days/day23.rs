use std::collections::{HashMap, VecDeque};
use std::time::Instant;

use log::{debug, trace};

use crate::shared::intcode::{IntCodeComputer, IntCodeStatus};
use crate::shared::io::parse_int_list;

pub fn solve(file_contents: String) -> (String, String) {
    let parse_timer = Instant::now();
    let input = parse_int_list(file_contents);
    debug!("File parse: ({:?})", parse_timer.elapsed());

    let part1_timer = Instant::now();
    let part1 = solve_part_1(&input);
    debug!("Part 1: {} ({:?})", part1, part1_timer.elapsed());

    let part2_timer = Instant::now();
    let part2 = solve_part_2(&input);
    debug!("Part 2: {} ({:?})", part2, part2_timer.elapsed());

    (part1.to_string(), part2.to_string())
}

fn solve_part_1(input: &[i64]) -> i64 {
    let mut controllers: VecDeque<NetworkInterfaceController> = VecDeque::with_capacity(50);
    for address in 0..50 {
        let mut computer = IntCodeComputer::new(input);
        computer.input.push_back(address);
        controllers.push_back(NetworkInterfaceController {
            network_address: address,
            computer,
        });
    }

    let mut packet_queue: HashMap<i64, Vec<NetworkPacket>> = HashMap::new();
    while let Some(mut controller) = controllers.pop_front() {
        if let Some(packets) = packet_queue.get_mut(&controller.network_address) {
            for packet in packets {
                trace!("sending packet {:?} to nic {}", packet, controller.network_address);
                controller.computer.input.push_back(packet.x);
                controller.computer.input.push_back(packet.y);
            }
        } else {
            controller.computer.input.push_back(-1);
        }

        while let Ok(status) = controller.computer.run_interactive(3) {
            match status {
                IntCodeStatus::OutputWaiting => {
                    if let Some(target_address) = controller.computer.output.pop_front()
                        && let Some(x) = controller.computer.output.pop_front()
                        && let Some(y) = controller.computer.output.pop_front() {
                        trace!("received packet {:?} addressed to {}", NetworkPacket::new(x, y), target_address);
                        if target_address == 255 {
                            // Stop if a value is output to address 255
                            return y;
                        }
                        packet_queue
                            .entry(target_address)
                            .and_modify(|q| q.push(NetworkPacket::new(x, y)))
                            .or_insert(vec![NetworkPacket::new(x, y)]);
                    }
                    break;
                },
                IntCodeStatus::InputRequired => break,
                IntCodeStatus::ProgramHalted => break
            }
        }

        // Return the controller to the queue when processing is finished
        controllers.push_back(controller);
    }

    unreachable!();
}

fn solve_part_2(input: &[i64]) -> i64 {
    unimplemented!();
}

struct NetworkInterfaceController {
    network_address: i64,
    computer: IntCodeComputer,
}

#[derive(Debug)]
struct NetworkPacket {
    x: i64,
    y: i64,
}

impl NetworkPacket {
    fn new(x: i64, y: i64) -> Self {
        NetworkPacket { x, y }
    }
}
