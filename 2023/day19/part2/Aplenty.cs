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

        bool startProcessing = false;
        foreach (string line in lines)
        {
          if (!startProcessing)
          {
            if (line == "")
            {
              // Stop once we've finished inputting all workflows
              startProcessing = true;
              break;
            }

            // Input workflows
            string[] lineParts = line.Trim('}').Split('{');
            string name = lineParts[0];
            string[] instructionStrings = lineParts[1].Split(',');
            List<Instruction> instructions = new List<Instruction>();
            foreach (string instructionString in instructionStrings)
            {
              if (instructionString.Contains(':'))
              {
                string[] instructionParts = instructionString.Split(':');
                string formula = instructionParts[0];
                char variable = formula[0];
                char op = formula[1];
                long value = long.Parse(formula.Substring(2));
                string result = instructionParts[1];
                instructions.Add(new Instruction(variable, op, value, result));
              }
              else
              {
                instructions.Add(new Instruction(instructionString));
              }
            }

            workflows.Add(name, new Workflow(name, instructions));
          }
        }

        long total = generateUniqueCombinations(workflows);
        Console.WriteLine($"Total value: {total}");

        stopWatch.Stop();
        Console.WriteLine($"Elapsed Time: {stopWatch.ElapsedMilliseconds} ms");
      }
    }
  }

  static long generateUniqueCombinations(Dictionary<string, Workflow> workflows)
  {
    long totalUniqueCombinations = 0;

    Queue<WorkflowRange> pathsToCheck = new Queue<WorkflowRange>();

    pathsToCheck.Enqueue(new WorkflowRange("in", new XmasRange()));

    while (pathsToCheck.Count > 0)
    {
      WorkflowRange currentWorkflowRange = pathsToCheck.Dequeue();
      Workflow currentWorkflow = workflows[currentWorkflowRange.WorkflowName];

      XmasRange currentRange = new XmasRange(currentWorkflowRange.Range);
      int instructionIndex = 1;
      foreach (Instruction instruction in currentWorkflow.Instructions)
      {
        string result = instruction.Result;
        if (instruction.Variable == '\0')
        {
          // We have no operation so no range culling needed
          switch (result)
          {
            case "A":
              totalUniqueCombinations += calculateCombinations(currentRange);
              break;
            case "R":
              break;
            default:
              pathsToCheck.Enqueue(new WorkflowRange(result, new XmasRange(currentRange)));
              break;
          }
        }
        else
        {
          // We need to get both results of the condition
          List<XmasRange> operationResults = instruction.getOperationOutcomes(currentRange);

          // Handle true path
          switch (result)
          {
            case "A":
              totalUniqueCombinations += calculateCombinations(operationResults[0]);
              break;
            case "R":
              break;
            default:
              pathsToCheck.Enqueue(new WorkflowRange(result, new XmasRange(operationResults[0])));
              break;
          }

          // Handle false path - Continue to next instruction
          currentRange = new XmasRange(operationResults[1]);
        }
        instructionIndex++;
      }
    }

    return totalUniqueCombinations;
  }

  static long calculateCombinations(XmasRange range)
  {
    long uniqueCombinations = 1;
    uniqueCombinations *= range.MaxX - range.MinX + 1;
    uniqueCombinations *= range.MaxM - range.MinM + 1;
    uniqueCombinations *= range.MaxA - range.MinA + 1;
    uniqueCombinations *= range.MaxS - range.MinS + 1;
    return uniqueCombinations;
  }
}

class XmasRange
{
  public static long MIN_VALUE = 1;
  public static long MAX_VALUE = 4000;

  public long MinX { get; set; }
  public long MaxX { get; set; }
  public long MinM { get; set; }
  public long MaxM { get; set; }
  public long MinA { get; set; }
  public long MaxA { get; set; }
  public long MinS { get; set; }
  public long MaxS { get; set; }

  public XmasRange()
  {
    MinX = MIN_VALUE;
    MaxX = MAX_VALUE;
    MinM = MIN_VALUE;
    MaxM = MAX_VALUE;
    MinA = MIN_VALUE;
    MaxA = MAX_VALUE;
    MinS = MIN_VALUE;
    MaxS = MAX_VALUE;
  }

  public XmasRange(XmasRange range)
  {
    MinX = range.MinX;
    MaxX = range.MaxX;
    MinM = range.MinM;
    MaxM = range.MaxM;
    MinA = range.MinA;
    MaxA = range.MaxA;
    MinS = range.MinS;
    MaxS = range.MaxS;
  }

  public void setMinValue(char name, long value)
  {
    switch (name)
    {
      case 'x':
        MinX = value;
        break;
      case 'm':
        MinM = value;
        break;
      case 'a':
        MinA = value;
        break;
      case 's':
        MinS = value;
        break;
      default:
        throw new Exception($"Attempting to set min invalid name: {name}");
    }
  }

  public void setMaxValue(char name, long value)
  {
    switch (name)
    {
      case 'x':
        MaxX = value;
        break;
      case 'm':
        MaxM = value;
        break;
      case 'a':
        MaxA = value;
        break;
      case 's':
        MaxS = value;
        break;
      default:
        throw new Exception($"Attempting to set max invalid name: {name}");
    }
  }

  public long getMinValue(char name)
  {
    switch (name)
    {
      case 'x':
        return MinX;
      case 'm':
        return MinM;
      case 'a':
        return MinA;
      case 's':
        return MinS;
      default:
        throw new Exception($"Attempting to get min invalid name: {name}");
    }
  }

  public long getMaxValue(char name)
  {
    switch (name)
    {
      case 'x':
        return MaxX;
      case 'm':
        return MaxM;
      case 'a':
        return MaxA;
      case 's':
        return MaxS;
      default:
        throw new Exception($"Attempting to get max invalid name: {name}");
    }
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
}

class Instruction
{
  public char Variable { get; set; }
  public char Operator { get; set; }
  public long Value { get; set; }
  public string Result { get; set; }

  public Instruction(char variable, char op, long value, string result)
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

  public List<XmasRange> getOperationOutcomes(XmasRange range)
  {
    XmasRange trueRange = new XmasRange(range);
    XmasRange falseRange = new XmasRange(range);

    long variableMinValue = range.getMinValue(Variable);
    long variableMaxValue = range.getMaxValue(Variable);

    switch (Operator)
    {
      case '<':
        trueRange.setMaxValue(Variable, Math.Min(variableMaxValue, Value - 1));
        falseRange.setMinValue(Variable, Math.Max(variableMinValue, Value));
        break;
      case '>':
        trueRange.setMinValue(Variable, Math.Max(variableMinValue, Value + 1));
        falseRange.setMaxValue(Variable, Math.Min(variableMaxValue, Value));
        break;
      default:
        throw new Exception($"Received invalid operator: {Operator}");
    }

    return new List<XmasRange>() { trueRange, falseRange };
  }
}

class WorkflowRange
{
  public string WorkflowName { get; set; }
  public XmasRange Range { get; set; }

  public WorkflowRange(string name, XmasRange range)
  {
    WorkflowName = name;
    Range = range;
  }
}
