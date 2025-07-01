using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

class NoMatterHowYouSliceIt
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        string[] lines = File.ReadAllLines(fileName);

        string pattern = @"#(\d+) @ (\d+),(\d+): (\d+)x(\d+)";
        Regex regex = new Regex(pattern, RegexOptions.Compiled);

        HashSet<string> coveredSquares = new HashSet<string>();
        HashSet<string> overlappingSquares = new HashSet<string>();

        foreach (string line in lines)
        {
          Match match = regex.Match(line);

          string id = match.Groups[1].Value;
          int startX = int.Parse(match.Groups[2].Value);
          int startY = int.Parse(match.Groups[3].Value);
          int width = int.Parse(match.Groups[4].Value);
          int height = int.Parse(match.Groups[5].Value);

          for (int x = startX; x < startX + width; x++)
          {
            for (int y = startY; y < startY + height; y++)
            {
              string position = $"{x},{y}";
              if (coveredSquares.Contains(position))
              {
                overlappingSquares.Add(position);
              }
              coveredSquares.Add(position);
            }
          }
        }
        Console.WriteLine($"Overlapping squares: {overlappingSquares.Count}");
      }
    }
  }
}
