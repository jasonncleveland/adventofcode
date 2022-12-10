using System;
using System.Collections.Generic;
using System.IO;

class CathodeRayTube
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

        int xRegister = 1;
        int clockCycle = 1;
        int totalSignalStrength = 0;

        // Get signal strength for current cycle
        totalSignalStrength += getSignalStrength(clockCycle, xRegister);

        foreach (string line in lines)
        {
          string[] lineParts = line.Split(' ');
          string instruction = lineParts[0];

          // Increment clock cycle for instruction read
          clockCycle++;

          switch (instruction)
          {
            case "addx":
              // Get signal strength for current cycle
              totalSignalStrength += getSignalStrength(clockCycle, xRegister);

              // Increment clock cycle for adding value
              clockCycle++;

              xRegister += int.Parse(lineParts[1]);
              break;
            case "noop":
              break;
            default:
              Console.WriteLine($"Invalid instruction: '{instruction}'");
              break;
          }

          // Get signal strength for current cycle
          totalSignalStrength += getSignalStrength(clockCycle, xRegister);
        }

        Console.WriteLine($"Total signal strength: {totalSignalStrength}");
      }
    }
  }

  static int getSignalStrength(int cycle, int registerValue)
  {
    int[] importantCycles = new int[] { 20, 60, 100, 140, 180, 220 };
    int index = Array.IndexOf(importantCycles, cycle);
    if (index > -1)
    {
      return cycle * registerValue;
    }
    return 0;
  }
}
