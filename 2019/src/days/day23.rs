use std::collections::{HashMap, VecDeque};
use std::time::Instant;

use aoc_helpers::io::parse_int_list;
use log::{debug, trace};

use crate::shared::intcode::{IntCodeComputer, IntCodeStatus};

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

        if let Ok(IntCodeStatus::OutputWaiting) = controller.computer.run_interactive(3)
            && let Some(target_address) = controller.computer.output.pop_front()
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

        // Return the controller to the queue when processing is finished
        controllers.push_back(controller);
    }

    unreachable!();
}

fn solve_part_2(input: &[i64]) -> i64 {
    let mut controllers: HashMap<i64, IntCodeComputer> = HashMap::new();
    for address in 0..50 {
        let mut computer = IntCodeComputer::new(input);
        computer.input.push_back(address);
        controllers.insert(address, computer);
    }

    let mut nat_packet = NetworkPacket::new(0, 0);
    let mut last_nat_packet = NetworkPacket::new(0, 0);
    loop {
        let mut is_idle = true;

        for address in 0..50 {
            if let Some(controller) = controllers.get_mut(&address) {
                if controller.input.is_empty() {
                    controller.input.push_back(-1);
                } else {
                    is_idle = false;
                }

                if let Ok(IntCodeStatus::OutputWaiting) = controller.run_interactive(3)
                    && let Some(target_address) = controller.output.pop_front()
                    && let Some(x) = controller.output.pop_front()
                    && let Some(y) = controller.output.pop_front() {
                    if target_address == 255 {
                        trace!("sending packet {:?} to NAT", NetworkPacket::new(x, y));
                        nat_packet = NetworkPacket::new(x, y);
                    } else {
                        trace!("sending packet {:?} to address {}", NetworkPacket::new(x, y), target_address);
                        controllers
                            .entry(target_address)
                            .and_modify(|c| c.input.extend([x, y]));
                    }
                }

            }
        }

        // If all controllers are idle for a cycle then we need to send the NAT packet to controller 0
        if is_idle {
            if last_nat_packet == nat_packet {
                trace!("received packet {:?} twice in a row", nat_packet);
                return nat_packet.y;
            }

            if let Some(controller) = controllers.get_mut(&0) {
                trace!("sending NAT packet {:?} to address 0", nat_packet);
                controller.input.push_back(nat_packet.x);
                controller.input.push_back(nat_packet.y);
            }
            last_nat_packet = NetworkPacket::new(nat_packet.x, nat_packet.y);
        }
    }
}

struct NetworkInterfaceController {
    network_address: i64,
    computer: IntCodeComputer,
}

#[derive(Clone, Debug, PartialEq)]
struct NetworkPacket {
    x: i64,
    y: i64,
}

impl NetworkPacket {
    fn new(x: i64, y: i64) -> Self {
        NetworkPacket { x, y }
    }
}
