using System;
using System.IO;
using System.Collections.Generic;

class DoesntHeHaveInternElvesForThis
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        string[] lines = File.ReadAllLines(fileName);

        int numNiceStrings = 0;

        foreach (string line in lines)
        {
          bool containsDoubleRepeatingLetter = false;
          bool containsInteruptedRepeatingLetter = false;

          List<RepeatingLetters> repeatingLetters = new List<RepeatingLetters>();

          for (int i = 0; i < line.Length; i++)
          {
            // Check for repeating letter pair
            if (i >= 1)
            {
              RepeatingLetters firstPair = repeatingLetters.Find(r => r.FirstLetter == line[i-1] && r.SecondLetter == line[i]);
              if (firstPair != null)
              {
                if (i - 1 > firstPair.SecondIndex)
                {
                  containsDoubleRepeatingLetter = true;
                }
              }
              else
              {
                repeatingLetters.Add(new RepeatingLetters(line[i-1], line[i], i - 1, i));
              }
            }

            // Check for repeating letter interupted by exactly one letter
            if (i >= 2 && line[i] == line[i-2])
            {
              containsInteruptedRepeatingLetter = true;
            }
          }

          if (containsDoubleRepeatingLetter && containsInteruptedRepeatingLetter)
          {
            numNiceStrings++;
          }
        }

        Console.WriteLine($"Number of nice strings: {numNiceStrings}");
      }
    }
  }
}

class RepeatingLetters
{
  public char FirstLetter { get; set; }
  public char SecondLetter { get; set; }
  public int FirstIndex { get; set; }
  public int SecondIndex { get; set; }

  public RepeatingLetters(char firstLetter, char secondLetter, int firstIndex, int secondIndex)
  {
    FirstLetter = firstLetter;
    SecondLetter = secondLetter;
    FirstIndex = firstIndex;
    SecondIndex = secondIndex;
  }
}
