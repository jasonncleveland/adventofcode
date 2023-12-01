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
          char firstSeenDigit = '\0';
          char lastSeenDigit = '\0';
          foreach (char character in line)
          {
            if (char.IsDigit(character))
            {
              if (firstSeenDigit == '\0')
              {
                firstSeenDigit = character;
              }
              lastSeenDigit = character;
            }
          }
          int calibrationValue = int.Parse($"{firstSeenDigit}{lastSeenDigit}");
          calibrationValuesTotal += calibrationValue;
        }
        Console.WriteLine($"Total calibration value: {calibrationValuesTotal}");
      }
    }
  }
}
