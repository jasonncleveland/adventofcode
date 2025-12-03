use std::collections::HashMap;
use std::time::Instant;

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

fn parse_input(file_contents: &str) -> Vec<Reaction> {
    let mut reactions: Vec<Reaction> = Vec::new();
    for line in file_contents.lines() {
        if let Some((left, right)) = line.split_once(" => ")
            && let Some((output_quantity, output_name)) = right.split_once(' ')
            && let Ok(value) = output_quantity.parse::<i64>()
        {
            let mut reaction = Reaction::new(Chemical::new(output_name.to_string(), value));
            for input in left.split(", ") {
                if let Some((input_quantity, input_name)) = input.split_once(' ')
                    && let Ok(value) = input_quantity.parse::<i64>()
                {
                    reaction.add_input(Chemical::new(input_name.to_string(), value));
                }
            }
            reactions.push(reaction);
        }
    }
    reactions
}

fn solve_part_1(reactions: &Vec<Reaction>) -> i64 {
    calculate_required_ore(reactions, 1)
}

fn solve_part_2(reactions: &Vec<Reaction>) -> i64 {
    let total_ore = 1_000_000_000_000;

    // Use topological sort to find the best fuel amount
    let mut fuel = 1_000_000_000_000;
    let mut lowest_invalid_fuel = 1_000_000_000_000;
    let mut highest_valid_fuel = 1;
    loop {
        let ore_required = calculate_required_ore(reactions, fuel);
        if ore_required < total_ore {
            highest_valid_fuel = fuel;
        } else {
            lowest_invalid_fuel = fuel;
        }

        let difference = lowest_invalid_fuel - highest_valid_fuel;
        if difference == 0 || difference == 1 {
            // If the difference is 0 or 1 then we have found the correct value
            return highest_valid_fuel;
        }
        fuel = highest_valid_fuel + difference / 2;
    }
}

fn calculate_required_ore(reactions: &Vec<Reaction>, fuel_quantity: i64) -> i64 {
    let mut topo: HashMap<String, Vec<String>> = HashMap::new();
    for reaction in reactions {
        for input in &reaction.inputs {
            if !topo.contains_key(&input.name) {
                topo.insert(input.name.to_owned(), Vec::new());
            }
            if !topo.contains_key(&reaction.output.name) {
                topo.insert(reaction.output.name.to_owned(), Vec::new());
            }
            if let Some(dst) = topo.get_mut(&reaction.output.name) {
                dst.push(input.name.to_owned());
            }
        }
    }
    let mut sorted = topological_sort(&mut topo);
    sorted.reverse();

    let mut chemicals_required: HashMap<String, i64> = HashMap::new();
    chemicals_required.insert("FUEL".to_owned(), fuel_quantity);

    for chemical_name in sorted {
        if let Some(reaction) = reactions.iter().find(|c| c.output.name == chemical_name)
            && let Some((_, required_quantity)) = chemicals_required.remove_entry(&chemical_name)
        {
            let mut reaction_count = 1;
            if required_quantity > reaction.output.quantity {
                let quotient = required_quantity / reaction.output.quantity;
                let remainder = required_quantity % reaction.output.quantity;
                reaction_count *= quotient;
                if remainder > 0 {
                    reaction_count += 1;
                }
            }
            for input in &reaction.inputs {
                match chemicals_required.get(&input.name) {
                    Some(amount) => {
                        chemicals_required
                            .insert(input.name.clone(), amount + input.quantity * reaction_count);
                    }
                    None => {
                        chemicals_required
                            .insert(input.name.clone(), input.quantity * reaction_count);
                    }
                };
            }
        }
    }

    if let Some(ore) = chemicals_required.get("ORE") {
        return *ore;
    }
    unreachable!()
}

fn topological_sort(items: &mut HashMap<String, Vec<String>>) -> Vec<String> {
    // Find the item with an in-degree of 0
    let mut sorted: Vec<String> = Vec::new();
    while let Some(found) = find_next_item(items) {
        sorted.push(found.to_owned());
        remove_item(items, found);
    }
    sorted
}

