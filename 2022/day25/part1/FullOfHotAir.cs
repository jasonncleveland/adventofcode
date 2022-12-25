using System;
using System.Collections.Generic;
using System.IO;

class FullOfHotAir
{
  static readonly List<char> numerals = new List<char>(new char[] { '=', '-', '0', '1', '2' });

  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        string fileContents = File.ReadAllText(fileName);
        string[] lines = fileContents.Split('\n');

        long totalSum = 0;
        foreach (string line in lines)
        {
          long convertedSnafu = convertFromSnafu(line);
          totalSum += convertedSnafu;
        }

        Console.WriteLine($"Total decimal sum: {totalSum}");
        Console.WriteLine($"Total SNAFU sum: {convertToSnafu(totalSum)}");
      }
    }
  }

  static long convertFromSnafu(string snafu)
  {
    char[] snafuCharacters = snafu.ToCharArray();
    Array.Reverse(snafuCharacters);
    string reversedSnafu = string.Join("", snafuCharacters);
    long convertedSnafu = 0;
    for (int i = 0; i < reversedSnafu.Length; i++)
    {
      char numeral = reversedSnafu[i];
      int numeralIndex = numerals.IndexOf(numeral);
      long convertedNumeral = ((long) Math.Pow(5, i)) * (numeralIndex - 2);
      convertedSnafu += convertedNumeral;
    }
    return convertedSnafu;
  }

  static string convertToSnafu(long decimalNumber)
  {
    long quotient, remainder;
    List<char> reversedSnafu = new List<char>();

    quotient = decimalNumber;
    do
    {
      remainder = quotient % 5;
      quotient = quotient / 5;

      if (remainder == 3)
      {
        reversedSnafu.Add('=');
        quotient += 1;
      }
      else if (remainder == 4)
      {
        reversedSnafu.Add('-');
        quotient += 1;
      }
      else
      {
        reversedSnafu.Add((char) ('0' + remainder));
      }
    }
    while (quotient > 0);

    char[] snafu = reversedSnafu.ToArray();
    Array.Reverse(snafu);

    return string.Join("", snafu);
  }
}
