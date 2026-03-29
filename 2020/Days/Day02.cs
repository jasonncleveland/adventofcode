using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using AdventOfCode.Shared.Solver;

using Microsoft.Extensions.Logging;

namespace AdventOfCode.Y2020.Days;

internal sealed class Day02 : AbstractDaySolver<IReadOnlyList<string>>
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
        string pattern = @"(?<low>\d+)-(?<high>\d+) (?<expected>\w): (?<password>\w+)";
        var regex = new Regex(pattern, RegexOptions.Compiled);

        int validPasswordCount = 0;
        foreach (string line in lines)
        {
            Match match = regex.Match(line);

            int lowValue = int.Parse(match.Groups["low"].Value);
            int highValue = int.Parse(match.Groups["high"].Value);
            char expectedValue = char.Parse(match.Groups["expected"].Value);
            string password = match.Groups["password"].Value;

            int expectedValueCount = 0;
            foreach (char letter in password)
            {
                if (letter == expectedValue) expectedValueCount++;
            }

            if (expectedValueCount >= lowValue && expectedValueCount <= highValue)
            {
                validPasswordCount += 1;
            }
        }

        return validPasswordCount.ToString();
    }

    protected override string SolvePart2(ILogger logger, IReadOnlyList<string> lines)
    {
        string pattern = @"(?<first>\d+)-(?<second>\d+) (?<expected>\w): (?<password>\w+)";
        var regex = new Regex(pattern, RegexOptions.Compiled);

        int validPasswordCount = 0;
        foreach (string line in lines)
        {
            Match match = regex.Match(line);

            int firstPosition = int.Parse(match.Groups["first"].Value);
            int secondPosition = int.Parse(match.Groups["second"].Value);
            char expectedValue = char.Parse(match.Groups["expected"].Value);
            string password = match.Groups["password"].Value;

            bool valueInFirstPosition = password[firstPosition - 1] == expectedValue;
            bool valueInSecondPosition = password[secondPosition - 1] == expectedValue;
            if (valueInFirstPosition && !valueInSecondPosition || valueInSecondPosition && !valueInFirstPosition)
            {
                validPasswordCount += 1;
            }
        }

        return validPasswordCount.ToString();
    }
}