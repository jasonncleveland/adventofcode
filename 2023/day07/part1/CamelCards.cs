using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

class CamelCards
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

        List<HandDetails> hands = new List<HandDetails>();
        foreach (string line in lines)
        {
          string[] lineParts = line.Split(' ');
          string hand = lineParts[0];
          int bid = int.Parse(lineParts[1]);
          hands.Add(new HandDetails(hand, bid));
        }

        hands.Sort();

        int total = 0;
        int rank = 1;
        foreach (HandDetails hand in hands)
        {
          total += rank * hand.Bid;
          rank++;
        }
        Console.WriteLine($"Total value: {total}");

        stopWatch.Stop();
        Console.WriteLine($"Elapsed Time: {stopWatch.ElapsedMilliseconds} ms");
      }
    }
  }
}

enum CardHand
{
  FiveOfAKind = 1,
  FourOfAKind = 2,
  FullHouse = 3,
  ThreeOfAKind = 4,
  TwoPair = 5,
  OnePair = 6,
  HighCard = 7,
  Invalid = -1
}

class HandDetails: IComparable<HandDetails>
{
  public static readonly Dictionary<char, int> CardValues = new Dictionary<char, int>()
  {
    { 'A', 1 },
    { 'K', 2 },
    { 'Q', 3 },
    { 'J', 4 },
    { 'T', 5 },
    { '9', 6 },
    { '8', 7 },
    { '7', 8 },
    { '6', 9 },
    { '5', 10 },
    { '4', 11 },
    { '3', 12 },
    { '2', 13 },
  };
  public string Hand { get; set; }
  public int Bid { get; set; }

  public HandDetails(string hand, int bid)
  {
    Hand = hand;
    Bid = bid;
  }

  public int CompareTo(HandDetails other)
  {
    CardHand ourHandType = this.GetHandType();
    CardHand theirHandType = other.GetHandType();
    int result = 0;
    if (ourHandType == theirHandType)
    {
      result = 0;
      for (int i = 0; i < Hand.Length; i++)
      {
        if (Hand[i] != other.Hand[i])
        {
          result = HandDetails.CardValues[other.Hand[i]] - HandDetails.CardValues[Hand[i]];
          break;
        }
      }
    }
    else
    {
      result = theirHandType - ourHandType;
    }

    return result;
  }

  public CardHand GetHandType()
  {
    Dictionary<char, int> cardCounts = new Dictionary<char, int>();
    foreach (char card in Hand)
    {
      if (!cardCounts.ContainsKey(card))
      {
        cardCounts.Add(card, 0);
      }

      cardCounts[card] += 1;
    }
    switch (cardCounts.Keys.Count)
    {
      case 1:
        return CardHand.FiveOfAKind;
      case 2:
        foreach (char value in cardCounts.Values)
        {
          if (value == 4)
          {
            return CardHand.FourOfAKind;
          }
        }
        return CardHand.FullHouse;
      case 3:
        foreach (char value in cardCounts.Values)
        {
          if (value == 3)
          {
            return CardHand.ThreeOfAKind;
          }
        }
        return CardHand.TwoPair;
      case 4:
        return CardHand.OnePair;
      case 5:
        return CardHand.HighCard;
    }

    return CardHand.Invalid;
  }
}
