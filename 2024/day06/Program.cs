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
            // Stop if we are out of bounds
            if (newRow < 0 || newRow > lines.Length - 1 || newColumn < 0 || newColumn > lines[newRow].Length - 1)
            {
                break;
            }
            // If we reach an obstacle, turn right 90 degrees
            if (lines[newRow][newColumn] == '#')
            {
                direction = direction.GetNextDirection();
                continue;
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

        // Get the guard starting postion
        (int row, int column) guardStartingPosition = (0, 0);
        for (int row = 0; row < lines.Length; row++)
        {
            for (int column = 0; column < lines[row].Length; column++)
            {
                if (lines[row][column] == '^')
                {
                    guardStartingPosition = (row, column);
                }
            }
        }
        (int row, int column) guardPosition = guardStartingPosition;

        // Get the list of visited positions
        HashSet<(int, int)> visited = new();
        HashSet<(int, int, Direction)> visitedWithDirection = new();
        Direction direction = Direction.Up;
        int newRow = guardPosition.row;
        int newColumn = guardPosition.column;
        while (true)
        {
            (newRow, newColumn) = direction.GetNextPosition(guardPosition.row, guardPosition.column);
            // Stop if we are out of bounds
            if (newRow < 0 || newRow > lines.Length - 1 || newColumn < 0 || newColumn > lines[newRow].Length - 1)
            {
                break;
            }
            // If we reach an obstacle, turn right 90 degrees
            if (lines[newRow][newColumn] == '#')
            {
                // Change direction
                direction = direction.GetNextDirection();
                continue;
            }

            guardPosition = (newRow, newColumn);
            visited.Add((newRow, newColumn));
        }
        guardPosition = guardStartingPosition;
        foreach ((int, int) obstaclePosition in visited)
        {
            bool loopDetected = detectLoop(lines, guardPosition, obstaclePosition);
            if (loopDetected)
            {
                total += 1;
            }
        }
        return total;
    }

    static bool detectLoop(string[] lines, (int row, int column) startPosition, (int row, int column) obstaclePosition)
    {
        (int row, int column) guardPosition = startPosition;
        HashSet<(int, int, Direction)> visitedWithDirection = new();
        Direction direction = Direction.Up;
        int newRow = guardPosition.row;
        int newColumn = guardPosition.column;
        bool hasLoop = false;
        while (true)
        {
            (newRow, newColumn) = direction.GetNextPosition(guardPosition.row, guardPosition.column);
            if (newRow < 0 || newRow > lines.Length - 1 || newColumn < 0 || newColumn > lines[newRow].Length - 1)
            {
                break;
            }
            // If we reach an obstacle, turn right 90 degrees
            if (lines[newRow][newColumn] == '#' || newRow == obstaclePosition.row && newColumn == obstaclePosition.column)
            {
                // Change direction
                direction = direction.GetNextDirection();
                continue;
            }

            guardPosition = (newRow, newColumn);
            if (visitedWithDirection.Contains((newRow, newColumn, direction)))
            {
                hasLoop = true;
                break;
            }
            visitedWithDirection.Add((newRow, newColumn, direction));
        }
        return hasLoop;
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

    public static Direction GetNextDirection(this Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                return Direction.Right;
            case Direction.Down:
                return Direction.Left;
            case Direction.Left:
                return Direction.Up;
            case Direction.Right:
                return Direction.Down;
            default:
                throw new Exception($"Invalid directiomn given {direction}");
        }
    }
}