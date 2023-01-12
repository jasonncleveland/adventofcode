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

        string pattern = @"(?:(?<supernet>\w+)\[(?<hypernet>\w+)\])*(?<supernet>\w+)?";
        Regex regex = new Regex(pattern, RegexOptions.Compiled);

        int validAddresses = 0;
        foreach (string line in lines)
        {
          Match match = regex.Match(line);

          bool isValidAddress = false;

          // Find all ABA sequences
          HashSet<string> abaSequences = new HashSet<string>();
          foreach (Capture c in match.Groups["supernet"].Captures)
          {
            List<string> sequences = getAbaSequences(c.Value);
            abaSequences.UnionWith(sequences);
          }

          // Check each ABA sequence for matching BAB sequence
          foreach (Capture c in match.Groups["hypernet"].Captures)
          {
            if (containsBabSequence(abaSequences, c.Value))
            {
              isValidAddress = true;
              break;
            }
          }
          if (isValidAddress) validAddresses++;
        }
        Console.WriteLine($"Number of valid IPs: {validAddresses}");
      }
    }
  }

  static List<string> getAbaSequences(string input)
  {
    List<string> sequences = new List<string>();
    if (input.Length < 3)
    {
      return sequences;
    }

    for (int i = 2; i < input.Length; i++)
    {
      if (input[i] != input[i - 1] && input[i - 2] == input[i])
      {
        sequences.Add(new string(new char[] { input[i - 2], input[i - 1], input[i] }));
      }
    }
    return sequences;
  }

  static bool containsBabSequence(HashSet<string> abaSequences, string input)
  {
    if (input.Length < 3)
    {
      return false;
    }

    for (int i = 2; i < input.Length; i++)
    {
      foreach (string abaSequence in abaSequences)
      {
        if (
          input[i] == abaSequence[1]
          && input[i - 1] == abaSequence[0]
          && input[i] != input[i - 1]
          && input[i - 2] == input[i]
        )
        {
          return true;
        }
      }
    }
    return false;
  }
}
