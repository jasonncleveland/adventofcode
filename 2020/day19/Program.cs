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

        int lineIndex = 0;

        // Read matching rules
        Dictionary<int, char> charMap = new();
        Dictionary<int, List<List<int>>> rulesMap = new();
        for (;; lineIndex++)
        {
            string line = lines[lineIndex];
            if (string.IsNullOrEmpty(line))
            {
                break;
            }
            string[] lineParts = line.Split(": ");
            int ruleId = int.Parse(lineParts[0]);
            if (lineParts[1].Contains('"'))
            {
                charMap.Add(ruleId, lineParts[1][1]);
            }
            else
            {
                string[] ruleOptions = lineParts[1].Split('|');
                List<List<int>> ruleOptionsList = new();
                foreach (string ruleOption in ruleOptions)
                {
                    int[] rules = Array.ConvertAll<string, int>(ruleOption.Split(' ', StringSplitOptions.RemoveEmptyEntries), rule => int.Parse(rule));
                    ruleOptionsList.Add(new(rules));
                }
                rulesMap.Add(ruleId, ruleOptionsList);
            }
        }

        // Skip empty line
        lineIndex++;

        // Read messages
        for (; lineIndex < lines.Length; lineIndex++)
        {
            // This solution is slow and takes ~5 minutes to complete
            string line = lines[lineIndex];
            List<List<char>> validString = TraverseRuleRec(rulesMap, charMap, 0, line);
            if (validString.Count > 0 && validString.Find(s => s.Count == line.Length && string.Join("", s) == line) != null)
            {
                total += 1;
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

    static List<List<char>> TraverseRuleRec(Dictionary<int, List<List<int>>> rulesMap, Dictionary<int, char> charMap, int ruleId, string target)
    {
        if (charMap.ContainsKey(ruleId))
        {
            return new()
            {
                new()
                {
                    charMap[ruleId]
                }
            };
        }
        else if (rulesMap.ContainsKey(ruleId))
        {
            List<List<char>> possibleCombinations = new();
            foreach (List<int> ruleOption in rulesMap[ruleId])
            {
                List<List<char>> allEndings = new();
                for (int i = 0; i < ruleOption.Count; i++)
                {
                    int ruleOptionId = ruleOption[i];
                    List<List<char>> endings = TraverseRuleRec(rulesMap, charMap, ruleOptionId, target);
                    if (i == 0)
                    {
                        allEndings.AddRange(endings);
                    }
                    else
                    {
                        List<List<char>> allEndingsCopy = new();
                        foreach (List<char> accEnding in allEndings)
                        {
                            foreach (List<char> currentEnding in endings)
                            {
                                List<char> combined = new();
                                combined.AddRange(accEnding);
                                combined.AddRange(currentEnding);
                                allEndingsCopy.Add(combined);
                            }
                        }
                        allEndings = allEndingsCopy;
                    }
                }
                possibleCombinations.AddRange(allEndings);
            }

            return possibleCombinations;
        }
        else
        {
            throw new Exception($"This is not the conditional branch you are looking for");
        }
    }
}