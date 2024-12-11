using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

public class Program
{
    static Dictionary<(long number, long iteration), long> cache = new();
    static long functionCalls = 0;
    static long cacheHits = 0;
    static long cacheMisses = 0;
    

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
                long part2 = SolvePart2(lines[0]);
                part2Timer.Stop();
                Console.WriteLine($"Part 2: {part2} ({part2Timer.Elapsed.TotalMilliseconds} ms)");

                Console.WriteLine($"Recursive function calls with cache: {functionCalls}. Cache hits: {cacheHits}, Cache miss: {cacheMisses}, Cache items: {cache.Keys.Count}");
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
        long total = 0;

        string[] lineParts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        long[] numbers = new long[lineParts.Length];
        for (int i = 0; i < lineParts.Length; i++)
        {
            numbers[i] = long.Parse(lineParts[i]);
        }

        int iterations = 25;
        foreach (long number in numbers)
        {
            total += countStonesRec(number, iterations);
        }

        return total;
    }

    static long SolvePart2(string line)
    {
        long total = 0;

        string[] lineParts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        long[] numbers = new long[lineParts.Length];
        for (int i = 0; i < lineParts.Length; i++)
        {
            numbers[i] = long.Parse(lineParts[i]);
        }

        long iterations = 75;
        foreach (long number in numbers)
        {
            total += countStonesRec(number, iterations);
        }
        return total;
    }

    static long countStonesRec(long number, long iterations)
    {
        functionCalls++;
        if (iterations == 0)
        {
            return 1;
        }

        if (cache.ContainsKey((number, iterations)))
        {
            cacheHits++;
            return cache[(number, iterations)];
        }
        cacheMisses++;

        long digits = getDigits(number);
        if (number == 0)
        {
            long result = countStonesRec(1, iterations - 1);
            cache.Add((number, iterations), result);
            return result;
        }
        else if (digits % 2 == 0)
        {
            (long left, long right) split = splitNumber(number, digits);
            long result = countStonesRec(split.left, iterations - 1) + countStonesRec(split.right, iterations - 1);
            cache.Add((number, iterations), result);
            return result;
        }
        else
        {
            long result = countStonesRec(number * 2024, iterations - 1);
            cache.Add((number, iterations), result);
            return result;
        }
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