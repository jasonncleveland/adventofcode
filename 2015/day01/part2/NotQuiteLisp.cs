using System;
using System.IO;

class NotQuiteLisp
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        string fileContents = File.ReadAllText(fileName);

        int currentFloor = 0;

        int characterCount = 0;

        foreach (char letter in fileContents)
        {
          characterCount++;

          if (letter == '(')
          {
            currentFloor++;
          }
          else if(letter == ')')
          {
            currentFloor--;
          }
          else
          {
            Console.WriteLine($"Invalid character found: '{letter}'");
          }

          if (currentFloor < 0)
          {
            Console.WriteLine("Entering the basement!");
            break;
          }
        }

        Console.WriteLine($"Character count when entering basement: {characterCount}");
      }
    }
  }
}
