using System;
using System.Collections.Generic;

using AdventOfCode.Shared.Solver;

using Microsoft.Extensions.Logging;

namespace AdventOfCode.Y2020.Days;

internal sealed class Day07 : AbstractDaySolver<Dictionary<string, Luggage>>
{
    protected override Dictionary<string, Luggage> ParseInput(ILogger logger, string fileContents)
    {
        var lines = fileContents.Split("\n", StringSplitOptions.RemoveEmptyEntries);

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
                    childItem.AddContainedIn(parentItem);
                    parentItem.AddContains(childItem, childCount);
                }
            }
        }
        return luggage;
    }

    protected override string SolvePart1(ILogger logger, Dictionary<string, Luggage> luggage)
    {
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
                if (checkedBags.Add(item.Name))
                {
                    bagsToCheck.Enqueue(item);
                }
            }
        }

        return uniqueBagColours.Count.ToString();
    }

    protected override string SolvePart2(ILogger logger, Dictionary<string, Luggage> luggage)
    {
        Luggage sgb = luggage["shiny gold"];

        int bagCount = 0;
        Queue<Luggage> bagsToCheck = new();
        bagsToCheck.Enqueue(sgb);
        while (bagsToCheck.Count > 0)
        {
            Luggage bagToCheck = bagsToCheck.Dequeue();

            foreach ((Luggage item, int count) in bagToCheck.Contains)
            {
                for (int i = 0; i < count; i++)
                {
                    bagsToCheck.Enqueue(item);
                    bagCount++;
                }
            }
        }

        return bagCount.ToString();
    }
}

internal sealed class Luggage
{
    public string Name { get; }
    public HashSet<Luggage> ContainedIn { get; } = [];
    public List<(Luggage, int)> Contains { get; } = [];

    public Luggage(string name)
    {
        Name = name;
    }

    public void AddContainedIn(Luggage item)
    {
        ContainedIn.Add(item);
    }

    public void AddContains(Luggage item, int count)
    {
        Contains.Add((item, count));
    }
}