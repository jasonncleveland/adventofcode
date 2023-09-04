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

        // Find the guard that slept the most
        int mostAsleepGuardId = -1;
        int mostTimeAsleep = int.MinValue;
        foreach (KeyValuePair<int, GuardSleepSchedule> sleepInfo in guards)
        {
          GuardSleepSchedule guard = sleepInfo.Value;
          if (guard.MinutesSlept > mostTimeAsleep)
          {
            mostAsleepGuardId = guard.GuardId;
            mostTimeAsleep = guard.MinutesSlept;
          }
        }

        // Find which minute the guard was most often asleep at
        GuardSleepSchedule sleepyGuard = guards[mostAsleepGuardId];
        int mostAsleepMinute = -1;
        int mostTimesAsleep = int.MinValue;
        for (int i = 0; i < 60; i ++)
        {
          if (sleepyGuard.Minutes[i] > mostTimesAsleep)
          {
            mostAsleepMinute = i;
            mostTimesAsleep = sleepyGuard.Minutes[i];
          }
        }
        Console.WriteLine($"Guard #{sleepyGuard.GuardId} was asleep most often at minute {mostAsleepMinute} ({sleepyGuard.GuardId * mostAsleepMinute})");
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
