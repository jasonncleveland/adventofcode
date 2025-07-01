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

        int uniqueCharactersNeeded = 14;

        foreach (string line in lines)
        {
          for (int i = (uniqueCharactersNeeded - 1); i < line.Length; i++)
          {
            HashSet<char> uniqueCharacters = new HashSet<char>();

            string segment = "";
            for (int j = 0; j < (uniqueCharactersNeeded); j++)
            {
              segment += line[i - j];
              uniqueCharacters.Add(line[i - j]);
            }
            if (uniqueCharacters.Count == uniqueCharactersNeeded)
            {
              Console.WriteLine($"The segment '{segment}' contains all unique characters. {i + 1} characters have been processed.");
              break;
            }
          }
        }
      }
    }
  }
}
