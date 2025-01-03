using System;

class Boss(int hp, int damage) : ICloneable
{
    public int HP { get; set; } = hp;
    public int Damage { get; set; } = damage;
    public int Armor { get; set; } = 0;

    public object Clone()
    {
        return this.MemberwiseClone();
    }

    public override string ToString()
    {
        return $"{HP} hp {Damage} damage";
    }
}
