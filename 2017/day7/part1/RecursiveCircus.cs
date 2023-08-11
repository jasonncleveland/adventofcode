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
          RecursiveProgram newProgram = new RecursiveProgram(name, weight);
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
          if (programs.ContainsKey(name))
          {
            newProgram.Parent = programs[name].Parent;
          }
          programs[name] = newProgram;
        }

        // Find program with no parent
        foreach (KeyValuePair<string, RecursiveProgram> program in programs)
        {
          if (program.Value.Parent == null)
          {
            Console.WriteLine($"Bottom program: {program.Key}");
            break;
          }
        }
      }
    }
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
