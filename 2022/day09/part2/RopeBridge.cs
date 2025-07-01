using System;
using System.Collections.Generic;
using System.IO;

class RopeBridge
{
  static int numTails = 10;
  static int startRow = 0;
  static int startColumn = 0;

  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        string fileContents = File.ReadAllText(fileName);
        string[] lines = fileContents.Split('\n');

        HashSet<string> visitedSpots = new HashSet<string>();

        // Stored as X, Y
        int[][] currentPositions = new int[numTails][];
        for (int i = 0; i < numTails; i++)
        {
          currentPositions[i] = new int[] { startColumn, startRow };
        }

        visitedSpots.Add(string.Join(",", currentPositions[numTails - 1]));

        foreach (string line in lines)
        {
          string[] lineParts = line.Split(' ');
          string move = lineParts[0];
          int spaces = int.Parse(lineParts[1]);

          moveRope(visitedSpots, currentPositions, move, spaces);
        }

        Console.WriteLine($"Total unique spaces visited by the tail: {visitedSpots.Count}");
      }
    }
  }

  static void moveRope(HashSet<string> visitedSpots, int[][] currentPositions, string move, int spaces)
  {
    for (int i = 0; i < spaces; i++)
    {
      switch (move)
      {
        case "R":
          currentPositions[0][0] += 1;
          break;
        case "L":
          currentPositions[0][0] -= 1;
          break;
        case "U":
          currentPositions[0][1] += 1;
          break;
        case "D":
          currentPositions[0][1] -= 1;
          break;
        default:
          Console.WriteLine("invalid move option");
          break;
      }
      moveTails(currentPositions);
      visitedSpots.Add(string.Join(",", currentPositions[numTails - 1]));
    }
  }

  static void moveTails(int[][] currentPositions)
  {
    for (int i = 1; i < numTails; i++)
    {
      if (!isTailValid(currentPositions[i-1], currentPositions[i]))
      {
        moveTail(currentPositions[i-1], currentPositions[i]);
      }
    }
  }

  static void moveTail(int[] currentHeadPosition, int[] currentTailPosition)
  {
    if (currentHeadPosition[0] == currentTailPosition[0])
    {
      if (currentHeadPosition[1] > currentTailPosition[1])
      {
        currentTailPosition[1] += 1;
      }
      else
      {
        currentTailPosition[1] -= 1;
      }
    }
    else if (currentHeadPosition[1] == currentTailPosition[1])
    {
      if (currentHeadPosition[0] > currentTailPosition[0])
      {
        currentTailPosition[0] += 1;
      }
      else
      {
        currentTailPosition[0] -= 1;
      }
    }
    else
    {
      if (isTailValid(currentHeadPosition, new int[] { currentTailPosition[0] + 1, currentTailPosition[1] + 1 }))
      {
        currentTailPosition[0] = currentTailPosition[0] + 1;
        currentTailPosition[1] = currentTailPosition[1] + 1;
      }
      else if (isTailValid(currentHeadPosition, new int[] { currentTailPosition[0] + 1, currentTailPosition[1] - 1 }))
      {
        currentTailPosition[0] = currentTailPosition[0] + 1;
        currentTailPosition[1] = currentTailPosition[1] - 1;
      }
      else if (isTailValid(currentHeadPosition, new int[] { currentTailPosition[0] - 1, currentTailPosition[1] + 1 }))
      {
        currentTailPosition[0] = currentTailPosition[0] - 1;
        currentTailPosition[1] = currentTailPosition[1] + 1;
      }
      else if (isTailValid(currentHeadPosition, new int[] { currentTailPosition[0] - 1, currentTailPosition[1] - 1 }))
      {
        currentTailPosition[0] = currentTailPosition[0] - 1;
        currentTailPosition[1] = currentTailPosition[1] - 1;
      }
      else
      {
        Console.WriteLine("You should never be here");
      }
    }
  }

  static bool isTailValid(int[] currentHeadPosition, int[] currentTailPosition)
  {
    int xDiff = Math.Abs(currentHeadPosition[0] - currentTailPosition[0]);
    int yDiff = Math.Abs(currentHeadPosition[1] - currentTailPosition[1]);
    return !(xDiff > 1 || yDiff > 1);
  }
}
