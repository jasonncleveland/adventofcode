using System;
using System.IO;

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
        string[] naughtySubstrings = new string[] { "ab", "cd", "pq", "xy" };

        foreach (string line in lines)
        {
          // Check if string contains any forbidden strings
          bool containsNaughtyString = !Array.TrueForAll<string>(naughtySubstrings, s => line.IndexOf(s) < 0);

          bool containsRepeatingLetter = false;

          int vowelCount = 0;
          char lastLetter = '\0';

          foreach (char letter in line)
          {
            // Check for repeating letters
            if (lastLetter != '\0' && lastLetter == letter)
            {
              containsRepeatingLetter = true;
            }

            // Check for vowels
            if (letter == 'a' || letter == 'e' || letter == 'i' || letter == 'o' || letter == 'u')
            {
              vowelCount++;
            }

            lastLetter = letter;
          }
          bool containsAtLeastThreeVowels = vowelCount >= 3;

          if (containsAtLeastThreeVowels && containsRepeatingLetter && !containsNaughtyString)
          {
            numNiceStrings++;
          }
        }

        Console.WriteLine($"Number of nice strings: {numNiceStrings}");
      }
    }
  }
}
