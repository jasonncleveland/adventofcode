using System;
using System.Diagnostics;
using System.IO;

class CubeConundrum
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

        int total = 0;
        foreach (string line in lines)
        {
          string[] data = line.Split(':');
          string[] rolls = data[1].Split(';');

          int maxRedCubes = int.MinValue;
          int maxGreenCubes = int.MinValue;
          int maxBlueCubes = int.MinValue;
          foreach (string roll in rolls)
          {
            int foundRedCubes = 0;
            int foundGreenCubes = 0;
            int foundBlueCubes = 0;
            string[] rollsMatches = roll.Trim().Split(',');
            foreach (string rollMatch in rollsMatches)
            {
              string[] resultParts = rollMatch.Split(' ', StringSplitOptions.RemoveEmptyEntries);
              int cubeCount = int.Parse(resultParts[0]);
              string cubeColour = resultParts[1];
              switch (cubeColour)
              {
                case "red":
                  foundRedCubes += cubeCount;
                  break;
                case "green":
                  foundGreenCubes += cubeCount;
                  break;
                case "blue":
                  foundBlueCubes += cubeCount;
                  break;
              }
            }
            if (foundRedCubes > maxRedCubes) maxRedCubes = foundRedCubes;
            if (foundGreenCubes > maxGreenCubes) maxGreenCubes = foundGreenCubes;
            if (foundBlueCubes > maxBlueCubes) maxBlueCubes = foundBlueCubes;
          }
          total += maxRedCubes * maxGreenCubes * maxBlueCubes;
        }
        Console.WriteLine($"Total value: {total}");

        stopWatch.Stop();
        Console.WriteLine($"Elapsed Time: {stopWatch.ElapsedMilliseconds} ms");
      }
    }
  }
}
