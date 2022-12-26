using System;
using System.Collections.Generic;
using System.IO;

class BoilingBoulders
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

        List<CoordinatesData> coordinates = new List<CoordinatesData>();

        // Read input
        int maxX = 0, maxY = 0, maxZ = 0;
        foreach (string line in lines)
        {
          string[] lineData = line.Split(',');

          // Calculate the max values to build model
          int x = int.Parse(lineData[0]);
          int y = int.Parse(lineData[1]);
          int z = int.Parse(lineData[2]);

          if (x > maxX) maxX = x;
          if (y > maxY) maxY = y;
          if (z > maxZ) maxZ = z;

          coordinates.Add(new CoordinatesData(x, y, z));
        }

        // Build empty model (add 2 so that there is a outer perimiter)
        bool[,,] lava = new bool[maxX + 2, maxY + 2, maxZ + 2];
        for (int x = 0; x < lava.GetLength(0); x++)
        {
          for (int y = 0; y < lava.GetLength(1); y++)
          {
            for (int z = 0; z < lava.GetLength(2); z++)
            {
              lava[x, y, z] = false;
            }
          }
        }

        // Mark where lava droplets are
        foreach (CoordinatesData coordinatesData in coordinates)
        {
          lava[coordinatesData.X, coordinatesData.Y, coordinatesData.Z] = true;
        }

        // Count number of unconnected sides
        int totalUnconnectedSides = 0;
        for (int x = 0; x < lava.GetLength(0); x++)
        {
          for (int y = 0; y < lava.GetLength(1); y++)
          {
            for (int z = 0; z < lava.GetLength(2); z++)
            {
              if (lava[x, y, z])
              {
                totalUnconnectedSides += countUnconnectedSides(lava, x, y, z);
              }
            }
          }
        }

        Console.WriteLine($"total unconnected sides: {totalUnconnectedSides}");
      }
    }
  }

  static int countUnconnectedSides(bool[,,] lava, int x, int y, int z)
  {
    int connectedSides = 0;

    if (x - 1 >= 0 && lava[x - 1, y, z]) connectedSides++;
    if (x + 1 < lava.GetLength(0) && lava[x + 1, y, z]) connectedSides++;
    if (y - 1 >= 0 && lava[x, y - 1, z]) connectedSides++;
    if (y + 1 < lava.GetLength(1) && lava[x, y + 1, z]) connectedSides++;
    if (z - 1 >= 0 && lava[x, y, z - 1]) connectedSides++;
    if (z + 1 < lava.GetLength(2) && lava[x, y, z + 1]) connectedSides++;

    return 6 - connectedSides;
  }
}

class CoordinatesData {
  public int X { get; set; }
  public int Y { get; set; }
  public int Z { get; set; }

  public CoordinatesData(int x, int y, int z)
  {
    X = x;
    Y = y;
    Z = z;
  }
}
