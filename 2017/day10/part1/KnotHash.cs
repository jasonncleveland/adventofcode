using System;
using System.Collections.Generic;
using System.IO;

class KnotHash
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      int size = args.Length > 1 ? int.Parse(args[1]) : 256;
      if (File.Exists(fileName))
      {
        string[] lines = File.ReadAllLines(fileName);

        List<int> numbers = new List<int>();
        for (int i = 0; i < size; i++)
        {
          numbers.Add(i);
        }

        foreach (string line in lines)
        {
          List<int> inputs = new List<int>(Array.ConvertAll<string, int>(line.Split(','), element => int.Parse(element)));

          int currentPosition = 0;
          int skipValue = 0;
          foreach (int input in inputs)
          {
            // Create a sub list with the value to reverse
            List<int> rangeToReverse = new List<int>();
            for (int i = 0; i < input; i++)
            {
              rangeToReverse.Add(numbers[(currentPosition + i) % numbers.Count]);
            }

            // Replace the values with the reversed values
            for (int i = 0; i < input; i++)
            {
              numbers[(currentPosition + i) % numbers.Count] = rangeToReverse[rangeToReverse.Count - 1 - i];
            }

            currentPosition = (currentPosition + input + skipValue) % numbers.Count;
            skipValue = (skipValue + 1) % numbers.Count;
          }
          Console.WriteLine($"End Result: {numbers[0]} * {numbers[1]} = {numbers[0] * numbers[1]}");
        }
      }
    }
  }
}
