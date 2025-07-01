using System;
using System.Diagnostics;
using System.IO;
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

        string pattern = @"mul\((?<left>\d+),(?<right>\d+)\)";
        Regex regex = new Regex(pattern, RegexOptions.Compiled);

        foreach (string line in lines)
        {
            MatchCollection matches = regex.Matches(line);

            foreach (Match match in matches)
            {
                int left = int.Parse(match.Groups["left"].Value);
                int right = int.Parse(match.Groups["right"].Value);
                total += left * right;
            }
        }

        return total;
    }

    static long SolvePart2(string[] lines)
    {
        long total = 0;

        string pattern = @"mul\((?<left>\d+),(?<right>\d+)\)|(?<start>do\(\))|(?<stop>don\'t\(\))";
        Regex regex = new Regex(pattern, RegexOptions.Compiled);

        // The multiplication disable carries over between lines
        bool enableMultiplication = true;
        foreach (string line in lines)
        {
            MatchCollection matches = regex.Matches(line);
        
            foreach (Match match in matches)
            {
                if (!string.IsNullOrEmpty(match.Groups["left"].Value) && !string.IsNullOrEmpty(match.Groups["right"].Value))
                {
                    if (enableMultiplication)
                    {
                        int left = int.Parse(match.Groups["left"].Value);
                        int right = int.Parse(match.Groups["right"].Value);
                        total += left * right;
                    }
                }
                else if (!string.IsNullOrEmpty(match.Groups["start"].Value))
                {
                    enableMultiplication = true;
                }
                else if (!string.IsNullOrEmpty(match.Groups["stop"].Value))
                {
                    enableMultiplication = false;
                }
            }
        }

        return total;
    }
}