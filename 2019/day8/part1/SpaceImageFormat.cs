using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

class SpaceImageFormat
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      int width = args.Length > 1 ? int.Parse(args[1]) : 25;
      int height = args.Length > 2 ? int.Parse(args[2]) : 6;
      if (File.Exists(fileName))
      {
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();

        string[] lines = File.ReadAllLines(fileName);

        Console.WriteLine($"Width: {width} Height: {height}");
        int layerSize = width * height;

        int minZeroCount = int.MaxValue;
        int oneCount = 0;
        int twoCount = 0;
        foreach (string line in lines)
        {
          for (int i = 0; i < line.Length; i += layerSize)
          {
            string layer = line.Substring(i, layerSize);

            int zeroCount = layer.Where(item => item == '0').Count();
            if (zeroCount < minZeroCount)
            {
              minZeroCount = zeroCount;
              oneCount = layer.Where(item => item == '1').Count();
              twoCount = layer.Where(item => item == '2').Count();
            }
          }
          Console.WriteLine($"Total value: {oneCount * twoCount}");
        }

        stopWatch.Stop();
        Console.WriteLine($"Elapsed Time: {stopWatch.ElapsedMilliseconds} ms");
      }
    }
  }
}
