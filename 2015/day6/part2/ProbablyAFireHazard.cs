using System;
using System.IO;
using System.Text.RegularExpressions;

class ProbablyAFireHazard
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        string[] lines = File.ReadAllLines(fileName);

        int[,] lights = new int[1000,1000];
        for (int x = 0; x < 1000; x++)
        {
          for (int y = 0; y < 1000; y++)
          {
            lights[x,y] = 0;
          }
        }

        Regex rx = new Regex(@"(.*) (\d+),(\d+) through (\d+),(\d+)");

        foreach (string line in lines)
        {
          Match match = rx.Match(line);
          string operation = match.Groups[1].Value;
          int startX = int.Parse(match.Groups[2].Value);
          int startY = int.Parse(match.Groups[3].Value);
          int endX = int.Parse(match.Groups[4].Value);
          int endY = int.Parse(match.Groups[5].Value);
          for (int x = startX; x <= endX; x++)
          {
            for (int y = startY; y <= endY; y++)
            {
              switch (operation)
              {
                case "turn on":
                  lights[x,y] += 1;
                  break;
                case "turn off":
                  lights[x,y] = (lights[x,y] > 0) ? lights[x,y] - 1 : 0;
                  break;
                case "toggle":
                  lights[x,y] += 2;
                  break;
                default:
                  throw new ArgumentException($"Invalid operation: {operation}");
              }
            }
          }
        }

        int totalBrightness = 0;
        for (int x = 0; x < 1000; x++)
        {
          for (int y = 0; y < 1000; y++)
          {
            totalBrightness += lights[x,y];
          }
        }
        Console.WriteLine($"Total brightness: {totalBrightness}");
      }
    }
  }
}
