using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

public class Program
{
    static void Main(string[] args)
    {
        if (args.Length > 0)
        {
            string fileName = args[0];
            if (File.Exists(fileName))
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                string[] lines = File.ReadAllLines(fileName);
                stopWatch.Stop();
                Console.WriteLine($"File read ({stopWatch.Elapsed.TotalMilliseconds} ms)");

                Stopwatch part1Timer = new Stopwatch();
                part1Timer.Start();
                long part1 = SolvePart1(lines);
                part1Timer.Stop();
                Console.WriteLine($"Part 1: {part1} ({part1Timer.Elapsed.TotalMilliseconds} ms)");

                Stopwatch part2Timer = new Stopwatch();
                part2Timer.Start();
                long part2 = SolvePart2(lines);
                part2Timer.Stop();
                Console.WriteLine($"Part 2: {part2} ({part2Timer.Elapsed.TotalMilliseconds} ms)");
            }
            else
            {
                throw new ArgumentException("Invalid file name provided. Please provide a valid file name.");
            }
        }
        else
        {
            throw new ArgumentException("Input data file name not provided. Please provide the file name as an argument: dotnet run <file-name>");
        }
    }

    static long SolvePart1(string[] lines)
    {
        long total = 0;

        foreach (string line in lines)
        {
            total += EvaluateWithBracketsRec(line, EvaluateSimple);
        }

        return total;
    }

    static long SolvePart2(string[] lines)
    {
        long total = 0;

        foreach (string line in lines)
        {
            total += EvaluateWithBracketsRec(line, EvaluateAdvanced);
        }

        return total;
    }

    static long EvaluateWithBracketsRec(string input, Evaluate evaluateFunc)
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
                        openBracketCount--;
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

    delegate long Evaluate(string formula);

    static long EvaluateSimple(string input)
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

    static long EvaluateAdvanced(string input)
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