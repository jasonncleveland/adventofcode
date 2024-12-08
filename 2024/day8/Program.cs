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
        Dictionary<char, List<(int, int)>> items = new();
        HashSet<(int, int)> uniqueLocations = new();
        for (int row = 0; row < lines.Length; row++)
        {
            for (int column = 0; column < lines[row].Length; column++)
            {
                if (lines[row][column] != '.')
                {
                    if (!items.ContainsKey(lines[row][column]))
                    {
                        items.Add(lines[row][column], new List<(int, int)>());
                    }
                    items[lines[row][column]].Add((row, column));
                }
            }
        }

        HashSet<(int, int)> antinodeLocations = new();
        foreach (KeyValuePair<char, List<(int, int)>> item in items)
        {
            foreach ((int row, int column) firstLocation in item.Value)
            {
                foreach ((int row, int column) secondLocation in item.Value)
                {
                    if (firstLocation == secondLocation)
                    {
                        continue;
                    }

                    int rowDelta = secondLocation.row - firstLocation.row;
                    int columnDelta = secondLocation.column - firstLocation.column;
                    if (firstLocation.row - rowDelta >= 0 && firstLocation.row - rowDelta < lines.Length && firstLocation.column - columnDelta >= 0 && firstLocation.column - columnDelta < lines.Length)
                    {
                        antinodeLocations.Add((firstLocation.row - rowDelta, firstLocation.column - columnDelta));
                    }
                    if (secondLocation.row + rowDelta >= 0 && secondLocation.row + rowDelta < lines.Length && secondLocation.column + columnDelta >= 0 && secondLocation.column + columnDelta < lines.Length)
                    {
                        antinodeLocations.Add((secondLocation.row + rowDelta, secondLocation.column + columnDelta));
                    }
                }
            }
        }

        return antinodeLocations.Count;
    }

    static long SolvePart2(string[] lines)
    {
        long total = 0;

        // TODO: Implement logic to solve part 2

        return total;
    }
}