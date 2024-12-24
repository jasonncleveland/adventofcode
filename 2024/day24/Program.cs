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

        (Dictionary<string, long> wires, Queue<(string left, string operation, string right, string output)> instructions) = ParseInput(lines);

        while (instructions.Count > 0)
        {
            (string left, string operation, string right, string output) instruction = instructions.Dequeue();

            if (!wires.ContainsKey(instruction.left) || !wires.ContainsKey(instruction.right))
            {
                instructions.Enqueue(instruction);
                continue;
            }

            switch (instruction.operation)
            {
                case "AND":
                    wires[instruction.output] = wires[instruction.left] & wires[instruction.right];
                    break;
                case "OR":
                    wires[instruction.output] = wires[instruction.left] | wires[instruction.right];
                    break;
                case "XOR":
                    wires[instruction.output] = wires[instruction.left] ^ wires[instruction.right];
                    break;
                default:
                    throw new Exception($"Invalid operation received: {instruction.operation}");
            }
        }

        return GetWireValue(wires, 'z');
    }

    static long SolvePart2(string[] lines)
    {
        long total = 0;

        // TODO: Implement logic to solve part 2

        return total;
    }

    static (Dictionary<string, long>, Queue<(string left, string operation, string right, string output)>) ParseInput(string[] lines)
    {
        Dictionary<string, long> wires = new();
        Queue<(string left, string operation, string right, string output)> instructions = new();

        bool readingInstructons = false;
        foreach (string line in lines)
        {
            if (string.IsNullOrEmpty(line))
            {
                readingInstructons = true;
                continue;
            }

            if (readingInstructons)
            {
                string[] lineParts = line.Split(" -> ");
                string[] instructionParts = lineParts[0].Split(' ');
                string output = lineParts[1];
                string operation = instructionParts[1];
                string left = instructionParts[0];
                string right = instructionParts[2];
                instructions.Enqueue((left, operation, right, output));
            }
            else
            {
                string[] lineParts = line.Split(':');
                string name = lineParts[0];
                long value = long.Parse(lineParts[1]);
                wires[name] = value;
            }
        }

        return (wires, instructions);
    }

    static long GetWireValue(Dictionary<string, long> wires, char group)
    {
        long total = 0;

        for (long n = 0;; n++)
        {
            string key = $"{group}{n.ToString("D2")}";
            if (wires.ContainsKey(key))
            {
                long value = wires[key];
                total += (long) Math.Pow(2, n) * value;
            }
            else
            {
                break;
            }
        }

        return total;
    }
}