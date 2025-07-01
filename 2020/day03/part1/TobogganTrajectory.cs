using System;
using System.IO;

class TobogganTrajectory
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        string[] lines = File.ReadAllLines(fileName);


        int numTreesEncountered = 0;
        int x = 0;
        foreach (string line in lines)
        {
          char value = line[x % line.Length];
          if (value == '#')
          {
            numTreesEncountered += 1;
          }
          x += 3;
        }
        Console.WriteLine($"Number of trees encountered: {numTreesEncountered}");
      }
    }
  }
}
