using System;
using System.Collections.Generic;
using System.IO;

class UnstableDiffusion
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

        int rowCount = lines.Length;
        int columnCount = lines[0].Length;
        int extraX = 10;
        int extraY = 10;

        // Initialize Map
        char[,] map = new char[extraY * 2 + rowCount, extraX * 2 + columnCount];
        for (int y = 0; y < map.GetLength(0); y++)
        {
          for (int x = 0; x < map.GetLength(1); x++)
          {
            map[y, x] = '.';
          }
        }

        // Read Input
        for (int y = 0; y < lines.Length; y++)
        {
          for (int x = 0; x < lines[y].Length; x++)
          {
            map[extraY + y, extraX + x] = lines[y][x];
          }
        }

        List<char> possibleMoves = new List<char>(new char[] { 'N', 'S', 'W', 'E' });

        // Move elves
        for (int i = 0; i < 10; i++)
        {
          bool isFinished = moveElves(map, possibleMoves);
          if (isFinished)
          {
            break;
          }
        }

        // Calculate bounding box
        int emptyGroundTiles = calculateEmptyGroundTiles(map);
        Console.WriteLine($"Empty ground tiles: {emptyGroundTiles}");
      }
    }
  }

  static int calculateEmptyGroundTiles(char[,] map)
  {
    int northBoundary = -1;
    int southBoundary = -1;
    int westBoundary = -1;
    int eastBoundary = -1;

    // Calculate boundaries
    for (int y = 0; y < map.GetLength(0); y++)
    {
      for (int x = 0; x < map.GetLength(1); x++)
      {
        if (map[y, x] == '#')
        {
          if (northBoundary < 0 || y < northBoundary)
          {
            northBoundary = y;
          }
          if (y > southBoundary)
          {
            southBoundary = y;
          }
          if (westBoundary < 0 || x < westBoundary)
          {
            westBoundary = x;
          }
          if (x > eastBoundary)
          {
            eastBoundary = x;
          }
        }
      }
    }

    // Calculate empty ground tiles
    int emptyGroundTiles = 0;
    for (int y = northBoundary; y <= southBoundary; y++)
    {
      for (int x = westBoundary; x <= eastBoundary; x++)
      {
        if (map[y, x] == '.')
        {
          emptyGroundTiles++;
        }
      }
    }
    return emptyGroundTiles;
  }

  static bool moveElves(char[,] map, List<char> possibleMoves)
  {
    bool areElvesSeparated = true;

    List<MoveData> elfMoves = new List<MoveData>();

    // Propose moves
    for (int y = 0; y < map.GetLength(0); y++)
    {
      for (int x = 0; x < map.GetLength(1); x++)
      {
        if (map[y,x] == '#')
        {
          if (!isElfSeparated(map, y, x))
          {
            areElvesSeparated = false;
            MoveData move = proposeMove(map, y, x, possibleMoves);
            if (move != null)
            {
              elfMoves.Add(move);
            }
          }
        }
      }
    }

    // Filter out duplicate moves
    List<MoveData> filteredElfMoves = new List<MoveData>(elfMoves);
    foreach (MoveData move in elfMoves)
    {
      int index = elfMoves.FindIndex(m => m.EndY == move.EndY && m.EndX == move.EndX);
      int lastIndex = elfMoves.FindLastIndex(m => m.EndY == move.EndY && m.EndX == move.EndX);
      if (index != lastIndex)
      {
        filteredElfMoves.RemoveAll(m => m.EndY == move.EndY && m.EndX == move.EndX);
      }
    }

    // Perform moves
    foreach (MoveData move in filteredElfMoves)
    {
      map[move.StartY, move.StartX] = '.';
      map[move.EndY, move.EndX] = '#';
    }

    // Move first move to end of list
    char firstMove = possibleMoves[0];
    possibleMoves.RemoveAt(0);
    possibleMoves.Add(firstMove);

    return areElvesSeparated;
  }

  static MoveData proposeMove(char[,] map, int y, int x, List<char> possibleMoves)
  {
    foreach (char move in possibleMoves)
    {
      if (move == 'N' && isPositionEmpty(map, y - 1, x) && isPositionEmpty(map, y - 1, x - 1) && isPositionEmpty(map, y - 1, x + 1))
      {
        return new MoveData(move, y, x, y - 1, x);
      }
      else if (move == 'S' && isPositionEmpty(map, y + 1, x) && isPositionEmpty(map, y + 1, x - 1) && isPositionEmpty(map, y + 1, x + 1))
      {
        return new MoveData(move, y, x, y + 1, x);
      }
      else if (move == 'W' && isPositionEmpty(map, y, x - 1) && isPositionEmpty(map, y - 1, x - 1) && isPositionEmpty(map, y + 1, x - 1))
      {
        return new MoveData(move, y, x, y, x - 1);
      }
      else if (move == 'E' && isPositionEmpty(map, y, x + 1) && isPositionEmpty(map, y - 1, x + 1) && isPositionEmpty(map, y + 1, x + 1))
      {
        return new MoveData(move, y, x, y, x + 1);
      }
    }
    return null;
  }

  static bool isElfSeparated(char[,] map, int y, int x)
  {
    bool isElfSeparated = true;

    return (
      isElfSeparated
      // North
      && isPositionEmpty(map, y - 1, x)
      // South
      && isPositionEmpty(map, y + 1, x)
      // West
      && isPositionEmpty(map, y, x - 1)
      // East
      && isPositionEmpty(map, y, x + 1)
      // North West
      && isPositionEmpty(map, y - 1, x - 1)
      // North East
      && isPositionEmpty(map, y - 1, x + 1)
      // South West
      && isPositionEmpty(map, y + 1, x - 1)
      // South East
      && isPositionEmpty(map, y + 1, x + 1)
    );
  }

  static bool isPositionEmpty(char[,] map, int y, int x)
  {
    if (x < 0 && x > map.GetLength(1) - 1 || y < 0 || y > map.GetLength(0) - 1)
    {
      throw new IndexOutOfRangeException("Coordinates outside map range");
    }
    return map[y, x] == '.';
  }

  static void printMap(char[,] map)
  {
    for (int y = 0; y < map.GetLength(0); y++)
    {
      for (int x = 0; x < map.GetLength(1); x++)
      {
        Console.Write(map[y, x]);
      }
      Console.WriteLine();
    }
  }
}

class MoveData {
  public char Move { get; set; }
  public int StartY { get; set; }
  public int StartX { get; set; }
  public int EndY { get; set; }
  public int EndX { get; set; }

  public MoveData(char move, int startY, int startX, int endY, int endX)
  {
    Move = move;
    StartY = startY;
    StartX = startX;
    EndY = endY;
    EndX = endX;
  }
}
