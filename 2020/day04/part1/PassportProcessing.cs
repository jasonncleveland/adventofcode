using System;
using System.Collections.Generic;
using System.IO;

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
            if (requiredPassportFields.Contains(values[0])) validPassportFieldCount++;
          }
          if (validPassportFieldCount == requiredPassportFields.Count) validPassportCount++;
        }
        Console.WriteLine($"Number of valid passports: {validPassportCount}");
      }
    }
  }
}
