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
            result = SimulateSeatChanges(grid, 4);
        } while (result > 0);

        return CountOccurences(grid, '#');
    }

    static long SolvePart2(string[] lines)
    {
        List<List<char>> grid = ParseInput(lines);

        int result;
        do
        {
            result = SimulateSeatChanges(grid, 5, true);
        } while (result > 0);

        return CountOccurences(grid, '#');
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

    static int SimulateSeatChanges(List<List<char>> grid, int leniency, bool recurse = false)
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
                    int neighboursCount = CountNeighbours(gridCopy, row, column, recurse);
                    if (neighboursCount == 0)
                    {
                        grid[row][column] = '#';
                        seatsChanged++;
                    }
                }
                else if (gridCopy[row][column] == '#')
                {
                    // See how many occupied seats are around this seat
                    int neighboursCount = CountNeighbours(gridCopy, row, column, recurse);
                    if (neighboursCount >= leniency)
                    {
                        grid[row][column] = 'L';
                        seatsChanged++;
                    }
                }
            }
        }

        return seatsChanged;
    }

    static int CountNeighbours(List<List<char>> grid, int row, int column, bool recurse)
    {
        int neighbours = 0;

        // NW
        neighbours += CountNeighboursRec(grid, row, column, (-1, -1), recurse);
        // N
        neighbours += CountNeighboursRec(grid, row, column, (-1, 0), recurse);
        // NE
        neighbours += CountNeighboursRec(grid, row, column, (-1, 1), recurse);
        // W
        neighbours += CountNeighboursRec(grid, row, column, (0, -1), recurse);
        // E
        neighbours += CountNeighboursRec(grid, row, column, (0, 1), recurse);
        // SW
        neighbours += CountNeighboursRec(grid, row, column, (1, -1), recurse);
        // S
        neighbours += CountNeighboursRec(grid, row, column, (1, 0), recurse);
        // SE
        neighbours += CountNeighboursRec(grid, row, column, (1, 1), recurse);

        return neighbours;
    }

    static int CountNeighboursRec(List<List<char>> grid, int row, int column, (int row, int column) delta, bool recurse = false)
    {
        int nextRow = row + delta.row;
        int nextColumn = column + delta.column;

        if (nextRow < 0 || nextRow >= grid.Count || nextColumn < 0 || nextColumn >= grid[nextRow].Count)
        {
            return 0;
        }

        if (grid[nextRow][nextColumn] != '.')
        {
            return grid[nextRow][nextColumn] == '#' ? 1 : 0;
        }

        if (!recurse)
        {
            return 0;
        }
        return CountNeighboursRec(grid, nextRow, nextColumn, delta, recurse);
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