using System;
using System.Collections.Generic;
using System.IO;

class CampCleanup
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

        int numOverlappedPairs = 0;
        foreach (string line in lines)
        {
          string[] elfPairs = line.Split(',');
          string firstElf = elfPairs[0];
          string secondElf = elfPairs[1];

          int[] firstElfNumbers = Array.ConvertAll<string, int>(firstElf.Split('-'), new Converter<string, int>(strNum => int.Parse(strNum)));
          int[] secondElfNumbers = Array.ConvertAll<string, int>(secondElf.Split('-'), new Converter<string, int>(strNum => int.Parse(strNum)));

          bool secondOverlapsFirst = firstElfNumbers[0] <= secondElfNumbers[0] && secondElfNumbers[0] <= firstElfNumbers[1];
          bool firstOverlapsSecond = secondElfNumbers[0] <= firstElfNumbers[0] && firstElfNumbers[0] <= secondElfNumbers[1];

          bool isOverlap = secondOverlapsFirst || firstOverlapsSecond;
          if (isOverlap) {
            numOverlappedPairs++;
          }
        }
        Console.WriteLine($"Number of overlapping pairs: {numOverlappedPairs}");
      }
    }
  }
}
