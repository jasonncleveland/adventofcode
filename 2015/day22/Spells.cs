using System;

abstract class Spell : ICloneable
{
    public string Name { get; set; }
    public int Cost { get; set; }
    public bool HasAction { get; set; } = false;
    public bool HasEffect { get; set; } = false;
    public int EffectDuration { get; set; } = 0;
    
    public virtual void ProcessAction(Player player, Boss boss)
    {
        throw new Exception("Not Implemented");
    }

    public virtual void StartEffect(Player player, Boss boss)
    {
        throw new Exception("Not Implemented");
    }

    public virtual void ProcessEffect(Player player, Boss boss)
    {
        throw new Exception("Not Implemented");
    }

    public virtual void EndEffect(Player player, Boss boss)
    {
        throw new Exception("Not Implemented");
    }

    public object Clone()
    {
        return this.MemberwiseClone();
    }

    public override string ToString()
    {
        return $"{Name} - {Cost}";
    }
}

class MagicMissileSpell : Spell
{
    public MagicMissileSpell()
    {
        Name = "Magic Missile";
        Cost = 53;
        HasAction = true;
    }

    public override void ProcessAction(Player player, Boss boss)
    {
        boss.HP -= 4;
    }
}

class DrainSpell : Spell
{
    public DrainSpell()
    {
        Name = "Drain";
        Cost = 73;
        HasAction = true;
    }

    public override void ProcessAction(Player player, Boss boss)
    {
        boss.HP -= 2;
        player.HP += 2;
    }
}

class ShieldSpell : Spell
{
    public ShieldSpell()
    {
        Name = "Shield";
        Cost = 113;
        HasEffect = true;
    }

    public override void StartEffect(Player player, Boss boss)
    {
        player.Armor = 7;
        EffectDuration = 6;
    }

    public override void ProcessEffect(Player player, Boss boss)
    {
        EffectDuration--;
    }

    public override void EndEffect(Player player, Boss boss)
    {
        player.Armor = 0;
    }
}

class PoisonSpell : Spell
{
    public PoisonSpell()
    {
        Name = "Poison";
        Cost = 173;
        HasEffect = true;
    }

    public override void StartEffect(Player player, Boss boss)
    {
        EffectDuration = 6;
    }

    public override void ProcessEffect(Player player, Boss boss)
    {
        boss.HP -= 3;
        EffectDuration--;
    }

    public override void EndEffect(Player player, Boss boss)
    {
    }
}

class RechargeSpell : Spell
{
    public RechargeSpell()
    {
        Name = "Recharge";
        Cost = 229;
        HasEffect = true;
    }

    public override void StartEffect(Player player, Boss boss)
    {
        EffectDuration = 5;
    }

    public override void ProcessEffect(Player player, Boss boss)
    {
        player.Mana += 101;
        EffectDuration--;
    }

    public override void EndEffect(Player player, Boss boss)
    {
    }
}
