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
          List<char> decompressedCharacters = new List<char>();
          int start = 0, end = 0;
          bool inMarker = false;
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
              for (int j = 0; j < repeatCount; j++)
              {
                decompressedCharacters.AddRange(repeatString);
              }
              inMarker = false;
              i += repeatLength;
            }
            else
            {
              if (!inMarker)
              {
                decompressedCharacters.Add(character);
              }
            }
          }
          Console.WriteLine(decompressedCharacters.ToArray());
          Console.WriteLine($"Decompressed length: {decompressedCharacters.Count}");
        }
      }
    }
  }
}
