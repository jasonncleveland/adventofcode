using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

class PipeMaze
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

        List<List<char>> tiles = new List<List<char>>();

        foreach (string line in lines)
        {
          tiles.Add(new List<char>(line));
        }
        traversePipes(tiles);

        stopWatch.Stop();
        Console.WriteLine($"Elapsed Time: {stopWatch.ElapsedMilliseconds} ms");
      }
    }
  }

  static void traversePipes(List<List<char>> tiles)
  {
    int startRow = -1;
    int startColumn = -1;
    for (int row = 0; row < tiles.Count; row++)
    {
      for (int column = 0; column < tiles[row].Count; column++)
      {
        if (tiles[row][column] == 'S')
        {
          startRow = row;
          startColumn = column;
        }
      }
    }
    // Get the starting pipe type and replace S with the pipe type
    char startingPipeType = getStartingPipeType(tiles, startRow, startColumn);
    tiles[startRow][startColumn] = startingPipeType;

    // Do BFS to traverse all pipes
    Queue<CoordinatePosition> coordinatesToVisit = new Queue<CoordinatePosition>();
    HashSet<string> visitedPipes = new HashSet<string>();
    string coordinate = $"{startRow},{startColumn}";
    coordinatesToVisit.Enqueue(new CoordinatePosition(coordinate));
    visitedPipes.Add(coordinate);
    CoordinatePosition farthestPipe = coordinatesToVisit.Peek();
    while (coordinatesToVisit.Count > 0)
    {
      CoordinatePosition coordinatePosition = coordinatesToVisit.Dequeue();
      List<int> coordinates = parseCoordinates(coordinatePosition.Coordinate);
      int row = coordinates[0];
      int column = coordinates[1];
      if (coordinatePosition.Distance > farthestPipe.Distance)
      {
        farthestPipe = coordinatePosition;
      }

      List<string> neighbourPipes = getPipeConnections(getPipe(tiles, row, column), row, column);
      foreach (string neighbourPipe in neighbourPipes)
      {
        if (!visitedPipes.Contains(neighbourPipe))
        {
          coordinatesToVisit.Enqueue(new CoordinatePosition(neighbourPipe, coordinatePosition.Distance + 1));
          visitedPipes.Add(neighbourPipe);
        }
      }
    }
    Console.WriteLine($"Farthest pipe distance at {farthestPipe.Coordinate} is {farthestPipe.Distance}");
  }

  static char getStartingPipeType(List<List<char>> tiles, int row, int column)
  {
    List<char> pipeTypes = new List<char>() { '|', '-', 'L', 'J', '7', 'F' };
    foreach (char pipeType in pipeTypes)
    {
      List<string> possibleConnections = getPipeConnections(pipeType, row, column);
      possibleConnections = possibleConnections.Where(connection => isValidCoordinate(tiles, connection)).ToList();
      if (possibleConnections.Count != 2) continue;
      bool isValid = true;
      foreach (string possibleConnection in possibleConnections)
      {
        List<int> coordinates = parseCoordinates(possibleConnection);
        List<string> neighbourPipeConnections = getPipeConnections(tiles[coordinates[0]][coordinates[1]], possibleConnection);
        // If we cannot connect to the neighbouring pipe, discard the possible connection
        if (!possibleConnections.Contains($"{coordinates[0]},{coordinates[1]}") || !neighbourPipeConnections.Contains($"{row},{column}"))
        {
          isValid = false;
          break;
        }
      }
      if (isValid)
      {
        return pipeType;
      }
    }
    throw new Exception($"The coordinates {row},{column} can not contain a valid pipe");
  }

  static List<string> getPipeConnections(char pipeType, string coordinatesString)
  {
    string[] coordinates = coordinatesString.Split(',');
    return getPipeConnections(pipeType, int.Parse(coordinates[0]), int.Parse(coordinates[1]));
  }

  static List<string> getPipeConnections(char pipeType, int row, int column)
  {
    switch (pipeType)
    {
      case '|':
        return new List<string>() { $"{row - 1},{column}", $"{row + 1},{column}" };
      case '-':
        return new List<string>() { $"{row},{column - 1}", $"{row},{column + 1}" };
      case 'L':
        return new List<string>() { $"{row - 1},{column}", $"{row},{column + 1}" };
      case 'J':
        return new List<string>() { $"{row - 1},{column}", $"{row},{column - 1}" };
      case '7':
        return new List<string>() { $"{row},{column - 1}", $"{row + 1},{column}" };
      case 'F':
        return new List<string>() { $"{row},{column + 1}", $"{row + 1},{column}" };
      default:
        return new List<string>();
    }
  }

  static bool isValidCoordinate(List<List<char>> tiles, string coordinatesString)
  {
    string[] coordinates = coordinatesString.Split(',');
    return isValidCoordinate(tiles, int.Parse(coordinates[0]), int.Parse(coordinates[1]));
  }

  static bool isValidCoordinate(List<List<char>> tiles, int row, int column)
  {
    return row >= 0 && row < tiles.Count && column >= 0 && column < tiles[row].Count;
  }

  static List<int> parseCoordinates(string coordinatesString)
  {
    string[] coordinates = coordinatesString.Split(',');
    return new List<int>() { int.Parse(coordinates[0]), int.Parse(coordinates[1]) };
  }

  static char getPipe(List<List<char>> tiles, int row, int column)
  {
    return tiles[row][column];
  }
}

class CoordinatePosition
{
  public string Coordinate { get; set; }
  public int Distance { get; set; }

  public CoordinatePosition(string coordinate, int distance = 0)
  {
    Coordinate = coordinate;
    Distance = distance;
  }
}
