using System;
using System.IO;
using System.Collections.Generic;

class PerfectlySphericalHousesInAVacuum
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        string[] lines = File.ReadAllLines(fileName);

        foreach (string line in lines)
        {
          HashSet<string> visitedHouses = new HashSet<string>();

          int x1 = 0, y1 = 0, x2 = 0, y2 = 0;
          visitedHouses.Add($"{x1},{y1}");
          visitedHouses.Add($"{x2},{y2}");

          for (int i = 0; i < line.Length; i++)
          {
            char direction = line[i];
            bool isRoboSanta = i % 2 == 0;
            switch(direction)
            {
              case '>':
                if (isRoboSanta) x1++;
                else x2++;
                break;
              case '<':
                if (isRoboSanta) x1--;
                else x2--;
                break;
              case 'v':
                if (isRoboSanta) y1++;
                else y2++;
                break;
              case '^':
                if (isRoboSanta) y1--;
                else y2--;
                break;
              default:
                throw new InvalidOperationException($"Invalid direction: '{direction}'");
            }
            if (isRoboSanta) visitedHouses.Add($"{x1},{y1}");
            else visitedHouses.Add($"{x2},{y2}");
          }

          Console.WriteLine($"Total number of houses delivered too: {visitedHouses.Count}");
        }
      }
    }
  }
}
