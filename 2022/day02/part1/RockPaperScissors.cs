using System;
using System.Collections.Generic;
using System.IO;

enum PlayOption: int
{
  Rock = 1,
  Paper = 2,
  Scissors = 3
}

enum Outcome
{
  Win = 6,
  Loss = 0,
  Draw = 3
}

class RockPaperScissors
{
  static void Main(string[] args)
  {
    Dictionary<PlayOption, PlayOption> RulesDictionary = new Dictionary<PlayOption, PlayOption>()
    {
      { PlayOption.Rock, PlayOption.Scissors },
      { PlayOption.Scissors, PlayOption.Paper },
      { PlayOption.Paper, PlayOption.Rock }
    };
    Dictionary<string, PlayOption> PlaysDictionary = new Dictionary<string, PlayOption>()
    {
      { "A", PlayOption.Rock },
      { "B", PlayOption.Paper },
      { "C", PlayOption.Scissors },
      { "X", PlayOption.Rock },
      { "Y", PlayOption.Paper },
      { "Z", PlayOption.Scissors },
    };

    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        string fileContents = File.ReadAllText(fileName);
        string[] lines = fileContents.Split('\n');

        int totalScore = 0;
        foreach (string line in lines)
        {
          string[] splitLine = line.Split(' ');
          PlayOption opponentChoice = PlaysDictionary[splitLine[0]];
          PlayOption myChoice = PlaysDictionary[splitLine[1]];

          int roundScore = 0;

          // Add score for which option was played
          roundScore += (int) myChoice;

          // Add score for result
          PlayOption victoryPlay = RulesDictionary[myChoice];
          if (opponentChoice == victoryPlay)
          {
            roundScore += (int) Outcome.Win;
          }
          else
          {
            if (opponentChoice == myChoice)
            {
              roundScore += (int) Outcome.Draw;
            }
            else
            {
              roundScore += (int) Outcome.Loss;
            }
          }

          totalScore += roundScore;
        }
        Console.WriteLine($"Total score: {totalScore}");
      }
    }
  }
}
