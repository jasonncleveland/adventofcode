using System;
using System.Collections.Generic;
using System.Linq;

using AdventOfCode.Shared.Solver;

using Microsoft.Extensions.Logging;

namespace AdventOfCode.Y2020.Days;

internal sealed class Day25 : AbstractDaySolver<IReadOnlyList<long>>
{
    protected override IReadOnlyList<long> ParseInput(ILogger logger, string fileContents)
    {
        return fileContents
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(long.Parse)
            .ToList()
            .AsReadOnly();
    }

    protected override string SolvePart1(ILogger logger, IReadOnlyList<long> input)
    {
        var subjectNumber = 7;

        var doorPublicKey = input[0];
        var cardPublicKey = input[^1];

        // Brute force the loop size
        var doorLoopSize = 1;
        var doorValue = 1;
        while (true)
        {
            doorValue *= subjectNumber;
            doorValue %= 20201227;
            if (doorValue == doorPublicKey)
            {
                break;
            }
            doorLoopSize++;
        }

        // Calculate the encryption key using the loop size
        long value = 1;
        for (var i = 0; i < doorLoopSize; i++)
        {
            value *= cardPublicKey;
            value %= 20201227;
        }

        return value.ToString();
    }

    protected override string SolvePart2(ILogger logger, IReadOnlyList<long> input)
    {
        return "Merry Christmas!";
    }
}