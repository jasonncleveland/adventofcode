using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

class SandSlabs
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

        // Store the bricks
        List<Brick> bricks = new List<Brick>();
        for (int brickId = 0; brickId < lines.Length; brickId++)
        {
          string line = lines[brickId];
          string[] lineParts = line.Split('~');
          string[] startCoordinate = lineParts[0].Split(',');
          string[] endCoordinate = lineParts[1].Split(',');
          int startX = int.Parse(startCoordinate[0]);
          int startY = int.Parse(startCoordinate[1]);
          int startZ = int.Parse(startCoordinate[2]);
          int endX = int.Parse(endCoordinate[0]);
          int endY = int.Parse(endCoordinate[1]);
          int endZ = int.Parse(endCoordinate[2]);
          bricks.Add(new Brick(brickId, startX, startY, startZ, endX, endY, endZ));
        }
        // Sort the bricks by Z coordinate
        bricks.Sort();
        Console.WriteLine($"Count: {bricks.Count}");

        // Settle bricks
        List<Brick> settledBricks;
        int fallenBricks;
        fallenBricks = settleBricks(bricks, out settledBricks);

        // Test removing each brick and see how many bricks fall
        int removedBricks = 0;
        for (int i = 0; i < settledBricks.Count; i++)
        {
          List<Brick> filteredBricks = settledBricks.GetRange(0, i - 0);
          filteredBricks.AddRange(settledBricks.GetRange(i + 1, settledBricks.Count - i - 1));
          fallenBricks = settleBricks(filteredBricks, out _, true);
          if (fallenBricks < 1) removedBricks += 1;
        }
        Console.WriteLine($"Can remove {removedBricks} bricks");

        stopWatch.Stop();
        Console.WriteLine($"Elapsed Time: {stopWatch.ElapsedMilliseconds} ms");
      }
    }
  }

  static int settleBricks(List<Brick> bricks, out List<Brick> settledBricks, bool exitEarly = false)
  {
    settledBricks = new List<Brick>();
    Dictionary<string, int> highestBrickIndex = new Dictionary<string, int>();
    int totalSettledBricks = 0;

    foreach (Brick brick in bricks)
    {
      Brick settledBrick = new Brick(brick);
      bool hasBrickSettled = false;
      bool canBrickSettle = true;
      while (canBrickSettle && settledBrick.StartZ > 1)
      {
        // Check that all parts of the brick can drop down
        int desiredZ = settledBrick.StartZ - 1;
        for (int x = settledBrick.StartX; x <= settledBrick.EndX; x++)
        {
          for (int y = settledBrick.StartY; y <= settledBrick.EndY; y++)
          {
            string key = $"{x},{y}";
            if (highestBrickIndex.ContainsKey(key) && highestBrickIndex[key] >= desiredZ)
            {
              canBrickSettle = false;
              break;
            }
          }
        }

        if (!canBrickSettle)
        {
          break;
        }
        hasBrickSettled = true;
        // Move the brick down
        settledBrick.EndZ -= 1;
        settledBrick.StartZ -= 1;
      }

      if (hasBrickSettled)
      {
        totalSettledBricks += 1;
        // If the flag is set and any bricks fall, return early to save processing
        if (exitEarly)
        {
          return totalSettledBricks;
        }
      }

      // Mark the brick space as occupied
      for (int x = settledBrick.StartX; x <= settledBrick.EndX; x++)
      {
        for (int y = settledBrick.StartY; y <= settledBrick.EndY; y++)
        {
          string key = $"{x},{y}";
          if (!highestBrickIndex.ContainsKey(key))
          {
            canBrickSettle = false;
            highestBrickIndex.Add(key, 0);
          }
          highestBrickIndex[key] = settledBrick.EndZ;
        }
      }
      settledBricks.Add(settledBrick);
    }
    return totalSettledBricks;
  }
}

class Brick: IComparable<Brick>
{
  public int Id { get; set; }
  public int StartX { get; set; }
  public int StartY { get; set; }
  public int StartZ { get; set; }
  public int EndX { get; set; }
  public int EndY { get; set; }
  public int EndZ { get; set; }

  public Brick(int id, int startX, int startY, int startZ, int endX, int endY, int endZ)
  {
    Id = id;
    StartX = startX;
    StartY = startY;
    StartZ = startZ;
    EndX = endX;
    EndY = endY;
    EndZ = endZ;
  }

  public Brick(Brick brick)
  {
    Id = brick.Id;
    StartX = brick.StartX;
    StartY = brick.StartY;
    StartZ = brick.StartZ;
    EndX = brick.EndX;
    EndY = brick.EndY;
    EndZ = brick.EndZ;
  }

  public int CompareTo(Brick other)
  {
    return StartZ - other.StartZ;
  }
}
