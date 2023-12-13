using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

class PointOfIncidence
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

        List<List<List<char>>> patterns = new List<List<List<char>>>();

        // Parse input
        List<List<char>> newPattern = new List<List<char>>();
        foreach (string line in lines)
        {
          if (String.IsNullOrEmpty(line))
          {
            patterns.Add(newPattern);
            newPattern = new List<List<char>>();
          }
          else
          {
            newPattern.Add(new List<char>(line));
          }
        }
        patterns.Add(newPattern);

        // Search for a perfect reflection
        // A perfect reflection extends all the way to at least one edge
        long total = 0;
        foreach (List<List<char>> pattern in patterns)
        {
          string mirrorDirection = null;
          int reflectionLength = 0;
          bool foundPerfectReflection = false;

          // Check rows
          for (int row = 1; row < pattern.Count; row++)
          {
            // Find equal rows
            List<char> row1 = pattern[row - 1];
            List<char> row2 = pattern[row];
            bool areRowsEqual = true;
            for (int column = 0; column < pattern[row].Count; column++)
            {
              if (row1[column] != row2[column])
              {
                areRowsEqual = false;
                break;
              }
            }
            // Check for perfect reflection
            if (areRowsEqual)
            {
              int reflection = 0;
              bool isPerfectReflection = true;
              for (int rowOffset = 1; rowOffset < pattern.Count; rowOffset++)
              {
                // If either side goes out of bounds then we can finish
                if ((row - rowOffset - 1) < 0 || (row + rowOffset) >= pattern.Count)
                {
                  // We only care about the number of rows above the mirror line
                  reflection = row;
                  break;
                }
                List<char> offsetRow1 = pattern[row - rowOffset - 1];
                List<char> offsetRow2 = pattern[row + rowOffset];
                bool areOffsetRowsEqual = true;
                for (int column = 0; column < pattern[row].Count; column++)
                {
                  if (offsetRow1[column] != offsetRow2[column])
                  {
                    areOffsetRowsEqual = false;
                    break;
                  }
                }
                if (!areOffsetRowsEqual)
                {
                  isPerfectReflection = false;
                  break;
                }
              }
              // This assumes there is only one perfect reflection in each pattern
              if (isPerfectReflection)
              {
                foundPerfectReflection = true;
                reflectionLength = reflection;
                mirrorDirection = "rows";
                break;
              }
            }
          }

          // Check columns
          for (int column = 1; column < pattern[0].Count; column++)
          {
            // Don't bother checking if we've already found a perfect reflection
            if (foundPerfectReflection) break;

            // Find equal columns
            bool areColumnsEqual = true;
            for (int row = 0; row < pattern.Count; row++)
            {
              if (pattern[row][column - 1] != pattern[row][column])
              {
                areColumnsEqual = false;
                break;
              }
            }
            // Check for perfect reflection
            if (areColumnsEqual)
            {
              int reflection = 0;
              bool isPerfectReflection = true;
              for (int columnOffset = 1; columnOffset < pattern[0].Count; columnOffset++)
              {
                // If either side goes out of bounds then we can finish
                if ((column - columnOffset - 1) < 0 || (column + columnOffset) >= pattern[0].Count)
                {
                  // We only care about the number of columns left of the mirror line
                  reflection = column;
                  break;
                }
                bool areOffsetColumnsEqual = true;
                for (int row = 0; row < pattern.Count; row++)
                {
                  if (pattern[row][column - columnOffset - 1] != pattern[row][column + columnOffset])
                  {
                    areOffsetColumnsEqual = false;
                    break;
                  }
                }
                if (!areOffsetColumnsEqual)
                {
                  isPerfectReflection = false;
                  break;
                }
              }
              // This assumes there is only one perfect reflection in each pattern
              if (isPerfectReflection)
              {
                foundPerfectReflection = true;
                reflectionLength = reflection;
                mirrorDirection = "columns";
                break;
              }
            }
          }

          // Only increment the total if a perfect reflection is found
          if (foundPerfectReflection)
          {
            switch (mirrorDirection)
            {
              case "rows":
                total += reflectionLength * 100;
                break;
              case "columns":
                total += reflectionLength;
                break;
              default:
                throw new Exception($"Invalid mirror direction '{mirrorDirection}'");
            }
          }
        }
        Console.WriteLine($"Total value: {total}");

        stopWatch.Stop();
        Console.WriteLine($"Elapsed Time: {stopWatch.ElapsedMilliseconds} ms");
      }
    }
  }
}
