using System;
using System.IO;

class CorporatePolicy
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
          string password = line;
          char[] characters = password.ToCharArray();
          int currentCharacterIndex = characters.Length - 1;
          do
          {
            do
            {
              char newLetter = (char) (characters[currentCharacterIndex] + 1);
              if (newLetter > 'z')
              {
                characters[currentCharacterIndex] = 'a';
                currentCharacterIndex--;
              }
              else
              {
                characters[currentCharacterIndex] = newLetter;
                currentCharacterIndex = password.Length - 1;
                break;
              }
            }
            while (true);

            password = new string(characters);
          }
          while (!isPasswordValid(password));
          Console.WriteLine($"Next valid password: {password}");
        }
      }
    }
  }

  static bool isPasswordValid(string password)
  {
    // Passwords may not contain the letters i, o, or l, 
    // as these letters can be mistaken for other characters and are therefore confusing.
    if (password.IndexOf('i') > -1 || password.IndexOf('o') > -1 || password.IndexOf('l') > -1)
    {
      return false;
    }

    // Passwords must include one increasing straight of at least three letters,
    // like abc, bcd, cde, and so on, up to xyz. They cannot skip letters; abd doesn't count.
    bool hasIncreasingStraight = false;
    char s1 = password[0], s2 = password[0], s3 = password[0];
    foreach (char letter in password)
    {
      s1 = s2;
      s2 = s3;
      s3 = letter;
      if (s3 - 1 == s2 && s2 - 1 == s1)
      {
        hasIncreasingStraight = true;
        break;
      }
    }
    if (!hasIncreasingStraight) return false;

    // Passwords must contain at least two different, non-overlapping pairs of letters, like aa, bb, or zz.
    bool hasTwoRepeatingPairs = false;
    char firstPairLetter = '\0';
    for (int i = 1; i < password.Length; i++)
    {
      if (password[i] == password[i - 1])
      {
        if (firstPairLetter != '\0' && password[i] != firstPairLetter)
        {
          hasTwoRepeatingPairs = true;
          break;
        }
        else
        {
          firstPairLetter = password[i];
        }
      }
    }
    return hasTwoRepeatingPairs;
  }
}
