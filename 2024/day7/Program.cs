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

        List<(long, List<long>)> equations = new();
        foreach (string line in lines)
        {
            string[] lineParts = line.Split(':');
            long testValue = long.Parse(lineParts[0]);
            List<long> numbers = new List<long>();
            foreach (string number in lineParts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries))
            {
                numbers.Add(long.Parse(number));
            }
            equations.Add((testValue, numbers));
        }

        foreach ((long testValue, List<long> numbers) in equations)
        {
            bool result = validateEquationRec(testValue, numbers);
            if (result)
            {
                total += testValue;
            }
        }

        return total;
    }

    static long SolvePart2(string[] lines)
    {
        long total = 0;

        // TODO: Implement logic to solve part 2

        return total;
    }

    static bool validateEquationRec(long testValue, List<long> numbers, long workingTotal = 0, string equation = "")
    {
        if (workingTotal == testValue && numbers.Count == 0)
        {
            return true;
        }
        if (numbers.Count == 0)
        {
            return false;
        }
        if (workingTotal > testValue)
        {
            return false;
        }

        long number = numbers[0];
        numbers.RemoveAt(0);

        bool isValid = false;
        if (workingTotal == 0)
        {
            // Attempt to add the numbers
            isValid = validateEquationRec(testValue, new List<long>(numbers), number, $"{number}");
        }
        else
        {
            // Attempt to add the numbers
            bool isAddValid = validateEquationRec(testValue, new List<long>(numbers), workingTotal + number, equation + $" + {number}");

            // Attempt to multiple the numbers
            bool isMultiplyValid = validateEquationRec(testValue, new List<long>(numbers), workingTotal * number, equation + $" * {number}");
            isValid = isAddValid || isMultiplyValid;
        }
        return isValid;
    }
}