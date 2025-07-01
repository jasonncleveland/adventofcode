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
    Dictionary<PlayOption, Dictionary<Outcome, PlayOption>> ResultsDictionary = new Dictionary<PlayOption, Dictionary<Outcome, PlayOption>>()
    {
      {
        PlayOption.Rock,
        new Dictionary<Outcome, PlayOption>()
        {
          { Outcome.Win, PlayOption.Paper },
          { Outcome.Loss, PlayOption.Scissors },
          { Outcome.Draw, PlayOption.Rock },
        }
      },
      {
        PlayOption.Scissors,
        new Dictionary<Outcome, PlayOption>()
        {
          { Outcome.Win, PlayOption.Rock },
          { Outcome.Loss, PlayOption.Paper },
          { Outcome.Draw, PlayOption.Scissors },
        }
      },
      {
        PlayOption.Paper,
        new Dictionary<Outcome, PlayOption>()
        {
          { Outcome.Win, PlayOption.Scissors },
          { Outcome.Loss, PlayOption.Rock },
          { Outcome.Draw, PlayOption.Paper },
        }
      }
    };
    Dictionary<string, PlayOption> PlaysDictionary = new Dictionary<string, PlayOption>()
    {
      { "A", PlayOption.Rock },
      { "B", PlayOption.Paper },
      { "C", PlayOption.Scissors },
    };
    Dictionary<string, Outcome> OutcomesDictionary = new Dictionary<string, Outcome>()
    {
      { "X", Outcome.Loss },
      { "Y", Outcome.Draw },
      { "Z", Outcome.Win },
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
          Outcome expectedOutcome = OutcomesDictionary[splitLine[1]];

          // Get the required play for the expected outcome
          PlayOption myChoice = ResultsDictionary[opponentChoice][expectedOutcome];

          int roundScore = 0;

          // Add score for which option was played
          roundScore += (int) myChoice;

          // Add score for result
          roundScore += (int) expectedOutcome;

          totalScore += roundScore;
        }
        Console.WriteLine($"Total score: {totalScore}");
      }
    }
  }
}
