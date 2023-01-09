using System;
using System.Collections.Generic;
using System.IO;

class BathroomSecurity
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        string[] lines = File.ReadAllLines(fileName);

        List<List<char>> keypad = new List<List<char>>
        {
          new List<char> { '1', '2', '3' },
          new List<char> { '4', '5', '6' },
          new List<char> { '7', '8', '9' },
        };

        char currentNumber = '5';
        int y = 1, x = 1;
        Console.Write($"Passcode: ");
        foreach (string line in lines)
        {
          foreach (char move in line)
          {
            int nextY = y, nextX = x;
            switch (move)
            {
              case 'U':
                nextY--;
                break;
              case 'D':
                nextY++;
                break;
              case 'L':
                nextX--;
                break;
              case 'R':
                nextX++;
                break;
              default:
                throw new ArgumentException($"Invalid move: {move}");
            }
            if (isValidMove(keypad, nextY, nextX))
            {
              y = nextY;
              x = nextX;
              currentNumber = keypad[y][x];
            }
          }
          Console.Write(currentNumber);
        }
        Console.WriteLine();
      }
    }
  }

  static bool isValidMove(List<List<char>> keypad, int y, int x)
  {
    return y >=0 && y < keypad.Count && x >= 0 && x < keypad[y].Count;
  }
}
