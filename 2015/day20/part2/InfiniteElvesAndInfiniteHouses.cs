using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

class InfiniteElvesAndInfiniteHouses
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();

        string[] lines = File.ReadAllLines(fileName);

        Dictionary<int, int> housesVisited = new Dictionary<int, int>();
        // Stop delivering presents after 50 houses
        int maxPresents = 50;

        foreach (string line in lines)
        {
          Console.WriteLine(line);
          int threshold = int.Parse(line);

          for (int i = 1;; i++)
          {
            HashSet<int> factors = calculateFactors(i);
            int presentsCount = 0;
            foreach (int factor in factors)
            {
              if (!housesVisited.ContainsKey(factor))
              {
                housesVisited.Add(factor, 0);
              }
              housesVisited[factor] += 1;

              if (housesVisited[factor] <= maxPresents)
              {
                presentsCount += factor * 11;
              }
            }
            if (presentsCount >= threshold)
            {
              Console.WriteLine($"First house to receive at least {threshold} presents: {i}");
              break;
            }
          }
        }

        stopWatch.Stop();
        Console.WriteLine($"Elapsed Time: {stopWatch.ElapsedMilliseconds} ms");
      }
    }
  }

  static HashSet<int> calculateFactors(int number)
  {
    HashSet<int> factors = new HashSet<int>();

    int sqrt = (int) Math.Ceiling(Math.Sqrt(number));

    for (int i = 1; i <= sqrt; i++)
    {
      if (number % i == 0)
      {
        // Add the found factor and its pair
        factors.Add(i);
        factors.Add(number / i);
      }
    }

    return factors;
  }
}
