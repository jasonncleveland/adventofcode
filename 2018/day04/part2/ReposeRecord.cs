using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

class ReposeRecord
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        List<string> lines = new List<string>(File.ReadAllLines(fileName));
        lines.Sort();

        string timePattern = @"\[\d{4}-\d{2}-\d{2} \d{2}:(?<minute>\d{2})\] (?<log>.*)";
        string guardPattern = @"Guard #(?<guardId>\d+) begins shift";
        Regex timeRegex = new Regex(timePattern, RegexOptions.Compiled);
        Regex guardRegex = new Regex(guardPattern, RegexOptions.Compiled);

        Dictionary<int, GuardSleepSchedule> guards = new Dictionary<int, GuardSleepSchedule>();

        GuardSleepSchedule currentGuard = null;
        bool isGuardAsleep = false;
        int sleepMinute = -1;
        foreach (string line in lines)
        {
          Match match = timeRegex.Match(line);

          int minute = int.Parse(match.Groups["minute"].Value);
          string log = match.Groups["log"].Value;
          if (log.StartsWith("Guard"))
          {
            Match guardMatch = guardRegex.Match(log);

            int guardId = int.Parse(guardMatch.Groups["guardId"].Value);

            if (!guards.ContainsKey(guardId))
            {
              GuardSleepSchedule newGuard = new GuardSleepSchedule(guardId);
              guards.Add(guardId, newGuard);
              currentGuard = newGuard;
            }
            else
            {
              currentGuard = guards[guardId];
            }
          }
          else if (!isGuardAsleep && log.Equals("falls asleep"))
          {
            isGuardAsleep = true;
            sleepMinute = minute;
          }
          else if (isGuardAsleep && log.Equals("wakes up"))
          {
            isGuardAsleep = false;
            currentGuard.MinutesSlept += minute - sleepMinute;
            for (int currentMinute = sleepMinute; currentMinute < minute; currentMinute++)
            {
              currentGuard.Minutes[currentMinute]++;
            }
          }
        }

        // Find the guard that slept the most often at the same minute
        int sleepiestGuardId = -1;
        int minuteMostOftenAsleep = -1;
        int timesAsleep = int.MinValue;
        foreach (KeyValuePair<int, GuardSleepSchedule> sleepInfo in guards)
        {
          GuardSleepSchedule guard = sleepInfo.Value;
          int mostAsleepMinute = -1;
          int mostTimesAsleep = int.MinValue;
          for (int i = 0; i < 60; i ++)
          {
            if (guard.Minutes[i] > mostTimesAsleep)
            {
              mostAsleepMinute = i;
              mostTimesAsleep = guard.Minutes[i];
            }
          }
          if (mostTimesAsleep > timesAsleep)
          {
            sleepiestGuardId = guard.GuardId;
            minuteMostOftenAsleep = mostAsleepMinute;
            timesAsleep = mostTimesAsleep;
          }
        }
        Console.WriteLine($"Guard #{sleepiestGuardId} was asleep most often at minute {minuteMostOftenAsleep} ({sleepiestGuardId * minuteMostOftenAsleep})");
      }
    }
  }
}

class GuardSleepSchedule
{
  public int GuardId { get; }
  public Dictionary<int, int> Minutes { get; }
  public int MinutesSlept { get; set; }

  public GuardSleepSchedule(int guardId)
  {
    GuardId = guardId;
    Minutes = new Dictionary<int, int>();
    for (int i = 0; i < 60; i++)
    {
      Minutes.Add(i, 0);
    }
    MinutesSlept = 0;
  }
}
