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

        Dictionary<int, int> copies = new Dictionary<int, int>();

        int scratchCardsProcessed = 0;
        int cardId = 1;
        foreach (string line in lines)
        {
          string[] data = line.Split(':');
          string[] cards = data[1].Split('|');
          int numberOfCopies = 0;
          copies.TryGetValue(cardId, out numberOfCopies);

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
          IEnumerable<int> overlap = winningNumbers.Intersect(numbersToCheck);
          scratchCardsProcessed += 1 + numberOfCopies;
          if (overlap.Count() > 0)
          {
            // Rather than reprocessing each card if there are copies, we can
            // just multiply the result by the number of copies "processed"
            for (int i = 1; i <= overlap.Count(); i++)
            {
              if (!copies.ContainsKey(cardId + i))
              {
                copies.Add(cardId + i, 0);
              }
              copies[cardId + i] += 1 + numberOfCopies;
            }
          }
          cardId++;
        }
        Console.WriteLine($"Total value: {scratchCardsProcessed}");

        stopWatch.Stop();
        Console.WriteLine($"Elapsed Time: {stopWatch.ElapsedMilliseconds} ms");
      }
    }
  }
}
