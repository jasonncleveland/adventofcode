using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

class NeverTellMeTheOdds
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      long minTest = args.Length > 1 ? long.Parse(args[1]) : 200000000000000;
      long maxTest = args.Length > 2 ? long.Parse(args[2]) : 400000000000000;
      if (File.Exists(fileName))
      {
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();

        string[] lines = File.ReadAllLines(fileName);

        // Store the hailstones
        List<Hailstone> hailstones = new List<Hailstone>();
        for (int i = 0; i < lines.Length; i++)
        {
          string line = lines[i];
          string[] lineParts = line.Split('@');
          string[] positions = lineParts[0].Split(',');
          string[] velocities = lineParts[1].Split(',');
          double positionX = double.Parse(positions[0]);
          double positionY = double.Parse(positions[1]);
          double positionZ = double.Parse(positions[2]);
          double velocityX = double.Parse(velocities[0]);
          double velocityY = double.Parse(velocities[1]);
          double velocityZ = double.Parse(velocities[2]);
          
          // Slope: m = (y₂ - y₁)/(x₂ - x₁)
          double slope = ((positionY + velocityY) - positionY) / ((positionX + velocityX) - positionX);
          // Y-Intersect: b = y₁ - x₁(y₂ - y₁)/(x₂ - x₁)
          double intercept = positionY - positionX * slope;
          hailstones.Add(new Hailstone(i, positionX, positionY, positionZ, velocityX, velocityY, velocityZ, slope, intercept));
        }

        double total = 0;
        HashSet<string> checkedLines = new HashSet<string>();
        foreach (Hailstone first in hailstones)
        {
          foreach (Hailstone second in hailstones)
          {
            if (first == second)
            {
              continue;
            }

            // Prevent checking the same two lines
            string firstKey = $"{first.Id},{second.Id}";
            string secondKey = $"{second.Id},{first.Id}";
            if (checkedLines.Contains(firstKey) || checkedLines.Contains(secondKey))
            {
              continue;
            }
            checkedLines.Add(firstKey);
            checkedLines.Add(secondKey);

            // Assuming x and y are equal, m₁x + b₁ = m₂x + b₂, so x = (b₂ - b₁) / (m₁ - m₂)
            double intersectX = (second.Intercept - first.Intercept) / (first.Slope - second.Slope);
            // x is the same at the intersection so we can use either slope formula to get the y
            // y = mx + b
            double intersectY = first.Slope * intersectX + first.Intercept;

            // Lines are parallel so no intersection
            if (double.IsInfinity(intersectX) || double.IsInfinity(intersectY))
            {
              continue;
            }

            bool isInFuture = true;
            if (first.VelocityX > 0)
            {
              isInFuture = isInFuture && intersectX >= first.PositionX;
            }
            else
            {
              isInFuture = isInFuture && intersectX <= first.PositionX;
            }
            if (first.VelocityY > 0)
            {
              isInFuture = isInFuture && intersectY >= first.PositionY;
            }
            else
            {
              isInFuture = isInFuture && intersectY <= first.PositionY;
            }
            if (second.VelocityX > 0)
            {
              isInFuture = isInFuture && intersectX >= second.PositionX;
            }
            else
            {
              isInFuture = isInFuture && intersectX <= second.PositionX;
            }
            if (second.VelocityY > 0)
            {
              isInFuture = isInFuture && intersectY >= second.PositionY;
            }
            else
            {
              isInFuture = isInFuture && intersectY <= second.PositionY;
            }
            if (isInFuture && intersectX >= minTest && intersectX <= maxTest && intersectY >= minTest && intersectY <= maxTest)
            {
              total += 1;
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

class Hailstone
{
  public int Id { get; set; }
  public double PositionX { get; set; }
  public double PositionY { get; set; }
  public double PositionZ { get; set; }
  public double VelocityX { get; set; }
  public double VelocityY { get; set; }
  public double VelocityZ { get; set; }
  public double Slope { get; set; }
  public double Intercept { get; set; }

  public Hailstone(int id, double positionX, double positionY, double positionZ, double velocityX, double velocityY, double velocityZ, double slope, double intercept)
  {
    Id = id;
    PositionX = positionX;
    PositionY = positionY;
    PositionZ = positionZ;
    VelocityX = velocityX;
    VelocityY = velocityY;
    VelocityZ = velocityZ;
    Slope = slope;
    Intercept = intercept;
  }

  public Hailstone(Hailstone hailstone)
  {
    Id = hailstone.Id;
    PositionX = hailstone.PositionX;
    PositionY = hailstone.PositionY;
    PositionZ = hailstone.PositionZ;
    VelocityX = hailstone.VelocityX;
    VelocityY = hailstone.VelocityY;
    VelocityZ = hailstone.VelocityZ;
    Slope = hailstone.Slope;
    Intercept = hailstone.Intercept;
  }
}
