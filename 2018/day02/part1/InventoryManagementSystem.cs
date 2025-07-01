using System;
using System.Collections.Generic;
using System.IO;

class InventoryManagementSystem
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        string[] lines = File.ReadAllLines(fileName);

        int hasTwoCount = 0, hasThreeCount = 0;
        foreach (string line in lines)
        {
          Dictionary<char, int> map = new Dictionary<char, int>();

          foreach (char item in line)
          {
            if (!map.ContainsKey(item))
            {
              map.Add(item, 0);
            }
            map[item] += 1;
          }

          bool hasTwo = false, hasThree = false;
          foreach (KeyValuePair<char, int> items in map)
          {
            if (items.Value == 2) hasTwo = true;
            if (items.Value == 3) hasThree = true;
          }
          if (hasTwo) hasTwoCount += 1;
          if (hasThree) hasThreeCount += 1;
        }
        int checksum = hasTwoCount * hasThreeCount;
        Console.WriteLine($"Checksum: {hasTwoCount} * {hasThreeCount} = {checksum}");
      }
    }
  }
}
