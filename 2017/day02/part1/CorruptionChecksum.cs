using System;
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
          int lowest = int.MaxValue;
          int highest = int.MinValue;

          MatchCollection matches = regex.Matches(line);

          foreach (Match match in matches)
          {
            int number = int.Parse(match.Groups[1].Value);
            if (number > highest) highest = number;
            if (number < lowest) lowest = number;
          }
          int difference = Math.Abs(highest - lowest);
          checksum += difference;
        }
        Console.WriteLine($"Checksum: {checksum}");
      }
    }
  }
}
