using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

class LensLibrary
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

        foreach (string line in lines)
        {
          int total = 0;
          string[] steps = line.Split(',');
          foreach (string step in steps)
          {
            int stepTotal = 0;
            foreach (char character in step)
            {
              stepTotal += character;
              stepTotal *= 17;
              stepTotal %= 256;
            }
            total += stepTotal;
          }
          Console.WriteLine($"Total value: {total}");
        }

        stopWatch.Stop();
        Console.WriteLine($"Elapsed Time: {stopWatch.ElapsedMilliseconds} ms");
      }
    }
  }
}
