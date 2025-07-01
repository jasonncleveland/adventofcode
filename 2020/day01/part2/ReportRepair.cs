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
            for (int k = 0; k < lines.Length; k++)
            {
              if (i == j || i == k || j == k) continue;

              int a = int.Parse(lines[i]);
              int b = int.Parse(lines[j]);
              int c = int.Parse(lines[k]);

              if (a + b + c == 2020)
              {
                Console.WriteLine($"{a} + {b} + {c} = 2020. {a} * {b} * {c} = {a * b * c}");
                return;
              }
            }
          }
        }
      }
    }
  }
}
