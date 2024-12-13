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

        HashSet<(double, double)> visited = new();

        for (int row = 0; row < lines.Length; row++)
        {
            for (int column = 0; column < lines[row].Length; column++)
            {
                if (!visited.Contains((row, column)))
                {
                    (int area, int perimeter, int size) result = getGroupSizeRec(lines, row, column, visited);
                    total += result.area * result.perimeter;
                }
            }
        }

        return total;
    }

    static long SolvePart2(string[] lines)
    {
        long total = 0;

        HashSet<(double, double)> visited = new();

        for (int row = 0; row < lines.Length; row++)
        {
            for (int column = 0; column < lines[row].Length; column++)
            {
                if (!visited.Contains((row, column)))
                {
                    HashSet<(double, double)> visitedSides = new();
                    (int area, int perimeter, int size) result = getGroupSizeRec(lines, row, column, visitedSides);
                    visited.UnionWith(visitedSides);
                    total += result.area * result.size;
                }
            }
        }

        return total;
    }

    static (int area, int perimeter, int size) getGroupSizeRec(string[] lines, int row, int column, HashSet<(double, double)> visited)
    {
        visited.Add((row, column));

        char label = lines[row][column];

        int area = 1;
        int perimeter = 0;
        int size = 0;

        // Check neighbours to see if we are a corner
        size = countCorners(lines, row, column);

        // Check neighbours to get area and perimeter
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
                        (int area, int perimeter, int size) result = getGroupSizeRec(lines, nextRow, nextColumn, visited);
                        area += result.area;
                        perimeter += result.perimeter;
                        size += result.size;
                    }
                }
            }
            else
            {
                // Perimeter should still increase when on the outside edges
                perimeter++;
            }
        }

        return (area, perimeter, size);
    }

    static int countCorners(string[] lines, int row, int column)
    {
        char label = lines[row][column];

        // Get values of adjacent locations
        char nw = getCharacterAt(lines, row - 1, column - 1);
        char n = getCharacterAt(lines, row - 1, column);
        char ne = getCharacterAt(lines, row - 1, column + 1);
        char w = getCharacterAt(lines, row, column - 1);
        char e = getCharacterAt(lines, row, column + 1);
        char se = getCharacterAt(lines, row + 1, column + 1);
        char s = getCharacterAt(lines, row + 1, column);
        char sw = getCharacterAt(lines, row + 1, column - 1);

        // There must be at least 2 different nighbours and they must cannot be in the same direction (e.g. up and down, left and right)
        int corners = 0;

        // Check for normal corners

        // Top left
        if (w != label && n != label)
        {
            corners++;
        }
        // Top right
        if (e != label && n != label)
        {
            corners++;
        }
        // Bottom left
        if (w != label && s != label)
        {
            corners++;
        }
        // Bottom right
        if (e != label && s != label)
        {
            corners++;
        }

        // Check for inner corners

        // Top left inner corner
        if (nw != label && w == label && n == label)
        {
            corners++;
        }
        // Top right inner corner
        if (ne != label && e == label && n == label)
        {
            corners++;
        }
        // Bottom left inner corner
        if (sw != label && w == label && s == label)
        {
            corners++;
        }
        // Bottom right inner corner
        if (se != label && e == label && s == label)
        {
            corners++;
        }

        return corners;
    }

    static char getCharacterAt(string[] lines, int row, int column)
    {
        if (row >= 0 && row < lines.Length && column >= 0 && column < lines[row].Length)
        {
            return lines[row][column];
        }
        // Return dot (.) if the given location is out of bounds
        return '.';
    }

    static IEnumerable<(int, int)> getNeighbours(int row, int column)
    {
        yield return (row - 1, column);
        yield return (row + 1, column);
        yield return (row, column - 1);
        yield return (row, column + 1);
    }
}