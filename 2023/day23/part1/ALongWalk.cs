using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

class ALongWalk
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();

        string[] lines = File.ReadAllLines(fileName);

        List<List<char>> grid = new List<List<char>>();

        int startRow = 0;
        int startColumn = lines[startRow].IndexOf('.');
        int endRow = lines.Length - 1;
        int endColumn = lines[endRow].IndexOf('.');
        for (int row = 0; row < lines.Length; row++)
        {
          List<char> tiles = new List<char>();
          for (int column = 0; column < lines[row].Length; column++)
          {
            tiles.Add(lines[row][column]);
          }
          grid.Add(tiles);
        }

        Console.WriteLine($"Start ({startRow},{startColumn}) End: ({endRow},{endColumn})");

        int total = traverseGrid(grid, startRow, startColumn, endRow, endColumn);
        Console.WriteLine($"Total value: {total}");

        stopWatch.Stop();
        Console.WriteLine($"Elapsed Time: {stopWatch.ElapsedMilliseconds} ms");
      }
    }
  }

  static int traverseGrid(List<List<char>> grid, int startRow, int startColumn, int targetRow, int targetColumn)
  {
    Queue<GridCoordinate> coordinatesToCheck = new Queue<GridCoordinate>();
    HashSet<string> visitedCoordinates = new HashSet<string>();

    visitedCoordinates.Add($"{startRow},{startColumn}");
    coordinatesToCheck.Enqueue(new GridCoordinate(startRow, startColumn, visitedCoordinates, 0));

    int maxSteps = 0;
    while (coordinatesToCheck.Count > 0)
    {
      GridCoordinate currentCoordinate = coordinatesToCheck.Dequeue();
      int currentRow = currentCoordinate.Row;
      int currentColumn = currentCoordinate.Column;
      HashSet<string> currentVisitedCoordinates = currentCoordinate.VisitedCoordinates;
      int currentSteps = currentCoordinate.Steps;

      // Ignore invalid coordinates
      if (!isValidCoordinate(grid, currentRow, currentColumn))
      {
        continue;
      }

      if (currentRow == targetRow && currentColumn == targetColumn)
      {
        if (currentSteps > maxSteps) maxSteps = currentSteps;
        continue;
      }
      else
      {
        switch (grid[currentRow][currentColumn])
        {
          case '.':
            // Check all directions if empty space
            // Up
            if (!currentVisitedCoordinates.Contains($"{currentRow - 1},{currentColumn}"))
            {
              coordinatesToCheck.Enqueue(new GridCoordinate(currentRow - 1, currentColumn, currentVisitedCoordinates, currentSteps + 1));
            }
            // Down
            if (!currentVisitedCoordinates.Contains($"{currentRow + 1},{currentColumn}"))
            {
              coordinatesToCheck.Enqueue(new GridCoordinate(currentRow + 1, currentColumn, currentVisitedCoordinates, currentSteps + 1));
            }
            // Left
            if (!currentVisitedCoordinates.Contains($"{currentRow},{currentColumn - 1}"))
            {
              coordinatesToCheck.Enqueue(new GridCoordinate(currentRow, currentColumn - 1, currentVisitedCoordinates, currentSteps + 1));
            }
            // Right
            if (!currentVisitedCoordinates.Contains($"{currentRow},{currentColumn + 1}"))
            {
              coordinatesToCheck.Enqueue(new GridCoordinate(currentRow, currentColumn + 1, currentVisitedCoordinates, currentSteps + 1));
            }
            break;
          case '<':
            // Left
            if (!currentVisitedCoordinates.Contains($"{currentRow},{currentColumn - 1}"))
            {
              coordinatesToCheck.Enqueue(new GridCoordinate(currentRow, currentColumn - 1, currentVisitedCoordinates, currentSteps + 1));
            }
            break;
          case '>':
            // Right
            if (!currentVisitedCoordinates.Contains($"{currentRow},{currentColumn + 1}"))
            {
              coordinatesToCheck.Enqueue(new GridCoordinate(currentRow, currentColumn + 1, currentVisitedCoordinates, currentSteps + 1));
            }
            break;
          case '^':
            // Up
            if (!currentVisitedCoordinates.Contains($"{currentRow - 1},{currentColumn}"))
            {
              coordinatesToCheck.Enqueue(new GridCoordinate(currentRow - 1, currentColumn, currentVisitedCoordinates, currentSteps + 1));
            }
            break;
          case 'v':
            // Down
            if (!currentVisitedCoordinates.Contains($"{currentRow + 1},{currentColumn}"))
            {
              coordinatesToCheck.Enqueue(new GridCoordinate(currentRow + 1, currentColumn, currentVisitedCoordinates, currentSteps + 1));
            }
            break;
        }
      }
    }
    return maxSteps;
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
  public HashSet<string> VisitedCoordinates { get; set; }
  public int Steps { get; set; }

  public GridCoordinate(int row, int column, HashSet<string> visitedCoordinates, int steps = 0)
  {
    Row = row;
    Column = column;
    VisitedCoordinates = new HashSet<string>(visitedCoordinates);
    VisitedCoordinates.Add($"{row},{column}");
    Steps = steps;
  }
}
