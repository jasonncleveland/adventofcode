using System;
using System.Collections.Generic;
using System.IO;

class ExplosivesInCyberspace
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
          long decompressedStringLength = decompressString(line);
          Console.WriteLine($"Decompressed length: {decompressedStringLength}");
        }
      }
    }
  }

  static long decompressString(string line)
  {
    long characterCount = 0;

    bool inMarker = false;
    int start = 0, end = 0;

    for (int i = 0; i < line.Length; i++)
    {
      char character = line[i];
      if (character == '(')
      {
        start = i;
        inMarker = true;
      }
      else if (character == ')')
      {
        end = i;
        string[] operands = line.Substring(start + 1, end - start - 1).Split('x');
        int repeatLength = int.Parse(operands[0]);
        int repeatCount = int.Parse(operands[1]);
        char[] repeatString = line.Substring(end + 1, repeatLength).ToCharArray();
        long decompressedRepeatStringLength = decompressString(new string(repeatString));
        characterCount += decompressedRepeatStringLength * repeatCount;
        inMarker = false;
        i += repeatLength;
      }
      else
      {
        if (!inMarker)
        {
          characterCount++;
        }
      }
    }

    return characterCount;
  }
}
