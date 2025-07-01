using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

        List<List<int>> reports = parseInput(lines);
        foreach (List<int> report in reports)
        {
            Queue<int> reportQueue = new(report);
            if (reportQueue.Count < 2)
            {
                Console.WriteLine($"Found report with <2 levels: {string.Join(",", reportQueue)}");
                continue;
            }
            if (checkReport(reportQueue)) total++;
        }

        return total;
    }

    static long SolvePart2(string[] lines)
    {
        long total = 0;

        List<List<int>> reports = parseInput(lines);
        foreach (List<int> report in reports)
        {
            if (report.Count < 2)
            {
                Console.WriteLine($"Found report with <2 levels: {string.Join(",", report)}");
                continue;
            }

            Queue<int> levelQueue = new(report);
            if (checkReport(levelQueue))
            {
                total++;
                continue;
            }

            // Brute force the solution by checkin every combination with one item removed
            // Exit early if we find a valid solution because "optimization"
            for (int i = 0; i < report.Count; i++)
            {
                var left = report.Slice(0, i);
                var right = report.Slice(i + 1, report.Count - i - 1);
                var combined = left.Concat(right);
                levelQueue = new(combined);

                if (checkReport(levelQueue))
                {
                    total++;
                    break;
                }
            }
        }

        return total;
    }
    
    static List<List<int>> parseInput(string[] lines)
    {
        List<List<int>> rows = new();
        foreach (string line in lines)
        {
            if (line.StartsWith("#")) continue;
            List<int> row = new();
            string[] lineParts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            foreach (string item in lineParts)
            {
                row.Add(int.Parse(item));
            }
            rows.Add(row);
        }
        return rows;
    }

    static bool checkReport(Queue<int> reportQueue)
    {
        bool isValid = true;
        int previousLevel = reportQueue.Dequeue();
        int currentLevel = reportQueue.Dequeue();
        bool isIncreasing = currentLevel > previousLevel;

        while (true)
        {
            int diff = Math.Abs(currentLevel - previousLevel);

            // Difference must be between 1 and 3
            if (diff < 1 || diff > 3)
            {
                isValid = false;
                break;
            }

            // Must be always increasing or always decreasing
            if (isIncreasing && currentLevel < previousLevel || !isIncreasing && currentLevel > previousLevel)
            {
                isValid = false;
                break;
            }

            // Stop when there are no items left to check
            if (reportQueue.Count == 0)
            {
                break;
            }

            // Get the next item to check
            previousLevel = currentLevel;
            currentLevel = reportQueue.Dequeue();
        }
        return isValid;
    }
}