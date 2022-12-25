using System;
using System.Collections.Generic;
using System.IO;

class HillClimbingAlgorithm
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

        List<List<char>> heightMap = new List<List<char>>();

        foreach (string line in lines)
        {
          heightMap.Add(new List<char>(line.ToCharArray()));
        }

        // Find start and end location
        int[] startLocation = new int[2];
        int[] endLocation = new int[2];
        for (int y = 0; y < heightMap.Count; y++)
        {
          for (int x = 0; x < heightMap[y].Count; x++)
          {
            if (heightMap[y][x] == 'S')
            {
              startLocation = new int[] { x, y };
              heightMap[y][x] = 'a';
            }
            else if (heightMap[y][x] == 'E')
            {
              endLocation = new int[] { x, y };
              heightMap[y][x] = 'z';
            }
          }
        }

        // Calculate shortest path
        int shortestPath = findShortestPath(heightMap, startLocation[0], startLocation[1], endLocation[0], endLocation[1]);
        Console.WriteLine($"Shortest path to end: {shortestPath}");
      }
    }
  }

  static int findShortestPath(List<List<char>> heightMap, int startX, int startY, int endX, int endY)
  {
    Queue<MapPosition> spotsToCheck = new Queue<MapPosition>();
    bool[][] visited = new bool[heightMap.Count][];
    for (int y = 0; y < visited.Length; y++)
    {
      visited[y] = new bool[heightMap[y].Count];
      for (int x = 0; x < heightMap[y].Count; x++)
      {
        visited[y][x] = false;
      }
    }

    // Enqueue starting position
    spotsToCheck.Enqueue(new MapPosition(startY, startX, 0));
    visited[startY][startX] = true;

    while (spotsToCheck.Count > 0)
    {
      MapPosition spotToCheck = spotsToCheck.Dequeue();
      int currentY = spotToCheck.Y;
      int currentX = spotToCheck.X;
      int pathLength = spotToCheck.Distance;

      // Check if we've reached the end
      if (currentY == endY && currentX == endX)
      {
        return pathLength;
      }

      int up = currentY - 1;
      int down = currentY + 1;
      int left = currentX - 1;
      int right = currentX + 1;

      // Check up
      if (
        up >= 0
        && !visited[up][currentX]
        && canMoveToSpot(heightMap, currentY, currentX, up, currentX)
      )
      {
        spotsToCheck.Enqueue(new MapPosition(up, currentX, pathLength + 1));
        visited[up][currentX] = true;
      }

      // Check down
      if (
        down < heightMap.Count
        && !visited[down][currentX]
        && canMoveToSpot(heightMap, currentY, currentX, down, currentX)
      )
      {
        spotsToCheck.Enqueue(new MapPosition(down, currentX, pathLength + 1));
        visited[down][currentX] = true;
      }

      // Check left
      if (
        left >= 0
        && !visited[currentY][left]
        && canMoveToSpot(heightMap, currentY, currentX, currentY, left)
      )
      {
        spotsToCheck.Enqueue(new MapPosition(currentY, left, pathLength + 1));
        visited[currentY][left] = true;
      }

      // Check right
      if (
        right < heightMap[0].Count
        && !visited[currentY][right]
        && canMoveToSpot(heightMap, currentY, currentX, currentY, right)
      )
      {
        spotsToCheck.Enqueue(new MapPosition(currentY, right, pathLength + 1));
        visited[currentY][right] = true;
      }
    }

    return -1;
  }

  static bool canMoveToSpot(List<List<char>> heightMap, int startY, int startX, int endY, int endX)
  {
    if (heightMap[startY][startX] == 'S' || heightMap[endY][endX] == 'E')
    {
      return true;
    }
    return heightMap[startY][startX] + 1 >= heightMap[endY][endX];
  }
}

class MapPosition
{
  public int Y { get; set; }
  public int X { get; set; }
  public int Distance { get; set; }

  public MapPosition(int y, int x, int distance)
  {
    Y = y;
    X = x;
    Distance = distance;
  }
}
