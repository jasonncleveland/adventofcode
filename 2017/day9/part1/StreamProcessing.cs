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
            if (character == '<') inGarbage = true;
            if (character == '>') inGarbage = false;
            if (!inGarbage && character == '{') depth++;
            if (!inGarbage && character == '}')
            {
              groupCount++;
              totalScore += depth;
              depth--;
            }
          }
          Console.WriteLine($"Total Score: {totalScore} Group Count: {groupCount}");
        }
      }
    }
  }
}
