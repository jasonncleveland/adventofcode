using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

class AuntSue
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        string[] lines = File.ReadAllLines(fileName);

        string[] compounds = new string[] {
          "children",
          "cats",
          "samoyeds",
          "pomeranians",
          "akitas",
          "vizslas",
          "goldfish",
          "trees",
          "cars",
          "perfumes"
        };
        Dictionary<string, int> mysterySue = new Dictionary<string, int>()
        {
          { "children", 3 },
          { "cats", 7 },
          { "samoyeds", 2 },
          { "pomeranians", 3 },
          { "akitas", 0 },
          { "vizslas", 0 },
          { "goldfish", 5 },
          { "trees", 3 },
          { "cars", 2 },
          { "perfumes", 1 }
        };

        foreach (string line in lines)
        {
          bool isPossibleSue = true;
          foreach (string compound in compounds)
          {
            if (Regex.IsMatch(line, String.Format(@"{0}: (?<{0}>\d+)", compound)))
            {
              int compoundValue = int.Parse(Regex.Match(line, String.Format(@"{0}: (?<{0}>\d+)", compound)).Groups[1].Value.ToString());
              if (compoundValue != mysterySue[compound])
              {
                isPossibleSue = false;
                break;
              }
            }
          }
          if (isPossibleSue)
          {
            Console.WriteLine(line);
          }
        }
      }
    }
  }
}
