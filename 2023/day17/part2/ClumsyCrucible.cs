using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

/*
 * To compile: dotnet build part2/build.csproj
 * To run: dotnet run --project part2/build.csproj -- problemData.txt
 */
class ClumsyCrucible
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

        List<List<int>> grid = new List<List<int>>();

        foreach (string line in lines)
        {
          List<int> row = new List<int>();
          foreach (char character in line)
          {
            row.Add(character - '0');
          }
          grid.Add(row);
        }

        int total = traverseGrid(grid, 0, 0, grid.Count - 1, grid[grid.Count - 1].Count - 1);
        Console.WriteLine($"Total value: {total}");

        stopWatch.Stop();
        Console.WriteLine($"Elapsed Time: {stopWatch.ElapsedMilliseconds} ms");
      }
    }
  }

  static int traverseGrid(List<List<int>> grid, int startRow, int startColumn, int targetRow, int targetColumn)
  {
    // Use A* algorithm to find the best path
    Console.WriteLine($"Finding best path from ({startRow},{startColumn}) to ({targetRow},{targetColumn})");
    PriorityQueue<DirectionStep, int> directionsToCheck = new PriorityQueue<DirectionStep, int>();
    Dictionary<string, int> costs = new Dictionary<string, int>();

    directionsToCheck.Enqueue(new DirectionStep(startRow, startColumn, Direction.ORIGIN, 1), 0);
    costs.Add($"{startRow},{startColumn},{Direction.ORIGIN},{1}", 0);

    while (directionsToCheck.Count > 0)
    {
      DirectionStep currentStep = directionsToCheck.Dequeue();
      int row = currentStep.Row;
      int column = currentStep.Column;
      Direction direction = currentStep.Direction;
      int straightLineCount = currentStep.StraightLineCount;

      string key = $"{row},{column},{direction},{straightLineCount}";

      // We can only travel at most 10 spaces before needing to change direction
      if (straightLineCount > 10)
      {
        continue;
      }

      // Stop when we reach our goal
      // We must travel at least 4 spaces to reach the goal
      // Using A* we can make the assumption that the first time we find the target is the best
      if (row == targetRow && column == targetColumn && straightLineCount >= 4)
      {
        return costs[key];
      }

      List<GridCoordinate> neighbours = getNeighbours(row, column, direction, straightLineCount);
      foreach (GridCoordinate neighbour in neighbours)
      {
        if (!isValidCoordinate(grid, neighbour.Row, neighbour.Column))
        {
          continue;
        }
        int newStraightLineCost = straightLineCount + 1;
        if (direction != neighbour.Direction)
        {
          // If we change direction, reset the straight line count
          newStraightLineCost = 1;
        }
        string neighbourKey = $"{neighbour.Row},{neighbour.Column},{neighbour.Direction},{newStraightLineCost}";
        int newCost = costs[key] + grid[neighbour.Row][neighbour.Column];

        if (!costs.ContainsKey(neighbourKey) || newCost < costs[neighbourKey])
        {
          costs[neighbourKey] = newCost;
          // Use Manhattan distance as our heuristic function for A*
          int priority = newCost + getManhattanDistance(neighbour.Row, neighbour.Column, targetRow, targetColumn);
          directionsToCheck.Enqueue(new DirectionStep(neighbour.Row, neighbour.Column, neighbour.Direction, newStraightLineCost), newCost);
        }
      }
    }

    throw new Exception($"Could not find valid path from ({startRow},{startColumn}) to ({targetRow},{targetColumn})");
  }

  static bool isValidCoordinate(List<List<int>> grid, int row, int column)
  {
    return row >= 0 && row < grid.Count && column >= 0 && column < grid[row].Count;
  }

  static int getManhattanDistance(int startRow, int startColumn, int targetRow, int targetColumn)
  {
    return Math.Abs(startRow - targetRow) + Math.Abs(startColumn - targetColumn);
  }

  static List<GridCoordinate> getNeighbours(int row, int column, Direction direction, int straightLineCount)
  {
    List<GridCoordinate> neighbours = new List<GridCoordinate>();

    switch (direction)
    {
      case Direction.UP:
        // Move up, left, and right
        neighbours.Add(new GridCoordinate(row - 1, column, Direction.UP));
        // We must travel at least 4 spaces before changing direction
        if (straightLineCount >= 4)
        {
          neighbours.Add(new GridCoordinate(row, column + 1, Direction.RIGHT));
          neighbours.Add(new GridCoordinate(row, column - 1, Direction.LEFT));
        }
        break;
      case Direction.DOWN:
        // Move down, left, and right
        neighbours.Add(new GridCoordinate(row + 1, column, Direction.DOWN));
        // We must travel at least 4 spaces before changing direction
        if (straightLineCount >= 4)
        {
          neighbours.Add(new GridCoordinate(row, column + 1, Direction.RIGHT));
          neighbours.Add(new GridCoordinate(row, column - 1, Direction.LEFT));
        }
        break;
      case Direction.LEFT:
        // Move left, up, and down
        neighbours.Add(new GridCoordinate(row, column - 1, Direction.LEFT));
        // We must travel at least 4 spaces before changing direction
        if (straightLineCount >= 4)
        {
          neighbours.Add(new GridCoordinate(row + 1, column, Direction.DOWN));
          neighbours.Add(new GridCoordinate(row - 1, column, Direction.UP));
        }
        break;
      case Direction.RIGHT:
        // Move right, up, and down
        neighbours.Add(new GridCoordinate(row, column + 1, Direction.RIGHT));
        // We must travel at least 4 spaces before changing direction
        if (straightLineCount >= 4)
        {
          neighbours.Add(new GridCoordinate(row + 1, column, Direction.DOWN));
          neighbours.Add(new GridCoordinate(row - 1, column, Direction.UP));
        }
        break;
      default:
        // Move left, right, up, and down
        neighbours.Add(new GridCoordinate(row, column + 1, Direction.RIGHT));
        neighbours.Add(new GridCoordinate(row, column - 1, Direction.LEFT));
        neighbours.Add(new GridCoordinate(row + 1, column, Direction.DOWN));
        neighbours.Add(new GridCoordinate(row - 1, column, Direction.UP));
        break;
    }

    return neighbours;
  }
}

enum Direction
{
  LEFT,
  RIGHT,
  UP,
  DOWN,
  ORIGIN,
}

class DirectionStep
{
  public int Row { get; set; }
  public int Column { get; set; }
  public Direction Direction { get; set; }
  public int StraightLineCount { get; set; }

  public DirectionStep(int row, int column, Direction direction, int straightLineCount)
  {
    Row = row;
    Column = column;
    Direction = direction;
    StraightLineCount = straightLineCount;
  }
}

class GridCoordinate
{
  public int Row { get; set; }
  public int Column { get; set; }
  public Direction Direction { get; set; }

  public GridCoordinate(int row, int column, Direction direction)
  {
    Row = row;
    Column = column;
    Direction = direction;
  }
}
