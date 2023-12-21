using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

class StepCounter
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      int maxSteps = args.Length > 1 ? int.Parse(args[1]) : 64;
      if (File.Exists(fileName))
      {
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();

        string[] lines = File.ReadAllLines(fileName);

        List<List<char>> grid = new List<List<char>>();

        int startRow = -1;
        int startColumn = -1;
        for (int row = 0; row < lines.Length; row++)
        {
          List<char> tiles = new List<char>();
          for (int column = 0; column < lines[row].Length; column++)
          {
            if (lines[row][column] == 'S')
            {
              startRow = row;
              startColumn = column;
            }
            tiles.Add(lines[row][column]);
          }
          grid.Add(tiles);
        }

        int total = traverseGrid(grid, startRow, startColumn, maxSteps);
        Console.WriteLine($"Total value: {total}");

        stopWatch.Stop();
        Console.WriteLine($"Elapsed Time: {stopWatch.ElapsedMilliseconds} ms");
      }
    }
  }

  static int traverseGrid(List<List<char>> grid, int row, int column, int maxSteps)
  {
    Queue<GridCoordinate> coordinatesToCheck = new Queue<GridCoordinate>();
    HashSet<string> visitedCoordinates = new HashSet<string>();
    HashSet<string> validCoordinates = new HashSet<string>();

    coordinatesToCheck.Enqueue(new GridCoordinate(row, column, 0));
    visitedCoordinates.Add($"{row},{column},{0}");

    while (coordinatesToCheck.Count > 0)
    {
      GridCoordinate currentCoordinate = coordinatesToCheck.Dequeue();
      int currentRow = currentCoordinate.Row;
      int currentColumn = currentCoordinate.Column;
      int currentSteps = currentCoordinate.Steps;

      if (!isValidCoordinate(grid, currentRow, currentColumn))
      {
        continue;
      }

      if (currentSteps == maxSteps)
      {
        validCoordinates.Add($"{currentRow},{currentColumn},{currentSteps}");
        continue;
      }
      else
      {
        // Up
        if (!visitedCoordinates.Contains($"{currentRow - 1},{currentColumn},{currentSteps + 1}"))
        {
          coordinatesToCheck.Enqueue(new GridCoordinate(currentRow - 1, currentColumn, currentSteps + 1));
          visitedCoordinates.Add($"{currentRow - 1},{currentColumn},{currentSteps + 1}");
        }
        // Down
        if (!visitedCoordinates.Contains($"{currentRow + 1},{currentColumn},{currentSteps + 1}"))
        {
          coordinatesToCheck.Enqueue(new GridCoordinate(currentRow + 1, currentColumn, currentSteps + 1));
          visitedCoordinates.Add($"{currentRow + 1},{currentColumn},{currentSteps + 1}");
        }
        // Left
        if (!visitedCoordinates.Contains($"{currentRow},{currentColumn - 1},{currentSteps + 1}"))
        {
          coordinatesToCheck.Enqueue(new GridCoordinate(currentRow, currentColumn - 1, currentSteps + 1));
          visitedCoordinates.Add($"{currentRow},{currentColumn - 1},{currentSteps + 1}");
        }
        // Right
        if (!visitedCoordinates.Contains($"{currentRow},{currentColumn + 1},{currentSteps + 1}"))
        {
          coordinatesToCheck.Enqueue(new GridCoordinate(currentRow, currentColumn + 1, currentSteps + 1));
          visitedCoordinates.Add($"{currentRow},{currentColumn + 1},{currentSteps + 1}");
        }
      }
    }
    return validCoordinates.Count;
  }

  static bool isValidCoordinate(List<List<char>> grid, int row, int column)
  {
    return row >= 0 && row < grid.Count && column >= 0 && column < grid[row].Count && grid[row][column] != '#';
  }
}

class GridCoordinate
{
  public int Row { get; set; }
  public int Column { get; set; }
  public int Steps { get; set; }

  public GridCoordinate(int row, int column, int steps = 0)
  {
    Row = row;
    Column = column;
    Steps = steps;
  }
}
