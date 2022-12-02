using System;
using System.IO;

class CalorieCounting
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        string fileContents = File.ReadAllText(fileName);
        string[] lines = fileContents.Split('\n');
        int individualTotal = 0;
        int maxIndividualTotal = 0;
        foreach (string line in lines)
        {
          if (line.Equals(""))
          {
            if (individualTotal > maxIndividualTotal) {
              maxIndividualTotal = individualTotal;
            }
            individualTotal = 0;
          }
          else
          {
            int amount;
            if (int.TryParse(line, out amount))
            {
              individualTotal += amount;
            }
          }
        }
        Console.WriteLine($"Max value: {maxIndividualTotal}");
      }
    }
  }
}
