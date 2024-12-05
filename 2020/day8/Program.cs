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
        List<(string operation, int offset)> operations = parseInput(lines);
        Dictionary<int, List<(string, int)>> mapping = new();
        Dictionary<int, List<(string, int)>> flippedMapping = new();

        // Create mapping of instructions to output
        for (int instructionPointer = 0; instructionPointer < operations.Count; instructionPointer++)
        {
            (string operation, int offset) = operations[instructionPointer];

            int nextInstructionPointer = instructionPointer;
            int flippedNextInstructionPointer = instructionPointer;
            string flippedOperation = operation;
            switch (operation)
            {
                case "acc":
                    nextInstructionPointer += 1;
                    flippedNextInstructionPointer += 1;
                    break;
                case "jmp":
                    nextInstructionPointer += offset;
                    flippedNextInstructionPointer += 1;
                    flippedOperation = "nop";
                    break;
                case "nop":
                    nextInstructionPointer += 1;
                    flippedNextInstructionPointer += offset;
                    flippedOperation = "jmp";
                    break;
                default:
                    throw new Exception($"Invalid operation received: {operation}");
            }
            if (!mapping.ContainsKey(nextInstructionPointer))
            {
                mapping.Add(nextInstructionPointer, []);
            }
            if (!flippedMapping.ContainsKey(flippedNextInstructionPointer))
            {
                flippedMapping.Add(flippedNextInstructionPointer, []);
            }
            // Store the instruction and flipped instruction that was executed to reach the current instruction pointer
            mapping[nextInstructionPointer].Add((operation, instructionPointer));
            flippedMapping[flippedNextInstructionPointer].Add((flippedOperation, instructionPointer));
        }


        // Find the operation to flip by working backwards from the last instruction
        int reverseInstructionPointer = operations.Count;
        HashSet<int> checkedInstructions = new();
        checkedInstructions.Add(reverseInstructionPointer);
        (string operation, int instructionPointer) instructionToFlip;
        bool success = findOperationToFlipRec(mapping, flippedMapping, reverseInstructionPointer, checkedInstructions, out instructionToFlip);

        // Change the correct instruction
        operations[instructionToFlip.instructionPointer] = (instructionToFlip.operation, operations[instructionToFlip.instructionPointer].offset);

        // Run the instructions with the correct operation flipped
        return executeProgram(operations, true);
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

    static long executeProgram(List<(string, int)> operations, bool loopError = false)
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
                // Stop or throw an error if an infinite loop is detected
                if (loopError)
                {
                    throw new Exception($"Infinite loop detected at instruction: {operation} {(offset < 0 ? "" : "+")}{offset}, instruction pointer: {instructionPointer}");
                }
                break;
            }
            if (instructionPointer < 0 || instructionPointer >= operations.Count)
            {
                // Stop if the instruction pointer is outside the valid range
                break;
            }
            checkedInstructions.Add(instructionPointer);
        }

        return accumulator;
    }

    /**
     * Recursively traverse from the final operation to the start to find the correct operation flow.
     * Multiple instructions can result in the same instruction pointer so this function will search all possible paths.
     *
     * The function will return true or false depending on whether the operation succeds.
     * If the operation succeeds, the operation to flip will be stored in the output parameter 'instructionToFlip'.
     *
     * A HashSet checkedInstructions is used to ensure the same instruction is not visited multiple times.
     * This prevents an infinite loop from occuring.
     */
    static bool findOperationToFlipRec(Dictionary<int, List<(string, int)>> mapping, Dictionary<int, List<(string, int)>> flippedMapping, int instructionPointer, HashSet<int> checkedInstructions, out (string, int) instructionToFlip, int flippedOperations = 0)
    {
        instructionToFlip = (null, 0);

        if (instructionPointer == 0)
        {
            // We have reached the start of the program so the current path is valid!
            return true;
        }

        // Test both the normal operation and its flipped counterpart
        if (mapping.ContainsKey(instructionPointer))
        {
            foreach ((string operation, int nextInstructionPointer) instruction in mapping[instructionPointer])
            {
                if (checkedInstructions.Contains(instruction.nextInstructionPointer))
                {
                    // Stop if we find an infinite loop
                    continue;
                }
                HashSet<int> checkedInstructionsCopy = new(checkedInstructions);
                checkedInstructionsCopy.Add(instruction.nextInstructionPointer);
                bool success = findOperationToFlipRec(mapping, flippedMapping, instruction.nextInstructionPointer, checkedInstructionsCopy, out instructionToFlip, flippedOperations);
                if (success)
                {
                    return true;
                }
            }
        }
        // Check the flipped operations only if we have not flipped any operation yet
        if (flippedOperations < 1 && flippedMapping.ContainsKey(instructionPointer))
        {
            foreach ((string operation, int nextInstructionPointer) instruction in flippedMapping[instructionPointer])
            {
                if (checkedInstructions.Contains(instruction.nextInstructionPointer))
                {
                    // Stop if we find an infinite loop
                    continue;
                }
                HashSet<int> checkedInstructionsCopy = new(checkedInstructions);
                checkedInstructionsCopy.Add(instruction.nextInstructionPointer);
                bool success = findOperationToFlipRec(mapping, flippedMapping, instruction.nextInstructionPointer, checkedInstructionsCopy, out instructionToFlip, flippedOperations + 1);
                if (success)
                {
                    instructionToFlip = instruction;
                    return true;
                }
            }
        }
        return false;
    }
}