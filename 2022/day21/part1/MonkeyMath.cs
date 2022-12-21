using System;
using System.Collections.Generic;
using System.IO;

class MonkeyMath
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        string fileContents = File.ReadAllText(fileName);
        string[] lines = fileContents.Split('\n');

        Dictionary<string, long> monkeyResults = new Dictionary<string, long>();
        List<MonkeyData> monkeiesToProcess = new List<MonkeyData>();

        // Read input
        foreach (string line in lines)
        {
          string[] lineParts = line.Split(':');
          long result;
          bool success = long.TryParse(lineParts[1].Trim(), out result);
          if (success)
          {
            monkeyResults.Add(lineParts[0].Trim(), result);
          }
          else
          {
            monkeiesToProcess.Add(new MonkeyData(lineParts[0].Trim(), lineParts[1].Trim()));
          }
        }

        // Process monkies
        while (monkeiesToProcess.Count > 0)
        {
          List<MonkeyData> mutableList = new List<MonkeyData>(monkeiesToProcess);
          foreach (MonkeyData monkey in monkeiesToProcess)
          {
            string[] operationParts = monkey.Operation.Split(' ');
            long aResult, bResult;
            if (
              monkeyResults.TryGetValue(operationParts[0], out aResult)
              && monkeyResults.TryGetValue(operationParts[2], out bResult)
            )
            {
              long result = evaluateOperation(operationParts[1], aResult, bResult);
              monkeyResults.Add(monkey.Name, result);
              mutableList.Remove(monkey);
            }
          }
          monkeiesToProcess = mutableList;
        }

        Console.WriteLine($"root monkey yells: {monkeyResults["root"]}");
      }
    }
  }

  static long evaluateOperation(string operation, long leftValue, long rightValue)
  {
    switch (operation)
    {
      case "+":
        return leftValue + rightValue;
      case "-":
        return leftValue - rightValue;
      case "*":
        return leftValue * rightValue;
      case "/":
        return leftValue / rightValue;
      default:
        throw new ArgumentException("Invalid operation");
    }
  }
}

public class MonkeyData {
  public string Name { get; set; }
  public string Operation { get; set; }

  public MonkeyData(string name, string operation)
  {
    Name = name;
    Operation = operation;
  }
}
