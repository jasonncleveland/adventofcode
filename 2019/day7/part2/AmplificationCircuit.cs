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
          List<int> phaseSettingsOptions = new List<int>() { 5, 6, 7, 8, 9 };
          List<List<int>> permutations = generatePermutations(phaseSettingsOptions);

          List<int> bestPermutation = new List<int>();
          int maxSignal = int.MinValue;
          foreach (List<int> phaseSettings in permutations)
          {
            List<Amplifier> amplifiers = initializeAmplifiers(line, phaseSettings);
            for (int i = 0;; i++)
            {
              // Use modulo to calculate the index to infinitely loop through the amplifiers
              Amplifier amplifier = amplifiers[i % amplifiers.Count];
              try
              {
                runProgram(amplifier);
              }
              catch (EndOfProgram)
              {
                // One of the amps has halted
                // If it is the last amp (E), check the value for largest
                if (amplifier.Name == 'E')
                {
                  if (amplifier.OutputValues.Peek() > maxSignal)
                  {
                    maxSignal = amplifier.OutputValues.Dequeue();
                    bestPermutation = phaseSettings;
                  }
                  break;
                }
              }
            }
          }
          Console.WriteLine($"Max thruster signal: {maxSignal} from {string.Join(',', bestPermutation)}");
        }

        stopWatch.Stop();
        Console.WriteLine($"Elapsed Time: {stopWatch.ElapsedMilliseconds} ms");
      }
    }
  }

  static List<Amplifier> initializeAmplifiers(string program, List<int> phaseSettings)
  {
    List<Amplifier> amplifiers = new List<Amplifier>();
    // Initialize amps
    for (int i = 0; i < phaseSettings.Count; i++)
    {
      Amplifier amplifier = new Amplifier((char) ('A' + i), program);
      amplifiers.Add(amplifier);
    }
    // Link outputs to inputs
    for (int i = 0; i < amplifiers.Count; i++)
    {
      Amplifier currentAmplifier = amplifiers[i % amplifiers.Count];
      Amplifier nextAmplifier = amplifiers[(i + 1) % amplifiers.Count];
      nextAmplifier.InputValues = currentAmplifier.OutputValues;
    }
    // Set phase settings
    for (int i = 0; i < amplifiers.Count; i++)
    {
      Amplifier currentAmplifier = amplifiers[i];
      currentAmplifier.InputValues.Enqueue(phaseSettings[i]);
    }
    // Set the initial signal for the first amp
    amplifiers[0].InputValues.Enqueue(0);
    return amplifiers;
  }

  static void runProgram(Amplifier amplifier)
  {
    do
    {
      try
      {
        amplifier.InstructionPointer = processInstruction(amplifier);
      }
      catch (NoInputValues)
      {
        // Stop processing if there are no input values
        return;
      }
    } while (amplifier.InstructionPointer != -1);

    throw new EndOfProgram();
  }

  static int processInstruction(Amplifier amplifier)
  {
    int[] memory = amplifier.Memory;
    int instructionPointer = amplifier.InstructionPointer;
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
        if (amplifier.InputValues.Count > 0)
        {
          parameter1Address = parameter1Mode == 0 ? memory[instructionPointer + 1] : instructionPointer + 1;
          // Console.WriteLine($"> Storing {amplifier.InputValues.Peek()} at address {parameter1Address}");
          memory[parameter1Address] = amplifier.InputValues.Dequeue();
          instructionPointer += 2;
          break;
        }
        else
        {
          throw new NoInputValues();
        }
      case 4:
        // Load
        parameter1Address = parameter1Mode == 0 ? memory[instructionPointer + 1] : instructionPointer + 1;
        // Console.WriteLine($"> Fetching value at address {parameter1Address}");
        amplifier.OutputValues.Enqueue(memory[parameter1Address]);
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
        throw new Exception($"Invalid code {opCode}");
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

class Amplifier
{
  public char Name { get; set; }
  public int[] Memory { get; set; }
  public int InstructionPointer { get; set; }
  public Queue<int> InputValues { get; set; }
  public Queue<int> OutputValues { get; set; }

  public Amplifier(char name, string program)
  {
    Name = name;
    Memory = Array.ConvertAll<string, int>(program.Split(','), operation => int.Parse(operation));
    InstructionPointer = 0;
    InputValues = new Queue<int>();
    OutputValues = new Queue<int>();
  }
}

class NoInputValues : Exception
{
}

class EndOfProgram : Exception
{
}
