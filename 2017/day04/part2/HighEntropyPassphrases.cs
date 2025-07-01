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

          bool allUniqueWords = true;
          for (int i = 0; i < words.Length; i++)
          {
            for (int j = 0; j < words.Length; j++)
            {
              // Don't compare the same word
              if (i == j) continue;

              // Words of differing length can't be compared
              if (words[i].Length != words[j].Length) continue;

              Dictionary<char, int> firstWordLetters = new Dictionary<char, int>();
              Dictionary<char, int> secondWordLetters = new Dictionary<char, int>();

              // Count letters in first word
              foreach (char letter in words[i])
              {
                if (!firstWordLetters.ContainsKey(letter)) firstWordLetters.Add(letter, 0);
                firstWordLetters[letter] += 1;
              }

              // Count letters in second word
              foreach (char letter in words[j])
              {
                if (!secondWordLetters.ContainsKey(letter)) secondWordLetters.Add(letter, 0);
                secondWordLetters[letter] += 1;
              }

              bool isValid = false;
              foreach (KeyValuePair<char, int> letterInfo in firstWordLetters)
              {
                if (!secondWordLetters.ContainsKey(letterInfo.Key) || secondWordLetters[letterInfo.Key] != letterInfo.Value)
                {
                  isValid = true;
                  break;
                }
              }
              allUniqueWords = allUniqueWords && isValid;
            }
          }
          if (allUniqueWords) validPassphrases++;
        }
        Console.WriteLine($"Number of valid passphrases: {validPassphrases}");
      }
    }
  }
}
