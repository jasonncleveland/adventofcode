using System;
using System.Collections.Generic;
using System.IO;

class LikeAGifForYourYard
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        string[] lines = File.ReadAllLines(fileName);

        char[,] lights = new char[100,100];
        for (int y = 0; y < lines.Length; y++)
        {
          for (int x = 0; x < lines[y].Length; x++)
          {
            lights[y, x] = lines[y][x];
          }
        }

        // Turn on corner lights
        lights[0, 0] = '#';
        lights[0, lights.GetLength(1) - 1] = '#';
        lights[lights.GetLength(0) - 1, 0] = '#';
        lights[lights.GetLength(0) - 1, lights.GetLength(1) - 1] = '#';

        for (int i = 0; i < 100; i++)
        {
          lights = toggleLights(lights);
        }

        int numLitLights = 0;
        for (int y = 0; y < lights.GetLength(0); y++)
        {
          for (int x = 0; x < lights.GetLength(1); x++)
          {
            if (lights[y, x] == '#') numLitLights++;
          }
        }
        Console.WriteLine($"Lit lights: {numLitLights}");
      }
    }
  }

  static void printLights(char[,] lights)
  {
    Console.WriteLine("Lights:");
    for (int y = 0; y < lights.GetLength(0); y++)
    {
      for (int x = 0; x < lights.GetLength(1); x++)
      {
        Console.Write(lights[y, x]);
      }
      Console.WriteLine();
    }
  }

  static int checkNeighbours(char[,] lights, int y, int x)
  {
    int numLightsOn = 0;

    // U1 L1
    if (x - 1 >= 0 && y - 1 >= 0 && lights[y - 1, x - 1] == '#') numLightsOn++;
    // U1
    if (y - 1 >= 0 && lights[y - 1, x] == '#') numLightsOn++;
    // U1 R1
    if (x + 1 < lights.GetLength(0) && y - 1 >= 0 && lights[y - 1, x + 1] == '#') numLightsOn++;
    // L1
    if (x - 1 >= 0 && lights[y, x - 1] == '#') numLightsOn++;
    // R1
    if (x + 1 < lights.GetLength(0) && lights[y, x + 1] == '#') numLightsOn++;
    // D1 L1
    if (x - 1 >= 0 && y + 1 < lights.GetLength(1) && lights[y + 1, x - 1] == '#') numLightsOn++;
    // D1
    if (y + 1 < lights.GetLength(1) && lights[y + 1, x] == '#') numLightsOn++;
    // D1 R1
    if (x + 1 < lights.GetLength(0) && y + 1 < lights.GetLength(1) && lights[y + 1, x + 1] == '#') numLightsOn++;

    return numLightsOn;
  }

  static char[,] toggleLights(char[,] lights)
  {
    char[,] newLights = new char[100,100];
    for (int y = 0; y < lights.GetLength(0); y++)
    {
      for (int x = 0; x < lights.GetLength(1); x++)
      {
        char currentLight = lights[y, x];
        int litNeighbours = checkNeighbours(lights, y, x);
        // Corner lights always stay on
        if (
          y == 0 && x == 0
          || y == 0 && x == lights.GetLength(1) - 1
          || y == lights.GetLength(0) - 1 && x == 0
          || y == lights.GetLength(0) - 1 && x == lights.GetLength(1) - 1
        )
        {
          newLights[y, x] = '#';
          continue;
        }
        if (currentLight == '#')
        {
          if (litNeighbours == 2 || litNeighbours == 3)
          {
            newLights[y, x] = '#';
          }
          else
          {
            newLights[y, x] = '.';
          }
        }
        else if (currentLight == '.')
        {
          if (litNeighbours == 3)
          {
            newLights[y, x] = '#';
          }
          else
          {
            newLights[y, x] = '.';
          }
        }
        else
        {
          throw new ArgumentException($"Invalid array value: {newLights[y, x]}");
        }
      }
    }
    return newLights;
  }
}
