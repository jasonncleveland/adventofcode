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

                string[] lines = File.ReadAllLines(fileName);

                stopWatch.Start();
                long part1 = SolvePart1(lines);
                stopWatch.Stop();
                Console.WriteLine($"Part 1: {part1} ({stopWatch.ElapsedMilliseconds} ms)");

                stopWatch.Reset();
                stopWatch.Start();
                long part2 = SolvePart2(lines);
                stopWatch.Stop();
                Console.WriteLine($"Part 2: {part2} ({stopWatch.ElapsedMilliseconds} ms)");
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

        // TODO: Implement logic to solve part 2

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