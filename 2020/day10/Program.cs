using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

public class Program
{
    static Dictionary<(int, int), long> cache = new();
    static int cacheHits = 0;
    static int cacheMisses = 0;

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
        List<int> adapters = ParseInput(lines);

        adapters.Sort();

        Dictionary<int, int> differences = new()
        {
            { 1, 0 },
            { 3, 1 },
        };

        int previousAdapter = 0;
        foreach (int adapter in adapters)
        {
            int diff = adapter - previousAdapter;
            previousAdapter = adapter;
            differences[diff] += 1;
        }

        return differences[1] * differences[3];
    }

    static long SolvePart2(string[] lines)
    {
        List<int> adapters = ParseInput(lines);

        adapters.Sort();

        // Calculate and add the final adapter
        int deviceAdapter = adapters[adapters.Count - 1] + 3;
        adapters.Add(deviceAdapter);

        return FindAdapterCombinationsRec(adapters);
    }

    static List<int> ParseInput(string[] lines)
    {
        List<int> adapters = new();

        foreach (string line in lines)
        {
            adapters.Add(int.Parse(line));
        }

        return adapters;
    }

    static long FindAdapterCombinationsRec(List<int> adapters, int previousAdapter = 0, int level = 0)
    {
        if (previousAdapter == adapters.Last())
        {
            return 1;
        }

        if (cache.ContainsKey((previousAdapter, level)))
        {
            cacheHits++;
            return cache[(previousAdapter, level)];
        }
        cacheMisses++;

        IEnumerable<int> reachableAdapters = adapters.Where(adapter => adapter - previousAdapter >= 1 && adapter - previousAdapter <= 3);
    
        long combinations = 0;
        foreach (int adapter in reachableAdapters)
        {
            combinations += FindAdapterCombinationsRec(adapters, adapter, level + 1);
        }

        cache[(previousAdapter, level)] = combinations;
        return combinations;
    }
}