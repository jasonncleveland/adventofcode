using System;
using System.Collections.Generic;
using System.Linq;

using AdventOfCode.Shared.Solver;

using Microsoft.Extensions.Logging;

namespace AdventOfCode.Y2020.Days;

internal sealed class Day13 : AbstractDaySolver<IReadOnlyList<string>>
{
    protected override IReadOnlyList<string> ParseInput(ILogger logger, string fileContents)
    {
        return fileContents
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .ToList()
            .AsReadOnly();
    }

    protected override string SolvePart1(ILogger logger, IReadOnlyList<string> lines)
    {
        int startTime = int.Parse(lines[0]);

        List<int> buses = new();
        foreach (string bus in lines[1].Split(","))
        {
            if (int.TryParse(bus, out int busNumber))
            {
                buses.Add(busNumber);
            }
        }

        for (int currentTime = startTime; ; currentTime++)
        {
            foreach (int bus in buses)
            {
                if (currentTime % bus == 0)
                {
                    return ((currentTime - startTime) * bus).ToString();
                }
            }
        }
    }

    protected override string SolvePart2(ILogger logger, IReadOnlyList<string> lines)
    {
        string[] lineParts = lines[1].Split(",");

        Queue<(int number, int offset)> buses = new();
        for (int i = 0; i < lineParts.Length; i++)
        {
            if (int.TryParse(lineParts[i], out int bus))
            {
                buses.Enqueue((bus, i));
            }
        }

        long offset = 1;
        (int number, int offset) currentBus = buses.Dequeue();
        for (long currentTime = currentBus.number; ; currentTime += offset)
        {
            if ((currentTime + currentBus.offset) % currentBus.number == 0)
            {
                offset *= currentBus.number;
                if (buses.Count == 0)
                {
                    return currentTime.ToString();
                }
                currentBus = buses.Dequeue();
            }
        }
    }
}