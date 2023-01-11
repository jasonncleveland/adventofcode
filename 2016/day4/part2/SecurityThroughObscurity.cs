using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

class SecurityThroughObscurity
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        string[] lines = File.ReadAllLines(fileName);

        string pattern = @"([a-z\-]+)\-(\d+)\[(\w+)\]";
        Regex regex = new Regex(pattern, RegexOptions.Compiled);

        foreach (string line in lines)
        {
          Match match = regex.Match(line);
          string encryptedName = match.Groups[1].Value;
          int sectorId = int.Parse(match.Groups[2].Value);
          string checksum = match.Groups[3].Value;

          List<LetterData> letterCounts = new List<LetterData>();
          foreach (char letter in encryptedName)
          {
            if (letter == '-') {
              continue;
            }

            if (letterCounts.FindIndex(l => l.Letter == letter) < 0)
            {
              letterCounts.Add(new LetterData(letter));
            }
            LetterData letterData = letterCounts.Find(l => l.Letter == letter);
            letterData.Count++;
          }

          letterCounts.Sort((a, b) => {
            // Sort by count descending, then letter ascending
            int countResult = b.Count - a.Count;
            int letterResult = a.Letter - b.Letter;
            return countResult == 0 ? letterResult : countResult;
          });

          bool isValidRoom = true;
          for (int i = 0; i < checksum.Length; i++)
          {
            char expectedLetter = checksum[i];
            char actualLetter = letterCounts[i].Letter;
            if (expectedLetter != actualLetter)
            {
              isValidRoom = false;
            }
          }
          if (!isValidRoom)
          {
            continue;
          }

          char[] decryptedLetters = encryptedName.ToCharArray();
          for (int i = 0; i < decryptedLetters.Length; i++)
          {
            if (decryptedLetters[i] == '-' || decryptedLetters[i] == ' ')
            {
              decryptedLetters[i] = ' ';

            }
            else
            {
              int letterIndex = decryptedLetters[i] - 'a';
              int offsetIndex = letterIndex + sectorId;
              int newLetterIndex = offsetIndex % 26;
              char newLetter = (char) (newLetterIndex + 'a');
              decryptedLetters[i] = newLetter;
            }
          }
          string decryptedName = new string(decryptedLetters);
          if (decryptedName == "northpole object storage")
          {
            Console.WriteLine($"({sectorId}): {decryptedName}");
          }
        }
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
