using System;
using System.Collections.Generic;
using System.IO;

class StreamProcessing
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
          int groupCount = 0;
          int totalScore = 0;
          int depth = 0;
          int garbageCharacterCount = 0;
          bool inGarbage = false;
          bool ignoreNextCharacter = false;
          foreach (char character in line)
          {
            if (ignoreNextCharacter) 
            {
              ignoreNextCharacter = false;
              continue;
            }

            if (character == '!')
            {
              ignoreNextCharacter = true;
              continue;
            }
            if (!inGarbage && character == '<')
            {
              inGarbage = true;
              continue;
            }
            if (inGarbage && character == '>')
            {
              inGarbage = false;
              continue;
            }
            if (!inGarbage && character == '{') depth++;
            if (!inGarbage && character == '}')
            {
              groupCount++;
              totalScore += depth;
              depth--;
            }
            if (inGarbage) garbageCharacterCount++;
          }
          Console.WriteLine($"Total Score: {totalScore} Garbage Character Count: {garbageCharacterCount} Group Count: {groupCount}");
        }
      }
    }
  }
}
