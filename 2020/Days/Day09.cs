using System;
using System.Collections.Generic;
using System.Linq;

using AdventOfCode.Shared.Solver;

using Microsoft.Extensions.Logging;

namespace AdventOfCode.Y2020.Days;

internal sealed class Day09 : AbstractDaySolver<IReadOnlyList<long>>
{
    protected override IReadOnlyList<long> ParseInput(ILogger logger, string fileContents)
    {
        return fileContents
            .Split("\n", StringSplitOptions.RemoveEmptyEntries)
            .Select(long.Parse)
            .ToList()
            .AsReadOnly();
    }

    protected override string SolvePart1(ILogger logger, IReadOnlyList<long> numbers)
    {
        return FindInvalidNumber(numbers, 25).ToString();
    }

    protected override string SolvePart2(ILogger logger, IReadOnlyList<long> numbers)
    {
        long invalidNumber = FindInvalidNumber(numbers, 25);

        for (int i = 0; i < numbers.Count; i++)
        {
            long number = numbers[i];
            long total = number;

            List<long> sequence = new();
            sequence.Add(number);

            bool isValid = false;
            for (int j = i + 1; j < numbers.Count; j++)
            {
                number = numbers[j];
                total += number;

                sequence.Add(number);

                if (total > invalidNumber)
                {
                    break;
                }

                if (total == invalidNumber)
                {
                    isValid = true;
                    break;
                }
            }

            if (isValid)
            {
                sequence.Sort();
                return (sequence[0] + sequence[^1]).ToString();
            }
        }

        throw new Exception("Found no valid sequence of numbers");
    }

    private static long FindInvalidNumber(IReadOnlyList<long> numbers, int history)
    {
        List<long> previousNumbers = new();

        foreach (long number in numbers)
        {
            if (previousNumbers.Count == history)
            {
                bool isValid = false;
                foreach (long previousNumber in previousNumbers)
                {
                    long diff = number - previousNumber;
                    if (diff == previousNumber)
                    {
                        // Ignore the same value
                        continue;
                    }

                    // Check if the difference exists in the list
                    long diffIndex = previousNumbers.IndexOf(diff);
                    if (diffIndex > -1)
                    {
                        isValid = true;
                        break;
                    }
                }

                if (!isValid)
                {
                    return number;
                }

                // Remove the oldest number from the list
                previousNumbers.RemoveAt(0);
            }
            // Add the new number to the end of the list
            previousNumbers.Add(number);
        }

        throw new Exception("Found no invalid number");
    }
}