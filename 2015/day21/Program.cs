using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

public class Program
{
    static void Main(string[] args)
    {
        if (args.Length > 0)
        {
            string fileName = args[0];
            if (File.Exists(fileName))
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                string[] lines = File.ReadAllLines(fileName);
                stopWatch.Stop();
                Console.WriteLine($"File read ({stopWatch.Elapsed.TotalMilliseconds} ms)");

                Stopwatch part1Timer = new Stopwatch();
                part1Timer.Start();
                long part1 = SolvePart1(lines);
                part1Timer.Stop();
                Console.WriteLine($"Part 1: {part1} ({part1Timer.Elapsed.TotalMilliseconds} ms)");

                Stopwatch part2Timer = new Stopwatch();
                part2Timer.Start();
                long part2 = SolvePart2(lines);
                part2Timer.Stop();
                Console.WriteLine($"Part 2: {part2} ({part2Timer.Elapsed.TotalMilliseconds} ms)");
            }
            else
            {
                throw new ArgumentException("Invalid file name provided. Please provide a valid file name.");
            }
        }
        else
        {
            throw new ArgumentException("Input data file name not provided. Please provide the file name as an argument: dotnet run <file-name>");
        }
    }

    static long SolvePart1(string[] lines)
    {
        (int hp, int damage, int armor) boss = ParseInput(lines);

        int minimumCost = int.MaxValue;

        int combinations = 0;
        HashSet<(int, int, int, int)> checkedCombinations = new();

        foreach (ShopItem weapon in Shop.Weapons)
        {
            foreach (ShopItem armor in Shop.Armor)
            {
                foreach (ShopItem firstRing in Shop.Rings)
                {
                    foreach (ShopItem secondRing in Shop.Rings)
                    {
                        if (secondRing.Name == firstRing.Name || checkedCombinations.Contains((weapon.Id, armor.Id, firstRing.Id, secondRing.Id)) || checkedCombinations.Contains((weapon.Id, armor.Id, secondRing.Id, firstRing.Id)))
                        {
                            continue;
                        }
                        checkedCombinations.Add((weapon.Id, armor.Id, firstRing.Id, secondRing.Id));
                        checkedCombinations.Add((weapon.Id, armor.Id, secondRing.Id, firstRing.Id));
                        combinations++;

                        int totalDamage = weapon.Damage + firstRing.Damage + secondRing.Damage;
                        int totalArmor = armor.Armor + firstRing.Armor + secondRing.Armor;
                        int totalCost = weapon.Cost + armor.Cost + firstRing.Cost + secondRing.Cost;
                        bool battleResult = SimulateBattle(boss, (100, totalDamage, totalArmor));
                        if (battleResult)
                        {
                            if (totalCost < minimumCost)
                            {
                                minimumCost = totalCost;
                            }
                        }
                    }
                }
            }
        }

        return minimumCost;
    }

    static long SolvePart2(string[] lines)
    {
        long total = 0;

        // TODO: Implement logic to solve part 2

        return total;
    }

    static (int hp, int damage, int armor) ParseInput(string[] lines)
    {
        int hp = int.Parse(lines[0].Split(':')[1]);
        int damage = int.Parse(lines[1].Split(':')[1]);
        int armor = int.Parse(lines[2].Split(':')[1]);

        return (hp, damage, armor);
    }

    static bool SimulateBattle((int hp, int damage, int armor) boss, (int hp, int damage, int armor) player)
    {
        for (int i = 0;; i++)
        {
            if (i % 2 == 0)
            {
                boss = Attack(player, boss);
                if (boss.hp <= 0)
                {
                    return true;
                }
            }
            else
            {
                player = Attack(boss, player);
                if (player.hp <= 0)
                {
                    return false;
                }
            }
        }
    }

    static (int hp, int damage, int armor) Attack((int hp, int damage, int armor) attacker, (int hp, int damage, int armor) defender)
    {
        // Attacker must deal at least 1 damage
        int damageDealt = Math.Max(attacker.damage - defender.armor, 1);
        return (defender.hp - damageDealt, defender.damage, defender.armor);
    }
}

struct ShopItem
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Cost { get; set; }
    public int Damage { get; set; }
    public int Armor { get; set; }

    public override string ToString()
    {
        return $"{Name} ({Id}) - {Cost}c: {Damage} damage, {Armor} armor";
    }
}

static class Shop
{
    public static List<ShopItem> Weapons { get; } = new List<ShopItem>
    {
        new ShopItem { Id = 11, Name = "Dagger", Cost = 8, Damage = 4, Armor = 0 },
        new ShopItem { Id = 12, Name = "Shortsword", Cost = 10, Damage = 5, Armor = 0 },
        new ShopItem { Id = 13, Name = "Warhammer", Cost = 25, Damage = 6, Armor = 0 },
        new ShopItem { Id = 14, Name = "Longsword", Cost = 40, Damage = 7, Armor = 0 },
        new ShopItem { Id = 15, Name = "Greataxe", Cost = 74, Damage = 8, Armor = 0 },
    };

    public static List<ShopItem> Armor { get; } = new List<ShopItem>
    {
        new ShopItem { Id = 21, Name = "None", Cost = 0, Damage = 0, Armor = 0 },
        new ShopItem { Id = 22, Name = "Leather", Cost = 13, Damage = 0, Armor = 1 },
        new ShopItem { Id = 23, Name = "Chainmail", Cost = 31, Damage = 0, Armor = 2 },
        new ShopItem { Id = 24, Name = "Splintmail", Cost = 53, Damage = 0, Armor = 3 },
        new ShopItem { Id = 25, Name = "Bandedmail", Cost = 75, Damage = 0, Armor = 4 },
        new ShopItem { Id = 26, Name = "Platemail", Cost = 102, Damage = 0, Armor = 5 },
    };

    public static List<ShopItem> Rings { get; } = new List<ShopItem>
    {
        new ShopItem { Id = 31, Name = "None 1", Cost = 0, Damage = 0, Armor = 0 },
        new ShopItem { Id = 32, Name = "None 2", Cost = 0, Damage = 0, Armor = 0 },
        new ShopItem { Id = 33, Name = "Damage +1", Cost = 25, Damage = 1, Armor = 0 },
        new ShopItem { Id = 34, Name = "Damage +2", Cost = 50, Damage = 2, Armor = 0 },
        new ShopItem { Id = 35, Name = "Damage +3", Cost = 100, Damage = 3, Armor = 0 },
        new ShopItem { Id = 36, Name = "Defense +1", Cost = 20, Damage = 0, Armor = 1 },
        new ShopItem { Id = 37, Name = "Defense +2", Cost = 40, Damage = 0, Armor = 2 },
        new ShopItem { Id = 38, Name = "Defense +3", Cost = 80, Damage = 0, Armor = 3 },
    };
}