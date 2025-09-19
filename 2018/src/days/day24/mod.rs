mod attack;
mod faction;
mod group;

use std::cmp::{Ordering, min};
use std::collections::{HashMap, HashSet};
use std::time::Instant;

use log::{debug, trace};
use regex::Regex;

use attack::Attack;
use faction::Faction;
use group::Group;

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

fn parse_input(file_contents: String) -> Vec<Group> {
    let mut groups: Vec<Group> = Vec::new();
    if let Ok(matcher) = Regex::new(
        r"(?<units>\d+) units each with (?<hp>\d+) hit points (\((?<modifiers>.*)\)\s)?with an attack that does (?<damage>\d+) (?<type>\w+) damage at initiative (?<initiative>\d+)",
    ) && let Some((immune_system, infection)) = file_contents.split_once("\n\n")
    {
        for (i, line) in immune_system.lines().skip(1).enumerate() {
            groups.push(parse_group(&matcher, line, Faction::ImmuneSystem, i + 1));
        }

        for (i, line) in infection.lines().skip(1).enumerate() {
            groups.push(parse_group(&matcher, line, Faction::Infection, i + 1));
        }
    }
    groups
}

fn parse_group(matcher: &Regex, line: &str, faction: Faction, id: usize) -> Group {
    if let Some(capture) = matcher.captures(line)
        && let Some(u) = capture.name("units")
        && let Ok(units) = u.as_str().parse::<i64>()
        && let Some(hp) = capture.name("hp")
        && let Ok(hit_points) = hp.as_str().parse::<i64>()
        && let Some(ad) = capture.name("damage")
        && let Ok(attack_damage) = ad.as_str().parse::<i64>()
        && let Some(at) = capture.name("type")
        && let Ok(attack_type) = Attack::new(at.as_str())
        && let Some(i) = capture.name("initiative")
        && let Ok(initiative) = i.as_str().parse::<i64>()
    {
        let mut group = Group::new(
            faction,
            id,
            units,
            hit_points,
            attack_damage,
            attack_type,
            initiative,
        );

        if let Some(m) = capture.name("modifiers") {
            for modifier in m.as_str().split("; ") {
                if let Some((t, ats)) = modifier.split_once(" to ") {
                    let to_add = match t {
                        "weak" => &mut group.weaknesses,
                        "immune" => &mut group.immunities,
                        _ => unreachable!(),
                    };
                    for t in ats.split(", ") {
                        if let Ok(converted) = Attack::new(t) {
                            to_add.push(converted);
                        }
                    }
                }
            }
        }

        return group;
    }
    panic!("unable to parse group");
}

fn solve_part_1(input: &[Group]) -> i64 {
    let mut groups = input.to_owned();

    loop {
        match simulate_fight(&mut groups) {
            None => continue,
            Some(faction) => {
                debug!("Faction {} was victorious", faction);
                break;
            }
        }
    }

    groups.iter().map(|g| g.units).sum()
}

fn solve_part_2(input: &[Group]) -> i64 {
    let mut boost = 1;
    let mut highest_boost_with_death = 1;
    let mut lowest_boost_without_death = 1_000_000;
    let mut results: HashMap<i64, i64> = HashMap::new();
    loop {
        let mut groups = input.to_owned();
        for unit in groups
            .iter_mut()
            .filter(|g| g.faction == Faction::ImmuneSystem)
        {
            unit.attack_damage += boost;
        }

        trace!("Simulating fight with a boost of {boost}");

        loop {
            match simulate_fight(&mut groups) {
                None => continue,
                Some(faction) => {
                    trace!("Faction {} was victorious", faction);

                    let result = groups.iter().map(|g| g.units).sum();
                    results.insert(boost, result);

                    // Use binary search to minimize the number of simulations performed
                    if faction == Faction::ImmuneSystem {
                        lowest_boost_without_death = boost;
                    } else {
                        highest_boost_with_death = boost;
                    }
                    let difference = lowest_boost_without_death - highest_boost_with_death;
                    if difference == 0 || difference == 1 {
                        // If the difference is 0 or 1 then we have found the correct value
                        if let Some(result) = results.get(&lowest_boost_without_death) {
                            return *result;
                        }
                    }
                    boost = highest_boost_with_death + difference / 2;

                    break;
                }
            }
        }
    }
}

