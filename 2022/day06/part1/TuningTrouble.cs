using System;
using System.Collections.Generic;
using System.IO;

class TuningTrouble
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

        foreach (string line in lines)
        {
          for (int i = 3; i < line.Length; i++)
          {
            HashSet<char> uniqueCharacters = new HashSet<char>();
            uniqueCharacters.Add(line[i-3]);
            uniqueCharacters.Add(line[i-2]);
            uniqueCharacters.Add(line[i-1]);
            uniqueCharacters.Add(line[i]);

            if (uniqueCharacters.Count == 4)
            {
              Console.WriteLine($"The segment '{line[i-3]}{line[i-2]}{line[i-1]}{line[i]}' contains all unique characters. {i + 1} characters have been processed.");
              break;
            }
          }
        }
      }
    }
  }
}
