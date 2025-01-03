using System;

class Player(int hp, int mana) : ICloneable
{
    public int HP { get; set; } = hp;
    public int Mana { get; set; } = mana;
    public int Armor { get; set; } = 0;

    public object Clone()
    {
        return this.MemberwiseClone();
    }

    public override string ToString()
    {
        return $"{HP} hp {Armor} armor {Mana} mana";
    }
}
