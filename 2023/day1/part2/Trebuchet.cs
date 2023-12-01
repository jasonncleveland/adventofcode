using System;
using System.IO;

class Trebuchet
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        string[] lines = File.ReadAllLines(fileName);

        int calibrationValuesTotal = 0;
        foreach (string line in lines)
        {
          char firstSeenDigit = findDigit(line);
          char lastSeenDigit = '\0';
          // Search the string in reverse to find the last digit
          for (int i = line.Length; i > 0; i--)
          {
            string substring = line.Substring(i - 1);
            lastSeenDigit = findDigit(substring);
            if (lastSeenDigit != '\0') break;
          }
          int calibrationValue = int.Parse($"{firstSeenDigit}{lastSeenDigit}");
          calibrationValuesTotal += calibrationValue;
        }

        Console.WriteLine($"Total calibration value: {calibrationValuesTotal}");
      }
    }
  }

  static char findDigit(string line)
  {
    for (int i = 0; i < line.Length; i++)
    {
      char character = line[i];
      if (char.IsDigit(character))
      {
        return character;
      }
      int remainingCharacters = line.Length - i;
      if (character == 'e' && remainingCharacters >= 5)
      {
        // eight
        if (line.Substring(i, 5).Contains("eight"))
        {
          return '8';
        }
      }
      else if (character == 'f' && remainingCharacters >= 4)
      {
        // five
        if (line.Substring(i, 4).Contains("five"))
        {
          return '5';
        }
        // four
        else if (line.Substring(i, 4).Contains("four"))
        {
          return '4';
        }
      }
      else if (character == 'n' && remainingCharacters >= 4)
      {
        // nine
        if (line.Substring(i, 4).Contains("nine"))
        {
          return '9';
        }
      }
      else if (character == 'o' && remainingCharacters >= 3)
      {
        // one
        if (line.Substring(i, 3).Contains("one"))
        {
          return '1';
        }
      }
      else if (character == 's' && remainingCharacters >= 3)
      {
        // seven
        if (remainingCharacters >= 5)
        {
          if (line.Substring(i, 5).Contains("seven"))
          {
            return '7';
          }
        }
        // six
        if (line.Substring(i, 3).Contains("six"))
        {
          return '6';
        }
      }
      else if (character == 't' && remainingCharacters >= 3)
      {
        // three
        if (remainingCharacters >= 5)
        {
          if (line.Substring(i, 5).Contains("three"))
          {
            return '3';
          }
        }
        // two
        if (line.Substring(i, 3).Contains("two"))
        {
          return '2';
        }
      }
    }
    return '\0';
  }
}
