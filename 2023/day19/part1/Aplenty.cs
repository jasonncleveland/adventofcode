using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

class Aplenty
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

        Dictionary<string, Workflow> workflows = new Dictionary<string, Workflow>();

        int total = 0;
        bool startProcessing = false;
        foreach (string line in lines)
        {
          if (!startProcessing)
          {
            if (line == "")
            {
              // The workflows stop and the parts begin after an empty line
              startProcessing = true;
              continue;
            }

            // Input workflows
            string[] lineParts = line.Trim('}').Split('{');
            string name = lineParts[0];
            string[] instructionStrings = lineParts[1].Split(',');
            List<Instruction> instructions = new List<Instruction>();
            foreach (string instructionString in instructionStrings)
            {
              // Parse the operation if present, or store just the result
              if (instructionString.Contains(':'))
              {
                string[] instructionParts = instructionString.Split(':');
                string formula = instructionParts[0];
                char variable = formula[0];
                char op = formula[1];
                int value = int.Parse(formula.Substring(2));
                string result = instructionParts[1];
                instructions.Add(new Instruction(variable, op, value, result));
              }
              else
              {
                string result = instructionString;
                instructions.Add(new Instruction(result));
              }
            }

            workflows.Add(name, new Workflow(name, instructions));
          }
          else
          {
            // Process parts
            MachinePart machinePart = new MachinePart();
            string[] partCategories = line.Trim('{').Trim('}').Split(',');
            foreach (string partCategory in partCategories)
            {
              char variable = partCategory[0];
              int value = int.Parse(partCategory.Substring(2));
              machinePart.setValue(variable, value);
            }
            total += processMachinePart(machinePart, workflows);
          }
        }
        Console.WriteLine($"Total value: {total}");

        stopWatch.Stop();
        Console.WriteLine($"Elapsed Time: {stopWatch.ElapsedMilliseconds} ms");
      }
    }
  }

  static int processMachinePart(MachinePart machinePart, Dictionary<string, Workflow> workflows)
  {
    Workflow currentWorkflow = workflows["in"];

    do
    {
      // Process the workflows until one accepts or rejects the part
      string result = currentWorkflow.processInstructions(machinePart);
      switch (result)
      {
        case "A":
          return machinePart.getRating();
        case "R":
          return 0;
        default:
          currentWorkflow = workflows[result];
          break;
      }
    } while (currentWorkflow != null);

    throw new Exception("Should never reach here");
  }
}

class MachinePart
{
  public int X { get; set; }
  public int M { get; set; }
  public int A { get; set; }
  public int S { get; set; }

  public MachinePart()
  {
  }

  public MachinePart(int x, int m, int a, int s)
  {
    X = x;
    M = m;
    A = a;
    S = s;
  }

  public void setValue(char name, int value)
  {
    switch (name)
    {
      case 'x':
        X = value;
        break;
      case 'm':
        M = value;
        break;
      case 'a':
        A = value;
        break;
      case 's':
        S = value;
        break;
      default:
        throw new Exception($"Attempting to set invalid name: {name}");
    }
  }

  public int getValue(char name)
  {
    switch (name)
    {
      case 'x':
        return X;
      case 'm':
        return M;
      case 'a':
        return A;
      case 's':
        return S;
      default:
        throw new Exception($"Attempting to get invalid name: {name}");
    }
  }

  public int getRating()
  {
    return X + M + A + S;
  }
}

class Workflow
{
  public string Name { get; set; }
  public List<Instruction> Instructions { get; set; }

  public Workflow(string name, List<Instruction> instructions)
  {
    Name = name;
    Instructions = instructions;
  }

  public string processInstructions(MachinePart machinePart)
  {
    // Process each instruction until we receive a valid return value
    foreach (Instruction instruction in Instructions)
    {
      string result = instruction.processInstruction(machinePart);
      if (result != null)
      {
        return result;
      }
    }

    throw new Exception("Should never reach here");
  }
}

class Instruction
{
  public char Variable { get; set; }
  public char Operator { get; set; }
  public int Value { get; set; }
  public string Result { get; set; }

  public Instruction(char variable, char op, int value, string result)
  {
    Variable = variable;
    Operator = op;
    Value = value;
    Result = result;
  }

  public Instruction(string result)
  {
    Variable = '\0';
    Result = result;
  }

  public string processInstruction(MachinePart machinePart)
  {
    if (Variable == '\0')
    {
      // If there is no operation then return the result
      return Result;
    }
    else
    {
      int variableValue = machinePart.getValue(Variable);
      switch (Operator)
      {
        case '<':
          return variableValue < Value ? Result : null;
        case '>':
          return variableValue > Value ? Result : null;
        default:
          throw new Exception($"Received invalid operator: {Operator}");
      }
    }
  }
}
