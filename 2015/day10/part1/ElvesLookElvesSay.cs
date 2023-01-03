using System;
using System.Collections.Generic;
using System.IO;

class ElvesLookElvesSay
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        string[] lines = File.ReadAllLines(fileName);

        foreach (string line in lines)
        {
          string currentLine = line;

          for (int i = 0; i < 40; i++)
          {
            List<string> newCharacters = new List<string>();
            char currentDigit = currentLine[0];
            int digitCount = 0;
            foreach (char digit in currentLine)
            {
              if (digit == currentDigit)
              {
                digitCount++;
              }
              else
              {
                newCharacters.Add(digitCount.ToString());
                newCharacters.Add(currentDigit.ToString());
                currentDigit = digit;
                digitCount = 1;
              }
            }
            newCharacters.Add(digitCount.ToString());
            newCharacters.Add(currentDigit.ToString());
            currentLine = string.Join("", newCharacters.ToArray());
            // Progress indicator
            Console.Write('.');
          }
          Console.WriteLine();
          Console.WriteLine($"Length of line after 40 ops: {currentLine.Length}");
        }
      }
    }
  }
}
