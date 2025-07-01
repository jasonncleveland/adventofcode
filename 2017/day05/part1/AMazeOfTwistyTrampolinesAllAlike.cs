using System;
using System.IO;

class AMazeOfTwistyTrampolinesAllAlike
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        string[] lines = File.ReadAllLines(fileName);

        int[] instructions = new int[lines.Length];

        for (int i = 0; i < lines.Length; i++)
        {
          instructions[i] = int.Parse(lines[i]);
        }

        int steps = 0;
        int index = 0;
        while (index >= 0 && index < instructions.Length)
        {
          int instruction = instructions[index];
          instructions[index] += 1;
          index += instruction;
          steps += 1;
        }
        Console.WriteLine($"Number of steps: {steps}");
      }
    }
  }
}
