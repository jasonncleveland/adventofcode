using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

class ParabolicReflectorDish
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      long cycles = args.Length > 1 ? int.Parse(args[1]) : 1000000000;
      if (File.Exists(fileName))
      {
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();

        string[] lines = File.ReadAllLines(fileName);

        // Parse input
        List<List<char>> platform = new List<List<char>>();
        foreach (string line in lines)
        {
          platform.Add(new List<char>(line));
        }

        HashSet<string> visitedStates = new HashSet<string>();
        bool foundLoop = false;
        string loopState = null;
        long lastFoundLoop = 0;
        long loopStart = -1;
        long loopLength = -1;

        // Tilt the table until we detect a loop
        for (long cycle = 1; cycle <= cycles; cycle++)
        {
          // Tilt table in each direction
          tiltTable(platform, 'N');
          tiltTable(platform, 'W');
          tiltTable(platform, 'S');
          tiltTable(platform, 'E');

          string currentState = string.Join('\n', platform.Select(row => string.Join("", row)));

          // Find when we encounter the duplicate state and calculate the number of cycles since it was last seen
          if (foundLoop && currentState == loopState)
          {
            loopLength = cycle - lastFoundLoop;
            lastFoundLoop = cycle;
            break;
          }

          // Find when we encounter the first duplicate state and store it
          if (!foundLoop && visitedStates.Contains(currentState))
          {
            foundLoop = true;
            loopState = currentState;
            lastFoundLoop = cycle;
            loopStart = cycle;
          }

          visitedStates.Add(currentState);
        }

        // Calculate remaining cycles from found loop to final cycle
        long remainingCycles = cycles - loopStart;
        long extraSteps = remainingCycles % loopLength;

        // Perform extra steps needed to get from loop state to final cycle
        for (long i = 0; i < extraSteps; i++)
        {
          tiltTable(platform, 'N');
          tiltTable(platform, 'W');
          tiltTable(platform, 'S');
          tiltTable(platform, 'E');
        }
        
        // Calculate load at final state
        int total = calculateLoad(platform);
        Console.WriteLine($"Total value: {total}");

        stopWatch.Stop();
        Console.WriteLine($"Elapsed Time: {stopWatch.ElapsedMilliseconds} ms");
      }
    }
  }

  static void tiltTable(List<List<char>> platform, char direction)
  {
    switch (direction)
    {
      case 'N':
        tiltNorth(platform);
        break;
      case 'S':
        tiltSouth(platform);
        break;
      case 'W':
        tiltWest(platform);
        break;
      case 'E':
        tiltEast(platform);
        break;
    }
  }

  static void tiltNorth(List<List<char>> platform)
  {
    // Start at the top index
    for (int row = 0; row < platform.Count; row++)
    {
      for (int column = 0; column < platform[row].Count; column++)
      {
        if (platform[row][column] == 'O')
        {
          int currentRow = row;
          while (currentRow > 0)
          {
            if (platform[currentRow - 1][column] == '.')
            {
              platform[currentRow - 1][column] = platform[currentRow][column];
              platform[currentRow][column] = '.';
            }
            else if (platform[currentRow - 1][column] == 'O' || platform[currentRow - 1][column] == '#')
            {
              // We hit a non-empty space, stop
              break;
            }
            currentRow--;
          }
        }
      }
    }
  }

  static void tiltSouth(List<List<char>> platform)
  {
    // Start from bottom index
    for (int row = platform.Count - 1; row >= 0; row--)
    {
      for (int column = 0; column < platform[row].Count; column++)
      {
        if (platform[row][column] == 'O')
        {
          int currentRow = row;
          while (currentRow < platform.Count - 1)
          {
            if (platform[currentRow + 1][column] == '.')
            {
              platform[currentRow + 1][column] = platform[currentRow][column];
              platform[currentRow][column] = '.';
            }
            else if (platform[currentRow + 1][column] == 'O' || platform[currentRow + 1][column] == '#')
            {
              // We hit a non-empty space, stop
              break;
            }
            currentRow++;
          }
        }
      }
    }
  }

  static void tiltWest(List<List<char>> platform)
  {
    // Start from left-most index
    for (int row = 0; row < platform.Count; row++)
    {
      for (int column = 0; column < platform[row].Count; column++)
      {
        if (platform[row][column] == 'O')
        {
          int currentColumn = column;
          while (currentColumn > 0)
          {
            if (platform[row][currentColumn - 1] == '.')
            {
              platform[row][currentColumn - 1] = platform[row][currentColumn];
              platform[row][currentColumn] = '.';
            }
            else if (platform[row][currentColumn - 1] == 'O' || platform[row][currentColumn - 1] == '#')
            {
              // We hit a non-empty space, stop
              break;
            }
            currentColumn--;
          }
        }
      }
    }
  }

  static void tiltEast(List<List<char>> platform)
  {
    // Start from right-most index
    for (int row = 0; row < platform.Count; row++)
    {
      for (int column = platform[row].Count - 1; column >= 0; column--)
      {
        if (platform[row][column] == 'O')
        {
          int currentColumn = column;
          while (currentColumn < platform[row].Count - 1)
          {
            if (platform[row][currentColumn + 1] == '.')
            {
              platform[row][currentColumn + 1] = platform[row][currentColumn];
              platform[row][currentColumn] = '.';
            }
            else if (platform[row][currentColumn + 1] == 'O' || platform[row][currentColumn + 1] == '#')
            {
              // We hit a non-empty space, stop
              break;
            }
            currentColumn++;
          }
        }
      }
    }
  }

  static int calculateLoad(List<List<char>> platform)
  {
    int total = 0;
    for (int row = 0; row < platform.Count; row++)
    {
      for (int column = 0; column < platform[row].Count; column++)
      {
        if (platform[row][column] == 'O')
        {
          total += platform.Count - row;
        }
      }
    }
    return total;
  }
}
