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

        string pattern = @"(?<first>\d+)-(?<second>\d+) (?<expected>\w): (?<password>\w+)";
        Regex regex = new Regex(pattern, RegexOptions.Compiled);

        int validPasswordCount = 0;
        foreach (string line in lines)
        {
          Match match = regex.Match(line);

          int firstPosition = int.Parse(match.Groups["first"].Value);
          int secondPosition = int.Parse(match.Groups["second"].Value);
          char expectedValue = char.Parse(match.Groups["expected"].Value);
          string password = match.Groups["password"].Value;

          bool valueInFirstPosition = password[firstPosition - 1] == expectedValue;
          bool valueInSecondPosition = password[secondPosition - 1] == expectedValue;
          if (valueInFirstPosition && !valueInSecondPosition || valueInSecondPosition && !valueInFirstPosition)
          {
            validPasswordCount += 1;
          }
        }
        Console.WriteLine($"Valid passwords: {validPasswordCount}");
      }
    }
  }
}
