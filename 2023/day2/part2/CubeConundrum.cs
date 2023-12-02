using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

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

        string rollsPattern = @"(\d+) (\w+)";
        Regex rollsRegex = new Regex(rollsPattern, RegexOptions.Compiled);

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
            MatchCollection rollsMatches = rollsRegex.Matches(roll);
            foreach (Match rollMatch in rollsMatches)
            {
              int cubeCount = int.Parse(rollMatch.Groups[1].Value);
              string cubeColour = rollMatch.Groups[2].Value;
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
