using System;
using System.Collections.Generic;
using System.IO;

class CalorieCounting
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        string fileContents = File.ReadAllText(fileName);
        string[] lines = fileContents.Split('\n');
        int individualTotal = 0;
        List<int> individualTotals = new List<int>();
        foreach (string line in lines)
        {
          if (line.Equals(""))
          {
            individualTotals.Add(individualTotal);
            individualTotal = 0;
          }
          else
          {
            int amount;
            if (int.TryParse(line, out amount))
            {
              individualTotal += amount;
            }
          }
        }
        individualTotals.Add(individualTotal);

        // Sort list descending
        individualTotals.Sort((a, b) => b.CompareTo(a));

        int top3Total = 0;
        for (int i = 0; i < 3; i++)
        {
          top3Total += individualTotals[i];
        }
        Console.WriteLine($"Value of top 3: {top3Total}");
      }
    }
  }
}
