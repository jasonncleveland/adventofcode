use std::fmt;
use std::str::FromStr;

#[derive(Clone, Debug, Eq, PartialEq, PartialOrd)]
pub enum Attack {
    Bludgeoning,
    Cold,
    Fire,
    Radiation,
    Slashing,
}

impl FromStr for Attack {
    type Err = ();

    fn from_str(s: &str) -> Result<Self, Self::Err> {
        match s {
            "bludgeoning" => Ok(Self::Bludgeoning),
            "cold" => Ok(Self::Cold),
            "fire" => Ok(Self::Fire),
            "radiation" => Ok(Self::Radiation),
            "slashing" => Ok(Self::Slashing),
            _ => Err(()),
        }
    }
}

impl fmt::Display for Attack {
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        match self {
            Attack::Bludgeoning => write!(f, "bludgeoning"),
            Attack::Cold => write!(f, "cold"),
            Attack::Fire => write!(f, "fire"),
            Attack::Radiation => write!(f, "radiation"),
            Attack::Slashing => write!(f, "slashing"),
        }
    }
}

impl Attack {
    pub fn new(attack_type: &str) -> Result<Self, ()> {
        Attack::from_str(attack_type)
    }
}
