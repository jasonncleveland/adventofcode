using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

public class Program
{
    static long minSize = long.MaxValue;
    static Dictionary<long, long> minQuantumEntanglement = new();
    static HashSet<string> cache = new();
    static long cacheHits = 0;
    static long cacheMisses = 0;

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
                Console.WriteLine($"Cache Items: {cache.Count} Hits: {cacheHits} Misses: {cacheMisses}");

                Stopwatch part2Timer = new Stopwatch();
                part2Timer.Start();
                long part2 = SolvePart2(lines);
                part2Timer.Stop();
                Console.WriteLine($"Part 2: {part2} ({part2Timer.Elapsed.TotalMilliseconds} ms)");
                Console.WriteLine($"Cache Items: {cache.Count} Hits: {cacheHits} Misses: {cacheMisses}");
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
        List<long> weights = ParseInput(lines);

        long sum = weights.Sum();
        long groupSum = sum / 3;

        cache = new();
        cacheHits = 0;
        cacheMisses = 0;

        minSize = long.MaxValue;
        minQuantumEntanglement = new();

        BuildGroupsRec(weights, groupSum, []);

        return minQuantumEntanglement[minSize];
    }

    static long SolvePart2(string[] lines)
    {
        List<long> weights = ParseInput(lines);

        long sum = weights.Sum();
        long groupSum = sum / 4;

        cache = new();
        cacheHits = 0;
        cacheMisses = 0;

        minSize = long.MaxValue;
        minQuantumEntanglement = new();

        BuildGroupsRec(weights, groupSum, []);

        return minQuantumEntanglement[minSize];
    }

    static List<long> ParseInput(string[] lines)
    {
        List<long> weights = new();

        foreach (string line in lines)
        {
            weights.Add(long.Parse(line));
        }

        return weights;
    }

    static void BuildGroupsRec(List<long> weights, long maxGroupSum, List<long> group)
    {
        if (group.Count > minSize)
        {
            // Once we've found a solution, we only care about shorter solutions so we can ignore any longer ones
            return;
        }

        group.Sort();
        string cacheKey = $"{string.Join(",", group)}";
        if (cache.Contains(cacheKey))
        {
            cacheHits++;
            return;
        }
        cacheMisses++;
        cache.Add(cacheKey);

        long groupSum = group.Sum();
        if (maxGroupSum == groupSum)
        {
            long quantumEntanglement = group.Aggregate(1L, (product, current) => product * current);
            if (group.Count <= minSize)
            {
                minSize = group.Count;
                if (!minQuantumEntanglement.ContainsKey(minSize))
                {
                    minQuantumEntanglement.Add(minSize, quantumEntanglement);
                }
                if (quantumEntanglement < minQuantumEntanglement[minSize])
                {
                    minQuantumEntanglement[minSize] = quantumEntanglement;
                }
            }
            return;
        }

        foreach (long weight in weights)
        {
            List<long> weightsCopy = new(weights);
            weightsCopy.Remove(weight);
            if (groupSum + weight <= maxGroupSum)
            {
                BuildGroupsRec(weightsCopy, maxGroupSum, [..group, weight]);
            }
        }
    }
}