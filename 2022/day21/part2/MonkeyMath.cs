using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

class MonkeyMath
{
  static string humanVariable = "x";
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        string fileContents = File.ReadAllText(fileName);
        string[] lines = fileContents.Split('\n');

        Dictionary<string, string> monkeyData = new Dictionary<string, string>();

        // Read input
        foreach (string line in lines)
        {
          string[] lineParts = line.Split(':');
          if (lineParts[0].Trim() == "root")
          {
            string[] operationParts = lineParts[1].Trim().Split(' ');
            operationParts[1] = "=";
            monkeyData.Add(lineParts[0].Trim(), string.Join(" ", operationParts));
          }
          else if (lineParts[0].Trim() == "humn")
          {
            monkeyData.Add(lineParts[0].Trim(), humanVariable);
          }
          else
          {
            monkeyData.Add(lineParts[0].Trim(), lineParts[1].Trim());
          }
        }

        string replacedEquation = replaceVariables(monkeyData, monkeyData["root"]);

        Console.WriteLine("Plug this equation int https://www.mathpapa.com/simplify-calculator/");
        Console.WriteLine(replacedEquation);
      }
    }
  }

  static string replaceVariables(Dictionary<string, string> monkeyData, string operation)
  {
    string value;
    if (monkeyData.TryGetValue(operation, out value))
    {
      int result;
      if (value == humanVariable || int.TryParse(value, out result))
      {
        // found a raw value
        return value;
      }
      else
      {
        // found an operation
        string[] operationParts = value.Split(' ');
        string leftOperation = replaceVariables(monkeyData, operationParts[0]);
        string rightOperation = replaceVariables(monkeyData, operationParts[2]);
        return $"({leftOperation} {operationParts[1]} {rightOperation})";
      }
    }
    else
    {
      // we were passed an operation
      string[] operationParts = operation.Split(' ');
      string leftOperation = replaceVariables(monkeyData, operationParts[0]);
      string rightOperation = replaceVariables(monkeyData, operationParts[2]);
      return $"({leftOperation} {operationParts[1]} {rightOperation})";
    }
  }
}
