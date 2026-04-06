using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using AdventOfCode.Shared.Solver;

using Microsoft.Extensions.Logging;

namespace AdventOfCode.Y2020.Days;

internal sealed class Day01 : AbstractDaySolver<IReadOnlyList<int>>
{
    protected override IReadOnlyList<int> ParseInput(ILogger logger, string fileContents)
    {
        return fileContents
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse)
            .ToList()
            .AsReadOnly();
    }

    protected override string SolvePart1(ILogger logger, IReadOnlyList<int> numbers)
    {
        foreach (var first in numbers)
        {
            foreach (var second in numbers)
            {
                if (first == second)
                {
                    continue;
                }

                if (first + second == 2020)
                {
                    return (first * second).ToString();
                }
            }
        }

        throw new UnreachableException();
    }

    protected override string SolvePart2(ILogger logger, IReadOnlyList<int> numbers)
    {
        foreach (var first in numbers)
        {
            foreach (var second in numbers)
            {
                foreach (var third in numbers)
                {
                    if (first == second || first == third || second == third)
                    {
                        continue;
                    }

                    if (first + second + third == 2020)
                    {
                        return (first * second * third).ToString();
                    }
                }
            }
        }

        throw new UnreachableException();
    }
}