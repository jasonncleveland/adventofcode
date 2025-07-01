using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

class GearRatios
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

        int totalPartNumberSum = 0;
        List<char> currentPartNumber = new List<char>();
        bool isValidPartNumber = false;
        for (int column = 0; column < lines.Length; column++)
        {
          for (int row = 0; row < lines[column].Length; row++)
          {
            char c = lines[column][row];
            if (char.IsDigit(c))
            {
              currentPartNumber.Add(c);
              if (!isValidPartNumber)
              {
                // Check all neighbours for symbols
                isValidPartNumber = checkNeighbours(lines, column, row);
              }
            }
            else
            {
              if (currentPartNumber.Count > 0)
              {
                if (isValidPartNumber)
                {
                  totalPartNumberSum += int.Parse(string.Join("", currentPartNumber));
                }
                currentPartNumber = new List<char>();
                isValidPartNumber = false;
              }
            }
          }
          // Handle case where there is a number at the end of a line
          if (currentPartNumber.Count > 0)
          {
            if (isValidPartNumber)
            {
              totalPartNumberSum += int.Parse(string.Join("", currentPartNumber));
            }
            currentPartNumber = new List<char>();
            isValidPartNumber = false;
          }
        }
        Console.WriteLine($"Total value: {totalPartNumberSum}");

        stopWatch.Stop();
        Console.WriteLine($"Elapsed Time: {stopWatch.ElapsedMilliseconds} ms");
      }
    }
  }

  /**
   * Check if the neighbour is not part of the current number and contains a symbol
   */
  static bool checkNeighbours(string[] input, int column, int row)
  {
    // U1 L1
    if (row - 1 >= 0 && column - 1 >= 0 && isSymbol(input, column - 1, row - 1)) return true;
    // U1
    if (column - 1 >= 0 && isSymbol(input, column - 1, row)) return true;
    // U1 R1
    if (row + 1 < input.Length && column - 1 >= 0 && isSymbol(input, column - 1, row + 1)) return true;
    // L1
    if (row - 1 >= 0 && isSymbol(input, column, row - 1)) return true;
    // R1
    if (row + 1 < input.Length && isSymbol(input, column, row + 1)) return true;
    // D1 L1
    if (row - 1 >= 0 && column + 1 < input[column].Length && isSymbol(input, column + 1, row - 1)) return true;
    // D1
    if (column + 1 < input[column].Length && isSymbol(input, column + 1, row)) return true;
    // D1 R1
    if (row + 1 < input.Length && column + 1 < input[column].Length && isSymbol(input, column + 1, row + 1)) return true;

    return false;
  }

  static bool isSymbol(string[] input, int column, int row)
  {
    return !char.IsDigit(input[column][row]) && input[column][row] != '.';
  }
}
