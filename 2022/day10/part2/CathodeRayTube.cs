using System;
using System.Collections.Generic;
using System.IO;

class CathodeRayTube
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        string fileContents = File.ReadAllText(fileName);
        string[] lines = fileContents.Split('\n');

        int xRegister = 1;
        int clockCycle = 0;

        char[][] display = new char[6][];
        for (int i = 0; i < 6; i++)
        {
          display[i] = new char[40];
        }

        foreach (string line in lines)
        {
          string[] lineParts = line.Split(' ');
          string instruction = lineParts[0];

          // Set pixel value for current clock cycle
          setPixelValue(display, clockCycle, xRegister);

          // Increment clock cycle for instruction read
          clockCycle++;

          switch (instruction)
          {
            case "addx":
              // Set pixel value for current clock cycle
              setPixelValue(display, clockCycle, xRegister);

              // Increment clock cycle for adding value
              clockCycle++;

              xRegister += int.Parse(lineParts[1]);
              break;
            case "noop":
              break;
            default:
              Console.WriteLine($"Invalid instruction: '{instruction}'");
              break;
          }
        }

        drawDisplay(display);
      }
    }
  }

  static void setPixelValue(char[][] display, int cycle, int registerValue)
  {
    int displayRow = cycle / 40;
    int displayColumn = cycle % 40;
    int[] spriteValues = new int[] { registerValue - 1, registerValue, registerValue + 1};
    if (Array.IndexOf(spriteValues, displayColumn) > -1)
    {
      // Draw a lit pixel
      display[displayRow][displayColumn] = '#';
    }
    else
    {
      // Draw a dark pixel
      display[displayRow][displayColumn] = '.';
    }
  }

  static void drawDisplay(char[][] display)
  {
    for (int i = 0; i < display.Length; i++)
    {
      for (int j = 0; j < display[i].Length; j++)
      {
        Console.Write(display[i][j]);
      }
      Console.WriteLine();
    }
  }
}

