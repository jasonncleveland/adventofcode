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
            Evaluate(line, out long result);
            total += result;
        }

        return total;
    }

    static long SolvePart2(string[] lines)
    {
        long total = 0;

        foreach (string line in lines)
        {
            total += EvaluateWithBracketsRec(line);
        }

        return total;
    }

    static int Evaluate(string input, out long result, int i = 0, int level = 0)
    {
        bool inNumber = false;
        long previousNumber = -1;
        char op = '\0';
        Stack<char> stack = new();
        for (; i < input.Length; i++)
        {
            char character = input[i];

            if (character == '(')
            {
                i = Evaluate(input, out long number, i + 1, level + 1);
                if (previousNumber != -1 && op != '\0')
                {
                    switch (op)
                    {
                        case '+':
                            previousNumber += number;
                            break;
                        case '*' :
                            previousNumber *= number;
                            break;
                        default:
                            throw new Exception($"Invalid operator: {op}");
                    }
                    // Reset the operator
                    op = '\0';
                }
                else
                {
                    previousNumber = number;
                }
                continue;
            }

            if (character == ')')
            {
                if (inNumber)
                {
                    long number = GetNumber(stack);
                    if (previousNumber != -1 && op != '\0')
                    {
                        switch (op)
                        {
                            case '+':
                                previousNumber += number;
                                break;
                            case '*' :
                                previousNumber *= number;
                                break;
                            default:
                                throw new Exception($"Invalid operator: {op}");
                        }
                        // Reset the operator
                        op = '\0';
                    }
                    else
                    {
                        previousNumber = number;
                    }
                }
                inNumber = false;
                result = previousNumber;
                return i;
            }

            if (char.IsDigit(character))
            {
                inNumber = true;
                stack.Push(character);
            }

            if (char.IsWhiteSpace(character) || i == input.Length - 1)
            {
                if (inNumber)
                {
                    long number = GetNumber(stack);
                    if (previousNumber != -1 && op != '\0')
                    {
                        switch (op)
                        {
                            case '+':
                                previousNumber += number;
                                break;
                            case '*' :
                                previousNumber *= number;
                                break;
                            default:
                                throw new Exception($"Invalid operator: {op}");
                        }
                        // Reset the operator
                        op = '\0';
                    }
                    else
                    {
                        previousNumber = number;
                    }
                }
                inNumber = false;
            }

            if (character == '+' || character == '*')
            {
                op = character;
            }
        }

        result = previousNumber;
        return -1;
    }

    static long GetNumber(Stack<char> stack)
    {
        long total = 0;

        total += stack.Pop() - '0';

        int power = 1;
        while (stack.Count > 0)
        {
            total += (stack.Pop() - '0') * (long)Math.Pow(10, power);
            power++;
        }

        return total;
    }

    static long EvaluateWithBracketsRec(string input)
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
                long result = EvaluateWithBracketsRec(enclosedFormula);
                formulaParts.Add(result.ToString());
                i = closeBracketIndex + 1;
            }
        }

        return EvaluateAdvanced(string.Join(" ", formulaParts));
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