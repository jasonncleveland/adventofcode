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

        List<(int, int)> trailHeads = new();
        for (int row = 0; row < lines.Length; row++)
        {
            for (int column = 0; column < lines[row].Length; column++)
            {
                if (lines[row][column] == '0')
                {
                    HashSet<(int, int)> visited = new();
                    HashSet<(int, int)> uniqueTops = findUniqueTopsRec(lines, row, column, visited);
                    total += uniqueTops.Count;
                }
            }
        }

        return total;
    }

    static long SolvePart2(string[] lines)
    {
        long total = 0;

        List<(int, int)> trailHeads = new();
        for (int row = 0; row < lines.Length; row++)
        {
            for (int column = 0; column < lines[row].Length; column++)
            {
                if (lines[row][column] == '0')
                {
                    total += findUniqueTrailsRec(lines, row, column);
                }
            }
        }

        return total;
    }

    static HashSet<(int, int)> findUniqueTopsRec(string[] map, int row, int column, HashSet<(int, int)> visited)
    {
        HashSet<(int, int)> tops = new();

        if (map[row][column] == '9')
        {
            tops.Add((row, column));
            return tops;
        }

        foreach (Direction direction in Enum.GetValues(typeof(Direction)))
        {
            (int nextRow, int nextColumn) = direction.GetNextPosition(row, column);
            if (nextRow >= 0 && nextRow < map.Length && nextColumn >= 0 && nextColumn < map[nextRow].Length)
            {
                // Only check the next height if it is 1 greater than the current height and not visited before
                if (map[nextRow][nextColumn] - map[row][column] == 1 && !visited.Contains((nextRow, nextColumn)))
                {
                    visited.Add((nextRow, nextColumn));
                    tops.UnionWith(findUniqueTopsRec(map, nextRow, nextColumn, visited));
                }
            }
        }

        return tops;
    }

    static int findUniqueTrailsRec(string[] map, int row, int column)
    {
        int total = 0;

        if (map[row][column] == '9')
        {
            return 1;
        }

        foreach (Direction direction in Enum.GetValues(typeof(Direction)))
        {
            (int nextRow, int nextColumn) = direction.GetNextPosition(row, column);
            if (nextRow >= 0 && nextRow < map.Length && nextColumn >= 0 && nextColumn < map[nextRow].Length)
            {
                // Only check the next height if it is 1 greater than the current height
                if (map[nextRow][nextColumn] - map[row][column] == 1)
                {
                    total += findUniqueTrailsRec(map, nextRow, nextColumn);
                }
            }
        }

        return total;
    }
}

enum Direction
{
    Up,
    Down,
    Left,
    Right
}

static class DirectionMethods
{
    public static (int, int) GetNextPosition(this Direction direction, int row, int column)
    {
        switch (direction)
        {
            case Direction.Up:
                return (row - 1, column);
            case Direction.Down:
                return (row + 1, column);
            case Direction.Left:
                return (row, column - 1);
            case Direction.Right:
                return (row, column + 1);
            default:
                throw new Exception($"Invalid directiomn given {direction}");
        }
    }
}