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
            int size = args.Length > 1 ? int.Parse(args[1]) : 71;
            int max = args.Length > 2 ? int.Parse(args[2]) : 1024;
            if (File.Exists(fileName))
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                string[] lines = File.ReadAllLines(fileName);
                stopWatch.Stop();
                Console.WriteLine($"File read ({stopWatch.Elapsed.TotalMilliseconds} ms)");

                Stopwatch part1Timer = new Stopwatch();
                part1Timer.Start();
                long part1 = SolvePart1(lines, size, max);
                part1Timer.Stop();
                Console.WriteLine($"Part 1: {part1} ({part1Timer.Elapsed.TotalMilliseconds} ms)");

                Stopwatch part2Timer = new Stopwatch();
                part2Timer.Start();
                (int x, int y) part2 = SolvePart2(lines, size, max);
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

    static long SolvePart1(string[] lines, int size, int max)
    {
        List<(int, int)> data = ParseInput(lines);

        char[,] grid = GetGridAtTime(data, size, max);

        (int row, int column) start = (0, 0);
        (int row, int column) end = (size - 1, size - 1);

        return TraverseGrid(grid, size, start, end);
    }

    static (int x, int y) SolvePart2(string[] lines, int size, int max)
    {
        List<(int, int)> data = ParseInput(lines);

        // Perform binary search to find the first index that prevents a path
        int index = BinarySearchBlocks(data, size, max + 1, data.Count - 1);
        if (index >= 0)
        {
            (int y, int x) = data[index];
            // Flip the y and x so that we return x, y format
            return (x, y);
        }

        return (-1, -1);
    }

    static List<(int y, int x)> ParseInput(string[] lines)
    {
        List<(int y, int x)> input = new();
        foreach (string line in lines)
        {
            string[] lineParts = line.Split(',');
            int x = int.Parse(lineParts[0]);
            int y = int.Parse(lineParts[1]);
            input.Add((y, x));
        }
        return input;
    }

    static void PrintGrid(char[,] grid, int size)
    {
        for (int row = 0; row < size; row++)
        {
            for (int column = 0; column < size; column++)
            {
                if (grid[row, column] == '#')
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                }
                Console.Write(grid[row, column]);
                Console.ForegroundColor = ConsoleColor.White;
            }
            Console.WriteLine();
        }
    }

    static int BinarySearchBlocks(List<(int, int)> data, int size, int min, int max)
    {
        (int row, int column) start = (0, 0);
        (int row, int column) end = (size - 1, size - 1);

        int time = min + (max - min) / 2;
        char[,] grid = GetGridAtTime(data, size, time);
        int path = TraverseGrid(grid, size, start, end);

        if (min == max)
        {
            // We have narrowed the search to one index so return whether there is a valid path or block
            // If there is a block then return the index of the block
            return path < 0 ? min - 1 : -1;
        }

        if (path < 0)
        {
            // We found no block so the block is either earlier or this index
            return BinarySearchBlocks(data, size, min, time);
        }
        else
        {
            // We found a valid path so the block must be later in the grid
            return BinarySearchBlocks(data, size, time + 1, max);
        }
    }

    static int TraverseGrid(char[,] grid, int size, (int row, int column) start, (int row, int column) end)
    {
        Queue<(int row, int column, int distance)> locationsToVisit = new();
        HashSet<(int row, int column)> visited = new();

        locationsToVisit.Enqueue((start.row, start.column, 0));
        visited.Add(start);

        while (locationsToVisit.Count > 0)
        {
            (int row, int column, int distance) current = locationsToVisit.Dequeue();

            if (current.row == end.row && current.column == end.column)
            {
                return current.distance;
            }

            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                (int row, int column) next = direction.GetNextPosition(current.row, current.column);
                if (InBounds(grid, size, next) && !visited.Contains(next))
                {
                    visited.Add(next);
                    locationsToVisit.Enqueue((next.row, next.column, current.distance + 1));
                }
            }
        }

        return -1;
    }

    static char[,] GetGridAtTime(List<(int, int)> data, int size, int time)
    {
        char[,] grid = new char[size, size];

        // Initialize the empty grid
        for (int row = 0; row < size; row++)
        {
            for (int column = 0; column < size; column++)
            {
                grid[row, column] = '.';
            }
        }

        // Drop bytes up to the given time
        for (int i = 0; i < time; i++)
        {
            (int row, int column) = data[i];
            grid[row, column] = '#';
        }

        return grid;
    }

    static bool InBounds(char[,] grid, int size, (int row, int column) position)
    {
        if (position.row < 0 || position.row > size - 1 || position.column < 0 || position.column > size - 1)
        {
            return false;
        }
        return grid[position.row, position.column] != '#';
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
    public static (int row, int column) GetNextPosition(this Direction direction, int row, int column)
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