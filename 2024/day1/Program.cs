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
        }
    }

    static int SolvePart1(string[] lines)
    {
        List<int> left = new List<int>();
        List<int> right = new List<int>();
        foreach (string line in lines)
        {
            string[] lineParts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (lineParts.Length != 2)
            {
                throw new Exception($"Invalid input: {line}");
            }
            left.Add(int.Parse(lineParts[0]));
            right.Add(int.Parse(lineParts[1]));
        }
        left.Sort();
        right.Sort();

        int total = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            int difference = Math.Abs(left[i] - right[i]);
            total += difference;
        }

        return total;

    }

    static int SolvePart2(string[] lines)
    {
        List<int> left = new List<int>();
        List<int> right = new List<int>();
        Dictionary<int, int> occurences = new();
        foreach (string line in lines)
        {
            string[] lineParts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (lineParts.Length != 2)
            {
                throw new Exception($"Invalid input: {line}");
            }

            int leftValue = int.Parse(lineParts[0]);
            int rightValue = int.Parse(lineParts[1]);
            left.Add(leftValue);
            right.Add(rightValue);
            if (!occurences.ContainsKey(rightValue))
            {
                occurences.Add(rightValue, 0);
            }

            occurences[rightValue] += 1;
        }

        int total = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            total += left[i] * occurences.GetValueOrDefault(left[i], 0);
        }

        return total;
    }
}