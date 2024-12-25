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

        List<List<int>> locks = new();
        List<List<int>> keys = new();

        // Each pattern is 7 by 5
        for (int row = 0; row < lines.Length; row += 8)
        {
            List<int> heights = new() { -1, -1, -1, -1, -1 };

            if (lines[row] == "#####")
            {
                // Found lock
                for (int i = 0; i < 7; i++)
                {
                    for (int column = 0; column < lines[row].Length; column++)
                    {
                        if (lines[row + i][column] == '.' && heights[column] == -1)
                        {
                            heights[column] = i - 1;
                        }
                    }
                }

                locks.Add(heights);
            }
            else
            {
                // Found key
                for (int i = 6; i >= 0; i--)
                {
                    for (int column = 0; column < lines[row].Length; column++)
                    {
                        if (lines[row + i][column] == '.' && heights[column] == -1)
                        {
                            heights[column] = 6 - i - 1;
                        }
                    }
                }

                keys.Add(heights);
            }
        }

        // Try all combinations of keys and locks
        foreach (List<int> lockHeights in locks)
        {
            foreach (List<int> keyHeights in keys)
            {
                bool isValid = true;
                for (int i = 0; i < 5; i++)
                {
                    if (lockHeights[i] + keyHeights[i] > 5)
                    {
                        isValid = false;
                        break;
                    }
                }

                if (isValid)
                {
                    total++;
                }
            }
        }

        return total;
    }
}