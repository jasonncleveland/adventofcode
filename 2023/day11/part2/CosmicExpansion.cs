using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

class CosmicExpansion
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      // Expansion factor is the number of times larger a universe is
      int expansionFactor = args.Length > 1 ? int.Parse(args[1]) : 1000000;
      if (File.Exists(fileName))
      {
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();

        string[] lines = File.ReadAllLines(fileName);

        List<List<char>> universe = new List<List<char>>();
        List<int> rowsToDuplicate = new List<int>();
        List<int> columnsToDuplicate = new List<int>();
        List<SpacePosition> galaxies = new List<SpacePosition>();

        // Parse input
        int rowIndex = 0;
        foreach (string line in lines)
        {
          universe.Add(new List<char>(line));
          // Find empty rows
          if (line.All(character => character == '.'))
          {
            rowsToDuplicate.Add(rowIndex);
          }
          rowIndex++;
        }

        // Find empty columns
        for (int column = 0; column < universe[0].Count; column++)
        {
          if (universe.All(row => row[column] == '.'))
          {
            columnsToDuplicate.Add(column);
          }
        }

        // Find galaxies
        int galaxyCount = 1;
        for (int row = 0; row < universe.Count; row++)
        {
          for (int column = 0; column < universe[row].Count; column++)
          {
            if (universe[row][column] == '#')
            {
              galaxies.Add(new SpacePosition(galaxyCount, row, column));
              galaxyCount += 1;
            }
          }
        }

        Console.WriteLine($"Found {galaxies.Count} galaxies resulting in {galaxies.Count * (galaxies.Count - 1) / 2} pairs");

        // The expanded universe is X times larger. That means we expand X - 1 times
        long total = calculateAllPathLengths(universe, rowsToDuplicate, columnsToDuplicate, expansionFactor - 1, galaxies);
        Console.WriteLine($"Total value: {total}");

        stopWatch.Stop();
        Console.WriteLine($"Elapsed Time: {stopWatch.ElapsedMilliseconds} ms");
      }
    }
  }

  static long calculateAllPathLengths(List<List<char>> universe, List<int> rowsToDuplicate, List<int> columnsToDuplicate, int expansionFactor, List<SpacePosition> galaxies)
  {
    HashSet<string> checkedPaths = new HashSet<string>();
    long totalPathLength = 0;

    // For each galaxy, find the distance between all other galaxies
    foreach (SpacePosition galaxy in galaxies)
    {
      foreach (SpacePosition otherGalaxy in galaxies)
      {
        // Don't find path between the same galaxy
        if (galaxy == otherGalaxy) continue;
        if (checkedPaths.Contains($"{galaxy.Id},{otherGalaxy.Id}")) continue;

        // Mark that the path has been checked in both directions
        checkedPaths.Add($"{galaxy.Id},{otherGalaxy.Id}");
        checkedPaths.Add($"{otherGalaxy.Id},{galaxy.Id}");
        int shortestPath = findShortestPath(galaxy, otherGalaxy, rowsToDuplicate, columnsToDuplicate, expansionFactor);
        totalPathLength += shortestPath;
      }
    }

    return totalPathLength;
  }

  static int findShortestPath(SpacePosition start, SpacePosition target, List<int> rowsToDuplicate, List<int> columnsToDuplicate, int expansionFactor)
  {
    // Find the number of empty rows before the given galaxy positions
    List<int> duplicateRowsStart = rowsToDuplicate.Where(row => row < start.Row).ToList();
    List<int> duplicateColumnsStart = columnsToDuplicate.Where(column => column < start.Column).ToList();
    List<int> duplicateRowsTarget = rowsToDuplicate.Where(row => row < target.Row).ToList();
    List<int> duplicateColumnsTarget = columnsToDuplicate.Where(column => column < target.Column).ToList();
    // Translate the original coordinates to the expanded coordinates
    int startRow = start.Row + duplicateRowsStart.Count * expansionFactor;
    int startColumn = start.Column + duplicateColumnsStart.Count * expansionFactor;
    int targetRow = target.Row + duplicateRowsTarget.Count * expansionFactor;
    int targetColumn = target.Column + duplicateColumnsTarget.Count * expansionFactor;
    // Calculate the Manhatten distance (|x1 - x2| + |y1 - y2|)
    return Math.Abs(targetRow - startRow) + Math.Abs(targetColumn - startColumn);
  }
}

class SpacePosition
{
  public int Id { get; set; }
  public int Row { get; set; }
  public int Column { get; set; }
  public int Distance { get; set; }

  public SpacePosition(int id, int row, int column, int distance = 0)
  {
    Id = id;
    Row = row;
    Column = column;
    Distance = distance;
  }
}