fn find_next_item(items: &HashMap<String, Vec<String>>) -> Option<String> {
    Some(items.iter().find(|(_, v)| v.is_empty())?.0.clone())
}

fn remove_item(items: &mut HashMap<String, Vec<String>>, name: String) {
    items.iter_mut().for_each(|(_, v)| v.retain(|x| x != &name));
    items.remove(&name);
}

#[derive(Debug, Eq, Hash, PartialEq)]
struct Chemical {
    name: String,
    quantity: i64,
}

impl Chemical {
    fn new(name: String, quantity: i64) -> Chemical {
        Chemical { name, quantity }
    }
}

#[derive(Debug)]
struct Reaction {
    inputs: Vec<Chemical>,
    output: Chemical,
}

impl Reaction {
    fn new(output: Chemical) -> Reaction {
        Reaction {
            inputs: vec![],
            output,
        }
    }

    fn add_input(&mut self, input: Chemical) {
        self.inputs.push(input);
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let input: [&str; 5] = [
            "10 ORE => 10 A
1 ORE => 1 B
7 A, 1 B => 1 C
7 A, 1 C => 1 D
7 A, 1 D => 1 E
7 A, 1 E => 1 FUEL",
            "9 ORE => 2 A
8 ORE => 3 B
7 ORE => 5 C
3 A, 4 B => 1 AB
5 B, 7 C => 1 BC
4 C, 1 A => 1 CA
2 AB, 3 BC, 4 CA => 1 FUEL",
            "157 ORE => 5 NZVS
165 ORE => 6 DCFZ
44 XJWVT, 5 KHKGT, 1 QDVJ, 29 NZVS, 9 GPVTF, 48 HKGWZ => 1 FUEL
12 HKGWZ, 1 GPVTF, 8 PSHF => 9 QDVJ
179 ORE => 7 PSHF
177 ORE => 5 HKGWZ
7 DCFZ, 7 PSHF => 2 XJWVT
165 ORE => 2 GPVTF
3 DCFZ, 7 NZVS, 5 HKGWZ, 10 PSHF => 8 KHKGT",
            "2 VPVL, 7 FWMGM, 2 CXFTF, 11 MNCFX => 1 STKFG
17 NVRVD, 3 JNWZP => 8 VPVL
53 STKFG, 6 MNCFX, 46 VJHF, 81 HVMC, 68 CXFTF, 25 GNMV => 1 FUEL
22 VJHF, 37 MNCFX => 5 FWMGM
139 ORE => 4 NVRVD
144 ORE => 7 JNWZP
5 MNCFX, 7 RFSQX, 2 FWMGM, 2 VPVL, 19 CXFTF => 3 HVMC
5 VJHF, 7 MNCFX, 9 VPVL, 37 CXFTF => 6 GNMV
145 ORE => 6 MNCFX
1 NVRVD => 8 CXFTF
1 VJHF, 6 MNCFX => 4 RFSQX
176 ORE => 6 VJHF",
            "171 ORE => 8 CNZTR
7 ZLQW, 3 BMBT, 9 XCVML, 26 XMNCP, 1 WPTQ, 2 MZWV, 1 RJRHP => 4 PLWSL
114 ORE => 4 BHXH
14 VRPVC => 6 BMBT
6 BHXH, 18 KTJDG, 12 WPTQ, 7 PLWSL, 31 FHTLT, 37 ZDVW => 1 FUEL
6 WPTQ, 2 BMBT, 8 ZLQW, 18 KTJDG, 1 XMNCP, 6 MZWV, 1 RJRHP => 6 FHTLT
15 XDBXC, 2 LTCX, 1 VRPVC => 6 ZLQW
13 WPTQ, 10 LTCX, 3 RJRHP, 14 XMNCP, 2 MZWV, 1 ZLQW => 1 ZDVW
5 BMBT => 4 WPTQ
189 ORE => 9 KTJDG
1 MZWV, 17 XDBXC, 3 XCVML => 2 XMNCP
12 VRPVC, 27 CNZTR => 2 XDBXC
15 KTJDG, 12 BHXH => 5 XCVML
3 BHXH, 2 VRPVC => 7 MZWV
121 ORE => 7 VRPVC
7 XCVML => 6 RJRHP
5 BHXH, 4 VRPVC => 5 LTCX",
        ];
        let expected: [i64; 5] = [31, 165, 13312, 180697, 2210736];

        for i in 0..input.len() {
            let parsed = parse_input(input[i]);
            assert_eq!(solve_part_1(&parsed), expected[i]);
        }
    }

