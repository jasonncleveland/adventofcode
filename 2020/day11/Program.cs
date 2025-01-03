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
        List<List<char>> grid = ParseInput(lines);

        int result;
        do
        {
            result = SimulateSeatChanges(grid);
        } while (result > 0);

        return CountOccurences(grid, '#');
    }

    static long SolvePart2(string[] lines)
    {
        long total = 0;

        // TODO: Implement logic to solve part 2

        return total;
    }

    static List<List<char>> ParseInput(string[] lines)
    {
        List<List<char>> grid = new();

        foreach (string line in lines)
        {
            grid.Add(new(line.ToCharArray()));
        }

        return grid;
    }

    static void PrintGrid(List<List<char>> grid)
    {
        for (int row = 0; row < grid.Count; row++)
        {
            for (int column = 0; column < grid[row].Count; column++)
            {
                Console.Write(grid[row][column]);
            }
            Console.WriteLine();
        }
    }

    static List<List<char>> CopyGrid(List<List<char>> grid)
    {
        List<List<char>> copy = new();

        foreach (List<char> row in grid)
        {
            copy.Add(new(row));
        }

        return copy;
    }

    static int SimulateSeatChanges(List<List<char>> grid)
    {
        int seatsChanged = 0;

        List<List<char>> gridCopy = CopyGrid(grid);

        for (int row = 0; row < grid.Count; row++)
        {
            for (int column = 0; column < grid[row].Count; column++)
            {
                if (gridCopy[row][column] == 'L')
                {
                    // See how many occupied seats are around this seat
                    int neighboursCount = CountNeighbours(gridCopy, row, column, '#');
                    if (neighboursCount == 0)
                    {
                        grid[row][column] = '#';
                        seatsChanged++;
                    }
                }
                else if (gridCopy[row][column] == '#')
                {
                    // See how many occupied seats are around this seat
                    int neighboursCount = CountNeighbours(gridCopy, row, column, '#');
                    if (neighboursCount >= 4)
                    {
                        grid[row][column] = 'L';
                        seatsChanged++;
                    }
                }
            }
        }

        return seatsChanged;
    }

    static int CountNeighbours(List<List<char>> grid, int row, int column, char neighbour)
    {
        int neighbours = 0;

        // NW
        if (column - 1 >= 0 && row - 1 >= 0 && grid[row - 1][column - 1] == '#')
        {
            neighbours++;
        }
        // N
        if (row - 1 >= 0 && grid[row - 1][column] == '#')
        {
            neighbours++;
        }
        // NE
        if (column + 1 < grid[row].Count && row - 1 >= 0 && grid[row - 1][column + 1] == '#')
        {
            neighbours++;
        }
        // E
        if (column + 1 < grid[row].Count && grid[row][column + 1] == '#')
        {
            neighbours++;
        }
        // SE
        if (column + 1 < grid[row].Count && row + 1 < grid.Count && grid[row + 1][column + 1] == '#')
        {
            neighbours++;
        }
        // S
        if (row + 1 < grid.Count && grid[row + 1][column] == '#')
        {
            neighbours++;
        }
        // SW
        if (column - 1 >= 0 && row + 1 < grid.Count && grid[row + 1][column - 1] == '#')
        {
            neighbours++;
        }
        // W
        if (column - 1 >= 0 && grid[row][column - 1] == '#')
        {
            neighbours++;
        }

        return neighbours;
    }

    static int CountOccurences(List<List<char>> grid, char value)
    {
        int total = 0;

        for (int row = 0; row < grid.Count; row++)
        {
            for (int column = 0; column < grid[row].Count; column++)
            {
                if (grid[row][column] == value)
                {
                    total++;
                }
            }
        }

        return total;
    }
}