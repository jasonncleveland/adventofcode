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

        int timesIncremented = 0;
        int[] measurements = new int[3];
        int lastMeasurmentTotal = int.MinValue;
        for (int i = 0; i < lines.Length; i++)
        {
          measurements[i % 3] = int.Parse(lines[i]);
          if (i >= 2)
          {
            int measurementTotal = measurements[0] + measurements[1] + measurements[2];
            if (lastMeasurmentTotal != int.MinValue && measurementTotal > lastMeasurmentTotal)
            {
              timesIncremented += 1;
            }
            lastMeasurmentTotal = measurementTotal;
          }
        }
        Console.WriteLine($"Times incremented: {timesIncremented}");
      }
    }
  }
}