    #[test]
    fn test_part_2() {
        let input: [&str; 3] = [
            "157 ORE => 5 NZVS
165 ORE => 6 DCFZ
44 XJWVT, 5 KHKGT, 1 QDVJ, 29 NZVS, 9 GPVTF, 48 HKGWZ => 1 FUEL
12 HKGWZ, 1 GPVTF, 8 PSHF => 9 QDVJ
179 ORE => 7 PSHF
177 ORE => 5 HKGWZ
7 DCFZ, 7 PSHF => 2 XJWVT
165 ORE => 2 GPVTF
3 DCFZ, 7 NZVS, 5 HKGWZ, 10 PSHF => 8 KHKGT",
            "2 VPVL, 7 FWMGM, 2 CXFTF, 11 MNCFX => 1 STKFG
17 NVRVD, 3 JNWZP => 8 VPVL
53 STKFG, 6 MNCFX, 46 VJHF, 81 HVMC, 68 CXFTF, 25 GNMV => 1 FUEL
22 VJHF, 37 MNCFX => 5 FWMGM
139 ORE => 4 NVRVD
144 ORE => 7 JNWZP
5 MNCFX, 7 RFSQX, 2 FWMGM, 2 VPVL, 19 CXFTF => 3 HVMC
5 VJHF, 7 MNCFX, 9 VPVL, 37 CXFTF => 6 GNMV
145 ORE => 6 MNCFX
1 NVRVD => 8 CXFTF
1 VJHF, 6 MNCFX => 4 RFSQX
176 ORE => 6 VJHF",
            "171 ORE => 8 CNZTR
7 ZLQW, 3 BMBT, 9 XCVML, 26 XMNCP, 1 WPTQ, 2 MZWV, 1 RJRHP => 4 PLWSL
114 ORE => 4 BHXH
14 VRPVC => 6 BMBT
6 BHXH, 18 KTJDG, 12 WPTQ, 7 PLWSL, 31 FHTLT, 37 ZDVW => 1 FUEL
6 WPTQ, 2 BMBT, 8 ZLQW, 18 KTJDG, 1 XMNCP, 6 MZWV, 1 RJRHP => 6 FHTLT
15 XDBXC, 2 LTCX, 1 VRPVC => 6 ZLQW
13 WPTQ, 10 LTCX, 3 RJRHP, 14 XMNCP, 2 MZWV, 1 ZLQW => 1 ZDVW
5 BMBT => 4 WPTQ
189 ORE => 9 KTJDG
1 MZWV, 17 XDBXC, 3 XCVML => 2 XMNCP
12 VRPVC, 27 CNZTR => 2 XDBXC
15 KTJDG, 12 BHXH => 5 XCVML
3 BHXH, 2 VRPVC => 7 MZWV
121 ORE => 7 VRPVC
7 XCVML => 6 RJRHP
5 BHXH, 4 VRPVC => 5 LTCX",
        ];
        let expected: [i64; 3] = [82892753, 5586022, 460664];

        for i in 0..input.len() {
            let parsed = parse_input(input[i]);
            assert_eq!(solve_part_2(&parsed), expected[i]);
        }
    }
}
