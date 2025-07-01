using System;
using System.IO;
using System.Text.RegularExpressions;

class TwoFactorAuthentication
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        string[] lines = File.ReadAllLines(fileName);

        char[,] pixels = new char[6, 50];
        for (int y = 0; y < pixels.GetLength(0); y++)
        {
          for (int x = 0; x < pixels.GetLength(1); x++)
          {
            pixels[y, x] = '.';
          }
        }

        Regex rectRegex = new Regex(@"rect (?<x>\d+)x(?<y>\d+)");
        Regex rotateRegex = new Regex(@"rotate (?<type>\w+) (\w)=(?<index>\d+) by (?<delta>\d+)");

        foreach (string line in lines)
        {
          if (line.StartsWith("rect"))
          {
            Match match = rectRegex.Match(line);
            int x = int.Parse(match.Groups["x"].Value);
            int y = int.Parse(match.Groups["y"].Value);
            for (int rectY = 0; rectY < y; rectY++)
            {
              for (int rectX = 0; rectX < x; rectX++)
              {
                pixels[rectY, rectX] = '#';
              }
            }
          }
          else if (line.StartsWith("rotate"))
          {
            Match match = rotateRegex.Match(line);
            string type = match.Groups["type"].Value;
            int index = int.Parse(match.Groups["index"].Value);
            int delta = int.Parse(match.Groups["delta"].Value);
            switch (type)
            {
              case "column":
                for (int i = 0; i < delta; i++)
                {
                  char charToInsert = pixels[pixels.GetLength(0) - 1, index];
                  for (int y = 0, s = pixels.GetLength(0); y < s; y++)
                  {
                    char removedChar = pixels[y, index];
                    pixels[y, index] = charToInsert;
                    charToInsert = removedChar;
                  }
                }
                break;
              case "row":
                for (int i = 0; i < delta; i++)
                {
                  char charToInsert = pixels[index, pixels.GetLength(1) - 1];
                  for (int x = 0, s = pixels.GetLength(1); x < s; x++)
                  {
                    char removedChar = pixels[index, x];
                    pixels[index, x] = charToInsert;
                    charToInsert = removedChar;
                  }
                }
                break;
              default:
                throw new Exception($"Invalid operation type: {type}");
            }
          }
          else
          {
            throw new Exception($"Invalid instruction: ${line}");
          }
        }

        printPixels(pixels);
      }
    }
  }

  static void printPixels(char[,] pixels)
  {

    for (int y = 0; y < pixels.GetLength(0); y++)
    {
      for (int x = 0; x < pixels.GetLength(1); x++)
      {
        Console.Write(pixels[y, x]);
      }
      Console.WriteLine();
    }
  }
}