fn simulate_fight(groups: &mut Vec<Group>) -> Option<Faction> {
    let immune_system = groups
        .iter()
        .filter(|g| g.faction == Faction::ImmuneSystem)
        .collect::<Vec<_>>();
    let infection = groups
        .iter()
        .filter(|g| g.faction == Faction::Infection)
        .collect::<Vec<_>>();

    trace!("Immune System:");
    immune_system
        .iter()
        .for_each(|g| trace!("Group {} contains {} units", g.id, g.units));
    trace!("Infection:");
    infection
        .iter()
        .for_each(|g| trace!("Group {} contains {} units", g.id, g.units));
    trace!("");

    if immune_system.is_empty() {
        return Some(Faction::Infection);
    }
    if infection.is_empty() {
        return Some(Faction::ImmuneSystem);
    }

    let starting_units = groups.iter().map(|g| g.units).sum::<i64>();
    trace!("Found {starting_units} units at the start of the fight");

    // Sort the groups by effective power and then initiative in descending order
    groups.sort_by(|a, b| match b.effective_power().cmp(&a.effective_power()) {
        Ordering::Equal => b.initiative.cmp(&a.initiative),
        other => other,
    });

    // Perform target selection phase
    let mut targets = select_targets(groups);
    trace!("");

    // Perform attacking phase
    attack_targets(groups, &mut targets);
    trace!("");

    let ending_units = groups.iter().map(|g| g.units).sum::<i64>();
    trace!(
        "Found {ending_units} units at the end of the fight ({starting_units} vs {ending_units})"
    );

    if starting_units == ending_units {
        // If no units we killed then we are soft-locked so the infection has won
        trace!("No units were killed during this turn so the fight is soft-locked and must halt");
        return Some(Faction::Infection);
    }

    None
}

fn select_targets(groups: &Vec<Group>) -> Vec<(Group, Group)> {
    let mut selected_targets: HashSet<(Faction, usize)> = HashSet::new();
    let mut targets: Vec<(Group, Group)> = Vec::new();
    for attacker in groups {
        let defenders = groups
            .iter()
            .filter(|g| g.faction != attacker.faction)
            .collect::<Vec<&Group>>();
        if let Some(defender) = attacker.find_target(&defenders, &selected_targets) {
            trace!(
                "{} group {} would deal defending group {} {} damage",
                attacker.faction,
                attacker.id,
                defender.id,
                attacker.calculate_damage(defender)
            );
            targets.push((attacker.clone(), defender.clone()));
            selected_targets.insert((defender.faction, defender.id));
        }
    }
    targets
}

fn attack_targets(groups: &mut Vec<Group>, targets: &mut Vec<(Group, Group)>) {
    targets.sort_by(|a, b| b.0.initiative.cmp(&a.0.initiative));

    for (a, d) in targets {
        if let Some(attacker) = groups
            .iter()
            .find(|g| g.faction == a.faction && g.id == a.id)
            && let Some(d_index) = groups
                .iter()
                .position(|g| g.faction == d.faction && g.id == d.id)
        {
            let damage = attacker.calculate_damage(&groups[d_index]);
            trace!(
                "{} group {} attacks {} group {} killing {}/{} units",
                attacker.faction,
                attacker.id,
                groups[d_index].faction,
                groups[d_index].id,
                min(damage / groups[d_index].hit_points, groups[d_index].units),
                groups[d_index].units
            );
            groups[d_index].units -= damage / groups[d_index].hit_points;
            if groups[d_index].units <= 0 {
                groups.remove(d_index);
            }
        }
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let input: [&str; 1] = [
            "Immune System:
17 units each with 5390 hit points (weak to radiation, bludgeoning) with an attack that does 4507 fire damage at initiative 2
989 units each with 1274 hit points (immune to fire; weak to bludgeoning, slashing) with an attack that does 25 slashing damage at initiative 3

Infection:
801 units each with 4706 hit points (weak to radiation) with an attack that does 116 bludgeoning damage at initiative 1
4485 units each with 2961 hit points (immune to radiation; weak to fire, cold) with an attack that does 12 slashing damage at initiative 4",
        ];
        let expected: [i64; 1] = [5216];

        for i in 0..input.len() {
            let input = parse_input(input[i].to_string());
            assert_eq!(solve_part_1(&input), expected[i]);
        }
    }

    #[test]
    fn test_part_2() {
        let input: [&str; 1] = [
            "Immune System:
17 units each with 5390 hit points (weak to radiation, bludgeoning) with an attack that does 4507 fire damage at initiative 2
989 units each with 1274 hit points (immune to fire; weak to bludgeoning, slashing) with an attack that does 25 slashing damage at initiative 3

Infection:
801 units each with 4706 hit points (weak to radiation) with an attack that does 116 bludgeoning damage at initiative 1
4485 units each with 2961 hit points (immune to radiation; weak to fire, cold) with an attack that does 12 slashing damage at initiative 4",
        ];
        let expected: [i64; 1] = [51];

        for i in 0..input.len() {
            let input = parse_input(input[i].to_string());
            assert_eq!(solve_part_2(&input), expected[i]);
        }
    }
}
