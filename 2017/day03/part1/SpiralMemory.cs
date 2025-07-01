using System;
using System.IO;

class SpiralMemory
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
          /*
           * The spiral memory has a common pattern. The lower right corner of
           * each layer is a perfect square of incrementing odd numbers.
           * e.g. 1, 9, 25
           * We can use this assumption to figure out which layer the target
           * number is in. From there we can calculate which row/column the
           * target number is in and then calculate the manhatten distance
           */
          Console.WriteLine($"\nTarget number: {line}");
          int input = int.Parse(line);

          int steps = 0;
          for (int i = 1; ; i += 2)
          {
            int square = i * i;
            if (input <= square)
            {
              // Difference is the number of squares between the perfect square
              // and the target number
              int difference = square - input;

              if (difference == 0)
              {
                // If the difference is 0 then we are at the center
                steps = 0;
                break;
              }

              // Treat the center as coordinate 0,0
              int x = i / 2;
              int y = i / 2;
              int rowLength = i - 1;

              if (square - rowLength < input)
              {
                // In the bottom row
                // Calculate x value by subtracting the difference
                x -= difference;
              }
              else if (square - rowLength - rowLength < input)
              {
                // In the left column
                // Calculate x value by subtracting the row length
                x -= rowLength;
                // Calculate y value by subtracting the difference minus the x distance
                y -= difference - rowLength;
              }
              else if (square - rowLength - rowLength - rowLength < input)
              {
                // In the top row
                // Move x value to left column
                x -= rowLength;
                // Calculate x value by subtracting the difference minus the
                // bottom row x and left column y
                x += difference - rowLength - rowLength;
                // Calculate y value by subtracting the row length
                y -= rowLength;
              }
              else
              {
                // In the right column
                // Move y value to top row
                y -= rowLength;
                // Calculate y value by subtracting the difference minus the
                // bottom row x and left column y and top row x
                y += (difference - rowLength - rowLength - rowLength);
              }
              Console.WriteLine($"Target number coordinates: {x}, {y}");

              // Calculate the Manhatten distance
              steps = Math.Abs(x) + Math.Abs(y);
              break;
            }
          }
          Console.WriteLine($"Steps required: {steps}");
        }
      }
    }
  }
}
