using System;
using System.IO;

class NoTimeForATaxicab
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
          string[] instructions = line.Split(", ");
          int x = 0, y = 0;
          int currentDegrees = 90;
          foreach (string instruction in instructions)
          {
            char direction = instruction[0];
            int numBlocks = int.Parse(instruction.Substring(1));
            switch (direction)
            {
              case 'L':
                currentDegrees += 90;
                break;
              case 'R':
                currentDegrees -= 90;
                break;
              default:
                throw new ArgumentException("Invalid direction");
            }
            int absDegree = ((currentDegrees % 360) + 360) % 360;
            switch (absDegree)
            {
              case 0:
                x += numBlocks;
                break;
              case 90:
                y += numBlocks;
                break;
              case 180:
                x -= numBlocks;
                break;
              case 270:
                y -= numBlocks;
                break;
              default:
                throw new Exception($"invalid degree value: {currentDegrees}");
            }
          }
          Console.WriteLine($"Total number of blocks: {Math.Abs(x) + Math.Abs(y)}");
        }
      }
    }
  }
}
