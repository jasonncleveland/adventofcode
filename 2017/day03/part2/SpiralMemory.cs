using System;
using System.Collections.Generic;
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
          Console.WriteLine($"\nTarget number: {line}");
          int targetValue = int.Parse(line);

          Dictionary<string, int> grid = new Dictionary<string, int>();

          int x = 0;
          int y = 0;

          // Set up first layer (center)
          grid.Add($"{x},{y}", 1);

          // Start with second layer
          x++;
          int rowLength = 3;
          int halfWidth = 1;
          int square = 9;
          int counter = 2;
          Direction currentDirection = Direction.Up;

          // Iterate until desired value is found
          while (true)
          {
            int nextValue = getAdjacentValues(grid, x, y);
            grid.Add($"{x},{y}", nextValue);

            // Check if value is above target value
            if (nextValue > targetValue)
            {
              Console.WriteLine($"Found first value above target: {nextValue}");
              break;
            }

            // Move to next layer
            if (counter == square)
            {
              rowLength += 2;
              halfWidth = rowLength / 2;
              square = rowLength * rowLength;
              currentDirection = Direction.Right;
            }

            // Move to next square
            if (currentDirection == Direction.Left || currentDirection == Direction.Right)
            {
              // Move left or right
              if (currentDirection == Direction.Left) x--;
              else if (currentDirection == Direction.Right) x++;

              // Check if direction needs to change
              if (x == halfWidth) currentDirection = Direction.Up;
              else if (x == halfWidth * -1) currentDirection = Direction.Down;
            }
            else if (currentDirection == Direction.Up || currentDirection == Direction.Down)
            {
              // Move up or down
              if (currentDirection == Direction.Up) y--;
              else if (currentDirection == Direction.Down) y++;

              // Check if direction needs to change
              if (y == halfWidth) currentDirection = Direction.Right;
              else if (y == halfWidth * -1) currentDirection = Direction.Left;
            }
            counter++;
          }
          break;
        }
      }
    }
  }

  static int getAdjacentValues(Dictionary<string, int> grid, int x, int y)
  {
    int total = 0;

    // Left Up
    if (grid.ContainsKey($"{x - 1},{y - 1}")) total += grid[$"{x - 1},{y - 1}"];
    // Up
    if (grid.ContainsKey($"{x},{y - 1}")) total += grid[$"{x},{y - 1}"];
    // Right Up
    if (grid.ContainsKey($"{x + 1},{y - 1}")) total += grid[$"{x + 1},{y - 1}"];
    // Left
    if (grid.ContainsKey($"{x - 1},{y}")) total += grid[$"{x - 1},{y}"];
    // Right
    if (grid.ContainsKey($"{x + 1},{y}")) total += grid[$"{x + 1},{y}"];
    // Left Down
    if (grid.ContainsKey($"{x - 1},{y + 1}")) total += grid[$"{x - 1},{y + 1}"];
    // Down
    if (grid.ContainsKey($"{x},{y + 1}")) total += grid[$"{x},{y + 1}"];
    // Right Down
    if (grid.ContainsKey($"{x + 1},{y + 1}")) total += grid[$"{x + 1},{y + 1}"];

    return total;
  }
}

enum Direction
{
  Right,
  Left,
  Up,
  Down
}
