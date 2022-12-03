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
        foreach (string line in lines)
        {
          int lineLength = line.Length;

          string firstCompartment = line.Substring(0, lineLength/2);
          string secondCompartment = line.Substring(lineLength/2, lineLength/2);

          foreach (char letter in firstCompartment)
          {
            if (secondCompartment.IndexOf(letter) > -1)
            {

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
