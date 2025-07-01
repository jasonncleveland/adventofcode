using System;
using System.IO;

class RucksackReorganization
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

        int prioritiesSum = 0;
        for (int i = 0; i < lines.Length; i += 3)
        {
          string firstLine = lines[i];
          string secondLine = lines[i + 1];
          string thirdLine = lines[i + 2];

          foreach (char letter in firstLine)
          {
            int secondLineIndex = secondLine.IndexOf(letter);
            int thirdLineIndex = thirdLine.IndexOf(letter);

            if (secondLineIndex > -1 && thirdLineIndex > -1)
            {
              // Calculate priority of shared letter
              int priority;
              if (char.IsUpper(letter))
              {
                priority = (int) letter - 'A' + 27;
              }
              else
              {
                priority = (int) letter - 'a' + 1;
              }
              prioritiesSum += priority;
              break;
            }
          }
        }
        Console.WriteLine($"Total priority: {prioritiesSum}");
      }
    }
  }
}
