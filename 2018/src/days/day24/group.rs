use std::collections::HashSet;
use std::fmt;

use super::attack::Attack;
use super::faction::Faction;

#[derive(Clone, Debug, Eq, PartialEq, PartialOrd)]
pub struct Group {
    pub faction: Faction,
    pub id: usize,
    pub units: i64,
    pub hit_points: i64,
    pub attack_damage: i64,
    pub attack_type: Attack,
    pub weaknesses: Vec<Attack>,
    pub immunities: Vec<Attack>,
    pub initiative: i64,
}

impl fmt::Display for Group {
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        write!(
            f,
            "{} {}. {} units each with {} hit points (weak to {}; immune to {}) with an attack that does {} {} damage at initiative {}",
            self.faction,
            self.id,
            self.units,
            self.hit_points,
            self.weaknesses
                .iter()
                .map(|t| t.to_string())
                .collect::<Vec<String>>()
                .join(", "),
            self.immunities
                .iter()
                .map(|t| t.to_string())
                .collect::<Vec<String>>()
                .join(", "),
            self.attack_damage,
            self.attack_type,
            self.initiative,
        )
    }
}

impl Group {
    pub fn new(
        faction: Faction,
        id: usize,
        units: i64,
        hit_points: i64,
        attack_damage: i64,
        attack_type: Attack,
        initiative: i64,
    ) -> Self {
        Self {
            faction,
            id,
            units,
            hit_points,
            attack_damage,
            attack_type,
            weaknesses: vec![],
            immunities: vec![],
            initiative,
        }
    }

    pub fn effective_power(&self) -> i64 {
        self.units * self.attack_damage
    }

    pub fn find_target<'a>(
        &self,
        enemies: &'a [&Group],
        selected_targets: &HashSet<(Faction, usize)>,
    ) -> Option<&'a Group> {
        let mut options: Vec<(i64, i64, i64, usize)> = Vec::new();
        for defender in enemies {
            if selected_targets.contains(&(defender.faction, defender.id)) {
                continue;
            }
            let potential_damage = self.calculate_damage(defender);
            if potential_damage > 0 {
                options.push((
                    potential_damage,
                    defender.effective_power(),
                    defender.initiative,
                    defender.id,
                ));
            }
        }

        options.sort_by(|a, b| b.cmp(a));
        if let Some((_, _, _, id)) = options.first()
            && let Some(target) = enemies.iter().find(|g| g.id == *id)
        {
            return Some(target);
        }
        None
    }

    pub fn calculate_damage(&self, other: &Group) -> i64 {
        if other.immunities.contains(&self.attack_type) {
            0
        } else if other.weaknesses.contains(&self.attack_type) {
            self.effective_power() * 2
        } else {
            self.effective_power()
        }
    }
}
