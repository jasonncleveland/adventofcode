using System;
using System.Collections.Generic;
using System.IO;

class AlchemicalReduction
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        string[] lines = File.ReadAllLines(fileName);

        foreach (string line in lines)
        {
          Console.WriteLine(line);

          LinkedList<char> polymer = new LinkedList<char>(line);
          Console.WriteLine($"Starting polymer length: {polymer.Count}");

          LinkedListNode<char> currentUnit = polymer.First;
          while (currentUnit != null)
          {
            // Lower and Uppercase letters are separated by 32
            if (currentUnit.Previous != null && Math.Abs(currentUnit.Value - currentUnit.Previous.Value) == 32)
            {
              var firstUnit = currentUnit.Previous;
              var secondUnit = currentUnit;
              
              currentUnit = currentUnit.Next;

              polymer.Remove(firstUnit);
              polymer.Remove(secondUnit);
            }
            else
            {
              currentUnit = currentUnit.Next;
            }
          }

          Console.WriteLine($"Final polymer length: {polymer.Count}");
        }
      }
    }
  }
}
