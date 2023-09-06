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

        foreach (string line in lines)
        {
          List<int> numbers = new List<int>();
          for (int i = 0; i < size; i++)
          {
            numbers.Add(i);
          }

          List<int> inputs = convertToAsciiCode(line);
          Console.WriteLine($"Input: {line} -> {string.Join(",", inputs)}");

          int currentPosition = 0;
          int skipValue = 0;
          for (int r = 0; r < 64; r++)
          {
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
          }

          // XOR ^ of any value with 0 is the value and arrays are initialized with 0 values
          int[] output = new int[16];
          for (int i = 0; i < numbers.Count; i++)
          {
            output[i / 16] ^= numbers[i];
          }
          Console.Write("Knot Hash: ");
          foreach (int element in output)
          {
            Console.Write(element.ToString("X"));
          }
          Console.WriteLine();
        }
      }
    }
  }

  static List<int> convertToAsciiCode(string input)
  {
    List<int> suffixValues = new List<int> { 17, 31, 73, 47, 23 };
    List<int> convertedInput = new List<int>();
    foreach (char character in input)
    {
      convertedInput.Add((int)character);
    }
    convertedInput.AddRange(suffixValues);
    return convertedInput;
  }
}
