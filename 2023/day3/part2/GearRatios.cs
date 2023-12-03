using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

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

        int totalGearRatioSum = 0;
        for (int column = 0; column < lines.Length; column++)
        {
          for (int row = 0; row < lines[column].Length; row++)
          {
            char c = lines[column][row];
            if (c == '*')
            {
              List<int> adjacentPartNumbers = checkNeighbours(lines, column, row);
              if (adjacentPartNumbers.Count == 2)
              {
                totalGearRatioSum += adjacentPartNumbers.Aggregate(1, (product, partNumber) => product * partNumber);
              }
            }
          }
        }
        Console.WriteLine($"Total value: {totalGearRatioSum}");

        stopWatch.Stop();
        Console.WriteLine($"Elapsed Time: {stopWatch.ElapsedMilliseconds} ms");
      }
    }
  }

  /**
   * Check if the neighbour is a a digit
   */
  static List<int> checkNeighbours(string[] input, int column, int row)
  {
    List<int> adjacentPartNumbers = new List<int>();
    HashSet<string> checkedCoordinates = new HashSet<string>();
    // U1 L1
    if (row - 1 >= 0 && column - 1 >= 0 && isPartNumber(input, column - 1, row - 1))
    {
      if (!checkedCoordinates.Contains($"{column - 1},{row - 1}"))
      {
        int foundNumber = getPartNumber(input, column - 1, row - 1, checkedCoordinates);
        adjacentPartNumbers.Add(foundNumber);
      }
    }
    // U1
    if (column - 1 >= 0 && isPartNumber(input, column - 1, row))
    {
      if (!checkedCoordinates.Contains($"{column - 1},{row}"))
      {
        int foundNumber = getPartNumber(input, column - 1, row, checkedCoordinates);
        adjacentPartNumbers.Add(foundNumber);
      }
    }
    // U1 R1
    if (row + 1 < input.Length && column - 1 >= 0 && isPartNumber(input, column - 1, row + 1))
    {
      if (!checkedCoordinates.Contains($"{column - 1},{row + 1}"))
      {
        int foundNumber = getPartNumber(input, column - 1, row + 1, checkedCoordinates);
        adjacentPartNumbers.Add(foundNumber);
      }
    }
    // L1
    if (row - 1 >= 0 && isPartNumber(input, column, row - 1))
    {
      if (!checkedCoordinates.Contains($"{column},{row - 1}"))
      {
        int foundNumber = getPartNumber(input, column, row - 1, checkedCoordinates);
        adjacentPartNumbers.Add(foundNumber);
      }
    }
    // R1
    if (row + 1 < input.Length && isPartNumber(input, column, row + 1))
    {
      if (!checkedCoordinates.Contains($"{column},{row + 1}"))
      {
        int foundNumber = getPartNumber(input, column, row + 1, checkedCoordinates);
        adjacentPartNumbers.Add(foundNumber);
      }
    }
    // D1 L1
    if (row - 1 >= 0 && column + 1 < input[column].Length && isPartNumber(input, column + 1, row - 1))
    {
      if (!checkedCoordinates.Contains($"{column + 1},{row - 1}"))
      {
        int foundNumber = getPartNumber(input, column + 1, row - 1, checkedCoordinates);
        adjacentPartNumbers.Add(foundNumber);
      }
    }
    // D1
    if (column + 1 < input[column].Length && isPartNumber(input, column + 1, row))
    {
      if (!checkedCoordinates.Contains($"{column + 1},{row}"))
      {
        int foundNumber = getPartNumber(input, column + 1, row, checkedCoordinates);
        adjacentPartNumbers.Add(foundNumber);
      }
    }
    // D1 R1
    if (row + 1 < input.Length && column + 1 < input[column].Length && isPartNumber(input, column + 1, row + 1))
    {
      if (!checkedCoordinates.Contains($"{column + 1},{row + 1}"))
      {
        int foundNumber = getPartNumber(input, column + 1, row + 1, checkedCoordinates);
        adjacentPartNumbers.Add(foundNumber);
      }
    }

    return adjacentPartNumbers;
  }

  static bool isPartNumber(string[] input, int column, int row)
  {
    return char.IsDigit(input[column][row]);
  }

  static int getPartNumber(string[] input, int column, int row, HashSet<string> checkedCoordinates)
  {
    List<char> partNumber = new List<char>();
    int index;

    // Add starting digit
    partNumber.Add(input[column][row]);
    checkedCoordinates.Add($"{column},{row}");

    // Get digits to the left
    index = row - 1;
    while (index >= 0 && char.IsDigit(input[column][index]))
    {
      partNumber.Insert(0, input[column][index]);
      checkedCoordinates.Add($"{column},{index}");
      index--;
    }

    // Get digits to the right
    index = row + 1;
    while (index < input[column].Length && char.IsDigit(input[column][index]))
    {
      partNumber.Add(input[column][index]);
      checkedCoordinates.Add($"{column},{index}");
      index++;
    }
    
    return int.Parse(string.Join("", partNumber));
  }
}
