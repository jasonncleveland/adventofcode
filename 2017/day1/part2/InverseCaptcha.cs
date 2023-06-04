using System;
using System.IO;

class InverseCaptcha
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        string[] lines = File.ReadAllLines(fileName);

        foreach (string line in lines)
        {
          int totalSum = 0;
          for (int i = 0, s = line.Length; i < s; i++)
          {
            int firstItem = int.Parse(line[i].ToString());
            int secondItem = int.Parse(line[(i + s/2) % s].ToString());
            if (firstItem == secondItem)
            {
              totalSum += firstItem;
            }
          }
          Console.WriteLine($"Total sum: {totalSum}");
        }
      }
    }
  }
}
