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

        List<List<char>> grid = parseInput(lines);

        for (int row = 0; row < grid.Count; row++)
        {
            for (int column = 0; column < grid[row].Count; column++)
            {
                if (grid[row][column] == 'X')
                {
                    foreach (Direction direction in Enum.GetValues(typeof(Direction)))
                    {
                        (int nextRow, int nextColumn) = direction.GetCoordinates(row, column);
                        bool result = findXmasRec(grid, nextRow, nextColumn, direction, 'X');
                        if (result) total++;
                    }
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

    static List<List<char>> parseInput(string[] lines)
    {
        List<List<char>> rows = new();

        foreach (string line in lines)
        {
            List<char> row = new();
            foreach (char character in line)
            {
                row.Add(character);
            }
            rows.Add(row);
        }
        return rows;
    }

    static bool findXmasRec(List<List<char>> grid, int row, int column, Direction direction, char character)
    {
        if (character == 'S')
        {
            // Base case
            return true;
        }
        if (row < 0 || row > grid.Count - 1 || column < 0 || column > grid[row].Count - 1)
        {
            // Out of bounds
            return false;
        }

        char nextLetter = '\0';
        switch (character)
        {
            case 'X':
            {
                if (grid[row][column] == 'M')
                {
                    nextLetter = 'M';
                    break;
                }
                return false;
            }
            case 'M':
            {
                if (grid[row][column] == 'A')
                {
                    nextLetter = 'A';
                    break;
                }
                return false;
            }
            case 'A':
            {
                if (grid[row][column] == 'S')
                {
                    return true;
                }
                return false;
            }
            default:
                return false;
        }

        (int nextRow, int nextColumn) = direction.GetCoordinates(row, column);
        return findXmasRec(grid, nextRow, nextColumn, direction, nextLetter);
    }
}

enum Direction
{
    N,
    NE,
    E,
    SE,
    S,
    SW,
    W,
    NW
}

static class DirectionMethods
{
    public static (int, int) GetCoordinates(this Direction direction, int row, int column)
    {
        switch (direction)
        {
            case Direction.N:
                return (row - 1, column);
            case Direction.NE:
                return (row - 1, column + 1);
            case Direction.E:
                return (row, column + 1);
            case Direction.SE:
                return (row + 1, column + 1);
            case Direction.S:
                return (row + 1, column);
            case Direction.SW:
                return (row + 1, column - 1);
            case Direction.W:
                return (row, column - 1);
            case Direction.NW:
                return (row - 1, column - 1);
            default:
                throw new Exception($"Invalid directiomn given {direction}");
        }
    }
}
