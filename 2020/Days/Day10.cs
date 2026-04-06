using System;
using System.Collections.Generic;
using System.Linq;

using AdventOfCode.Shared.Solver;

using Microsoft.Extensions.Logging;

namespace AdventOfCode.Y2020.Days;

internal sealed class Day10 : AbstractDaySolver<IReadOnlyList<int>>
{
    private static readonly Dictionary<(int, int), long> Cache = new();

    protected override IReadOnlyList<int> ParseInput(ILogger logger, string fileContents)
    {
        return fileContents
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse)
            .ToList()
            .AsReadOnly();
    }

    protected override string SolvePart1(ILogger logger, IReadOnlyList<int> input)
    {
        List<int> adapters = new(input);
        adapters.Sort();

        Dictionary<int, int> differences = new()
        {
            { 1, 0 },
            { 3, 1 },
        };

        int previousAdapter = 0;
        foreach (int adapter in adapters)
        {
            int diff = adapter - previousAdapter;
            previousAdapter = adapter;
            differences[diff] += 1;
        }

        return (differences[1] * differences[3]).ToString();
    }

    protected override string SolvePart2(ILogger logger, IReadOnlyList<int> input)
    {
        List<int> adapters = new(input);
        adapters.Sort();

        // Calculate and add the final adapter
        int deviceAdapter = adapters[^1] + 3;
        adapters.Add(deviceAdapter);

        return FindAdapterCombinationsRec(adapters).ToString();
    }

    private static long FindAdapterCombinationsRec(List<int> adapters, int previousAdapter = 0, int level = 0)
    {
        if (previousAdapter == adapters.Last())
        {
            return 1;
        }

        if (Cache.ContainsKey((previousAdapter, level)))
        {
            return Cache[(previousAdapter, level)];
        }

        IEnumerable<int> reachableAdapters = adapters.Where(adapter => adapter - previousAdapter >= 1 && adapter - previousAdapter <= 3);

        long combinations = 0;
        foreach (int adapter in reachableAdapters)
        {
            combinations += FindAdapterCombinationsRec(adapters, adapter, level + 1);
        }

        Cache[(previousAdapter, level)] = combinations;
        return combinations;
    }
}