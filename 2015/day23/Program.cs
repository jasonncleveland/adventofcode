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
        List<Instruction> instructions = ParseInput(lines);

        return ProcessInstructions(instructions);
    }

    static long SolvePart2(string[] lines)
    {
        long total = 0;

        // TODO: Implement logic to solve part 2

        return total;
    }

    static List<Instruction> ParseInput(string[] lines)
    {
        List<Instruction> instructions = new();

        foreach (string line in lines)
        {
            string operation;
            char register = '\0';
            int offset = 0;

            string[] lineParts = line.Split(',');
            if (lineParts.Length > 1)
            {
                // We have an instruction, register, and offset
                string[] operationParts = lineParts[0].Split(' ');
                operation = operationParts[0];
                register = operationParts[1][0];
                offset = int.Parse(lineParts[1]);
            }
            else
            {
                // We only have an instruction and register/offset
                string[] operationParts = lineParts[0].Split(' ');
                operation = operationParts[0];
                if (!int.TryParse(operationParts[1], out offset))
                {
                    // We have a register
                    register = operationParts[1][0];
                }
            }

            instructions.Add(new Instruction(operation, register, offset));
        }

        return instructions;
    }

    static long ProcessInstructions(List<Instruction> instructions)
    {
        int instructionPointer = 0;

        Dictionary<char, long> registers = new()
        {
            { 'a', 0 },
            { 'b', 0 },
        };

        while (instructionPointer >= 0 && instructionPointer < instructions.Count)
        {
            Instruction instruction = instructions[instructionPointer];

            switch (instruction.Operation)
            {
                case "hlf":
                {
                    long registerValue = registers[instruction.Register];
                    registers[instruction.Register] = registerValue / 2;
                    break;
                }
                case "tpl":
                {
                    long registerValue = registers[instruction.Register];
                    registers[instruction.Register] = registerValue * 3;
                    break;
                }
                case "inc":
                {
                    long registerValue = registers[instruction.Register];
                    registers[instruction.Register] = registerValue + 1;
                    break;
                }
                case "jmp":
                {
                    instructionPointer += instruction.Offset;
                    continue;
                }
                case "jie":
                {
                    long registerValue = registers[instruction.Register];
                    if (registerValue % 2 == 0)
                    {
                        instructionPointer += instruction.Offset;
                        continue;
                    }
                    break;
                }
                case "jio":
                {
                    long registerValue = registers[instruction.Register];
                    if (registerValue == 1)
                    {
                        instructionPointer += instruction.Offset;
                        continue;
                    }
                    break;
                }
            }

            instructionPointer += 1;
        }

        return registers['b'];
    }
}

class Instruction(string operation, char register, int offset)
{
    public string Operation { get; } = operation;
    public char Register { get; } = register;
    public int Offset { get; } = offset;

    public override string ToString()
    {
        return $"Instruction: {Operation} | Register: {Register} | Offset: {Offset}";
    }
}