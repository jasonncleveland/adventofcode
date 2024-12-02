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
        long total = 0;

        List<List<string>> groupedInput = groupInput(lines);
        foreach (List<string> group in groupedInput)
        {
            HashSet<char> uniqueAnswers = new();
            foreach (string line in group)
            {
                foreach (char character in line)
                {
                    uniqueAnswers.Add(character);
                }
            }
            total += uniqueAnswers.Count;
        }

        return total;
    }

    static long SolvePart2(string[] lines)
    {
        long total = 0;

        List<List<string>> groupedInput = groupInput(lines);
        foreach (List<string> group in groupedInput)
        {
            int commonAnswersCount = 0;
            Dictionary<char, int> occurences = new();
            foreach (string line in group)
            {
                foreach (char character in line)
                {
                    if (!occurences.ContainsKey(character))
                    {
                        occurences.Add(character, 0);
                    }
                    occurences[character] += 1;
                }
            }
            foreach ((char character, int foundCount) in occurences)
            {
                if (foundCount == group.Count)
                {
                    commonAnswersCount++;
                }
            }
            total += commonAnswersCount;
        }

        return total;
    }

    static List<List<string>> groupInput(string[] lines)
    {
        List<List<string>> groupedInput = new();

        List<string> currentGroup = new();
        groupedInput.Add(currentGroup);

        foreach (string line in lines)
        {
            // When we encounter an empty line, start a new group
            if (string.IsNullOrEmpty(line))
            {
                currentGroup = new();
                groupedInput.Add(currentGroup);
                continue;
            }
            currentGroup.Add(line);
        }

        return groupedInput;
    }
}