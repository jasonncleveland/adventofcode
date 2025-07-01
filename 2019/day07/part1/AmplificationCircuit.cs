using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

class AmplificationCircuit
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

        foreach (string line in lines)
        {
          List<int> phaseSettingsOptions = new List<int>() { 0, 1, 2, 3, 4 };

          List<List<int>> permutations = generatePermutations(phaseSettingsOptions);

          List<int> bestPermutation = new List<int>();
          int maxSignal = int.MinValue;
          foreach (List<int> phaseSettings in permutations)
          {
            int inputSignal = 0;
            foreach (int phaseSetting in phaseSettings)
            {
              inputSignal = runProgram(line, phaseSetting, inputSignal);
            }

            if (inputSignal > maxSignal)
            {
              maxSignal = inputSignal;
              bestPermutation = phaseSettings;
            }
          }
          Console.WriteLine($"Max thruster signal: {maxSignal} from {string.Join(',', bestPermutation)}");
        }

        stopWatch.Stop();
        Console.WriteLine($"Elapsed Time: {stopWatch.ElapsedMilliseconds} ms");
      }
    }
  }

  static int runProgram(string program, int phaseSetting, int inputSignal)
  {
    int[] memory = Array.ConvertAll<string, int>(program.Split(','), operation => int.Parse(operation));

    List<int> inputValues = new List<int>() { phaseSetting, inputSignal };

    int instructionPointer = 0;
    int outputValue = 0;
    do
    {
      instructionPointer = processInstruction(memory, instructionPointer, inputValues, ref outputValue);
      if (outputValue != 0)
      {
        // We have output so exit early
        return outputValue;
      }
    } while (instructionPointer != -1);

    // Reached the end of the program without setting an output value
    return 0;
  }

  static int processInstruction(int[] memory, int instructionPointer, List<int> inputValues, ref int outputValue)
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
        // Console.WriteLine($"> Computing {memory[parameter1Address]} + {memory[parameter2Address]} = {memory[parameter1Address] + memory[parameter2Address]} and storing at address {parameter3Address}");
        memory[parameter3Address] = memory[parameter1Address] + memory[parameter2Address];
        instructionPointer += 4;
        break;
      case 2:
        // Multiply
        parameter1Address = parameter1Mode == 0 ? memory[instructionPointer + 1] : instructionPointer + 1;
        parameter2Address = parameter2Mode == 0 ? memory[instructionPointer + 2] : instructionPointer + 2;
        parameter3Address = parameter3Mode == 0 ? memory[instructionPointer + 3] : instructionPointer + 3;
        // Console.WriteLine($"> Computing {memory[parameter1Address]} * {memory[parameter2Address]} = {memory[parameter1Address] * memory[parameter2Address]} and storing at address {parameter3Address}");
        memory[parameter3Address] = memory[parameter1Address] * memory[parameter2Address];
        instructionPointer += 4;
        break;
      case 3:
        // Store
        parameter1Address = parameter1Mode == 0 ? memory[instructionPointer + 1] : instructionPointer + 1;
        // Console.WriteLine($"> Storing {inputValues.First()} at address {parameter1Address}");
        memory[parameter1Address] = inputValues.First();
        instructionPointer += 2;
        inputValues.Remove(inputValues.First());
        break;
      case 4:
        // Load
        parameter1Address = parameter1Mode == 0 ? memory[instructionPointer + 1] : instructionPointer + 1;
        // Console.WriteLine($"> Fetching value at address {parameter1Address}");
        outputValue = memory[parameter1Address];
        instructionPointer += 2;
        break;
      case 5:
        // Jump if True
        parameter1Address = parameter1Mode == 0 ? memory[instructionPointer + 1] : instructionPointer + 1;
        parameter2Address = parameter2Mode == 0 ? memory[instructionPointer + 2] : instructionPointer + 2;
        // Console.WriteLine($"> Checking {memory[parameter1Address]} != 0 => {memory[parameter1Address] != 0} and jumping to address {parameter2Address} if true");
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
        // Console.WriteLine($"> Checking {memory[parameter1Address]} == 0 => {memory[parameter1Address] == 0} and jumping to address {parameter2Address} if true");
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
        // Console.WriteLine($"> Comparing {memory[parameter1Address]} < {memory[parameter2Address]} => {memory[parameter1Address] < memory[parameter2Address]} and storing at address {parameter3Address}");
        memory[parameter3Address] = memory[parameter1Address] < memory[parameter2Address] ? 1 : 0;
        instructionPointer += 4;
        break;
      case 8:
        // Equals
        parameter1Address = parameter1Mode == 0 ? memory[instructionPointer + 1] : instructionPointer + 1;
        parameter2Address = parameter2Mode == 0 ? memory[instructionPointer + 2] : instructionPointer + 2;
        parameter3Address = parameter3Mode == 0 ? memory[instructionPointer + 3] : instructionPointer + 3;
        // Console.WriteLine($"> Comparing {memory[parameter1Address]} == {memory[parameter2Address]} => {memory[parameter1Address] == memory[parameter2Address]} and storing at address {parameter3Address}");
        memory[parameter3Address] = memory[parameter1Address] == memory[parameter2Address] ? 1 : 0;
        instructionPointer += 4;
        break;
      default:
        // Console.WriteLine($"Invalid code {opCode}");
        instructionPointer = -1;
        break;
    }
    return instructionPointer;
  }

  static List<List<int>> generatePermutations(List<int> items)
  {
    List<List<int>> permutations = new List<List<int>>();
    
    if (items.Count == 1)
    {
      List<int> permutation = new List<int>();
      permutation.Add(items.First());
      permutations.Add(permutation);
      return permutations;
    }
    
    foreach (int item in items)
    {
      List<int> remaining = new List<int>(items);
      remaining.Remove(item);
      List<List<int>> subPermutations = generatePermutations(remaining);

      foreach (List<int> subPermutation in subPermutations)
      {
        subPermutation.Insert(0, item);
        permutations.Add(subPermutation);
      }
    }

    return permutations;
  }
}
