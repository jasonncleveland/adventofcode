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
                long part1 = SolvePart1Rev(lines);
                part1Timer.Stop();
                Console.WriteLine($"Part 1: {part1} ({part1Timer.Elapsed.TotalMilliseconds} ms)");

                Stopwatch part2Timer = new Stopwatch();
                part2Timer.Start();
                long part2 = SolvePart2Rev(lines);
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

        List<(long, List<long>)> equations = parseInput(lines);

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

    static long SolvePart1Rev(string[] lines)
    {
        long total = 0;

        List<(long, List<long>)> equations = parseInput(lines);

        foreach ((long testValue, List<long> numbers) in equations)
        {
            numbers.Reverse();
            bool result = validateEquationRevRec(testValue, numbers);
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

        List<(long, List<long>)> equations = parseInput(lines);

        foreach ((long testValue, List<long> numbers) in equations)
        {
            bool result = validateEquationRec(testValue, numbers, true);
            if (result)
            {
                total += testValue;
            }
        }

        return total;
    }

    static long SolvePart2Rev(string[] lines)
    {
        long total = 0;

        List<(long, List<long>)> equations = parseInput(lines);

        foreach ((long testValue, List<long> numbers) in equations)
        {
            numbers.Reverse();
            bool result = validateEquationRevRec(testValue, numbers, true);
            if (result)
            {
                total += testValue;
            }
        }

        return total;
    }

    static List<(long, List<long>)> parseInput(string[] lines)
    {
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

        return equations;
    }

    static bool validateEquationRec(long testValue, List<long> numbers, bool allowConcat = false, long workingTotal = 0)
    {
        List<long> numbersCopy = new(numbers);

        if (numbersCopy.Count == 0)
        {
            return workingTotal == testValue;
        }
        if (workingTotal > testValue)
        {
            return false;
        }

        long number = numbersCopy[0];
        numbersCopy.RemoveAt(0);

        if (workingTotal == 0)
        {
            // The first number does not have an operation
            return validateEquationRec(testValue, new List<long>(numbersCopy), allowConcat, number);
        }
        else
        {
            // Attempt to add the numbers (+)
            bool isAddValid = validateEquationRec(testValue, new List<long>(numbersCopy), allowConcat, workingTotal + number);
            if (isAddValid)
            {
                return true;
            }

            // Attempt to multiple the numbers (*)
            bool isMultiplyValid = validateEquationRec(testValue, new List<long>(numbersCopy), allowConcat, workingTotal * number);
            if (isMultiplyValid)
            {
                return true;
            }

            // Attempt to concatenate the numbers (||)
            if (allowConcat)
            {
                long concatenatedNumber = concatNumbers(workingTotal, number);
                bool isConcatValid = validateEquationRec(testValue, new List<long>(numbersCopy), allowConcat, concatenatedNumber);
                if (isConcatValid)
                {
                    return true;
                }
            }

            return false;
        }
    }

    static bool validateEquationRevRec(long testValue, List<long> numbers, bool allowConcat = false)
    {
        List<long> numbersCopy = new(numbers);

        if (numbersCopy.Count == 0)
        {
            return testValue == 0;
        }

        long number = numbersCopy[0];
        numbersCopy.RemoveAt(0);

        if (testValue - number >= 0)
        {
            // Attempt to subtract the numbers to determine if an add operation is valid (+)
            bool isAddValid = validateEquationRevRec(testValue - number, new List<long>(numbersCopy), allowConcat);
            if (isAddValid)
            {
                return true;
            }
        }
        if (testValue % number == 0)
        {
            // Attempt to divide the numbers to determine if a multiply operation is valid (*)
            bool isMultiplyValid = validateEquationRevRec(testValue / number, new List<long>(numbersCopy), allowConcat);
            if (isMultiplyValid)
            {
                return true;
            }
        }
        if (allowConcat && testValue.ToString().EndsWith(number.ToString()))
        {
            // Attempt to de-concatenate the numbers to determine if a concate operation is valid (||)
            int digitCount = number.ToString().Length;
            string stringNumber = testValue.ToString();
            long deconcatenatedNumber = long.Parse(stringNumber.Remove(stringNumber.Length - digitCount, digitCount));
            bool isConcatValid = validateEquationRevRec(deconcatenatedNumber, new List<long>(numbersCopy), allowConcat);
            if (isConcatValid)
            {
                return true;
            }
        }
        return false;
    }

    static long concatNumbers(long x, long y)
    {
        long pow = 10;
        while (y >= pow)
        {
            pow *= 10;
        }
        return x * pow + y;
    }
}