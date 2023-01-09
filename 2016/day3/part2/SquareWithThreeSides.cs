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
        for (int i = 0; i < lines.Length; i += 3)
        {
          string line1 = lines[i].Trim();
          string line2 = lines[i + 1].Trim();
          string line3 = lines[i + 2].Trim();
          string[] lineParts1 = Regex.Replace(line1, @"\s+", " ").Split(' ');
          string[] lineParts2 = Regex.Replace(line2, @"\s+", " ").Split(' ');
          string[] lineParts3 = Regex.Replace(line3, @"\s+", " ").Split(' ');
          for (int j = 0; j < 3; j++)
          {
            int a = int.Parse(lineParts1[j]);
            int b = int.Parse(lineParts2[j]);
            int c = int.Parse(lineParts3[j]);
            if (a + b > c && a + c > b && b + c > a)
            {
              possibleTriangleCount++;
            }
          }
        }
        Console.WriteLine($"Number of possible triangles: {possibleTriangleCount}");
      }
    }
  }
}
