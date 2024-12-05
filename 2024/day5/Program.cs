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

        (Dictionary<int, List<int>> pageOrderingRules, List<List<int>> updates) = parseInput(lines);

        foreach (List<int> pages in updates)
        {
            if (pages.Count % 2 == 0)
            {
                Console.WriteLine($"Pages: {string.Join(",", pages)} contains an even number of entries");
            }
            bool isValid = true;
            for (int i = 0; i < pages.Count; i++)
            {
                int page = pages[i];
                if (!pageOrderingRules.ContainsKey(page))
                {
                    continue;
                };
                foreach (int followingPage in pageOrderingRules[page])
                {
                    if (pages.Contains(followingPage) && pages.IndexOf(followingPage) < i)
                    {
                        isValid = false;
                        break;
                    }
                }
                if (!isValid) break;
            }

            if (isValid)
            {
                int middleIndex = pages.Count / 2;
                total += pages[middleIndex];
            }
        }

        return total;
    }

    static long SolvePart2(string[] lines)
    {
        long total = 0;

        // TODO: Implement logic to solve part 2

        return total;
    }

    static (Dictionary<int, List<int>>, List<List<int>>) parseInput(string[] lines)
    {
        Dictionary<int, List<int>> pageOrderingRules = new();
        List<List<int>> updates = new();
        bool isPart1 = true;
        foreach (string line in lines)
        {
            if (string.IsNullOrEmpty(line))
            {
                isPart1 = false;
            }
            else
            {
                if (isPart1)
                {
                    List<int> pageOrder = new();
                    string[] lineParts = line.Split('|');
                    int firstPage = int.Parse(lineParts[0]);
                    int secondPage = int.Parse(lineParts[1]);
                    if (!pageOrderingRules.ContainsKey(firstPage))
                    {
                        pageOrderingRules.Add(firstPage, new List<int>());
                    }
                    pageOrderingRules[firstPage].Add(secondPage);
                }
                else
                {
                    List<int> pages = new();
                    string[] lineParts = line.Split(',');
                    foreach (string linePart in lineParts)
                    {
                        pages.Add(int.Parse(linePart));
                    }
                    updates.Add(pages);
                }
            }
        }
        return (pageOrderingRules, updates);
    }
}