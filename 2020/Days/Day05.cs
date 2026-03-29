using System;
using System.Collections.Generic;

using AdventOfCode.Shared.Solver;

using Microsoft.Extensions.Logging;

namespace AdventOfCode.Y2020.Days;

internal sealed class Day05 : AbstractDaySolver<IReadOnlyList<(int, int)>>
{
    protected override IReadOnlyList<(int, int)> ParseInput(ILogger logger, string fileContents)
    {
        var lines = fileContents.Split("\n", StringSplitOptions.RemoveEmptyEntries);

        var boardingPasses = new List<(int, int)>();

        foreach (var line in lines)
        {
            int columnCount = 128;
            int columnLowerBound = 0;
            int columnUpperBound = columnCount - 1;
            int rowCount = 8;
            int rowLowerBound = 0;
            int rowUpperBound = rowCount - 1;
            foreach (char character in line)
            {
                switch (character)
                {
                    case 'F':
                        // Take the lower half of the column range
                        columnCount /= 2;
                        columnUpperBound -= columnCount;
                        break;
                    case 'B':
                        // Take the upper half of the column range
                        columnCount /= 2;
                        columnLowerBound += columnCount;
                        break;
                    case 'L':
                        // Take the lower half of the row range
                        rowCount /= 2;
                        rowUpperBound -= rowCount;
                        break;
                    case 'R':
                        // Take the upper half of the row range
                        rowCount /= 2;
                        rowLowerBound += rowCount;
                        break;
                    default:
                        throw new Exception($"Invalid letter received: '{character}'");
                }
            }

            // The upper and lower bounds must be equal at the end of the processing
            if (columnLowerBound != columnUpperBound || rowLowerBound != rowUpperBound)
            {
                throw new Exception($"Error processing boarding pass {line}. First 5 characters must be B or F and final 3 must be L or R");
            }
            boardingPasses.Add((columnLowerBound, rowLowerBound));
        }

        return boardingPasses;
    }

    protected override string SolvePart1(ILogger logger, IReadOnlyList<(int, int)> boardingPasses)
    {
        long max = long.MinValue;

        foreach ((int seatColumn, int seatRow) in boardingPasses)
        {
            long seatId = seatColumn * 8 + seatRow;
            if (seatId > max)
            {
                max = seatId;
            }
        }

        return max.ToString();
    }

    protected override string SolvePart2(ILogger logger, IReadOnlyList<(int, int)> boardingPasses)
    {
        long min = long.MaxValue;
        long max = long.MinValue;
        long foundSeatId = 0;

        List<long> seatIds = new();
        foreach ((int seatColumn, int seatRow) in boardingPasses)
        {
            long seatId = seatColumn * 8 + seatRow;
            seatIds.Add(seatId);
            if (seatId > max) max = seatId;
            if (seatId < min) min = seatId;
        }

        for (long i = min; i < max; i++)
        {
            if (seatIds.Contains(i - 1) && !seatIds.Contains(i) && seatIds.Contains(i + 1))
            {
                foundSeatId = i;
            }
        }

        return foundSeatId.ToString();
    }
}