using System;
using System.Collections.Generic;
using System.Linq;

using AdventOfCode.Shared.Solver;

using Microsoft.Extensions.Logging;

namespace AdventOfCode.Y2020.Days;

internal sealed class Day06 : AbstractDaySolver<IReadOnlyList<IReadOnlyList<IReadOnlyList<char>>>>
{
    protected override IReadOnlyList<IReadOnlyList<IReadOnlyList<char>>> ParseInput(ILogger logger, string fileContents)
    {
        var formResponses = fileContents.Split("\n\n", StringSplitOptions.RemoveEmptyEntries);

        var groups = new List<IReadOnlyList<IReadOnlyList<char>>>();

        foreach (var formResponse in formResponses)
        {
            var answers = formResponse.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            groups.Add(answers.Select(answer => answer.ToCharArray().ToList().AsReadOnly()).ToList().AsReadOnly());
        }

        return groups.AsReadOnly();
    }

    protected override string SolvePart1(ILogger logger, IReadOnlyList<IReadOnlyList<IReadOnlyList<char>>> groups)
    {
        long total = 0;

        foreach (var group in groups)
        {
            HashSet<char> uniqueAnswers = new();
            foreach (var answers in group)
            {
                foreach (char answer in answers)
                {
                    uniqueAnswers.Add(answer);
                }
            }
            total += uniqueAnswers.Count;
        }

        return total.ToString();
    }

    protected override string SolvePart2(ILogger logger, IReadOnlyList<IReadOnlyList<IReadOnlyList<char>>> groups)
    {
        long total = 0;

        foreach (var group in groups)
        {
            int commonAnswersCount = 0;
            Dictionary<char, int> occurences = new();
            foreach (var answers in group)
            {
                foreach (char answer in answers)
                {
                    occurences.TryAdd(answer, 0);
                    occurences[answer] += 1;
                }
            }
            foreach ((char character, int foundCount) in occurences)
            {
                if (foundCount == group.Count)
                {
                    commonAnswersCount++;
                }
            }
            total += commonAnswersCount;
        }

        return total.ToString();
    }
}