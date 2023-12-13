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
          // The smudge must be fixed for the reflection to be valid
          // There are two cases:
          // - A smudge in the first reflected row XOR a sumdge in one of the offset reflected rows
          // - A smudge in the first reflected column XOR a smudge in one of the offset reflected columns
          string mirrorDirection = null;
          int reflectionLength = 0;
          bool foundPerfectReflection = false;

          // Check rows
          for (int row = 1; row < pattern.Count; row++)
          {
            // Find equal rows
            List<char> row1 = pattern[row - 1];
            List<char> row2 = pattern[row];
            bool fixedSmudge = false;
            int differenceCount = 0;
            for (int column = 0; column < pattern[row].Count; column++)
            {
              if (row1[column] != row2[column])
              {
                differenceCount += 1;
                // Exit early to save some computation
                if (differenceCount > 1) break;
              }
            }
            // Check if there is a smudge that we can fix
            if (differenceCount == 1)
            {
              fixedSmudge = true;
            }
            // Check for perfect reflection
            if (differenceCount <= 1)
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
                int offsetDifferenceCount = 0;
                for (int column = 0; column < pattern[row].Count; column++)
                {
                  if (offsetRow1[column] != offsetRow2[column])
                  {
                    offsetDifferenceCount += 1;
                    // Exit early to save some computation
                    if (differenceCount > 1) break;
                  }
                }

                if (fixedSmudge)
                {
                  // We have already fixed the smudge so any differences means the pattern is invalid
                  if (offsetDifferenceCount > 0)
                  {
                    // The smudge has already been attempted to be fixed so we can stop searching
                    isPerfectReflection = false;
                    break;
                  }
                  else
                  {
                    // There are 0 differences so continue
                  }
                }
                else
                {
                  // We can fix exactly one smudge
                  if (offsetDifferenceCount == 1)
                  {
                    fixedSmudge = true;
                  }
                  else if (offsetDifferenceCount > 1)
                  {
                    // There are too many differences so the pattern is invalid
                    isPerfectReflection = false;
                    break;
                  }
                  else
                  {
                    // There are 0 differences so continue
                  }
                }
              }
              // This assumes there is only one perfect reflection in each pattern
              // We must fix a smudge for the reflection to be valid
              if (fixedSmudge && isPerfectReflection)
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
            bool fixedSmudge = false;
            int differenceCount = 0;
            for (int row = 0; row < pattern.Count; row++)
            {
              if (pattern[row][column - 1] != pattern[row][column])
              {
                differenceCount += 1;
                // Exit early to save some computation
                if (differenceCount > 1) break;
              }
            }
            if (differenceCount == 1)
            {
              fixedSmudge = true;
            }
            // Check for perfect reflection
            if (differenceCount <= 1)
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
                int offsetDifferenceCount = 0;
                for (int row = 0; row < pattern.Count; row++)
                {
                  if (pattern[row][column - columnOffset - 1] != pattern[row][column + columnOffset])
                  {
                    offsetDifferenceCount += 1;
                    // Exit early to save some computation
                    if (differenceCount > 1) break;
                  }
                }

                if (fixedSmudge)
                {
                  // We have already fixed the smudge so any differences means the pattern is invalid
                  if (offsetDifferenceCount > 0)
                  {
                    // The smudge has already been attempted to be fixed so we can stop searching
                    isPerfectReflection = false;
                    break;
                  }
                  else
                  {
                    // There are 0 differences so continue
                  }
                }
                else
                {
                  // We can fix exactly one smudge
                  if (offsetDifferenceCount == 1)
                  {
                    fixedSmudge = true;
                  }
                  else if (offsetDifferenceCount > 1)
                  {
                    // There are too many differences so the pattern is invalid
                    isPerfectReflection = false;
                    break;
                  }
                  else
                  {
                    // There are 0 differences so continue
                  }
                }
              }
              // This assumes there is only one perfect reflection in each pattern
              // We must fix a smudge for the reflection to be valid
              if (fixedSmudge && isPerfectReflection)
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
