using System;
using System.IO;

class ReportRepair
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        string[] lines = File.ReadAllLines(fileName);

        for (int i = 0; i < lines.Length; i++)
        {
          for (int j = 0; j < lines.Length; j++)
          {
            if (i == j) continue;

            int a = int.Parse(lines[i]);
            int b = int.Parse(lines[j]);
            if (a + b == 2020)
            {
              Console.WriteLine($"{a} + {b} = 2020. {a} * {b} = {a * b}");
              return;
            }
          }
        }
      }
    }
  }
}
