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
        int startTime = int.Parse(lines[0]);

        List<int> buses = new();
        foreach (string bus in lines[1].Split(","))
        {
            if (int.TryParse(bus, out int busNumber))
            {
                buses.Add(busNumber);
            }
        }

        for (int currentTime = startTime; currentTime < startTime + 50; currentTime++)
        {
            foreach (int bus in buses)
            {
                if (currentTime % bus == 0)
                {
                    return (currentTime - startTime) * bus;
                }
            }
        }

        return -1;
    }

    static long SolvePart2(string[] lines)
    {
        long total = 0;

        // TODO: Implement logic to solve part 2

        return total;
    }
}