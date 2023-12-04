using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

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

        foreach (string line in lines)
        {
          int threshold = int.Parse(line);

          for (int i = 1;; i++)
          {
            HashSet<int> factors = calculateFactors(i);
            int presentsCount = factors.Aggregate(0, (total, factor) => total += factor * 10);
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
