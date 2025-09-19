use std::fmt;

#[derive(Clone, Copy, Debug, Eq, Hash, Ord, PartialEq, PartialOrd)]
pub enum Faction {
    ImmuneSystem,
    Infection,
}

impl fmt::Display for Faction {
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        match self {
            Faction::ImmuneSystem => write!(f, "Immune System"),
            Faction::Infection => write!(f, "Infection"),
        }
    }
}
