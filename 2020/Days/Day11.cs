using System;
using System.Collections.Generic;
using System.Linq;

using AdventOfCode.Shared.Solver;

using Microsoft.Extensions.Logging;

namespace AdventOfCode.Y2020.Days;

internal sealed class Day11 : AbstractDaySolver<IReadOnlyList<IReadOnlyList<char>>>
{
    protected override IReadOnlyList<IReadOnlyList<char>> ParseInput(ILogger logger, string fileContents)
    {
        return fileContents
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.ToCharArray().ToList().AsReadOnly())
            .ToList()
            .AsReadOnly();
    }

    protected override string SolvePart1(ILogger logger, IReadOnlyList<IReadOnlyList<char>> grid)
    {
        List<List<char>> gridCopy = CopyGrid(grid);

        int result;
        do
        {
            result = SimulateSeatChanges(gridCopy, 4);
        } while (result > 0);

        return CountOccurrences(gridCopy, '#').ToString();
    }

    protected override string SolvePart2(ILogger logger, IReadOnlyList<IReadOnlyList<char>> grid)
    {
        List<List<char>> gridCopy = CopyGrid(grid);

        int result;
        do
        {
            result = SimulateSeatChanges(gridCopy, 5, true);
        } while (result > 0);

        return CountOccurrences(gridCopy, '#').ToString();
    }

    private static List<List<char>> CopyGrid(IReadOnlyList<IReadOnlyList<char>> grid)
    {
        List<List<char>> copy = new();

        foreach (var row in grid)
        {
            copy.Add(new(row));
        }

        return copy;
    }

    private static int SimulateSeatChanges(List<List<char>> grid, int leniency, bool recurse = false)
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

    private static int CountNeighbours(List<List<char>> grid, int row, int column, bool recurse)
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

    private static int CountNeighboursRec(List<List<char>> grid, int row, int column, (int row, int column) delta, bool recurse = false)
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

    private static int CountOccurrences(List<List<char>> grid, char value)
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