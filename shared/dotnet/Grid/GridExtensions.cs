using System.Collections.Generic;

namespace AdventOfCode.Shared.Grid;

public static class GridExtensions
{
    public static void Flip<T>(this List<List<T>> grid)
    {
        foreach (var row in grid)
        {
            row.Reverse();
        }
    }

    public static void Rotate<T>(this List<List<T>> grid)
    {
        // Transpose
        for (var row = 0; row < grid.Count; row++)
        {
            for (var column = row + 1; column < grid[row].Count; column++)
            {
                (grid[row][column], grid[column][row]) = (grid[column][row], grid[row][column]);
            }
        }
        // Swap
        grid.Flip<T>();
    }
}