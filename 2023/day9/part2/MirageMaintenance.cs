using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

class MirageMaintenance
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

        int total = 0;
        foreach (string line in lines)
        {
          string[] lineParts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
          List<int> lineNumbers = new List<int>();
          foreach (string number in lineParts)
          {
            lineNumbers.Add(int.Parse(number));
          }
          int previousValue = getPreviousValue(lineNumbers);
          total += previousValue;
        }
        Console.WriteLine($"Total value: {total}");

        stopWatch.Stop();
        Console.WriteLine($"Elapsed Time: {stopWatch.ElapsedMilliseconds} ms");
      }
    }
  }

  static int getPreviousValue(List<int> numbers)
  {
    List<int> differences = new List<int>();
    for (int i = 1; i < numbers.Count; i++)
    {
      differences.Add(numbers[i] - numbers[i - 1]);
    }
    if (differences.All(difference => difference == 0))
    {
      return numbers.First();
    }
    else
    {
      return numbers.First() - getPreviousValue(differences);
    }
  }
}
