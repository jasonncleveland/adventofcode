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
          HashSet<string> firstWirePositions = new HashSet<string>();
          Dictionary<string, int> firstWireSteps = new Dictionary<string, int>();
          calculatePositions(lines[i].Split(','), firstWirePositions, firstWireSteps);

          HashSet<string> secondWirePositions = new HashSet<string>();
          Dictionary<string, int> secondWireSteps = new Dictionary<string, int>();
          calculatePositions(lines[i + 1].Split(','), secondWirePositions, secondWireSteps);

          HashSet<string> intersections = new HashSet<string>(firstWirePositions);
          intersections.IntersectWith(secondWirePositions);

          int fewestSteps = int.MaxValue;
          foreach (string position in intersections)
          {
            int steps = firstWireSteps[position] + secondWireSteps[position];
            if (steps < fewestSteps) fewestSteps = steps;
          }
          Console.WriteLine($"Fewest number of steps: {fewestSteps}");
        }
      }
    }
  }

  static HashSet<string> calculatePositions(string[] moves, HashSet<string> positions, Dictionary<string, int> positionSteps)
  {
    int x = 0;
    int y = 0;
    int steps = 0;
    foreach (string move in moves)
    {
      char direction = move[0];
      int units = int.Parse(move.Substring(1));
      
      switch (direction)
      {
        case 'L':
          for (int i = 0; i < units; i++)
          {
            steps++;
            x--;
            string position = $"{x},{y}";
            positions.Add(position);
            if (!positionSteps.ContainsKey(position)) positionSteps.Add(position, int.MaxValue);
            if (steps < positionSteps[position]) positionSteps[position] = steps;
          }
          break;
        case 'R':
          for (int i = 0; i < units; i++)
          {
            steps++;
            x++;
            string position = $"{x},{y}";
            positions.Add(position);
            if (!positionSteps.ContainsKey(position)) positionSteps.Add(position, int.MaxValue);
            if (steps < positionSteps[position]) positionSteps[position] = steps;
          }
          break;
        case 'U':
          for (int i = 0; i < units; i++)
          {
            steps++;
            y--;
            string position = $"{x},{y}";
            positions.Add(position);
            if (!positionSteps.ContainsKey(position)) positionSteps.Add(position, int.MaxValue);
            if (steps < positionSteps[position]) positionSteps[position] = steps;
          }
          break;
        case 'D':
          for (int i = 0; i < units; i++)
          {
            steps++;
            y++;
            string position = $"{x},{y}";
            positions.Add(position);
            if (!positionSteps.ContainsKey(position)) positionSteps.Add(position, int.MaxValue);
            if (steps < positionSteps[position]) positionSteps[position] = steps;
          }
          break;
      }
    }

    return positions;
  }
}
