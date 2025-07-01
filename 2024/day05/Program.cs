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

        (List<List<int>> correctlyOrderedUpdates, _) = getSortedUpdates(pageOrderingRules, updates);

        foreach (List<int> pages in correctlyOrderedUpdates)
        {
            if (pages.Count % 2 == 1)
            {
                int middleIndex = pages.Count / 2;
                total += pages[middleIndex];
            }
            else
            {
                throw new Exception($"Pages: {string.Join(",", pages)} contains an even number of entries");
            }
        }

        return total;
    }

    static long SolvePart2(string[] lines)
    {
        long total = 0;

        (Dictionary<int, List<int>> pageOrderingRules, List<List<int>> updates) = parseInput(lines);

        (_, List<List<int>> incorrectlyOrderedUpdates) = getSortedUpdates(pageOrderingRules, updates);

        foreach (List<int> pages in incorrectlyOrderedUpdates)
        {
            // -1 before, 0 equal, 1 after
            pages.Sort((page1, page2) =>
            {
                if (pageOrderingRules.ContainsKey(page1))
                {
                    var rules = pageOrderingRules[page1];
                    if (rules.Contains(page2))
                    {
                        return -1;
                    }
                    else
                    {
                        return 1;
                    }
                }
                if (pageOrderingRules.ContainsKey(page2))
                {
                    var rules = pageOrderingRules[page2];
                    if (rules.Contains(page1))
                    {
                        return 1;
                    }
                    else
                    {
                        return -1;
                    }
                }
                return 0;
            });

            if (pages.Count % 2 == 1)
            {
                int middleIndex = pages.Count / 2;
                total += pages[middleIndex];
            }
            else
            {
                throw new Exception($"Pages: {string.Join(",", pages)} contains an even number of entries");
            }
        }

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

    static (List<List<int>>, List<List<int>>) getSortedUpdates(Dictionary<int, List<int>> pageOrderingRules, List<List<int>> updates)
    {
        List<List<int>> correctlyOrderedUpdates = new();
        List<List<int>> incorrectlyOrderedUpdates = new();

        foreach (List<int> pages in updates)
        {
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
                correctlyOrderedUpdates.Add(pages);
            }
            else
            {
                incorrectlyOrderedUpdates.Add(pages);
            }
        }

        return (correctlyOrderedUpdates, incorrectlyOrderedUpdates);
    }
}