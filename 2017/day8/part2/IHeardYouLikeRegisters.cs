using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

class IHeardYouLikeRegisters
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        string[] lines = File.ReadAllLines(fileName);

        string pattern = @"(?<target>\w+) (?<operation>\w+) (?<amount>-?\d+) if (?<source>\w+) (?<condition>.+) (?<value>-?\d+)";
        Regex regex = new Regex(pattern, RegexOptions.Compiled);

        Dictionary<string, int> registers = new Dictionary<string, int>();

        int largestEverValue = int.MinValue;
        foreach (string line in lines)
        {
          Match match = regex.Match(line);
          string destRegister = match.Groups["target"].Value;
          string operation = match.Groups["operation"].Value;
          int amount = int.Parse(match.Groups["amount"].Value);
          string srcRegister = match.Groups["source"].Value;
          string condition = match.Groups["condition"].Value;
          int value = int.Parse(match.Groups["value"].Value);

          // Set register values to 0 if non-existant
          if (!registers.ContainsKey(destRegister))
          {
            registers.Add(destRegister, 0);
          }
          if (!registers.ContainsKey(srcRegister))
          {
            registers.Add(srcRegister, 0);
          }

          bool isValidCondition = false;
          switch (condition)
          {
            case ">":
              isValidCondition = registers[srcRegister] > value;
              break;
            case ">=":
              isValidCondition = registers[srcRegister] >= value;
              break;
            case "<":
              isValidCondition = registers[srcRegister] < value;
              break;
            case "<=":
              isValidCondition = registers[srcRegister] <= value;
              break;
            case "==":
              isValidCondition = registers[srcRegister] == value;
              break;
            case "!=":
              isValidCondition = registers[srcRegister] != value;
              break;
            default:
              throw new Exception($"Invalid condition received '{condition}'");
          }

          if (isValidCondition)
          {
            int valueToStore;
            switch (operation)
            {
              case "inc":
                valueToStore = registers[destRegister] + amount;
                break;
              case "dec":
                valueToStore = registers[destRegister] - amount;
                break;
              default:
                throw new Exception($"Invalid operation received '{operation}'");
            }
            if (valueToStore > largestEverValue) largestEverValue = valueToStore;
            registers[destRegister] = valueToStore;
          }
        }
        Console.WriteLine($"Largest Ever Value: {largestEverValue}");
      }
    }
  }
}
