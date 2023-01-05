using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

class ReindeerOlympics
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        string[] lines = File.ReadAllLines(fileName);

        string pattern = @"(\w+) can fly (\d+) km/s for (\d+) seconds, but then must rest for (\d+) seconds.";
        Regex regex = new Regex(pattern, RegexOptions.Compiled);

        List<Reindeer> reindeerList = new List<Reindeer>();

        foreach (string line in lines)
        {
          Match match = regex.Match(line);

          string name = match.Groups[1].Value;
          int distance = int.Parse(match.Groups[2].Value);
          int duration = int.Parse(match.Groups[3].Value);
          int rest = int.Parse(match.Groups[4].Value);
          reindeerList.Add(new Reindeer(name, distance, duration, rest));
        }

        int totalSeconds = 2503;
        for (int i = 0; i < totalSeconds; i++)
        {
          int farthestRoundDistance = int.MinValue;
          List<Reindeer> roundLeaders = new List<Reindeer>();
          foreach (Reindeer reindeer in reindeerList)
          {
            reindeer.processTick(i);
            int travelledDistance = reindeer.getDistanceTravelled();
            if (travelledDistance > farthestRoundDistance)
            {
              roundLeaders.Clear();
              farthestRoundDistance = travelledDistance;
              roundLeaders.Add(reindeer);
            }
            else if (travelledDistance == farthestRoundDistance)
            {
              roundLeaders.Add(reindeer);
            }
          }
          foreach (Reindeer reindeer in roundLeaders)
          {
            reindeer.Points += 1;
          }
        }

        int farthestDistance = int.MinValue;
        List<Reindeer> leaders = new List<Reindeer>();
        foreach (Reindeer reindeer in reindeerList)
        {
          int travelledDistance = reindeer.getDistanceTravelled();
          if (travelledDistance > farthestDistance)
          {
            leaders.Clear();
            farthestDistance = travelledDistance;
            leaders.Add(reindeer);
          }
          else if (travelledDistance == farthestDistance)
          {
            leaders.Add(reindeer);
          }
        }

        int mostPoints = int.MinValue;
        foreach (Reindeer reindeer in reindeerList)
        {
          if (reindeer.Points > mostPoints)
          {
            mostPoints = reindeer.Points;
          }
        }
        Console.WriteLine($"Most points: {mostPoints}");
      }
    }
  }
}

enum ReindeerState
{
  MOVING,
  RESTING
}

class Reindeer
{
  public string Name { get; set; }
  public int Points { get; set; }

  private int _distancePerSecond;
  private int _moveDuration;
  private int _restDuration;

  private ReindeerState _state;
  private int _distanceTravelled;
  private int _moveStartSecond;
  private int _restStartSecond;

  public Reindeer(string name, int distance, int duration, int rest)
  {
    Name = name;
    Points = 0;
    _distancePerSecond = distance;
    _moveDuration = duration;
    _restDuration = rest;
    _state = ReindeerState.MOVING;
    _moveStartSecond = 0;
  }

  public void processTick(int tick)
  {
    if (_state == ReindeerState.MOVING && _moveStartSecond + _moveDuration == tick)
    {
      _state = ReindeerState.RESTING;
      _restStartSecond = tick;
    }
    else if (_state == ReindeerState.RESTING && _restStartSecond + _restDuration == tick)
    {
      _state = ReindeerState.MOVING;
      _moveStartSecond = tick;
    }

    if (_state == ReindeerState.MOVING)
    {
      _distanceTravelled += _distancePerSecond;
    }
  }

  public int getDistanceTravelled()
  {
    return _distanceTravelled;
  }
}
