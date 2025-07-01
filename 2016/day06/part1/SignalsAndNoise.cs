using System;
using System.Collections.Generic;
using System.IO;

class SignalsAndNoise
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        string[] lines = File.ReadAllLines(fileName);

        List<List<LetterData>> letterCounts = new List<List<LetterData>>();
        foreach (string line in lines)
        {
          for (int i = 0; i < line.Length; i++)
          {
            char letter = line[i];
            if (letterCounts.Count < i + 1)
            {
              letterCounts.Add(new List<LetterData>());
            }

            if (letterCounts[i].FindIndex(l => l.Letter == letter) < 0)
            {
              letterCounts[i].Add(new LetterData(letter));
            }
            LetterData letterData = letterCounts[i].Find(l => l.Letter == letter);
            letterData.Count++;
          }
        }

        List<char> message = new List<char>();
        foreach (List<LetterData> column in letterCounts)
        {
          column.Sort((a, b) => {
            int countResult = b.Count - a.Count;
            int letterResult = a.Letter - b.Letter;
            return countResult == 0 ? letterResult : countResult;
          });
          message.Add(column[0].Letter);
        }
        string correctedMessage = new string(message.ToArray());
        Console.WriteLine($"Error-corrected message: {correctedMessage}");
      }
    }
  }
}

class LetterData
{
  public char Letter { get; set; }
  public int Count { get; set; }

  public LetterData(char letter)
  {
    Letter = letter;
    Count = 0;
  }
}
