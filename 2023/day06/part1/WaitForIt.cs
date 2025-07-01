using System;
using System.Diagnostics;
using System.IO;

class WaitForIt
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

        string[] times = lines[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);
        string[] distances = lines[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);

        int total = 1;
        // Skip the first item
        for (int race = 1; race < times.Length; race++)
        {
          int time = int.Parse(times[race]);
          int distance = int.Parse(distances[race]);
          // Simulate races
          int winningRaces = 0;
          for (int i = 1; i < time; i++)
          {
            int finalRaceDistance = i * (time - i);
            if (finalRaceDistance > distance) winningRaces++;
          }
          total *= winningRaces;
        }
        Console.WriteLine($"Total value: {total}");

        stopWatch.Stop();
        Console.WriteLine($"Elapsed Time: {stopWatch.ElapsedMilliseconds} ms");
      }
    }
  }
}
