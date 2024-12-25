using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

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
                string part2 = SolvePart2(lines);
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

    static string SolvePart2(string[] lines)
    {
        (Dictionary<string, long> wires, Queue<(string left, string operation, string right, string output)> instructions) = ParseInput(lines);

        List<string> invalidOutputs = new();
        foreach ((string left, string operation, string right, string output) in instructions)
        {
            string combinedInputNames = $"{left[0]}{right[0]}";
            // All outputs to z must be the result of an XOR operation to calculate the remainder
            // The z45 is the final carry so it does not follow the general rule and can be ignored
            if (output.StartsWith('z') && operation != "XOR" && output != "z45")
            {
                invalidOutputs.Add(output);
            }
            // Only XOR gates can output to z so any other operations must be invalid
            if (!output.StartsWith('z') && combinedInputNames != "xy" && combinedInputNames != "yx" && operation == "XOR")
            {
                invalidOutputs.Add(output);
            }

            // The first XOR is a half-adder so it only has one operation so ignore
            if ((combinedInputNames == "xy" || combinedInputNames == "yx") && left != "x00" && left != "y00" && right != "x00" && right != "y00")
            {
                List<(string, string, string, string)> foundInstructions = instructions.Where(i => (i.left == output || i.right == output)).ToList();
                // If the operation is XOR for a starting gate, there must be 2 subsequent XOR and AND operations using the output
                if (operation == "XOR" && foundInstructions.Count != 2)
                {
                    invalidOutputs.Add(output);
                }

                // If the operation is AND for a starting gate, there must be 1 subsequent OR operation using the output
                if (operation == "AND" && foundInstructions.Count != 1)
                {
                    invalidOutputs.Add(output);
                }
            }
        }

        // Sort the invalid outputs alphabetically
        invalidOutputs.Sort();
        return string.Join(",", invalidOutputs.Distinct());
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