using System;
using System.IO;

class ChronalCalibration
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        string[] lines = File.ReadAllLines(fileName);

        int frequency = 0;
        foreach (string line in lines)
        {
          int change = int.Parse(line);
          frequency += change;
        }
        Console.WriteLine($"End frequency: {frequency}");
      }
    }
  }
}
