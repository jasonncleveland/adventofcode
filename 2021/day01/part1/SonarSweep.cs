using System;
using System.IO;

class SonarSweep
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        string[] lines = File.ReadAllLines(fileName);

        int lastInput = int.MinValue;
        int timesIncremented = 0;
        foreach (string line in lines)
        {
          int measurment = int.Parse(line);
          if (lastInput != int.MinValue && measurment > lastInput)
          {
            timesIncremented += 1;
          }
          lastInput = measurment;
        }
        Console.WriteLine($"Times incremented: {timesIncremented}");
      }
    }
  }
}
