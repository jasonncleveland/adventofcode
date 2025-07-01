using System;
using System.Collections.Generic;
using System.IO;

class TreetopTreeHouse
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

        List<List<int>> forest = new List<List<int>>();
        foreach (string line in lines)
        {
          List<int> row = new List<int>();
          foreach (char tree in line)
          {
            row.Add((int)tree - '0');
          }
          forest.Add(row);
        }

        int highestTreeScore = 0;

        for (int i = 0; i < forest.Count; i++)
        {
          for (int j = 0; j < forest[i].Count; j++)
          {
            int treeScore = getTreeScore(forest, i, j);
            if (treeScore > highestTreeScore)
            {
              highestTreeScore = treeScore;
            }
          }
        }

        Console.WriteLine($"Highest tree score: {highestTreeScore}");
      }
    }
  }

  static int getTreeScore(List<List<int>> forest, int row, int column)
  {
    // Check each direction
    int height = forest[row][column];
    return getTreeScoreHelper(forest, height, row, column, true, true)
        * getTreeScoreHelper(forest, height, row, column, true, false)
        * getTreeScoreHelper(forest, height, row, column, false, true)
        * getTreeScoreHelper(forest, height, row, column, false, false);
  }

  static int getTreeScoreHelper(List<List<int>> forest, int height, int row, int column, bool checkRow, bool incIndex)
  {
    if (checkRow)
    {
      // Calculate score for current row
      if (column == 0 || column == forest.Count - 1)
      {
        return 0;
      }
      else
      {
        int nextColumn = column + ((incIndex) ? 1 : -1);
        if (forest[row][nextColumn] >= height)
        {
          return 1;
        }
        else {
          return 1 + getTreeScoreHelper(forest, height, row, nextColumn, checkRow, incIndex);
        }
      }
    }
    else
    {
      // Calculate score for current column
      if (row == 0 || row == forest.Count - 1)
      {
        return 0;
      }
      else
      {
        int nextRow = row + ((incIndex) ? 1 : -1);
        if (forest[nextRow][column] >= height)
        {
          return 1;
        }
        else {
          return 1 + getTreeScoreHelper(forest, height, nextRow, column, checkRow, incIndex);
        }
      }
    }
  }
}
