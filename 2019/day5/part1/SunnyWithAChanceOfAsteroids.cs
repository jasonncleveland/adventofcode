using System;
using System.IO;

class SunnyWithAChanceOfAsteroids
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        string[] lines = File.ReadAllLines(fileName);

        foreach (string line in lines)
        {
          int[] memory = Array.ConvertAll<string, int>(line.Split(','), operation => int.Parse(operation));

          int instructionPointer = 0;
          int inputValue = 1;
          int outputValue = 0;
          do
          {
            instructionPointer = processInstruction(memory, instructionPointer, inputValue, ref outputValue);
          } while (instructionPointer != -1);
          Console.WriteLine($"Final Output Value: {outputValue}");
        }
      }
    }
  }

  static int processInstruction(int[] memory, int instructionPointer, int inputValue, ref int outputValue)
  {
    int instruction = memory[instructionPointer];
    if (instruction == 99) return -1;

    // Parse the instruction to get the op code and modes
    int opCode = instruction % 100;
    int parameter1Mode = instruction % 1000 / 100;
    int parameter2Mode = instruction % 10000 / 1000;
    int parameter3Mode = instruction % 100000 / 10000;

    int parameter1Address, parameter2Address, parameter3Address;

    switch (opCode)
    {
      case 1:
        parameter1Address = parameter1Mode == 0 ? memory[instructionPointer + 1] : instructionPointer + 1;
        parameter2Address = parameter2Mode == 0 ? memory[instructionPointer + 2] : instructionPointer + 2;
        parameter3Address = parameter3Mode == 0 ? memory[instructionPointer + 3] : instructionPointer + 3;
        memory[parameter3Address] = memory[parameter1Address] + memory[parameter2Address];
        instructionPointer += 4;
        break;
      case 2:
        parameter1Address = parameter1Mode == 0 ? memory[instructionPointer + 1] : instructionPointer + 1;
        parameter2Address = parameter2Mode == 0 ? memory[instructionPointer + 2] : instructionPointer + 2;
        parameter3Address = parameter3Mode == 0 ? memory[instructionPointer + 3] : instructionPointer + 3;
        memory[parameter3Address] = memory[parameter1Address] * memory[parameter2Address];
        instructionPointer += 4;
        break;
      case 3:
        parameter1Address = parameter1Mode == 0 ? memory[instructionPointer + 1] : instructionPointer + 1;
        memory[parameter1Address] = inputValue;
        instructionPointer += 2;
        break;
      case 4:
        parameter1Address = parameter1Mode == 0 ? memory[instructionPointer + 1] : instructionPointer + 1;
        outputValue = memory[parameter1Address];
        instructionPointer += 2;
        break;
      default:
        Console.WriteLine($"Invalid code {opCode}");
        instructionPointer = -1;
        break;
    }
    return instructionPointer;
  }
}
