using System;
using System.IO;
using System.Text.RegularExpressions;

class Matchsticks
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        string[] lines = File.ReadAllLines(fileName);

        int totalNumCodeCharacters = 0;
        int totalNumMemoryCharacters = 0;
        foreach (string line in lines)
        {
          string quotesStripped = line.Substring(1, line.Length - 2);
          int codeCharacters = line.Length;
          totalNumCodeCharacters += codeCharacters;
          int memoryCharacters = Regex.Unescape(quotesStripped).Length;
          totalNumMemoryCharacters += memoryCharacters;
        }
        Console.WriteLine($"code characters: {totalNumCodeCharacters}, memory characters: {totalNumMemoryCharacters}");
        Console.WriteLine($"{totalNumCodeCharacters} - {totalNumMemoryCharacters} = {totalNumCodeCharacters - totalNumMemoryCharacters}");
      }
    }
  }
}
