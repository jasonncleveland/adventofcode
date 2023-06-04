using System;
using System.Collections.Generic;
using System.IO;

class InventoryManagementSystem
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        string[] lines = File.ReadAllLines(fileName);

        for (int i = 0; i < lines.Length; i++)
        {
          for (int j = 0; j < lines.Length; j++)
          {
            if (i == j) continue;

            int differentCharacterCount = 0;
            List<char> commonCharacters = new List<char>();
            for (int k = 0; k < lines[i].Length; k++)
            {
              if (lines[i][k] != lines[j][k])
              {
                differentCharacterCount += 1;
              }
              else
              {
                commonCharacters.Add(lines[i][k]);
              }
            }
            if (differentCharacterCount == 1)
            {
              Console.WriteLine($"Similar lines: {lines[i]}, {lines[j]}");
              Console.WriteLine($"Common characters: {string.Join("", commonCharacters)}");
            }
          }
        }
      }
    }
  }
}
