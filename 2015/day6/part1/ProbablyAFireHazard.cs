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

        bool[,] lights = new bool[1000,1000];
        for (int x = 0; x < 1000; x++)
        {
          for (int y = 0; y < 1000; y++)
          {
            lights[x,y] = false;
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
                  lights[x,y] = true;
                  break;
                case "turn off":
                  lights[x,y] = false;
                  break;
                case "toggle":
                  lights[x,y] = !lights[x,y];
                  break;
                default:
                  throw new ArgumentException($"Invalid operation: {operation}");
              }
            }
          }
        }

        int numLitLights = 0;
        for (int x = 0; x < 1000; x++)
        {
          for (int y = 0; y < 1000; y++)
          {
            if (lights[x,y]) numLitLights++;
          }
        }
        Console.WriteLine($"Lit lights: {numLitLights}");
      }
    }
  }
}
