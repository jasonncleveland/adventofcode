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
            int history = args.Length > 1 ? int.Parse(args[1]) : 25;
            if (File.Exists(fileName))
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                string[] lines = File.ReadAllLines(fileName);
                stopWatch.Stop();
                Console.WriteLine($"File read ({stopWatch.Elapsed.TotalMilliseconds} ms)");

                Stopwatch part1Timer = new Stopwatch();
                part1Timer.Start();
                long part1 = SolvePart1(lines, history);
                part1Timer.Stop();
                Console.WriteLine($"Part 1: {part1} ({part1Timer.Elapsed.TotalMilliseconds} ms)");

                Stopwatch part2Timer = new Stopwatch();
                part2Timer.Start();
                long part2 = SolvePart2(lines, history);
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

    static long SolvePart1(string[] lines, int history)
    {
        List<long> numbers = ParseInput(lines);

        return FindInvalidNumber(numbers, history);
    }

    static long SolvePart2(string[] lines, int history)
    {
        List<long> numbers = ParseInput(lines);

        long invalidNumber = FindInvalidNumber(numbers, history);

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
                return sequence[0] + sequence[sequence.Count - 1];
            }
        }

        throw new Exception("Found no valid sequence of numbers");
    }

    static List<long> ParseInput(string[] lines)
    {
        List<long> numbers = new();

        foreach (string line in lines)
        {
            numbers.Add(long.Parse(line));
        }

        return numbers;
    }

    static long FindInvalidNumber(List<long> numbers, int history)
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