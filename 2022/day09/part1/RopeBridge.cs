using System;
using System.Collections.Generic;
using System.IO;

class RopeBridge
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

        HashSet<string> visitedSpots = new HashSet<string>();

        // Stored as X, Y
        int[] currentHeadPosition = { 0, 0 };
        int[] currentTailPosition = { 0, 0 };
        int[][] currentPositions = { currentHeadPosition, currentTailPosition };

        visitedSpots.Add(string.Join(",", currentHeadPosition));

        foreach (string line in lines)
        {
          // Console.WriteLine(line);
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
    int[] currentHeadPosition = currentPositions[0];
    int[] currentTailPosition = currentPositions[1];

    if (move == "L" || move == "R")
    {
      int multiplier = (move == "R") ? 1 : -1;
      for (int i = 0; i < spaces; i++)
      {
        currentHeadPosition[0] += multiplier;
        if (!isTailValid(currentHeadPosition, currentTailPosition))
        {
          currentTailPosition[0] = currentHeadPosition[0] - multiplier;
          currentTailPosition[1] = currentHeadPosition[1];
        }
        visitedSpots.Add(string.Join(",", currentTailPosition));
      }
    }
    else if (move == "U" || move == "D")
    {
      int multiplier = (move == "U") ? 1 : -1;
      for (int i = 0; i < spaces; i++)
      {
        currentHeadPosition[1] += multiplier;
        if (!isTailValid(currentHeadPosition, currentTailPosition))
        {
          currentTailPosition[0] = currentHeadPosition[0];
          currentTailPosition[1] = currentHeadPosition[1] - multiplier;
        }
        visitedSpots.Add(string.Join(",", currentTailPosition));
      }
    }
    else
    {
      Console.WriteLine("invalid move option");
    }
  }

  static bool isTailValid(int[] currentHeadPosition, int[] currentTailPosition)
  {
    int xDiff = Math.Abs(currentHeadPosition[0] - currentTailPosition[0]);
    int yDiff = Math.Abs(currentHeadPosition[1] - currentTailPosition[1]);
    return !(xDiff > 1 || yDiff > 1);
  }
}
