using System;
using System.Collections.Generic;
using System.IO;

class MemoryReallocation
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        string[] lines = File.ReadAllLines(fileName);

        List<int> memoryBank = new List<int>();

        foreach (string line in lines)
        {
          Console.WriteLine(line);
          string[] blocks = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
          foreach (string block in blocks) {
            memoryBank.Add(int.Parse(block));
          }
        }
        HashSet<string> visitedStates = new HashSet<string>();

        for (int i = 1; ; i++)
        {
          string newState = reallocateMemory(memoryBank);
          if (visitedStates.Contains(newState))
          {
            Console.WriteLine($"Number of redistribution cycles: {i}");
            break;
          }
          visitedStates.Add(newState);
        }
      }
    }
  }

  static string reallocateMemory(List<int> memoryBank)
  {
    int maxValue = int.MinValue;
    foreach (int bank in memoryBank)
    {
      if (bank > maxValue) maxValue = bank;
    }

    int indexToReallocate = memoryBank.IndexOf(maxValue);
    memoryBank[indexToReallocate] = 0;

    for (int i = 0;  i < maxValue; i++)
    {
      memoryBank[(indexToReallocate + i + 1) % memoryBank.Count]++;
    }

    return string.Join(" ", memoryBank);
  }
}
