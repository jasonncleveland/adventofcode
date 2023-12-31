using System;
using System.Diagnostics;
using System.IO;

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

        // Spaces are initialized with '\0' character
        char[,] screen = new char[height, width];

        foreach (string line in lines)
        {
          Console.WriteLine($"Line length: {line.Length} Layer count: {line.Length / layerSize}");
          for (int i = 0; i < line.Length; i += layerSize)
          {
            string layer = line.Substring(i, layerSize);
            for (int row = 0; row < height; row++)
            {
              for (int column = 0; column < width; column++)
              {
                if (screen[row, column] == '\0' || screen[row, column] == '2')
                {
                  screen[row, column] = layer[row * width + column];
                }
              }
            }
          }
        }

        for (int row = 0; row < screen.GetLength(0); row++)
        {
          for (int column = 0; column < screen.GetLength(1); column++)
          {
            // Map the 1's and 0'2 to #'s and .'s to make the output readable
            Console.Write(screen[row, column] == '1' ? '#' : '.');
          }
          Console.WriteLine();
        }

        stopWatch.Stop();
        Console.WriteLine($"Elapsed Time: {stopWatch.ElapsedMilliseconds} ms");
      }
    }
  }
}
