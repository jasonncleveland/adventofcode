using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

class PassportProcessing
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        string[] lines = File.ReadAllLines(fileName);

        List<string> requiredPassportFields = new List<string>
        {
          "byr",
          "iyr",
          "eyr",
          "hgt",
          "hcl",
          "ecl",
          "pid",
          // "cid", // Optional
        };
        List<string> validEyeColours = new List<string>
        {
          "amb",
          "blu",
          "brn",
          "gry",
          "grn",
          "hzl",
          "oth",
        };
        string heightPattern = @"(\d+)\w{2}";
        Regex heightRegex = new Regex(heightPattern, RegexOptions.Compiled);
        string hairColourPattern = @"#[0-9a-f]{6}";
        Regex hairColourRegex = new Regex(hairColourPattern, RegexOptions.Compiled);

        List<string> passports = new List<string>();

        // Parse passports
        List<string> passportLines = new List<string>();
        foreach (string line in lines)
        {
          if (line == "")
          {
            passports.Add(string.Join(" ", passportLines));
            passportLines = new List<string>();
          } else {
            passportLines.Add(line);
          }
        }
        // Add the last passport since we don't get an empty line at EOF
        passports.Add(string.Join(" ", passportLines));

        // Process passports
        int validPassportCount = 0;
        foreach (string passport in passports)
        {
          int validPassportFieldCount = 0;
          string[] passportFields = passport.Split(' ');
          foreach (string field in passportFields)
          {
            string[] values = field.Split(':');
            if (requiredPassportFields.Contains(values[0]))
            {
              switch (values[0])
              {
                case "byr":
                  int birthYear = int.Parse(values[1]);
                  if (birthYear >= 1920 && birthYear <= 2002) validPassportFieldCount++;
                  break;
                case "iyr":
                  int issueYear = int.Parse(values[1]);
                  if (issueYear >= 2010 && issueYear <= 2020) validPassportFieldCount++;
                  break;
                case "eyr":
                  int expirationYear = int.Parse(values[1]);
                  if (expirationYear >= 2020 && expirationYear <= 2030) validPassportFieldCount++;
                  break;
                case "hgt":
                  if (!heightRegex.IsMatch(values[1])) break;
                  Match match = heightRegex.Match(values[1]);
                  int height = int.Parse(match.Groups[1].Value);
                  if (values[1].Contains("cm"))
                  {
                    if (height >= 150 && height <= 193) validPassportFieldCount++;
                  }
                  else if (values[1].Contains("in"))
                  {
                    if (height >= 59 && height <= 76) validPassportFieldCount++;
                  }
                  break;
                case "hcl":
                  if (hairColourRegex.IsMatch(values[1])) validPassportFieldCount++;
                  break;
                case "ecl":
                  if (validEyeColours.Contains(values[1])) validPassportFieldCount++;
                  break;
                case "pid":
                  if (values[1].Length == 9) validPassportFieldCount++;
                  break;
              }
            }
          }
          if (validPassportFieldCount == requiredPassportFields.Count) validPassportCount++;
        }
        Console.WriteLine($"Number of valid passports: {validPassportCount}");
      }
    }
  }
}
