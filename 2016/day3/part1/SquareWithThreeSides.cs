using System;
using System.IO;
using System.Text.RegularExpressions;

class SquareWithThreeSides
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        string[] lines = File.ReadAllLines(fileName);

        int possibleTriangleCount = 0;
        foreach (string line in lines)
        {
          string[] lineParts = Regex.Replace(line.Trim(), @"\s+", " ").Split(' ');
          int a = int.Parse(lineParts[0]);
          int b = int.Parse(lineParts[1]);
          int c = int.Parse(lineParts[2]);
          if (a + b > c && a + c > b && b + c > a)
          {
            possibleTriangleCount++;
          }
        }
        Console.WriteLine($"Number of possible triangles: {possibleTriangleCount}");
      }
    }
  }
}
