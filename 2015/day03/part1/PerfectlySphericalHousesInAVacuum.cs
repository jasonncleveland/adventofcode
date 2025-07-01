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

          int x = 0, y = 0;
          visitedHouses.Add($"{x},{y}");

          foreach (char direction in line)
          {
            switch(direction)
            {
              case '>':
                x++;
                break;
              case '<':
                x--;
                break;
              case 'v':
                y++;
                break;
              case '^':
                y--;
                break;
              default:
                throw new InvalidOperationException($"Invalid direction: '{direction}'");
            }
            visitedHouses.Add($"{x},{y}");
          }

          Console.WriteLine($"Total number of houses delivered too: {visitedHouses.Count}");
        }
      }
    }
  }
}
