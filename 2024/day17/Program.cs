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
                string part1 = SolvePart1(lines);
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

    static string SolvePart1(string[] lines)
    {
        int registerA = int.Parse(lines[0].Split(':')[1].Trim());
        int registerB = int.Parse(lines[1].Split(':')[1].Trim());
        int registerC = int.Parse(lines[2].Split(':')[1].Trim());
        string program = lines[4].Split(':')[1].Trim();

        ThreeBitComputer computer = new ThreeBitComputer(registerA, registerB, registerC, program);
        computer.Process();

        return computer.GetOutput();
    }

    static long SolvePart2(string[] lines)
    {
        long total = 0;

        // TODO: Implement logic to solve part 2

        return total;
    }
}

class ThreeBitComputer
{
    private long RegisterA { get; set; }
    private long RegisterB { get; set; }
    private long RegisterC { get; set; }
    private List<long> Output { get; set; } = [];

    private List<int> _instructions;

    public ThreeBitComputer(int registerA, int registerB, int registerC, string program)
    {
        RegisterA = registerA;
        RegisterB = registerB;
        RegisterC = registerC;

        _instructions = new();
        foreach (string instruction in program.Split(','))
        {
            _instructions.Add(int.Parse(instruction));
        }
    }

    public string GetOutput()
    {
        return string.Join(",", Output);
    }

    public void Process()
    {
        long output;
        int instructionPointer = 0;
        while (instructionPointer >= 0)
        {
            (instructionPointer, output) = ProcessInstruction(instructionPointer);
            if (output >= 0)
            {
                Output.Add(output);
            }
        }
    }

    public (int instructionPointer, long output) ProcessInstruction(int instructionPointer = 0)
    {
        while (instructionPointer >= 0 && instructionPointer + 1 < _instructions.Count)
        {
            int opcode = _instructions[instructionPointer];
            int operand = _instructions[instructionPointer + 1];
            instructionPointer += 2;

            switch (opcode)
            {
                // adv
                case 0:
                {
                    // Division of A by 2^combo operand stored in A
                    long comboOperand = this._getComboOperand(operand);
                    long numerator = RegisterA;
                    long denominator = 1 << (int) comboOperand;
                    RegisterA = numerator / denominator;
                    break;
                }
                // bxl
                case 1:
                {
                    // Bitwise XOR B and operand
                    RegisterB = RegisterB ^ operand;
                    break;
                }
                // bst
                case 2:
                {
                    // Store combo operand mod 8 in B
                    long comboOperand = this._getComboOperand(operand);
                    RegisterB = comboOperand % 8;
                    break;
                }
                // jnz
                case 3:
                {
                    // Jump to operand value if register A is empty
                    if (RegisterA != 0)
                    {
                        instructionPointer = operand;
                    }
                    break;
                }
                // bxc
                case 4:
                {
                    // Bitwise XOR B and C
                    RegisterB = RegisterB ^ RegisterC;
                    break;
                }
                // out
                case 5:
                {
                    // Output combo operand
                    long comboOperand = this._getComboOperand(operand);
                    return (instructionPointer, comboOperand % 8);
                }
                // bdv
                case 6:
                {
                    // Division of A by 2^combo operand stored in B
                    long comboOperand = this._getComboOperand(operand);
                    long numerator = RegisterA;
                    long denominator = 1 << (int) comboOperand;
                    RegisterB = numerator / denominator;
                    break;
                }
                // cdv
                case 7:
                {
                    // Division of A by 2^combo operand stored in C
                    long comboOperand = this._getComboOperand(operand);
                    long numerator = RegisterA;
                    long denominator = 1 << (int) comboOperand;
                    RegisterC = numerator / denominator;
                    break;
                }
                default:
                    throw new Exception($"Invalid opcode found");
            }
        }
        return (-1, -1);
    }

    private long _getComboOperand(int operand)
    {
        switch (operand)
        {
            case 0:
            case 1:
            case 2:
            case 3:
                return operand;
            case 4:
                return RegisterA;
            case 5:
                return RegisterB;
            case 6:
                return RegisterC;
            default:
                throw new Exception($"Invalid operand {operand}");
        }
    }
}
