using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

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
        Boss boss = ParseInput(lines);
        Player player = new Player(50, 500);

        return SimulateBattle(player, boss);
    }

    static long SolvePart2(string[] lines)
    {
        long total = 0;

        // TODO: Implement logic to solve part 2

        return total;
    }

    static Boss ParseInput(string[] lines)
    {
        int hp = int.Parse(lines[0].Split(':')[1]);
        int damage = int.Parse(lines[1].Split(':')[1]);

        return new Boss(hp, damage);
    }

    static long SimulateBattle(Player player, Boss boss)
    {
        PriorityQueue<GameState, int> statesToCheck = new();

        List<Spell> spells = new()
        {
            new MagicMissileSpell(),
            new DrainSpell(),
            new ShieldSpell(),
            new PoisonSpell(),
            new RechargeSpell(),
        };

        statesToCheck.Enqueue(new()
        {
            TurnId = 0,
            ManaSpent = 0,
            Player = player,
            Boss = boss,
            AvailableSpells = spells,
            ActiveSpells = [],
        }, 0);

        while (statesToCheck.Count > 0)
        {
            GameState state = statesToCheck.Dequeue();

            List<int> effectsToRemove = new();
            foreach (Spell spell in state.ActiveSpells)
            {
                spell.ProcessEffect(state.Player, state.Boss);
                if (spell.EffectDuration == 0)
                {
                    spell.EndEffect(state.Player, state.Boss);
                    effectsToRemove.Add(state.ActiveSpells.IndexOf(spell));
                }
            }
            
            foreach (int indexToRemove in effectsToRemove)
            {
                Spell spell = state.ActiveSpells[indexToRemove];
                state.ActiveSpells.RemoveAt(indexToRemove);
                state.AvailableSpells.Add((Spell) spell.Clone());
            }

            // Check if the player or boss has died
            if (state.Boss.HP <= 0)
            {
                return state.ManaSpent;
            }
            if (state.Player.HP <= 0)
            {
                continue;
            }

            if (state.TurnId % 2 == 0)
            {
                // Player turn
                foreach (Spell spell in state.AvailableSpells)
                {
                    Player playerCopy = (Player) state.Player.Clone();
                    Boss bossCopy = (Boss) state.Boss.Clone();

                    List<Spell> availableSpells = state.AvailableSpells.Select(spell => (Spell) spell.Clone()).ToList();
                    List<Spell> activeSpells = state.ActiveSpells.Select(spell => (Spell) spell.Clone()).ToList();

                    playerCopy.Mana -= spell.Cost;

                    // If we spend more mana then available, stop
                    if (playerCopy.Mana < 0)
                    {
                        continue;
                    }
                
                    // Process the spell action if one is present
                    if (spell.HasAction)
                    {
                        spell.ProcessAction(playerCopy, bossCopy);
                    }

                    // If a spell manages to kill the boss, stop
                    if (bossCopy.HP <= 0)
                    {
                        return state.ManaSpent + spell.Cost;
                    }
                
                    // Add the spell to the list of active spells if an effect is present
                    if (spell.HasEffect)
                    {
                        spell.StartEffect(playerCopy, bossCopy);
                        int spellIndex = state.AvailableSpells.IndexOf(spell);
                        availableSpells.RemoveAt(spellIndex);
                        activeSpells.Add((Spell) spell.Clone());
                    }

                    statesToCheck.Enqueue(new()
                    {
                        TurnId = state.TurnId + 1,
                        ManaSpent = state.ManaSpent + spell.Cost,
                        Player = playerCopy,
                        Boss = bossCopy,
                        AvailableSpells = availableSpells,
                        ActiveSpells = activeSpells,
                    }, state.ManaSpent + spell.Cost);
                }
            }
            else
            {
                // Boss turn
                int damageDealt = Math.Max(state.Boss.Damage - state.Player.Armor, 1);
                state.Player.HP -= damageDealt;
                if (state.Player.HP <= 0)
                {
                    continue;
                }

                statesToCheck.Enqueue(new()
                {
                    TurnId = state.TurnId + 1,
                    ManaSpent = state.ManaSpent,
                    Player = (Player) state.Player.Clone(),
                    Boss = (Boss) state.Boss.Clone(),
                    AvailableSpells = state.AvailableSpells,
                    ActiveSpells = state.ActiveSpells,
                }, state.ManaSpent);
            }
        }

        return -1;
    }
}

class GameState
{
    public int TurnId { get; set; }
    public int ManaSpent { get; set; }
    public Player Player { get; set; }
    public Boss Boss { get; set; }
    public List<Spell> AvailableSpells { get; set; }
    public List<Spell> ActiveSpells { get; set; }
}
