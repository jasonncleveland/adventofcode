using System;
using System.IO;

/**
  * Hex math taken from https://www.redblobgames.com/grids/hexagons/
  */
class HexEd
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
          string[] directions = line.Split(',');

          int q = 0, s = 0, r = 0;
          foreach (string direction in directions)
          {
            switch (direction)
            {
              case "nw":
                q -= 1;
                s += 1;
                break;
              case "n":
                s += 1;
                r -= 1;
                break;
              case "ne":
                q += 1;
                r -= 1;
                break;
              case "sw":
                q -= 1;
                r += 1;
                break;
              case "s":
                s -= 1;
                r += 1;
                break;
              case "se":
                q += 1;
                s -= 1;
                break;
              default:
                throw new Exception($"Invalid direction found: {direction}");
            }
          }
          int manhattenDistance = (Math.Abs(q) + Math.Abs(s) + Math.Abs(r)) / 2;
          Console.WriteLine($"Manhatten Distance: {manhattenDistance}");
        }
      }
    }
  }
}
