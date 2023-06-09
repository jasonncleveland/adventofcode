using System;
using System.IO;

class Dive
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        string[] lines = File.ReadAllLines(fileName);

        int horizontalPosition = 0;
        int depth = 0;
        int aim = 0;
        foreach (string line in lines)
        {
          string[] instructionParts = line.Split(' ');
          string direction = instructionParts[0];
          int units = int.Parse(instructionParts[1]);
          switch (direction)
          {
            case "forward":
              horizontalPosition += units;
              depth += aim * units;
              break;
            case "down":
              aim += units;
              break;
            case "up":
              aim -= units;
              break;
          }
        }
        Console.WriteLine($"Result: {horizontalPosition * depth}");
      }
    }
  }
}
