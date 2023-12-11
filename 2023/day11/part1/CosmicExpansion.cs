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
          // Check and add duplicate row if empty
          if (line.All(character => character == '.'))
          {
            universe.Add(new List<char>(line));
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
        columnsToDuplicate.Reverse();

        // Add duplicate columns
        foreach (List<char> row in universe)
        {
          foreach (int columnToDuplicate in columnsToDuplicate)
          {
            row.Insert(columnToDuplicate, '.');
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

        int total = calculateAllPathLengths(universe, rowsToDuplicate, columnsToDuplicate, galaxies);
        Console.WriteLine($"Total value: {total}");

        stopWatch.Stop();
        Console.WriteLine($"Elapsed Time: {stopWatch.ElapsedMilliseconds} ms");
      }
    }
  }

  static int calculateAllPathLengths(List<List<char>> universe, List<int> rowsToDuplicate, List<int> columnsToDuplicate, List<SpacePosition> galaxies)
  {
    HashSet<string> checkedPaths = new HashSet<string>();
    int totalPathLength = 0;
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
        int shortestPath = findShortestPath(universe, rowsToDuplicate, columnsToDuplicate, galaxy, otherGalaxy);
        totalPathLength += shortestPath;
      }
    }

    return totalPathLength;
  }

  static int findShortestPath(List<List<char>> universe, List<int> rowsToDuplicate, List<int> columnsToDuplicate, SpacePosition start, SpacePosition target)
  {
    Queue<SpacePosition> spacesToVisit = new Queue<SpacePosition>();
    bool[][] visitedSpaces = new bool[universe.Count][];
    for (int row = 0; row < universe.Count; row++)
    {
      visitedSpaces[row] = new bool[universe[row].Count];
      for (int column = 0; column < universe[row].Count; column++)
      {
        visitedSpaces[row][column] = false;
      }
    }

    spacesToVisit.Enqueue(start);
    visitedSpaces[start.Row][start.Column] = true;

    while (spacesToVisit.Count > 0)
    {
      SpacePosition position = spacesToVisit.Dequeue();
      if (position.Row == target.Row && position.Column == target.Column)
      {
        return position.Distance;
      }

      int id = position.Id;
      int row = position.Row;
      int column = position.Column;
      int distance = position.Distance;
      
      int up = row - 1;
      int down = row + 1;
      int left = column - 1;
      int right = column + 1;

      // Check up
      if (
        up >= 0
        && !visitedSpaces[up][column]
      )
      {
        visitedSpaces[up][column] = true;
        spacesToVisit.Enqueue(new SpacePosition(id, up, column, distance + 1));
      }

      // Check down
      if (
        down < universe.Count
        && !visitedSpaces[down][column]
      )
      {
        visitedSpaces[down][column] = true;
        spacesToVisit.Enqueue(new SpacePosition(id, down, column, distance + 1));
      }

      // Check left
      if (
        left >= 0
        && !visitedSpaces[row][left]
      )
      {
        visitedSpaces[row][left] = true;
        spacesToVisit.Enqueue(new SpacePosition(id, row, left, distance + 1));
      }

      // Check right
      if (
        right < universe[row].Count
        && !visitedSpaces[row][right]
      )
      {
        visitedSpaces[row][right] = true;
        spacesToVisit.Enqueue(new SpacePosition(id, row, right, distance + 1));
      }
    }

    throw new Exception($"Could not find path between {start.Row},{start.Column} and {target.Row},{target.Column}");
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
