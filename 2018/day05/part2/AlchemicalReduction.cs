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

          int shortestPolymerLength = int.MaxValue;
          string removedUnit = null;
          for (char lowercase = 'a', uppercase = 'A'; lowercase <= 'z'; lowercase++, uppercase++)
          {
            string newPolymer = line.Replace(lowercase.ToString(), "").Replace(uppercase.ToString(), "");
            int newPolymerLength = calculatePolymerLength(newPolymer);
            if (newPolymerLength < shortestPolymerLength)
            {
              shortestPolymerLength = newPolymerLength;
              removedUnit = $"{lowercase}{uppercase}";
            }
          }
          Console.WriteLine($"Shortest polymer length: {shortestPolymerLength} created by removing {removedUnit}");
        }
      }
    }
  }

  static int calculatePolymerLength(string polymerString)
  {
    LinkedList<char> polymer = new LinkedList<char>(polymerString);

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

    return polymer.Count;
  }
}
