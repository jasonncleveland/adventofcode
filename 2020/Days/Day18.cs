using System;
using System.Collections.Generic;
using System.Linq;

using AdventOfCode.Shared.Solver;

using Microsoft.Extensions.Logging;

namespace AdventOfCode.Y2020.Days;

internal sealed class Day18 : AbstractDaySolver<IReadOnlyList<string>>
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
        long total = 0;

        foreach (string line in lines)
        {
            total += EvaluateWithBracketsRec(line, EvaluateSimple);
        }

        return total.ToString();
    }

    protected override string SolvePart2(ILogger logger, IReadOnlyList<string> lines)
    {
        long total = 0;

        foreach (string line in lines)
        {
            total += EvaluateWithBracketsRec(line, EvaluateAdvanced);
        }

        return total.ToString();
    }

    private static long EvaluateWithBracketsRec(string input, Evaluate evaluateFunc)
    {
        List<string> formulaParts = new();
        for (int i = 0; i < input.Length; i++)
        {
            char character = input[i];

            if (char.IsDigit(character))
            {
                formulaParts.Add(character.ToString());
            }

            if (character == '+' || character == '*')
            {
                formulaParts.Add(character.ToString());
            }

            if (character == '(')
            {
                int openBracketIndex = i;
                int closeBracketIndex = input.IndexOf(')', openBracketIndex);
                int openBracketCount = 0;
                do
                {
                    int nextOpenBracketIndex = input.IndexOf('(', openBracketIndex + 1);
                    int nextCloseBracketIndex = input.IndexOf(')', closeBracketIndex + 1);
                    if (nextOpenBracketIndex < 0)
                    {
                        break;
                    }

                    openBracketCount++;

                    if (closeBracketIndex < nextOpenBracketIndex)
                    {
                        break;
                    }

                    openBracketCount++;
                    openBracketIndex = nextOpenBracketIndex;
                    closeBracketIndex = nextCloseBracketIndex;
                } while (openBracketCount > 0);
                string enclosedFormula = input.Substring(i + 1, closeBracketIndex - 1 - i);
                long result = EvaluateWithBracketsRec(enclosedFormula, evaluateFunc);
                formulaParts.Add(result.ToString());
                i = closeBracketIndex + 1;
            }
        }

        return evaluateFunc(string.Join(" ", formulaParts));
    }

    private delegate long Evaluate(string formula);

    private static long EvaluateSimple(string input)
    {
        string[] formulaParts = input.Split(' ');

        long left = long.Parse(formulaParts[0]);

        for (int i = 2; i < formulaParts.Length; i += 2)
        {
            string op = formulaParts[i - 1];
            long right = long.Parse(formulaParts[i]);
            switch (op)
            {
                case "+":
                    left += right;
                    break;
                case "*":
                    left *= right;
                    break;
                default:
                    throw new Exception($"Invalid operator: {op}");
            }
        }

        return left;
    }

    private static long EvaluateAdvanced(string input)
    {
        string[] multiplicands = input.Split(" * ");

        long product = 1;

        foreach (string multiplicand in multiplicands)
        {
            string[] addends = multiplicand.Split(" + ");

            long sum = 0;

            foreach (string addend in addends)
            {
                sum += long.Parse(addend);
            }

            product *= sum;
        }

        return product;
    }
}