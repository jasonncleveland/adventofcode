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
        (int row, int column) guardPosition = (0, 0);
        for (int row = 0; row < lines.Length; row++)
        {
            for (int column = 0; column < lines[row].Length; column++)
            {
                if (lines[row][column] == '^')
                {
                    guardPosition = (row, column);
                }
            }
        }

        HashSet< (int, int)> visited = new();
        Direction direction = Direction.Up;
        int newRow = guardPosition.row;
        int newColumn = guardPosition.column;
        while (true)
        {
            (newRow, newColumn) = direction.GetNextPosition(guardPosition.row, guardPosition.column);
            if (newRow < 0 || newRow > lines.Length - 1 || newColumn < 0 || newColumn > lines[newRow].Length - 1)
            {
                break;
            }
            if (lines[newRow][newColumn] == '#')
            {
                switch (direction)
                {
                    case Direction.Up:
                        direction = Direction.Right;
                        break;
                    case Direction.Down:
                        direction = Direction.Left;
                        break;
                    case Direction.Left:
                        direction = Direction.Up;
                        break;
                    case Direction.Right:
                        direction = Direction.Down;
                        break;
                }
                (newRow, newColumn) = direction.GetNextPosition(guardPosition.row, guardPosition.column);
            }

            if (newRow < 0 || newRow > lines.Length - 1 || newColumn < 0 || newColumn > lines[newRow].Length - 1)
            {
                break;
            }
            guardPosition = (newRow, newColumn);
            visited.Add(guardPosition);
        }

        return visited.Count;
    }

    static long SolvePart2(string[] lines)
    {
        long total = 0;

        // TODO: Implement logic to solve part 2

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