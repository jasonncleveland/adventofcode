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

        string timeValue = lines[0].Split(':')[1].Replace(" ", "");
        string distanceValue = lines[1].Split(':')[1].Replace(" ", "");

        long time = long.Parse(timeValue);
        long distance = long.Parse(distanceValue);

        // Simulate races
        long winningRaces = 0;
        for (long i = 1; i < time; i++)
        {
          long finalRaceDistance = i * (time - i);
          if (finalRaceDistance > distance) winningRaces++;
        };
        Console.WriteLine($"Won {winningRaces} races");

        stopWatch.Stop();
        Console.WriteLine($"Elapsed Time: {stopWatch.ElapsedMilliseconds} ms");
      }
    }
  }
}