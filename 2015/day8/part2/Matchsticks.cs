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
        int totalNumEncodedCharacters = 0;
        foreach (string line in lines)
        {
          string escapedString = "";
          escapedString += "\"";
          foreach (char character in line)
          {
            if (character == '\\')
            {
              escapedString += "\\\\";
            }
            else if (character == '\"')
            {
              escapedString += "\\\"";
            }
            else
            {
              escapedString += character;
            }
          }
          escapedString += "\"";

          int codeCharacters = line.Length;
          totalNumCodeCharacters += codeCharacters;
          int escapedCharacters = escapedString.Length;
          totalNumEncodedCharacters += escapedCharacters;
        }
        Console.WriteLine($"code characters: {totalNumCodeCharacters}, encoded characters: {totalNumEncodedCharacters}");
        Console.WriteLine($"{totalNumEncodedCharacters} - {totalNumCodeCharacters} = {totalNumEncodedCharacters - totalNumCodeCharacters}");
      }
    }
  }
}
