using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

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
        Dictionary<int, string> charMap = new();
        Dictionary<int, string> rulesMap = new();
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
                charMap.Add(ruleId, lineParts[1][1].ToString());
            }
            else
            {
                string[] ruleOptions = lineParts[1].Split(" | ");
                List<string> ruleOptionsList = new();
                foreach (string ruleOption in ruleOptions)
                {
                    string[] ruleOptionParts = ruleOption.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    ruleOptionsList.Add(string.Join("", ruleOptionParts.Select(r => $"{{{r}}}")));
                }
                rulesMap.Add(ruleId, string.Join("|", ruleOptionsList));
            }
        }

        List<int> keys = rulesMap.Keys.ToList();
        keys.Sort();

        string pattern = @"\{(?<ruleId>\d+)\}";
        string outputRegex = $"^{rulesMap[0]}$";
        // Replace rules with rules
        bool changesMade = true;
        while (changesMade)
        {
            changesMade = false;
            foreach (Match match in Regex.Matches(outputRegex, pattern))
            {
                int nextRuleId = int.Parse(match.Groups["ruleId"].Value);
                if (rulesMap.ContainsKey(nextRuleId))
                {
                    outputRegex = outputRegex.Replace($"{{{nextRuleId}}}", $"(?:{rulesMap[nextRuleId]})");
                    changesMade = true;
                }
            }
        }

        // Replace ruleIds with chars
        foreach ((int ruleId, string character) in charMap)
        {
            outputRegex = outputRegex.Replace($"{{{ruleId}}}", $"(?:{character})");
        }

        Regex regex = new Regex(outputRegex, RegexOptions.Compiled);

        // Skip empty line
        lineIndex++;

        // Read and check messages
        for (; lineIndex < lines.Length; lineIndex++)
        {
            string line = lines[lineIndex];
            if (Regex.Match(line, outputRegex).Success)
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
}