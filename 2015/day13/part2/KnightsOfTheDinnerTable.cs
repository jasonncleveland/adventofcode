using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

class KnightsOfTheDinnerTable
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        string[] lines = File.ReadAllLines(fileName);

        List<Knight> knights = new List<Knight>();

        // Add me
        Knight me = new Knight("me");
        knights.Add(me);

        string pattern = @"(\w+) would (\w+) (\d+) happiness units by sitting next to (\w+)";
        Regex regex = new Regex(pattern, RegexOptions.Compiled);

        foreach (string line in lines)
        {
          Match match = regex.Match(line);
          string name = match.Groups[1].Value;
          string operation = match.Groups[2].Value;
          int delta = int.Parse(match.Groups[3].Value);
          string neighbour = match.Groups[4].Value;

          if (operation == "lose")
          {
            delta *= -1;
          }

          Knight knight = knights.Find(k => k.Name == name);
          if (knight == null)
          {
            knight = new Knight(name);
            knight.HappinessUnits.Add("me", 0);
            me.HappinessUnits.Add(name, 0);
            knights.Add(knight);
          }
          knight.HappinessUnits.Add(neighbour, delta);
        }

        var permutations = generatePermuations(knights);

        int highestHappiness = int.MinValue;
        foreach (var permutation in permutations)
        {
          int totalHappiness = 0;
          for (int i = 0; i < permutation.Count; i++)
          {
            int left = 0, right = 0;
            if (i == 0)
            {
              // left
              left = permutation[i].HappinessUnits[permutation[permutation.Count - 1].Name];
              // right
              right = permutation[i].HappinessUnits[permutation[i + 1].Name];
            }
            else if (i == permutation.Count - 1)
            {
              // left
              left = permutation[i].HappinessUnits[permutation[i - 1].Name];
              // right
              right = permutation[i].HappinessUnits[permutation[0].Name];
            }
            else
            {
              // left
              left = permutation[i].HappinessUnits[permutation[i - 1].Name];
              // right
              right = permutation[i].HappinessUnits[permutation[i + 1].Name];
            }
            totalHappiness += left + right;
          }
          if (totalHappiness > highestHappiness)
          {
            highestHappiness = totalHappiness;
          }
        }

        Console.WriteLine($"Highest happiness: {highestHappiness}");
      }
    }
  }

  static List<List<Knight>> generatePermuations(List<Knight> knights)
  {
    List<List<Knight>> permutations = new List<List<Knight>>();
    doPermute(knights, new List<Knight>(), permutations);
    return permutations;
  }

  static void doPermute(List<Knight> knights, List<Knight> permutation, List<List<Knight>> permutations)
  {
    if (knights.Count == 0)
    {
      permutations.Add(permutation);
      return;
    }
    foreach (Knight knight in knights)
    {
      List<Knight> remainingKnights = new List<Knight>(knights);
      List<Knight> newPermutation = new List<Knight>(permutation);
      remainingKnights.Remove(knight);
      newPermutation.Add(knight);
      doPermute(remainingKnights, newPermutation, permutations);
    }
  }
}

class Knight
{
  public string Name { get; set; }
  public Dictionary<string, int> HappinessUnits { get; set; } = new Dictionary<string, int>();

  public Knight(string name)
  {
    Name = name;
  }
}
