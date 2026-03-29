using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using AdventOfCode.Shared.Solver;

using Microsoft.Extensions.Logging;

namespace AdventOfCode.Y2020.Days;

internal sealed class Day04 : AbstractDaySolver<IReadOnlyList<IReadOnlyDictionary<string, string>>>
{
    protected override IReadOnlyList<IReadOnlyDictionary<string, string>> ParseInput(ILogger logger, string fileContents)
    {
        var passports = new List<Dictionary<string, string>>();
        var passportStrings = fileContents.Split("\n\n", StringSplitOptions.RemoveEmptyEntries);

        foreach (var passportString in passportStrings)
        {
            var passportFields = passportString.Split([' ', '\n'], StringSplitOptions.RemoveEmptyEntries);
            var passport = passportFields
                .Select(field => field.Split(':'))
                .ToDictionary(field => field[0], field => field[1]);
            passports.Add(passport);
        }

        return passports;
    }

    protected override string SolvePart1(ILogger logger, IReadOnlyList<IReadOnlyDictionary<string, string>> passports)
    {
        var requiredPassportFields = new List<string>
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

        // Process passports
        int validPassportCount = 0;
        foreach (var passport in passports)
        {
            int validPassportFieldCount = 0;
            foreach (var requiredPassportField in requiredPassportFields)
            {
                if (passport.ContainsKey(requiredPassportField))
                {
                    validPassportFieldCount++;
                }
            }
            if (validPassportFieldCount == requiredPassportFields.Count) validPassportCount++;
        }
        return validPassportCount.ToString();
    }

    protected override string SolvePart2(ILogger logger, IReadOnlyList<IReadOnlyDictionary<string, string>> passports)
    {
        var requiredPassportFields = new List<string>
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
        var validEyeColours = new List<string>
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
        var heightRegex = new Regex(heightPattern, RegexOptions.Compiled);
        string hairColourPattern = @"#[0-9a-f]{6}";
        var hairColourRegex = new Regex(hairColourPattern, RegexOptions.Compiled);

        // Process passports
        int validPassportCount = 0;
        foreach (var passport in passports)
        {
            int validPassportFieldCount = 0;
            foreach ((string field, string value) in passport)
            {
                if (requiredPassportFields.Contains(field))
                {
                    switch (field)
                    {
                        case "byr":
                            int birthYear = int.Parse(value);
                            if (birthYear >= 1920 && birthYear <= 2002) validPassportFieldCount++;
                            break;
                        case "iyr":
                            int issueYear = int.Parse(value);
                            if (issueYear >= 2010 && issueYear <= 2020) validPassportFieldCount++;
                            break;
                        case "eyr":
                            int expirationYear = int.Parse(value);
                            if (expirationYear >= 2020 && expirationYear <= 2030) validPassportFieldCount++;
                            break;
                        case "hgt":
                            if (!heightRegex.IsMatch(value)) break;
                            Match match = heightRegex.Match(value);
                            int height = int.Parse(match.Groups[1].Value);
                            if (value.Contains("cm"))
                            {
                                if (height >= 150 && height <= 193) validPassportFieldCount++;
                            }
                            else if (value.Contains("in"))
                            {
                                if (height >= 59 && height <= 76) validPassportFieldCount++;
                            }
                            break;
                        case "hcl":
                            if (hairColourRegex.IsMatch(value)) validPassportFieldCount++;
                            break;
                        case "ecl":
                            if (validEyeColours.Contains(value)) validPassportFieldCount++;
                            break;
                        case "pid":
                            if (value.Length == 9) validPassportFieldCount++;
                            break;
                    }
                }
            }
            if (validPassportFieldCount == requiredPassportFields.Count) validPassportCount++;
        }
        return validPassportCount.ToString();
    }
}