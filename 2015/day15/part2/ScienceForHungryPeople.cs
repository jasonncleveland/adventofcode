using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

class ScienceForHungryPeople
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        string[] lines = File.ReadAllLines(fileName);

        string pattern = @"(\w+): capacity (\-?\d+), durability (\-?\d+), flavor (\-?\d+), texture (\-?\d+), calories (\-?\d+)";
        Regex regex = new Regex(pattern, RegexOptions.Compiled);

        List<Cookie> cookiesList = new List<Cookie>();

        foreach (string line in lines)
        {
          Match match = regex.Match(line);

          string name = match.Groups[1].Value;
          int capacity = int.Parse(match.Groups[2].Value);
          int durability = int.Parse(match.Groups[3].Value);
          int flavor = int.Parse(match.Groups[4].Value);
          int texture = int.Parse(match.Groups[5].Value);
          int calories = int.Parse(match.Groups[6].Value);

          Cookie cookie = new Cookie()
          {
            Name = name,
            Capacity = capacity,
            Durability = durability,
            Flavor = flavor,
            Texture = texture,
            Calories = calories,
          };
          cookiesList.Add(cookie);
        }

        List<int> portions = new List<int>();
        for (int i = 0; i < cookiesList.Count; i++)
        {
          portions.Add(1);
        }

        List<List<int>> permutations = new List<List<int>>();
        calculatePortions(portions, 0, 100, permutations);

        long highestTotal = int.MinValue;
        foreach (var permutation in permutations)
        {
          int c = 0, d = 0, f = 0, t = 0, k = 0;
          for (int i = 0; i < permutation.Count; i++)
          {
            c += cookiesList[i].Capacity * permutation[i];
            d += cookiesList[i].Durability * permutation[i];
            f += cookiesList[i].Flavor * permutation[i];
            t += cookiesList[i].Texture * permutation[i];
            k += cookiesList[i].Calories * permutation[i];
          }
          long total = (c > 0 ? c : 0) * (d > 0 ? d : 0) * (f > 0 ? f : 0) * (t > 0 ? t : 0);
          if (total > highestTotal && k == 500)
          {
            highestTotal = total;
          }
        }
        Console.WriteLine($"Highest total: {highestTotal}");
      }
    }
  }

  /*
   * This is wildly inefficient
   */
  static void calculatePortions(List<int> portions, int index, int max, List<List<int>> permutations)
  {
    if (index == portions.Count)
    {
      return;
    }

    // Try all possible values for this index
    for (int i = portions[index]; i <= max; i++)
    {
      List<int> newList = new List<int>(portions);
      newList[index] = i;
      int sum = sumPortions(newList);
      if (sum == max)
      {
        permutations.Add(newList);
      }

      // Try all possible values for remaining indexes
      for (int j = index + 1; j < portions.Count; j++)
      {
        calculatePortions(newList, j, max, permutations);
      }
    }
  }

  static int sumPortions(List<int> portions)
  {
    int total = 0;
    foreach (int portion in portions)
    {
      total += portion;
    }
    return total;
  }

  static void printPortions(List<int> portions)
  {
    foreach (int portion in portions)
    {
      Console.Write($"{portion} ");
    }
    Console.WriteLine();
  }
}

class Cookie
{
  public string Name { get; set; }
  public int Capacity { get; set; }
  public int Durability { get; set; }
  public int Flavor { get; set; }
  public int Texture { get; set; }
  public int Calories { get; set; }
}
