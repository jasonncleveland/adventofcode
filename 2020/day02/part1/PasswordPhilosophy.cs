using System;
using System.IO;
using System.Text.RegularExpressions;

class PasswordPhilosophy
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        string[] lines = File.ReadAllLines(fileName);

        string pattern = @"(?<low>\d+)-(?<high>\d+) (?<expected>\w): (?<password>\w+)";
        Regex regex = new Regex(pattern, RegexOptions.Compiled);

        int validPasswordCount = 0;
        foreach (string line in lines)
        {
          Match match = regex.Match(line);

          int lowValue = int.Parse(match.Groups["low"].Value);
          int highValue = int.Parse(match.Groups["high"].Value);
          char expectedValue = char.Parse(match.Groups["expected"].Value);
          string password = match.Groups["password"].Value;

          int expectedValueCount = 0;
          foreach (char letter in password)
          {
            if (letter == expectedValue) expectedValueCount++;
          }
          if (expectedValueCount >= lowValue && expectedValueCount <= highValue)
          {
            validPasswordCount += 1;
          }
        }
        Console.WriteLine($"Valid passwords: {validPasswordCount}");
      }
    }
  }
}
