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
                long part1 = SolvePart1(lines[0]);
                part1Timer.Stop();
                Console.WriteLine($"Part 1: {part1} ({part1Timer.Elapsed.TotalMilliseconds} ms)");

                Stopwatch part2Timer = new Stopwatch();
                part2Timer.Start();
                long part2 = SolvePart2(lines[0]);
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

    static long SolvePart1(string line)
    {
        List<int> numbers = new List<int>(Array.ConvertAll(line.Split(','), item => int.Parse(item)));

        Dictionary<int, (int, int)> history = new();

        int turnNumber = 1;
        int lastSpokenNumber = 0;

        // Process starting numbers
        foreach (int number in numbers)
        {
            history[number] = (turnNumber, -1);
            lastSpokenNumber = number;
            turnNumber++;
        }

        // Continue until turn 2020 is reached
        for (; turnNumber <= 2020; turnNumber++)
        {
            (int, int) previous = history[lastSpokenNumber];
            if (previous.Item2 == -1)
            {
                lastSpokenNumber = 0;
            }
            else
            {
                lastSpokenNumber = previous.Item1 - previous.Item2;
            }

            if (!history.ContainsKey(lastSpokenNumber))
            {
                history[lastSpokenNumber] = (turnNumber, -1);
            }
            else
            {
                history[lastSpokenNumber] = (turnNumber, history[lastSpokenNumber].Item1);
            }
        }
        return lastSpokenNumber;
    }

    static long SolvePart2(string line)
    {
        List<int> numbers = new List<int>(Array.ConvertAll(line.Split(','), item => int.Parse(item)));

        Dictionary<int, (int, int)> history = new();

        int turnNumber = 1;
        int lastSpokenNumber = 0;

        // Process starting numbers
        foreach (int number in numbers)
        {
            history[number] = (turnNumber, -1);
            lastSpokenNumber = number;
            turnNumber++;
        }

        // Continue until turn 30000000 is reached
        for (; turnNumber <= 30000000; turnNumber++)
        {
            (int, int) previous = history[lastSpokenNumber];
            if (previous.Item2 == -1)
            {
                lastSpokenNumber = 0;
            }
            else
            {
                lastSpokenNumber = previous.Item1 - previous.Item2;
            }

            if (!history.ContainsKey(lastSpokenNumber))
            {
                history[lastSpokenNumber] = (turnNumber, -1);
            }
            else
            {
                history[lastSpokenNumber] = (turnNumber, history[lastSpokenNumber].Item1);
            }
        }
        return lastSpokenNumber;
    }
}