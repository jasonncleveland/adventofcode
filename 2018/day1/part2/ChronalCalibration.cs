using System;
using System.Collections.Generic;
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

        HashSet<int> frequencies = new HashSet<int>();

        bool foundDuplicate = false;
        int frequency = 0;
        while (!foundDuplicate)
        {
          foreach (string line in lines)
          {
            int change = int.Parse(line);
            frequency += change;
            if (frequencies.Contains(frequency))
            {
              Console.WriteLine($"Found duplicate frequency: {frequency}");
              foundDuplicate = true;
              break;
            }
            else
            {
              frequencies.Add(frequency);
            }
          }
        }
      }
    }
  }
}
