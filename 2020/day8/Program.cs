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
        List<(string, int)> operations = parseInput(lines);
        return executeProgram(operations);
    }

    static long SolvePart2(string[] lines)
    {
        long total = 0;

        // TODO: Implement logic to solve part 2

        return total;
    }

    static List<(string, int)> parseInput(string[] lines)
    {
        List<(string, int)> operations = new();

        foreach (string line in lines)
        {
            string[] lineParts = line.Split(' ');
            string operation = lineParts[0];
            int offset = int.Parse(lineParts[1]);
            operations.Add((operation, offset));
        }

        return operations;
    }

    static long executeProgram(List<(string, int)> operations)
    {
        long accumulator = 0;
        int instructionPointer = 0;

        HashSet<int> checkedInstructions = new();
        checkedInstructions.Add(instructionPointer);

        while (true)
        {
            (string operation, int offset) = operations[instructionPointer];
            switch (operation)
            {
                case "acc":
                    accumulator += offset;
                    instructionPointer += 1;
                    break;
                case "jmp":
                    instructionPointer += offset;
                    break;
                case "nop":
                    instructionPointer += 1;
                    break;
                default:
                    throw new Exception($"Invalid operation received: {operation}");
            }

            if (checkedInstructions.Contains(instructionPointer))
            {
                break;
            }
            checkedInstructions.Add(instructionPointer);
        }

        return accumulator;
    }
}