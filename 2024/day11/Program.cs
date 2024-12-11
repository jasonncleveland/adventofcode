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
                long part1 = SolvePart1(lines[0]);
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

    static long SolvePart1(string line)
    {
        string[] lineParts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        long[] numbers = new long[lineParts.Length];
        for (int i = 0; i < lineParts.Length; i++)
        {
            numbers[i] = long.Parse(lineParts[i]);
        }
        
        int iterations = 25;
        while (iterations-- > 0)
        {
            List<long> newNumbers = new();
            for (int i = 0; i < numbers.Length; i++)
            {
                long number = numbers[i];
                long digits = getDigits(number);
                if (number == 0)
                {
                    newNumbers.Add(1);
                }
                else if (digits % 2 == 0)
                {
                    (long left, long right) split = splitNumber(number, digits);
                    newNumbers.Add(split.left);
                    newNumbers.Add(split.right);
                }
                else
                {
                    newNumbers.Add(number * 2024);
                }
            }
            numbers = newNumbers.ToArray();
        }
        
        return numbers.Length;
    }

    static long SolvePart2(string[] lines)
    {
        long total = 0;

        // TODO: Implement logic to solve part 2

        return total;
    }

    static long getDigits(long number)
    {
        long digits = 1;
        long pow = 10;
        while (number >= pow)
        {
            pow *= 10;
            digits++;
        }
        return digits;
    }

    static (long, long) splitNumber(long number, long digits)
    {
        long splitPoint = (long) Math.Pow(10, digits / 2);
        return (number / splitPoint, number % splitPoint);
    }
}