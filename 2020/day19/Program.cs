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

        // Parse input into rules mapping and messages list
        (Dictionary<int, string> rules, List<string> messages) = ParseInput(lines);

        // Convert rules to regex
        string regexString = ConvertRulesToRegexString(rules);

        // Check messages
        foreach (string message in messages)
        {
            if (Regex.Match(message, regexString).Success)
            {
                total += 1;
            }
        }

        return total;
    }

    static long SolvePart2(string[] lines)
    {
        long total = 0;

        // Parse input into rules mapping and messages list
        (Dictionary<int, string> rules, List<string> messages) = ParseInput(lines);

        // Replace 8 with 8: 42 | 42 8
        // This is equivalent to 42+
        rules[8] = "{42}+";
        // Replace 11 with 11: 42 31 | 42 11 31
        // This is looking for the same number of 42 as 31
        // Hard code up to 4 pairs because laziness
        rules[11] = "{42}{31}|{42}{42}{31}{31}|{42}{42}{42}{31}{31}{31}|{42}{42}{42}{42}{31}{31}{31}{31}";

        // Convert rules to regex
        string regexString = ConvertRulesToRegexString(rules);

        // Check messages
        foreach (string message in messages)
        {
            if (Regex.Match(message, regexString).Success)
            {
                total += 1;
            }
        }

        return total;
    }

    static (Dictionary<int, string>, List<string>) ParseInput(string[] lines)
    {
        int lineIndex = 0;

        // Read matching rules
        Dictionary<int, string> rulesMap = new();
        for (; ; lineIndex++)
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
                rulesMap.Add(ruleId, lineParts[1][1].ToString());
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

        // Skip empty line
        lineIndex++;

        // Read messages
        List<string> messages = new();
        for (; lineIndex < lines.Length; lineIndex++)
        {
            messages.Add(lines[lineIndex]);
        }

        return (rulesMap, messages);
    }

    static string ConvertRulesToRegexString(Dictionary<int, string> rules)
    {
        string pattern = @"\{(?<ruleId>\d+)\}";
        string outputRegex = $"^{rules[0]}$";

        // Replace rules until no changes are made
        bool changesMade = true;
        while (changesMade)
        {
            changesMade = false;
            foreach (Match match in Regex.Matches(outputRegex, pattern))
            {
                int nextRuleId = int.Parse(match.Groups["ruleId"].Value);
                if (rules.ContainsKey(nextRuleId))
                {
                    outputRegex = outputRegex.Replace($"{{{nextRuleId}}}", $"(?:{rules[nextRuleId]})");
                    changesMade = true;
                }
            }
        }

        return outputRegex;
    }
}