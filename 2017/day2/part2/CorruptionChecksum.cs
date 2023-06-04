using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

class CorruptionChecksum
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        string[] lines = File.ReadAllLines(fileName);

        string pattern = @"(\d+\w*)+";
        Regex regex = new Regex(pattern, RegexOptions.Compiled);

        int checksum = 0;
        foreach (string line in lines)
        {
          MatchCollection matches = regex.Matches(line);

          // Parse row
          List<int> lineItems = new List<int>();
          foreach (Match match in matches)
          {
            int number = int.Parse(match.Groups[1].Value);
            lineItems.Add(number);
          }

          // Perform check
          for (int i = 0; i < lineItems.Count; i++)
          {
            for (int j = 0; j < lineItems.Count; j++)
            {
              if (i == j) continue;

              if (lineItems[i] % lineItems[j] == 0)
              {
                checksum += lineItems[i] / lineItems[j];
              }
            }
          }
        }
        Console.WriteLine($"Checksum: {checksum}");
      }
    }
  }
}
