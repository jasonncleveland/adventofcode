using System;
using System.Collections.Generic;
using System.Linq;

using AdventOfCode.Shared.Solver;

using Microsoft.Extensions.Logging;

namespace AdventOfCode.Y2020.Days;

internal sealed class Day03 : AbstractDaySolver<IReadOnlyList<string>>
{
    protected override IReadOnlyList<string> ParseInput(ILogger logger, string fileContents)
    {
        return fileContents
            .Split("\n", StringSplitOptions.RemoveEmptyEntries)
            .ToList()
            .AsReadOnly();
    }

    protected override string SolvePart1(ILogger logger, IReadOnlyList<string> lines)
    {
        int numTreesEncountered = 0;
        int x = 0;
        foreach (string line in lines)
        {
            char value = line[x % line.Length];
            if (value == '#')
            {
                numTreesEncountered += 1;
            }

            x += 3;
        }

        return numTreesEncountered.ToString();
    }

    protected override string SolvePart2(ILogger logger, IReadOnlyList<string> lines)
    {
        int[][] slopes = new int[][] { [1, 1], [3, 1], [5, 1], [7, 1], [1, 2] };

        long result = 1;
        foreach (int[] slope in slopes)
        {
            int xAdd = slope[0], yAdd = slope[1];

            int numTreesEncountered = 0;
            int x = 0;
            for (int y = 0; y < lines.Count; y += yAdd)
            {
                char value = lines[y][x % lines[y].Length];
                if (value == '#')
                {
                    numTreesEncountered += 1;
                }

                x += xAdd;
            }

            result *= numTreesEncountered;
        }

        return result.ToString();
    }
}