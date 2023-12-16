using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

class TheFloorWillBeLava
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

        foreach (string line in lines)
        {
          grid.Add(new List<char>(line));
        }

        // Assume the grid is a valid rectangle
        int maxRow = grid.Count;
        int maxColumn = grid[0].Count;
        int maxTotal = int.MinValue;
        List<Thread> threads = new List<Thread>();
        for (int row = 0; row < maxRow; row++)
        {
          // Check all left side entries
          Thread leftThread = new Thread(() => {
            int leftTotal = traverseGrid(grid, row, 0, Direction.RIGHT);
            if (leftTotal > maxTotal) maxTotal = leftTotal;
          });
          leftThread.Start();
          threads.Add(leftThread);

          // Check all right side entries
          Thread rightThread = new Thread(() => {
            int rightTotal = traverseGrid(grid, row, maxColumn - 1, Direction.LEFT);
            if (rightTotal > maxTotal) maxTotal = rightTotal;
          });
          rightThread.Start();
          threads.Add(rightThread);
        }
        for (int column = 0; column < maxColumn; column++)
        {
          // Check all top side entries
          Thread topThread = new Thread(() => {
            int topTotal = traverseGrid(grid, 0, column, Direction.DOWN);
            if (topTotal > maxTotal) maxTotal = topTotal;
          });
          topThread.Start();
          threads.Add(topThread);

          // Check all bottom side entries
          Thread bottomThread = new Thread(() => {
            int bottomTotal = traverseGrid(grid, maxRow, column, Direction.UP);
            if (bottomTotal > maxTotal) maxTotal = bottomTotal;
          });
          bottomThread.Start();
          threads.Add(bottomThread);
        }
        foreach (Thread thread in threads)
        {
          thread.Join();
        }
        Console.WriteLine($"Total value: {maxTotal}");

        stopWatch.Stop();
        Console.WriteLine($"Elapsed Time: {stopWatch.ElapsedMilliseconds} ms");
      }
    }
  }

  static int traverseGrid(List<List<char>> grid, int row, int column, Direction direction)
  {
    Queue<DirectionStep> directionsToCheck = new Queue<DirectionStep>();
    HashSet<string> checkedDirections = new HashSet<string>();
    HashSet<string> energizedCoordinates = new HashSet<string>();

    directionsToCheck.Enqueue(new DirectionStep(row, column, direction));
    checkedDirections.Add($"{row},{column},{direction}");

    while (directionsToCheck.Count > 0)
    {
      DirectionStep currentStep = directionsToCheck.Dequeue();
      row = currentStep.Row;
      column = currentStep.Column;
      direction = currentStep.Direction;

      if (!isValidCoordinate(grid, row, column))
      {
        continue;
      }
      energizedCoordinates.Add($"{row},{column}");

      switch (grid[row][column])
      {
        case '.':
          // Continue in the same direction
          if (!checkedDirections.Contains($"{getNextRow(row, direction)},{getNextColumn(column, direction)},{direction}"))
          {
            directionsToCheck.Enqueue(new DirectionStep(getNextRow(row, direction), getNextColumn(column, direction), direction));
            checkedDirections.Add($"{getNextRow(row, direction)},{getNextColumn(column, direction)},{direction}");
          }
          break;
        case '\\':
          if (direction == Direction.LEFT)
          {
            if (!checkedDirections.Contains($"{getNextRow(row, Direction.UP)},{column},{Direction.UP}"))
            {
              directionsToCheck.Enqueue(new DirectionStep(getNextRow(row, Direction.UP), column, Direction.UP));
              checkedDirections.Add($"{getNextRow(row, Direction.UP)},{column},{Direction.UP}");
            }
            break;
          }
          else if (direction == Direction.RIGHT)
          {
            if (!checkedDirections.Contains($"{getNextRow(row, Direction.DOWN)},{column},{Direction.DOWN}"))
            {
              directionsToCheck.Enqueue(new DirectionStep(getNextRow(row, Direction.DOWN), column, Direction.DOWN));
              checkedDirections.Add($"{getNextRow(row, Direction.DOWN)},{column},{Direction.DOWN}");
            }
            break;
          }
          else if (direction == Direction.UP)
          {
            if (!checkedDirections.Contains($"{row},{getNextColumn(column, Direction.LEFT)},{Direction.LEFT}"))
            {
              directionsToCheck.Enqueue(new DirectionStep(row, getNextColumn(column, Direction.LEFT), Direction.LEFT));
              checkedDirections.Add($"{row},{getNextColumn(column, Direction.LEFT)},{Direction.LEFT}");
            }
            break;
          }
          else if (direction == Direction.DOWN)
          {
            if (!checkedDirections.Contains($"{row},{getNextColumn(column, Direction.RIGHT)},{Direction.RIGHT}"))
            {
              directionsToCheck.Enqueue(new DirectionStep(row, getNextColumn(column, Direction.RIGHT), Direction.RIGHT));
              checkedDirections.Add($"{row},{getNextColumn(column, Direction.RIGHT)},{Direction.RIGHT}");
            }
            break;
          }
          else
          {
            throw new Exception($"Unexpected direction entering the '\\' mirror {direction}");
          }
        case '/':
          if (direction == Direction.LEFT)
          {
            if (!checkedDirections.Contains($"{getNextRow(row, Direction.DOWN)},{column},{Direction.DOWN}"))
            {
              directionsToCheck.Enqueue(new DirectionStep(getNextRow(row, Direction.DOWN), column, Direction.DOWN));
              checkedDirections.Add($"{getNextRow(row, Direction.DOWN)},{column},{Direction.DOWN}");
            }
            break;
          }
          else if (direction == Direction.RIGHT)
          {
            if (!checkedDirections.Contains($"{getNextRow(row, Direction.UP)},{column},{Direction.UP}"))
            {
              directionsToCheck.Enqueue(new DirectionStep(getNextRow(row, Direction.UP), column, Direction.UP));
              checkedDirections.Add($"{getNextRow(row, Direction.UP)},{column},{Direction.UP}");
            }
            break;
          }
          else if (direction == Direction.UP)
          {
            if (!checkedDirections.Contains($"{row},{getNextColumn(column, Direction.RIGHT)},{Direction.RIGHT}"))
            {
              directionsToCheck.Enqueue(new DirectionStep(row, getNextColumn(column, Direction.RIGHT), Direction.RIGHT));
              checkedDirections.Add($"{row},{getNextColumn(column, Direction.RIGHT)},{Direction.RIGHT}");
            }
            break;
          }
          else if (direction == Direction.DOWN)
          {
            if (!checkedDirections.Contains($"{row},{getNextColumn(column, Direction.LEFT)},{Direction.LEFT}"))
            {
              directionsToCheck.Enqueue(new DirectionStep(row, getNextColumn(column, Direction.LEFT), Direction.LEFT));
              checkedDirections.Add($"{row},{getNextColumn(column, Direction.LEFT)},{Direction.LEFT}");
            }
            break;
          }
          else
          {
            throw new Exception($"Unexpected direction entering the '/' mirror {direction}");
          }
        case '-':
          if (direction == Direction.LEFT || direction == Direction.RIGHT)
          {
            // Continue in the same direction
            if (!checkedDirections.Contains($"{row},{getNextColumn(column, direction)},{direction}"))
            {
              directionsToCheck.Enqueue(new DirectionStep(row, getNextColumn(column, direction), direction));
              checkedDirections.Add($"{row},{getNextColumn(column, direction)},{direction}");
            }
            break;
          }
          else
          {
            // Split the beam left and right
            if (!checkedDirections.Contains($"{row},{getNextColumn(column, Direction.LEFT)},{Direction.LEFT}"))
            {
              directionsToCheck.Enqueue(new DirectionStep(row, getNextColumn(column, Direction.LEFT), Direction.LEFT));
              checkedDirections.Add($"{row},{getNextColumn(column, Direction.LEFT)},{Direction.LEFT}");
            }
            if (!checkedDirections.Contains($"{row},{getNextColumn(column, Direction.RIGHT)},{Direction.RIGHT}"))
            {
              directionsToCheck.Enqueue(new DirectionStep(row, getNextColumn(column, Direction.RIGHT), Direction.RIGHT));
              checkedDirections.Add($"{row},{getNextColumn(column, Direction.RIGHT)},{Direction.RIGHT}");
            }
            break;
          }
        case '|':
          if (direction == Direction.UP || direction == Direction.DOWN)
          {
            // Continue in the same direction
            if (!checkedDirections.Contains($"{getNextRow(row, direction)},{column},{direction}"))
            {
              directionsToCheck.Enqueue(new DirectionStep(getNextRow(row, direction), column, direction));
              checkedDirections.Add($"{getNextRow(row, direction)},{column},{direction}");
            }
            break;
          }
          else
          {
            // Split the beam up and down
            if (!checkedDirections.Contains($"{getNextRow(row, Direction.UP)},{column},{Direction.UP}"))
            {
              directionsToCheck.Enqueue(new DirectionStep(getNextRow(row, Direction.UP), column, Direction.UP));
              checkedDirections.Add($"{getNextRow(row, Direction.UP)},{column},{Direction.UP}");
            }
            if (!checkedDirections.Contains($"{getNextRow(row, Direction.DOWN)},{column},{Direction.DOWN}"))
            {
              directionsToCheck.Enqueue(new DirectionStep(getNextRow(row, Direction.DOWN), column, Direction.DOWN));
              checkedDirections.Add($"{getNextRow(row, Direction.DOWN)},{column},{Direction.DOWN}");
            }
            break;
          }
        default:
          throw new Exception($"Found invalid character: '{grid[row][column]}'");
      }
    }
    return energizedCoordinates.Count;
  }

  static bool isValidCoordinate(List<List<char>> grid, int row, int column)
  {
    return row >= 0 && row < grid.Count && column >= 0 && column < grid[row].Count;
  }

  static int getNextRow(int row, Direction direction)
  {
    switch (direction)
    {
      case Direction.LEFT:
        return row;
      case Direction.RIGHT:
        return row;
      case Direction.UP:
        return row - 1;
      case Direction.DOWN:
        return row + 1;
      default:
        throw new Exception($"Found invalid direction: '{direction}'");
    }
  }

  static int getNextColumn(int column, Direction direction)
  {
    switch (direction)
    {
      case Direction.LEFT:
        return column - 1;
      case Direction.RIGHT:
        return column + 1;
      case Direction.UP:
        return column;
      case Direction.DOWN:
        return column;
      default:
        throw new Exception($"Found invalid direction: '{direction}'");
    }
  }
}

enum Direction
{
  LEFT,
  RIGHT,
  UP,
  DOWN,
}

class DirectionStep
{
  public int Row { get; set; }
  public int Column { get; set; }
  public Direction Direction { get; set; }

  public DirectionStep(int row, int column, Direction direction)
  {
    Row = row;
    Column = column;
    Direction = direction;
  }
}
