using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

class InternetProtocolVersion7
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        string[] lines = File.ReadAllLines(fileName);

        string pattern = @"(?:(?<other>\w+)\[(?<hypernet>\w+)\])*(?<other>\w+)?";
        Regex regex = new Regex(pattern, RegexOptions.Compiled);

        int validAddresses = 0;
        foreach (string line in lines)
        {
          Match match = regex.Match(line);

          bool isValidAddress = false;
          foreach (Capture c in match.Groups["other"].Captures)
          {
            if (containsAbbaSequence(c.Value))
            {
              isValidAddress = true;
              break;
            }
          }
          foreach (Capture c in match.Groups["hypernet"].Captures)
          {
            if (containsAbbaSequence(c.Value))
            {
              isValidAddress = false;
              break;
            }
          }
          if (isValidAddress) validAddresses++;
        }
        Console.WriteLine($"Number of valid IPs: {validAddresses}");
      }
    }
  }

  static bool containsAbbaSequence(string input)
  {
    if (input.Length < 4)
    {
      return false;
    }

    for (int i = 3; i < input.Length; i++)
    {
      if (input[i] != input[i - 1] && input[i - 3] == input[i] && input[i - 2] == input[i - 1])
      {
        return true;
      }
    }
    return false;
  }
}
