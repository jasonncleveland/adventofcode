using System;
using System.IO;

class IWasToldThereWouldBeNoMath
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        string[] lines = File.ReadAllLines(fileName);

        int totalWrappingPaperArea = 0;
        foreach (string line in lines)
        {
          string[] dimensions = line.Split('x');
          int length = int.Parse(dimensions[0]);
          int width = int.Parse(dimensions[1]);
          int height = int.Parse(dimensions[2]);

          int lengthArea = 2 * length * width;
          int widthArea = 2 * width * height;
          int heightArea = 2 * height * length;

          int slackArea = int.MaxValue;
          if (lengthArea / 2 < slackArea) slackArea = lengthArea / 2;
          if (widthArea / 2 < slackArea) slackArea = widthArea / 2;
          if (heightArea / 2 < slackArea) slackArea = heightArea / 2;

          totalWrappingPaperArea += lengthArea + widthArea + heightArea + slackArea;
        }

        Console.WriteLine($"Total wrapping paper required: {totalWrappingPaperArea}");
      }
    }
  }
}
