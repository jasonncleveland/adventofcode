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
        int verticalPosition = 0;
        foreach (string line in lines)
        {
          string[] instructionParts = line.Split(' ');
          string direction = instructionParts[0];
          int units = int.Parse(instructionParts[1]);
          switch (direction)
          {
            case "forward":
              horizontalPosition += units;
              break;
            case "down":
              verticalPosition += units;
              break;
            case "up":
              verticalPosition -= units;
              break;
          }
        }
        Console.WriteLine($"Result: {horizontalPosition * verticalPosition}");
      }
    }
  }
}
