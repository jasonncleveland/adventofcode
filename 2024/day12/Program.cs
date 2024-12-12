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

        HashSet<(int, int)> visited = new();

        for (int row = 0; row < lines.Length; row++)
        {
            for (int column = 0; column < lines[row].Length; column++)
            {
                if (!visited.Contains((row, column)))
                {
                    (int area, int perimeter) result = getGroupSizeRec(lines, row, column, visited);
                    total += result.area * result.perimeter;
                }
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

    static (int area, int perimeter) getGroupSizeRec(string[] lines, int row, int column, HashSet<(int, int)> visited)
    {
        visited.Add((row, column));

        char label = lines[row][column];

        int area = 1;
        int perimeter = 0;
        foreach ((int nextRow, int nextColumn) in getNeighbours(row, column))
        {
            if (nextRow >= 0 && nextRow < lines.Length && nextColumn >= 0 && nextColumn < lines[nextRow].Length)
            {
                if (label != lines[nextRow][nextColumn])
                {
                    // Increase the perimeter when we touch a group that doesn't have the same label
                    perimeter++;
                }
                else
                {
                    // Recursively check neighbours to calculate area and perimeter
                    if (!visited.Contains((nextRow, nextColumn)))
                    {
                        visited.Add((nextRow, nextColumn));
                        (int area, int perimeter) result = getGroupSizeRec(lines, nextRow, nextColumn, visited);
                        area += result.area;
                        perimeter += result.perimeter;
                    }
                }
            }
            else
            {
                // Perimeter should still increase when on the outside edges
                perimeter++;
            }
        }

        return (area, perimeter);
    }

    static IEnumerable<(int, int)> getNeighbours(int row, int column)
    {
        yield return (row - 1, column);
        yield return (row + 1, column);
        yield return (row, column - 1);
        yield return (row, column + 1);
    }
}