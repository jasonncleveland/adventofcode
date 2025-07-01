using System;
using System.Collections.Generic;
using System.IO;

class CrossedWires
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        string[] lines = File.ReadAllLines(fileName);

        for (int i = 0; i < lines.Length; i += 2)
        {
          HashSet<string> firstWirePositions = calculatePositions(lines[i].Split(','));
          HashSet<string> secondWirePositions = calculatePositions(lines[i + 1].Split(','));
          HashSet<string> intersections = new HashSet<string>(firstWirePositions);
          intersections.IntersectWith(secondWirePositions);
          int shortestDistance = int.MaxValue;
          foreach (string position in intersections)
          {
            string[] positionParts = position.Split(',');
            int x = int.Parse(positionParts[0]);
            int y = int.Parse(positionParts[1]);
            int distance = Math.Abs(x) + Math.Abs(y);
            if (distance < shortestDistance) shortestDistance = distance;
          }
          Console.WriteLine($"Shortest Manhatten distance: {shortestDistance}");
        }
      }
    }
  }

  static HashSet<string> calculatePositions(string[] moves)
  {
    HashSet<string> positions = new HashSet<string>();

    int x = 0;
    int y = 0;
    foreach (string move in moves)
    {
      char direction = move[0];
      int units = int.Parse(move.Substring(1));
      
      switch (direction)
      {
        case 'L':
          for (int i = 0; i < units; i++)
          {
            x--;
            positions.Add($"{x},{y}");
          }
          break;
        case 'R':
          for (int i = 0; i < units; i++)
          {
            x++;
            positions.Add($"{x},{y}");
          }
          break;
        case 'U':
          for (int i = 0; i < units; i++)
          {
            y--;
            positions.Add($"{x},{y}");
          }
          break;
        case 'D':
          for (int i = 0; i < units; i++)
          {
            y++;
            positions.Add($"{x},{y}");
          }
          break;
      }
    }

    return positions;
  }
}
