using System;
using System.Collections.Generic;
using System.IO;

class SecureContainer
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
          string[] lineParts = line.Split('-');
          int start = int.Parse(lineParts[0]);
          int end = int.Parse(lineParts[1]);
          Console.WriteLine($"Checking passwords from {start} to {end}");
          int validPasswords = 0;
          for (int i = start; i <= end; i++)
          {
            if (isPasswordValid(i.ToString()))
            {
              validPasswords++;
            }
          }
          Console.WriteLine($"Found {validPasswords} valid passwords");
        }
      }
    }
  }

  static bool isPasswordValid(string password)
  {
    bool repeatingDigit = false;
    bool alwaysIncreasing = true;
    char previousDigit = password[0];
    for (int i = 1; i < password.Length; i++)
    {
      char currentDigit = password[i];
      if (currentDigit == previousDigit)
      {
        repeatingDigit = true;
      }
      if (currentDigit < previousDigit)
      {
        alwaysIncreasing = false;
        break;
      }
      previousDigit = currentDigit;
    }
    return repeatingDigit && alwaysIncreasing;
  }
}
