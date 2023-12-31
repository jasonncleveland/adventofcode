using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

class MonitoringSystem
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

        List<Asteroid> asteroids = new List<Asteroid>();

        for (int y = 0; y < lines.Length; y++)
        {
          for (int x = 0; x < lines[y].Length; x++)
          {
            if (lines[y][x] == '#')
            {
              asteroids.Add(new Asteroid(x, y));
            }
          }
        }

        int maxAsteroids = int.MinValue;
        string bestAsteroid = null;
        foreach (Asteroid start in asteroids)
        {
          HashSet<string> foundSlopes = new HashSet<string>();
          foreach (Asteroid end in asteroids)
          {
            if (start == end)
            {
              continue;
            }

            // There can be two valid asteroids on opposite sides of the same slope
            // so we need to keep track of the directionality to avoid ignoring
            // any valid asteroids
            string directionality = $"{getDirectionality(end.X - start.X)},{getDirectionality(end.Y - start.Y)}";

            // If the X values are equal then we have a straight vertical line
            if (start.X == end.X)
            {
              foundSlopes.Add($"x = {start.X};{directionality}");
              continue;
            }

            // If the Y values are equal then we have a straight horizontal line
            if (start.Y == end.Y)
            {
              foundSlopes.Add($"y = {start.Y};{directionality}");
              continue;
            }

            // Calculate slope and intercept as fractions to preserve precision
            // Slope: m = (y₂ - y₁)/(x₂ - x₁)
            int slopeNumerator = end.Y - start.Y;
            int slopeDenominator = end.X - start.X;
            // If the numbers have the same sign, make them positive
            if (slopeNumerator * slopeDenominator > 0)
            {
              slopeNumerator = Math.Abs(slopeNumerator);
              slopeDenominator = Math.Abs(slopeDenominator);
            }
            // Reduce the slope fraction
            int slopeGcd = calculateGreatestCommonDivisor(Math.Abs(slopeNumerator), Math.Abs(slopeDenominator));
            // Y-Intersect: b = y₁ - x₁(y₂ - y₁)/(x₂ - x₁)
            // Y-Intersect: b = y₁ - x₁m
            int interceptNumerator = start.Y * (slopeDenominator / slopeGcd) - start.X * (slopeNumerator / slopeGcd);
            int interceptDenominator = slopeDenominator / slopeGcd;
            // If the numbers have the same sign, make them positive
            if (interceptNumerator * interceptDenominator > 0)
            {
              interceptNumerator = Math.Abs(interceptNumerator);
              interceptDenominator = Math.Abs(interceptDenominator);
            }
            // Ignore the intercept if it is 0
            if (interceptNumerator == 0)
            {
              foundSlopes.Add($"y = {slopeNumerator / slopeGcd}/{slopeDenominator / slopeGcd}x;{directionality}");
            }
            else
            {
              // Reduce the intercept fraction
              int interceptGcd = calculateGreatestCommonDivisor(Math.Abs(interceptNumerator), Math.Abs(interceptDenominator));
              foundSlopes.Add($"y = {slopeNumerator / slopeGcd}/{slopeDenominator / slopeGcd}x + {interceptNumerator / interceptGcd}/{interceptDenominator / interceptGcd};{directionality}");
            }
          }

          if (foundSlopes.Count > maxAsteroids)
          {
            maxAsteroids = foundSlopes.Count;
            bestAsteroid = $"{start.X},{start.Y}";
          }
        }
        Console.WriteLine($"Total value: {maxAsteroids} from {bestAsteroid}");

        stopWatch.Stop();
        Console.WriteLine($"Elapsed Time: {stopWatch.ElapsedMilliseconds} ms");
      }
    }
  }

  static int getDirectionality(int difference)
  {
    if (difference > 0) return 1;
    else if (difference < 0) return -1;
    else return 0;
  }

  static int calculateGreatestCommonDivisor(int a, int b)
  {
    if (b == 0)
    {
      return a;
    }

    if (b > a)
    {
      return calculateGreatestCommonDivisor(a, b % a);
    }
    else
    {
      return calculateGreatestCommonDivisor(b, a % b);
    }
  }
}

class Asteroid
{
  public int X { get; set; }
  public int Y { get; set; }

  public Asteroid(int x, int y)
  {
    X = x;
    Y = y;
  }
}
