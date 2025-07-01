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

        int totalRibbonLength = 0;
        foreach (string line in lines)
        {
          string[] dimensions = line.Split('x');
          int length = int.Parse(dimensions[0]);
          int width = int.Parse(dimensions[1]);
          int height = int.Parse(dimensions[2]);

          int lengthFacePerimiter = length * 2 + width * 2;
          int widthFacePerimiter = width * 2 + height * 2;
          int heightFacePerimiter = height * 2 + length * 2;

          int smallestPerimiter = int.MaxValue;
          if (lengthFacePerimiter < smallestPerimiter) smallestPerimiter = lengthFacePerimiter;
          if (widthFacePerimiter < smallestPerimiter) smallestPerimiter = widthFacePerimiter;
          if (heightFacePerimiter < smallestPerimiter) smallestPerimiter = heightFacePerimiter;

          int cubicVolume = length * width * height;

          totalRibbonLength += smallestPerimiter + cubicVolume;
        }

        Console.WriteLine($"Total ribbon length required: {totalRibbonLength}");
      }
    }
  }
}
