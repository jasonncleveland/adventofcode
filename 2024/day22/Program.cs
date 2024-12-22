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
            long secretNumber = long.Parse(line);
            for (int i = 0; i < 2000; i++)
            {
                secretNumber = CalculateNextSecretNumber(secretNumber);
            }
            total += secretNumber;
        }

        return total;
    }

    static long SolvePart2(string[] lines)
    {
        Dictionary<(long, long, long, long), long> sequences = new();

        foreach (string line in lines)
        {
            long secretNumber = long.Parse(line);
            long onesDigit = secretNumber % 10;
            (long, long, long, long) sequence = (0, 0, 0, 0);
            // Keep track of the sequences per monkey
            HashSet<(long, long, long, long)> foundSequences = new();
            for (int i = 0; i < 2000; i++)
            {
                secretNumber = CalculateNextSecretNumber(secretNumber);
                long priceDiff = secretNumber % 10 - onesDigit;
                sequence = (sequence.Item2, sequence.Item3, sequence.Item4, priceDiff);
                onesDigit = secretNumber % 10;
                if (i >= 3 && !foundSequences.Contains(sequence))
                {
                    // Store the value for the first time we see a given sequence
                    if (!sequences.ContainsKey(sequence))
                    {
                        sequences.Add(sequence, 0);
                    }
                    sequences[sequence] += onesDigit;
                    foundSequences.Add(sequence);
                }
            }
        }

        // Find the maximum number of bananas
        long maxBananas = long.MinValue;
        foreach (long bananaCount in sequences.Values)
        {
            if (bananaCount > maxBananas)
            {
                maxBananas = bananaCount;
            }
        }

        return maxBananas;
    }

    static long CalculateNextSecretNumber(long secretNumber)
    {
        long mixNumber;
        long moduleNumber = 16777216;

        // Calculate the result of multiplying the secret number by 64. Then, mix this result into the secret number. Finally, prune the secret number.
        mixNumber = secretNumber * 64;
        secretNumber = secretNumber ^ mixNumber;
        secretNumber = secretNumber % moduleNumber;
        // Calculate the result of dividing the secret number by 32. Round the result down to the nearest integer. Then, mix this result into the secret number. Finally, prune the secret number.
        mixNumber = secretNumber / 32;
        secretNumber = secretNumber ^ mixNumber;
        secretNumber = secretNumber % moduleNumber;
        // Calculate the result of multiplying the secret number by 2048. Then, mix this result into the secret number. Finally, prune the secret number.
        mixNumber = secretNumber * 2048;
        secretNumber = secretNumber ^ mixNumber;
        secretNumber = secretNumber % moduleNumber;

        return secretNumber;
    }
}