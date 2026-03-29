using System;
using System.Collections.Generic;
using System.Linq;

using AdventOfCode.Shared.Solver;

using Microsoft.Extensions.Logging;

namespace AdventOfCode.Y2020.Days;

internal sealed class Day15 : AbstractDaySolver<IReadOnlyList<int>>
{
    protected override IReadOnlyList<int> ParseInput(ILogger logger, string fileContents)
    {
        return fileContents
            .Split(",", StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse)
            .ToList()
            .AsReadOnly();
    }

    protected override string SolvePart1(ILogger logger, IReadOnlyList<int> numbers)
    {
        Dictionary<int, (int, int)> history = new();

        int turnNumber = 1;
        int lastSpokenNumber = 0;

        // Process starting numbers
        foreach (int number in numbers)
        {
            history[number] = (turnNumber, -1);
            lastSpokenNumber = number;
            turnNumber++;
        }

        // Continue until turn 2020 is reached
        for (; turnNumber <= 2020; turnNumber++)
        {
            (int, int) previous = history[lastSpokenNumber];
            if (previous.Item2 == -1)
            {
                lastSpokenNumber = 0;
            }
            else
            {
                lastSpokenNumber = previous.Item1 - previous.Item2;
            }

            if (!history.ContainsKey(lastSpokenNumber))
            {
                history[lastSpokenNumber] = (turnNumber, -1);
            }
            else
            {
                history[lastSpokenNumber] = (turnNumber, history[lastSpokenNumber].Item1);
            }
        }
        return lastSpokenNumber.ToString();
    }

    protected override string SolvePart2(ILogger logger, IReadOnlyList<int> numbers)
    {
        Dictionary<int, (int, int)> history = new();

        int turnNumber = 1;
        int lastSpokenNumber = 0;

        // Process starting numbers
        foreach (int number in numbers)
        {
            history[number] = (turnNumber, -1);
            lastSpokenNumber = number;
            turnNumber++;
        }

        // Continue until turn 30000000 is reached
        for (; turnNumber <= 30_000_000; turnNumber++)
        {
            (int, int) previous = history[lastSpokenNumber];
            if (previous.Item2 == -1)
            {
                lastSpokenNumber = 0;
            }
            else
            {
                lastSpokenNumber = previous.Item1 - previous.Item2;
            }

            if (!history.ContainsKey(lastSpokenNumber))
            {
                history[lastSpokenNumber] = (turnNumber, -1);
            }
            else
            {
                history[lastSpokenNumber] = (turnNumber, history[lastSpokenNumber].Item1);
            }
        }
        return lastSpokenNumber.ToString();
    }
}