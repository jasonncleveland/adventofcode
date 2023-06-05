using System;
using System.IO;

class TobogganTrajectory
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        string[] lines = File.ReadAllLines(fileName);

        int[][] slopes = new int[][]
        {
          new int[] { 1, 1 },
          new int[] { 3, 1 },
          new int[] { 5, 1 },
          new int[] { 7, 1 },
          new int[] { 1, 2 },
        };

        long result = 1;
        foreach (int[] slope in slopes)
        {
          int xAdd = slope[0], yAdd = slope[1];

          int numTreesEncountered = 0;
          int x = 0;
          for (int y = 0; y < lines.Length; y += yAdd)
          {
            char value = lines[y][x % lines[y].Length];
            if (value == '#')
            {
              numTreesEncountered += 1;
            }
            x += xAdd;
          }

          Console.WriteLine($"Number of trees encountered (right {xAdd}, down {yAdd}): {numTreesEncountered}");
          result *= numTreesEncountered;
        }
        Console.WriteLine($"Result: {result}");
      }
    }
  }
}
