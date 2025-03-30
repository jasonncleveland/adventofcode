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
        List<long> numbers = ParseInput(lines);

        long min = long.MaxValue;
        long max = long.MinValue;
        foreach (long number in numbers)
        {
            if (number > max) max = number;
            if (number < min) min = number;
        }

        long minFuel = long.MaxValue;
        for (long position = min; position <= max; position++)
        {
            long total = 0;
            foreach (long number in numbers)
            {
                long fuelUsed = Math.Abs(number - position);
                total += fuelUsed;

                if (total > minFuel) break;
            }
            if (total < minFuel) minFuel = total;
        }

        return minFuel;
    }

    static long SolvePart2(string[] lines)
    {
        List<long> numbers = ParseInput(lines);

        long min = long.MaxValue;
        long max = long.MinValue;
        foreach (long number in numbers)
        {
            if (number > max) max = number;
            if (number < min) min = number;
        }

        long minFuel = long.MaxValue;
        for (long position = min; position <= max; position++)
        {
            long total = 0;
            foreach (long number in numbers)
            {
                long fuelUsed = Math.Abs(number - position);
                total += fuelUsed * (fuelUsed + 1) / 2;

                if (total > minFuel) break;
            }
            if (total < minFuel) minFuel = total;
        }

        return minFuel;
    }

    static List<long> ParseInput(string[] lines)
    {
        List<long> numbers = new();

        foreach (string line in lines)
        {
            foreach (string item in line.Split(','))
            {
                numbers.Add(long.Parse(item));
            }
        }

        return numbers;
    }
}