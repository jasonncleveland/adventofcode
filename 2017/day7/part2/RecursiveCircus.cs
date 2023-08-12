using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

class RecursiveCircus
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        string[] lines = File.ReadAllLines(fileName);

        string pattern = @"(?<name>\w+)\s+\((?<weight>\d+)\)";
        Regex regex = new Regex(pattern, RegexOptions.Compiled);

        Dictionary<string, RecursiveProgram> programs = new Dictionary<string, RecursiveProgram>();

        // Parse input and create program objects
        foreach (string line in lines)
        {
          string[] lineParts = line.Split(" -> ");
          Match match = regex.Match(lineParts[0]);
          string name = match.Groups["name"].Value;
          int weight = int.Parse(match.Groups["weight"].Value);
          RecursiveProgram newProgram = null;
          if (programs.ContainsKey(name))
          {
            newProgram = programs[name];
            newProgram.Weight = weight;
          }
          else
          {
            newProgram = new RecursiveProgram(name, weight);
            programs.Add(name, newProgram);
          }
          // Add child nodes if present
          if (lineParts.Length == 2)
          {
            string[] childNames = lineParts[1].Split(", ");
            foreach (string childName in childNames)
            {
              RecursiveProgram childProgram;
              if (programs.ContainsKey(childName))
              {
                childProgram = programs[childName];
              }
              else
              {
                childProgram = new RecursiveProgram(childName, -1);
                programs.Add(childName, childProgram);
              }
              childProgram.Parent = newProgram;
              newProgram.Children.Add(childProgram);
            }
          }
        }

        // Find program with no parent
        RecursiveProgram bottomProgram = null;
        foreach (KeyValuePair<string, RecursiveProgram> program in programs)
        {
          if (program.Value.Parent == null)
          {
            bottomProgram = program.Value;
            break;
          }
        }

        // Find unbalanced program
        traverseTree(bottomProgram);
      }
    }
  }

  static int traverseTree(RecursiveProgram program)
  {
    if (program.Children.Count == 0) return program.Weight;

    List<int> childWeights = new List<int>();
    HashSet<int> uniqueWeights = new HashSet<int>();
    int childWeightTotal = 0;
    foreach (RecursiveProgram childProgram in program.Children)
    {
      int childWeight = traverseTree(childProgram);
      childWeights.Add(childWeight);
      uniqueWeights.Add(childWeight);
      childWeightTotal += childWeight;
    }

    if (uniqueWeights.Count > 1)
    {
      List<int> uniqueWeightItems = new List<int>();
      int oddWeight = 0;
      foreach (int weight in uniqueWeights)
      {
        bool isOddWeight = childWeights.IndexOf(weight) == childWeights.LastIndexOf(weight);
        if (isOddWeight)
        {
          RecursiveProgram oddProgram = program.Children[childWeights.IndexOf(weight)];
          Console.WriteLine($"Odd program: {oddProgram.Name} ({oddProgram.Weight})");
          oddWeight = oddProgram.Weight;
        }
        uniqueWeightItems.Add(weight);
      }
      int weightDifference = Math.Abs(uniqueWeightItems[0] - uniqueWeightItems[1]);
      Console.WriteLine($"weight needed to balance: {weightDifference}");
      Console.WriteLine($"correct weight: {oddWeight - weightDifference}");
      Environment.Exit(0);
    }

    return program.Weight + childWeightTotal;
  }
}

class RecursiveProgram
{
  public string Name { get; set; }
  public int Weight { get; set; }

  public RecursiveProgram Parent { get; set; }
  public List<RecursiveProgram> Children { get; set; }

  public RecursiveProgram(string name, int weight)
  {
    Name = name;
    Weight = weight;
    Parent = null;
    Children = new List<RecursiveProgram>();
  }
}
