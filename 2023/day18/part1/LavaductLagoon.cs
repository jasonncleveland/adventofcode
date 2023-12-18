using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

class LavaductLagoon
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

        int currentX = 0;
        int currentY = 0;

        List<CartesianPoint2D> coordinates = new List<CartesianPoint2D>();

        int perimeter = 0;
        foreach (string line in lines)
        {
          string[] lineParts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
          string direction = lineParts[0];
          int length = int.Parse(lineParts[1]);
          perimeter += length;

          int directionMultiplier = 1;
          switch (direction)
          {
            // Modify the x coordinate
            case "L":
              directionMultiplier = -1;
              goto case "R";
            case "R":
              currentX += length * directionMultiplier;
              break;
            // Modify the y coordinate
            case "U":
              directionMultiplier = -1;
              goto case "D";
            case "D":
              currentY += length * directionMultiplier;
              break;
          }
          coordinates.Add(new CartesianPoint2D(currentX, currentY));
        }

        int total = 0;
        // Use the Shoelace Theorem to calculate the area of the polygon using the found coordinates
        for (int i = 0; i < coordinates.Count; i++)
        {
          CartesianPoint2D current = coordinates[i];
          CartesianPoint2D next = coordinates[(i + 1) % coordinates.Count];
          total += current.X * next.Y - current.Y * next.X;
        }
        total /= 2;
        // The area is off because we aren't accounting for the thickness of the dug hole
        // Use Pick's Theorem to add half the perimeter plus 1 to get the correct area
        total += perimeter / 2 + 1;
        Console.WriteLine($"Total value: {total}");

        stopWatch.Stop();
        Console.WriteLine($"Elapsed Time: {stopWatch.ElapsedMilliseconds} ms");
      }
    }
  }
}

class CartesianPoint2D
{
  public int X { get; set; }
  public int Y { get; set; }

  public CartesianPoint2D(int x, int y)
  {
    X = x;
    Y = y;
  }
}
