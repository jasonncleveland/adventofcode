using System;
using System.Collections.Generic;
using System.IO;

class HighEntropyPassphrases
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        string[] lines = File.ReadAllLines(fileName);

        int validPassphrases = 0;
        foreach (string line in lines)
        {
          string[] words = line.Split(' ');

          HashSet<string> uniqueWords = new HashSet<string>();

          bool allUniqueWords = true;
          foreach (string word in words)
          {
            if (uniqueWords.Contains(word))
            {
              allUniqueWords = false;
              break;
            }
            uniqueWords.Add(word);
          }
          if (allUniqueWords) validPassphrases++;
        }
        Console.WriteLine($"Number of valid passphrases: {validPassphrases}");
      }
    }
  }
}
