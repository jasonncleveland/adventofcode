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

        int numVisibleTrees = 0;

        for (int i = 0; i < forest.Count; i++)
        {
          for (int j = 0; j < forest[i].Count; j++)
          {
            if (isTreeVisible(forest, i, j))
            {
              numVisibleTrees++;
            }
          }
        }

        Console.WriteLine($"Number of visible trees: {numVisibleTrees}");
      }
    }
  }

  static bool isTreeVisible(List<List<int>> forest, int row, int column)
  {
    // Check each direction (short circuit if we find a clear path)
    int height = forest[row][column];
    return isTreeVisibleHelper(forest, height, row, column, true, true)
        || isTreeVisibleHelper(forest, height, row, column, true, false)
        || isTreeVisibleHelper(forest, height, row, column, false, true)
        || isTreeVisibleHelper(forest, height, row, column, false, false);
  }

  static bool isTreeVisibleHelper(List<List<int>> forest, int height, int row, int column, bool checkRow, bool incIndex)
  {
    if (checkRow)
    {
      // Check if visible on current row
      if (column == 0 || column == forest.Count - 1)
      {
        return true;
      }
      else
      {
        int nextColumn = column + ((incIndex) ? 1 : -1);
        return height > forest[row][nextColumn] && isTreeVisibleHelper(forest, height, row, nextColumn, checkRow, incIndex);
      }
    }
    else
    {
      // Check if visible on current column
      if (row == 0 || row == forest.Count - 1)
      {
        return true;
      }
      else
      {
        int nextRow = row + ((incIndex) ? 1 : -1);
        return height > forest[nextRow][column] && isTreeVisibleHelper(forest, height, nextRow, column, checkRow, incIndex);
      }
    }
  }
}
