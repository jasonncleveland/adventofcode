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

        return TraverseMaze(lines, start, end);
    }

    static long SolvePart2(string[] lines)
    {
        long total = 0;

        // TODO: Implement logic to solve part 2

        return total;
    }

    static long TraverseMaze(string[] maze, (int row, int column) start, (int row, int column) end)
    {
        // Perform BFS with a priority queue
        PriorityQueue<(int row, int column, Direction direction, long cost), long> nodesToCheck = new();
        HashSet<(int, int, Direction)> visited = new();
    
        nodesToCheck.Enqueue((start.row, start.column, Direction.Right, 0), 0);
        visited.Add((start.row, start.column, Direction.Right));

        while (nodesToCheck.Count > 0)
        {
            (int row, int column, Direction direction, long cost) node = nodesToCheck.Dequeue();
        
            if (node.row == end.row && node.column == end.column)
            {
                return node.cost;
            }

            List<Direction> directions = new();
            // Move forward
            directions.Add(node.direction);
            // Turn 90 degrees
            directions.AddRange(node.direction.GetPerpendicularDirections());

            foreach (Direction direction in directions)
            {
                (int row, int column) next = (node.row, node.column);
                if (node.direction == direction)
                {
                    next = direction.GetNextPosition((node.row, node.column));
                }
                if (InBounds(maze, next) && !visited.Contains((next.row, next.column, direction)))
                {
                    visited.Add((next.row, next.column, direction));

                    long newCost = node.cost;
                    if (node.direction != direction)
                    {
                        // Direction is different so we are changing direction
                        newCost += 1000;
                    }
                    else
                    {
                        // Direction is the same so we are moving forward
                        newCost += 1;
                    }

                    nodesToCheck.Enqueue((next.row, next.column, direction, newCost), newCost);
                }
            }
        }
    
        return -1;
    }

    static bool InBounds(string[] maze, (int row, int column) position)
    {
        if (position.row < 0 || position.row > maze.Length - 1 || position.column < 0 || position.column > maze[position.row].Length)
        {
            return false;
        }
        return maze[position.row][position.column] != '#';
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

    public static List<Direction> GetPerpendicularDirections(this Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
            case Direction.Down:
                return [Direction.Left, Direction.Right];
            case Direction.Left:
            case Direction.Right:
                return [Direction.Up, Direction.Down];
            default:
                throw new Exception($"Invalid directiomn given {direction}");
        }
    }
}