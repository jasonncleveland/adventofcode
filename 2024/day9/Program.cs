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
        List<int> output = new();
        int fileId = 0;
        for (int i = 0; i < line.Length; i++)
        {
            bool isFile = i % 2 == 0;
            int number = line[i] - '0';
            for (int j = 0; j < number; j++)
            {
                if (isFile)
                {
                    output.Add(fileId);
                }
                else
                {
                    output.Add(-1);
                }
            }
            if (isFile)
            {
                fileId++;
            }
        }

        int dotIndex = output.IndexOf(-1);
        if (dotIndex >= 0)
        {
            for (int i = output.Count - 1; i >= 0; i--)
            {
                if (output[i] == -1)
                {
                    continue;
                }
                if (i <= dotIndex)
                {
                    break;
                }

                if (output[i] != -1)
                {
                    output[dotIndex] = output[i];
                    output[i] = -1;
                    dotIndex = output.IndexOf(-1);
                }

                if (dotIndex < 0)
                {
                    break;
                }
            }
        }

        long checksum = 0;
        for (int i = 0; i < output.Count; i++)
        {
            if (output[i] != -1)
            {
                checksum += i * output[i];
            }
        }
        return checksum;
    }

    static long SolvePart2(string[] lines)
    {
        long total = 0;

        // TODO: Implement logic to solve part 2

        return total;
    }
}