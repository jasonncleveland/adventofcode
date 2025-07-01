using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

class Scratchcards
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();

        string[] lines = File.ReadAllLines(fileName);

        int total = 0;
        foreach (string line in lines)
        {
          string[] data = line.Split(':');
          string[] cards = data[1].Split('|');

          List<List<int>> parsedCards = new List<List<int>>();
          foreach (string card in cards)
          {
            List<int> parsedNumbers = new List<int>();
            List<char> currentNumber = new List<char>();
            foreach (char character in card.Trim())
            {
              if (char.IsDigit(character))
              {
                currentNumber.Add(character);
              }
              else
              {
                if (currentNumber.Count > 0)
                {
                  int number = int.Parse(string.Join("", currentNumber));
                  parsedNumbers.Add(number);
                  currentNumber = new List<char>();
                }
              }
            }
            // Add final number
            if (currentNumber.Count > 0)
            {
              int number = int.Parse(string.Join("", currentNumber));
              parsedNumbers.Add(number);
              currentNumber = new List<char>();
            }
            parsedCards.Add(parsedNumbers);
          }

          List<int> winningNumbers = parsedCards.First();
          List<int> numbersToCheck = parsedCards.Last();
          IEnumerable<int> commonNumbers = winningNumbers.Intersect(numbersToCheck);
          if (commonNumbers.Count() > 0)
          {
            int cardScore = 1;
            for (int i = 1; i < commonNumbers.Count(); i++)
            {
              cardScore *= 2;
            }
            total += cardScore;
          }
        }
        Console.WriteLine($"Total value: {total}");

        stopWatch.Stop();
        Console.WriteLine($"Elapsed Time: {stopWatch.ElapsedMilliseconds} ms");
      }
    }
  }
}
