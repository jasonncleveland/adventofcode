using System;
using System.IO;

class SunnyWithAChanceOfAsteroids
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      int inputValue = args.Length > 1 ? int.Parse(args[1]) : 5;
      if (File.Exists(fileName))
      {
        string[] lines = File.ReadAllLines(fileName);

        foreach (string line in lines)
        {
          int[] memory = Array.ConvertAll<string, int>(line.Split(','), operation => int.Parse(operation));

          int instructionPointer = 0;
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
        // Add
        parameter1Address = parameter1Mode == 0 ? memory[instructionPointer + 1] : instructionPointer + 1;
        parameter2Address = parameter2Mode == 0 ? memory[instructionPointer + 2] : instructionPointer + 2;
        parameter3Address = parameter3Mode == 0 ? memory[instructionPointer + 3] : instructionPointer + 3;
        Console.WriteLine($"> Computing {memory[parameter1Address]} + {memory[parameter2Address]} = {memory[parameter1Address] + memory[parameter2Address]} and storing at address {parameter3Address}");
        memory[parameter3Address] = memory[parameter1Address] + memory[parameter2Address];
        instructionPointer += 4;
        break;
      case 2:
        // Multiply
        parameter1Address = parameter1Mode == 0 ? memory[instructionPointer + 1] : instructionPointer + 1;
        parameter2Address = parameter2Mode == 0 ? memory[instructionPointer + 2] : instructionPointer + 2;
        parameter3Address = parameter3Mode == 0 ? memory[instructionPointer + 3] : instructionPointer + 3;
        Console.WriteLine($"> Computing {memory[parameter1Address]} * {memory[parameter2Address]} = {memory[parameter1Address] * memory[parameter2Address]} and storing at address {parameter3Address}");
        memory[parameter3Address] = memory[parameter1Address] * memory[parameter2Address];
        instructionPointer += 4;
        break;
      case 3:
        // Store
        parameter1Address = parameter1Mode == 0 ? memory[instructionPointer + 1] : instructionPointer + 1;
        Console.WriteLine($"> Storing {inputValue} at address {parameter1Address}");
        memory[parameter1Address] = inputValue;
        instructionPointer += 2;
        break;
      case 4:
        // Load
        parameter1Address = parameter1Mode == 0 ? memory[instructionPointer + 1] : instructionPointer + 1;
        Console.WriteLine($"> Fetching value at address {parameter1Address}");
        outputValue = memory[parameter1Address];
        instructionPointer += 2;
        break;
      case 5:
        // Jump if True
        parameter1Address = parameter1Mode == 0 ? memory[instructionPointer + 1] : instructionPointer + 1;
        parameter2Address = parameter2Mode == 0 ? memory[instructionPointer + 2] : instructionPointer + 2;
        Console.WriteLine($"> Checking {memory[parameter1Address]} != 0 => {memory[parameter1Address] != 0} and jumping to address {parameter2Address} if true");
        if (memory[parameter1Address] != 0)
        {
          instructionPointer = memory[parameter2Address];
        }
        else
        {
          instructionPointer += 3;
        }
        break;
      case 6:
        // Jump if False
        parameter1Address = parameter1Mode == 0 ? memory[instructionPointer + 1] : instructionPointer + 1;
        parameter2Address = parameter2Mode == 0 ? memory[instructionPointer + 2] : instructionPointer + 2;
        Console.WriteLine($"> Checking {memory[parameter1Address]} == 0 => {memory[parameter1Address] == 0} and jumping to address {parameter2Address} if true");
        if (memory[parameter1Address] == 0)
        {
          instructionPointer = memory[parameter2Address];
        }
        else
        {
          instructionPointer += 3;
        }
        break;
      case 7:
        // Less Than
        parameter1Address = parameter1Mode == 0 ? memory[instructionPointer + 1] : instructionPointer + 1;
        parameter2Address = parameter2Mode == 0 ? memory[instructionPointer + 2] : instructionPointer + 2;
        parameter3Address = parameter3Mode == 0 ? memory[instructionPointer + 3] : instructionPointer + 3;
        Console.WriteLine($"> Comparing {memory[parameter1Address]} < {memory[parameter2Address]} => {memory[parameter1Address] < memory[parameter2Address]} and storing at address {parameter3Address}");
        memory[parameter3Address] = memory[parameter1Address] < memory[parameter2Address] ? 1 : 0;
        instructionPointer += 4;
        break;
      case 8:
        // Equals
        parameter1Address = parameter1Mode == 0 ? memory[instructionPointer + 1] : instructionPointer + 1;
        parameter2Address = parameter2Mode == 0 ? memory[instructionPointer + 2] : instructionPointer + 2;
        parameter3Address = parameter3Mode == 0 ? memory[instructionPointer + 3] : instructionPointer + 3;
        Console.WriteLine($"> Comparing {memory[parameter1Address]} == {memory[parameter2Address]} => {memory[parameter1Address] == memory[parameter2Address]} and storing at address {parameter3Address}");
        memory[parameter3Address] = memory[parameter1Address] == memory[parameter2Address] ? 1 : 0;
        instructionPointer += 4;
        break;
      default:
        Console.WriteLine($"Invalid code {opCode}");
        instructionPointer = -1;
        break;
    }
    return instructionPointer;
  }
}
