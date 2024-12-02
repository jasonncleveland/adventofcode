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
        Dictionary<string, Luggage> luggage = parseInput(lines);

        Luggage sgb = luggage["shiny gold"];

        HashSet<string> uniqueBagColours = new();
        HashSet<string> checkedBags = new();
        Queue<Luggage> bagsToCheck = new(sgb.ContainedIn);
        while (bagsToCheck.Count > 0)
        {
            Luggage bagToCheck = bagsToCheck.Dequeue();
            uniqueBagColours.Add(bagToCheck.Name);

            foreach (Luggage item in bagToCheck.ContainedIn)
            {
                if (!checkedBags.Contains(item.Name)) {
                    checkedBags.Add(item.Name);
                    bagsToCheck.Enqueue(item);
                }
            }
        }

        return uniqueBagColours.Count;
    }

    static long SolvePart2(string[] lines)
    {
        long total = 0;

        // TODO: Implement logic to solve part 2

        return total;
    }

    static Dictionary<string, Luggage> parseInput(string[] lines)
    {
        Dictionary<string, Luggage> luggage = new();
        foreach (string line in lines)
        {
            string[] lineParts = line.Split(" contain ");
            string parent = lineParts[0];
            string parentName = parent.Remove(parent.Length - 4, 4).TrimEnd();
            if (!luggage.ContainsKey(parentName))
            {
                luggage.Add(parentName, new Luggage(parentName));
            }
            Luggage parentItem = luggage[parentName];
            bool hasChildren = !lineParts[1].Contains("no other bags");
            if (hasChildren)
            {
                string[] children = lineParts[1].TrimEnd('.').Split(", ");
                foreach (string child in children)
                {
                    string[] childParts = child.Split(" ", 2);
                    string childName = childParts[1];
                    childName = childName.Remove(childName.Length - 4, 4).TrimEnd();
                    int childCount = int.Parse(childParts[0]);
                    if (!luggage.ContainsKey(childName))
                    {
                        luggage.Add(childName, new Luggage(childName));
                    }
                    Luggage childItem = luggage[childName];
                    childItem.Count = childCount;
                    childItem.AddContainedIn(parentItem);
                    parentItem.AddContains(childItem);
                }
            }
        }
        return luggage;
    }
}

public class Luggage
{
    public string Name { get; }
    public int Count { get; set; }
    public HashSet<Luggage> ContainedIn { get; } = [];
    public HashSet<Luggage> Contains { get; } = [];

    public Luggage(string name)
    {
        Name = name;
    }

    public void AddContainedIn(Luggage item)
    {
        ContainedIn.Add(item);
    }

    public void AddContains(Luggage item)
    {
        Contains.Add(item);
    }
}