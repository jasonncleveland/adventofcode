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
            int minShortcutLength = args.Length > 1 ? int.Parse(args[1]) : 100;
            if (File.Exists(fileName))
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                string[] lines = File.ReadAllLines(fileName);
                stopWatch.Stop();
                Console.WriteLine($"File read ({stopWatch.Elapsed.TotalMilliseconds} ms)");

                Stopwatch part1Timer = new Stopwatch();
                part1Timer.Start();
                long part1 = SolvePart1(lines, minShortcutLength);
                part1Timer.Stop();
                Console.WriteLine($"Part 1: {part1} ({part1Timer.Elapsed.TotalMilliseconds} ms)");

                Stopwatch part2Timer = new Stopwatch();
                part2Timer.Start();
                long part2 = SolvePart2(lines, minShortcutLength);
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

    static long SolvePart1(string[] lines, int minShortcutLength)
    {
        long total = 0;

        (int row, int column) start = (0, 0);
        (int row, int column) end = (0, 0);
        for (int row = 0; row < lines.Length; row++)
        {
            for (int column = 0; column < lines[row].Length; column++)
            {
                if (lines[row][column] == 'S')
                {
                    start = (row, column);
                }
                if (lines[row][column] == 'E')
                {
                    end = (row, column);
                }
            }
        }

        Dictionary<(int, int), int> minCostToReach = new();
        HashSet<(int, int)> cheatLocations = new();
        int orignalDistance = TraverseMaze(lines, start, end, minCostToReach, cheatLocations);

        foreach ((int row, int column) cheat in cheatLocations)
        {
            int shortcutCount = TraverseMazeCheats(lines, cheat, minCostToReach, 2, minShortcutLength);
            if (shortcutCount > 0)
            {
                total += shortcutCount;
            }
        }

        return total;
    }

    static long SolvePart2(string[] lines, int minShortcutLength)
    {
        long total = 0;

        (int row, int column) start = (0, 0);
        (int row, int column) end = (0, 0);
        int totalPathTiles = 0;
        for (int row = 0; row < lines.Length; row++)
        {
            for (int column = 0; column < lines[row].Length; column++)
            {
                if (lines[row][column] == 'S')
                {
                    start = (row, column);
                    totalPathTiles++;
                }
                if (lines[row][column] == 'E')
                {
                    end = (row, column);
                    totalPathTiles++;
                }
                if (lines[row][column] == '.')
                {
                    totalPathTiles++;
                }
            }
        }

        Dictionary<(int, int), int> minCostToReach = new();
        HashSet<(int, int)> cheatLocations = new();
        int orignalDistance = TraverseMaze(lines, start, end, minCostToReach, cheatLocations);

        foreach ((int row, int column) first in cheatLocations)
        {
            foreach ((int row, int column) second in cheatLocations)
            {
                int manhattanDistance = CalculateManhattanDistance(first, second);
                if (minCostToReach[second] > minCostToReach[first] && manhattanDistance <= 20)
                {
                    int pathDistance = minCostToReach[second] - minCostToReach[first];
                    int diff = pathDistance - manhattanDistance;
                    if (diff >= minShortcutLength)
                    {
                        total++;
                    }
                }
            }
        }

        return total;
    }

    static int TraverseMaze(string[] maze, (int row, int column) start, (int row, int column) end, Dictionary<(int, int), int> minCostToReach, HashSet<(int, int)> cheatLocations)
    {
        // Perform BFS
        Queue<(int row, int column, int distance)> locationsToVisit = new();
        HashSet<(int row, int column)> visited = new();

        locationsToVisit.Enqueue((start.row, start.column, 0));
        visited.Add(start);

        while (locationsToVisit.Count > 0)
        {
            (int row, int column, int distance) current = locationsToVisit.Dequeue();
            (int row, int column) currentPosition = (current.row, current.column);

            // Add position to list of cheat start location
            cheatLocations.Add(currentPosition);

            if (!minCostToReach.ContainsKey(currentPosition) || minCostToReach[currentPosition] > current.distance)
            {
                minCostToReach[currentPosition] = current.distance;
            }

            if (current.row == end.row && current.column == end.column)
            {
                return current.distance;
            }

            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                (int row, int column) next = direction.GetNextPosition(currentPosition);
                if (InBounds(maze, next) && !visited.Contains(next))
                {
                    visited.Add(next);
                    if (maze[next.row][next.column] != '#')
                    {
                        locationsToVisit.Enqueue((next.row, next.column, current.distance + 1));
                    }
                }
            }
        }

        return -1;
    }

    static int TraverseMazeCheats(string[] maze, (int row, int column) cheat, Dictionary<(int, int), int> minCostToReach, int cheatDistance, int minShortcutLength = 1)
    {
        // Perform BFS
        Queue<(int row, int column, int distance, bool inWall)> locationsToVisit = new();
        HashSet<(int row, int column)> visited = new();

        locationsToVisit.Enqueue((cheat.row, cheat.column, 0, false));
        visited.Add(cheat);

        int shortcutCount = 0;
        while (locationsToVisit.Count > 0)
        {
            (int row, int column, int distance, bool inWall) current = locationsToVisit.Dequeue();
            (int row, int column) currentPosition = (current.row, current.column);

            if (current.distance == 1 && !current.inWall)
            {
                // Ignore if attempting to check neighbour on path as first move
                continue;
            }

            if (current.distance > cheatDistance)
            {
                // Ignore if we've gone past the limit
                continue;
            }

            // Must vist wall before seeing path
            if (current.distance > 1 && !current.inWall)
            {
                // We have found another location on the path
                if (minCostToReach.ContainsKey(currentPosition))
                {
                    // Ignore if the shortcut goes to a path location before the cheat location
                    if (minCostToReach[currentPosition] < minCostToReach[cheat])
                    {
                        continue;
                    }
                    int shortcutLength = minCostToReach[currentPosition] - minCostToReach[cheat] - current.distance;
                    if (shortcutLength >= minShortcutLength)
                    {
                        shortcutCount++;
                    }
                }
            }

            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                (int row, int column) next = direction.GetNextPosition(currentPosition);
                if (InBounds(maze, next) && !visited.Contains(next))
                {
                    visited.Add(next);
                    locationsToVisit.Enqueue((next.row, next.column, current.distance + 1, maze[next.row][next.column] == '#'));
                }
            }
        }

        return shortcutCount;
    }

    static bool InBounds(string[] maze, (int row, int column) position)
    {
        if (position.row < 0 || position.row > maze.Length - 1 || position.column < 0 || position.column > maze[position.row].Length - 1)
        {
            return false;
        }
        return true;
    }

    static int CalculateManhattanDistance((int row, int column) start, (int row, int column) end)
    {
        return Math.Abs(start.row - end.row) + Math.Abs(start.column - end.column);
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
    public static (int row, int column) GetNextPosition(this Direction direction, (int row, int column) position)
    {
        switch (direction)
        {
            case Direction.Up:
                return (position.row - 1, position.column);
            case Direction.Down:
                return (position.row + 1, position.column);
            case Direction.Left:
                return (position.row, position.column - 1);
            case Direction.Right:
                return (position.row, position.column + 1);
            default:
                throw new Exception($"Invalid directiomn given {direction}");
        }
    }
}