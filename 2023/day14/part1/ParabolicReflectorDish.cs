using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

class ParabolicReflectorDish
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

        // Parse input
        List<List<char>> platform = new List<List<char>>();
        foreach (string line in lines)
        {
          platform.Add(new List<char>(line));
        }

        // Tilt table to push all rocks as far north as possible
        for (int row = 0; row < platform.Count; row++)
        {
          for (int column = 0; column < platform[row].Count; column++)
          {
            if (platform[row][column] == 'O')
            {
              int currentRow = row;
              while (currentRow > 0)
              {
                if (platform[currentRow - 1][column] == '.')
                {
                  platform[currentRow - 1][column] = platform[currentRow][column];
                  platform[currentRow][column] = '.';
                }
                else if (platform[currentRow - 1][column] == 'O' || platform[currentRow - 1][column] == '#')
                {
                  // We hit a non-empty space, stop
                  break;
                }
                currentRow--;
              }
            }
          }
        }
        
        // Calculate load
        int total = 0;
        for (int row = 0; row < platform.Count; row++)
        {
          for (int column = 0; column < platform[row].Count; column++)
          {
            if (platform[row][column] == 'O')
            {
              total += platform.Count - row;
            }
          }
        }
        Console.WriteLine($"Total value: {total}");

        stopWatch.Stop();
        Console.WriteLine($"Elapsed Time: {stopWatch.ElapsedMilliseconds} ms");
      }
    }
  }
}
