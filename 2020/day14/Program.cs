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

        Dictionary<int, long> memory = new();
        List<(int index, char value)> bitmasks = new();

        foreach (string line in lines)
        {
            string[] lineParts = line.Split(" = ");
            if (line.StartsWith("mask"))
            {
                bitmasks = new();
                string mask = lineParts[1];
                for (int i = 0; i < mask.Length; i++)
                {
                    if (mask[i] == '1' || mask[i] == '0')
                    {
                        bitmasks.Add((i, mask[i]));
                    }
                }
            }
            else
            {
                int address = int.Parse(lineParts[0].Substring(4, lineParts[0].Length - 5));
                long value = long.Parse(lineParts[1]);
                char[] binary = Convert.ToString(value, 2).PadLeft(36, '0').ToCharArray();
                foreach ((int index, char value) bitmask in bitmasks)
                {
                    binary[bitmask.index] = bitmask.value;
                }
                long maskedValue = Convert.ToInt64(new string(binary), 2);
                memory[address] = maskedValue;
            }
        }

        foreach (long value in memory.Values)
        {
            total += value;
        }

        return total;
    }

    static long SolvePart2(string[] lines)
    {
        long total = 0;

        // TODO: Implement logic to solve part 2

        return total;
    }
}