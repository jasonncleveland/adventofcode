using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

public class Program
{
    static Dictionary<string, long> cache = new();

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
        long total = 0;

        string[] lineParts = lines[0].Split(',', StringSplitOptions.TrimEntries);
        List<string> towels = new(lineParts);

        List<string> patterns = new();
        for (int i = 2; i < lines.Length; i++)
        {
            if (CheckTowelPatternRec(towels, lines[i]))
            {
                total++;
            }
        }

        return total;
    }

    static long SolvePart2(string[] lines)
    {
        long total = 0;

        string[] lineParts = lines[0].Split(',', StringSplitOptions.TrimEntries);
        List<string> towels = new(lineParts);

        List<string> patterns = new();
        for (int i = 2; i < lines.Length; i++)
        {
            total += CountTowelPatternRec(towels, lines[i]);
        }

        return total;
    }

    static bool CheckTowelPatternRec(List<string> towels, string pattern, int patternStart = 0)
    {
        if (patternStart == pattern.Length)
        {
            return true;
        }

        bool valid = false;

        for (int patternLength = 1; patternLength <= (pattern.Length - patternStart); patternLength++)
        {
            string newPattern = pattern.Substring(patternStart, patternLength);
            if (towels.Contains(newPattern))
            {
                if (CheckTowelPatternRec(towels, pattern, patternStart + patternLength))
                {
                    valid = true;
                    break;
                }
            }
        }

        return valid;
    }

    static long CountTowelPatternRec(List<string> towels, string pattern)
    {
        if (pattern.Length == 0)
        {
            return 1;
        }

        if (cache.ContainsKey(pattern))
        {
            return cache[pattern];
        }

        long total = 0;

        foreach (string towel in towels)
        {
            if (towel.Length <= pattern.Length && pattern.StartsWith(towel))
            {
                total += CountTowelPatternRec(towels, pattern.Substring(towel.Length));
            }
        }

        cache.Add(pattern, total);

        return total;
    }
}