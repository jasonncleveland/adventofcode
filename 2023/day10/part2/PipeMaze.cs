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

    char[,] tileStatus = new char[tiles.Count,tiles[0].Count];
    for (int row = 0; row < tileStatus.GetLength(0); row++)
    {
      for (int column = 0; column < tileStatus.GetLength(1); column++)
      {
        tileStatus[row, column] = tiles[row][column];
      }
    }

    // Do BFS to traverse all pipes
    Queue<CoordinatePosition> coordinatesToVisit = new Queue<CoordinatePosition>();
    HashSet<string> visitedPipes = new HashSet<string>();
    string startingCoordinates = $"{startRow},{startColumn}";
    coordinatesToVisit.Enqueue(new CoordinatePosition(startingCoordinates));
    visitedPipes.Add(startingCoordinates);
    CoordinatePosition farthestPipe = coordinatesToVisit.Peek();
    while (coordinatesToVisit.Count > 0)
    {
      CoordinatePosition coordinatePosition = coordinatesToVisit.Dequeue();
      List<int> coordinates = parseCoordinates(coordinatePosition.Coordinate);
      int row = coordinates[0];
      int column = coordinates[1];
      // Mark the main pipe for later reference
      tileStatus[row, column] = 'P';
      if (coordinatePosition.Distance > farthestPipe.Distance)
      {
        farthestPipe = coordinatePosition;
      }

      char pipeType = getTileValue(tiles, row, column);
      List<string> neighbourPipes = getPipeConnections(pipeType, row, column);
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

    int itemsInsideMainLoop = 0;
    for (int row = 0; row < tileStatus.GetLength(0); row++)
    {
      for (int column = 0; column < tileStatus.GetLength(1); column++)
      {
        if (tileStatus[row, column] != 'P')
        {
          if (isInsideLoop(tiles, tileStatus, row, column))
          {
            itemsInsideMainLoop += 1;
          }
        }
      }
    }
    Console.WriteLine($"Found {itemsInsideMainLoop} items inside the main loop");
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
        Console.WriteLine($"The starting pipe type is {pipeType}");
        return pipeType;
      }
    }
    throw new Exception($"The coordinates {row},{column} can not contain a valid pipe");
  }

  static bool isInsideLoop(List<List<char>> tiles, char[,] tileStatus, int row, int column)
  {
    // We want to check if we cross any pipe boundaries.
    // Only check for vertical pipes |, and F and 7 corner pipes to prevent the
    // case where we cross over a F-J or L-7 combination of pipes.
    // Checking for either F and 7 or L and J will work, but only one.
    List<char> validPipes = new List<char> { '|', 'F', '7' };
    // List<char> validPipes = new List<char> { '|', 'L', 'J' };

    // Expand to the left and right to see if we have an odd number of pipes on either side
    // Crossing an odd number of boundaries means that the item is inside the loop
    int currentColumn;
    int pipesEncounteredLeft = 0;
    int pipesEncounteredRight = 0;

    // Check left
    currentColumn = column;
    while (currentColumn >= 0)
    {
      if (tileStatus[row, currentColumn] == 'P' && validPipes.Contains(tiles[row][currentColumn])) pipesEncounteredLeft++;
      currentColumn -= 1;
    }
    // Check right
    currentColumn = column;
    while (currentColumn < tiles[row].Count)
    {
      if (tileStatus[row, currentColumn] == 'P' && validPipes.Contains(tiles[row][currentColumn])) pipesEncounteredRight++;
      currentColumn += 1;
    }
    
    // Check if we have passed over an odd number of pipes getting to either the left or right side
    if (pipesEncounteredLeft > 0 && pipesEncounteredRight > 0 && (pipesEncounteredLeft % 2 != 0 && pipesEncounteredRight % 2 != 0))
    {
      return true;
    }
    return false;
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

  static char getTileValue(List<List<char>> tiles, int row, int column)
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
