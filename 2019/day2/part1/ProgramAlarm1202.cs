using System;
using System.IO;

class ProgramAlarm1202
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
          int[] operations = Array.ConvertAll<string, int>(line.Split(','), operation => int.Parse(operation));
          // Pre-run steps
          operations[1] = 12;
          operations[2] = 2;
          for (int i = 0; i < operations.Length; i += 4)
          {
            int opCode = operations[i];
            if (opCode == 99) break;

            int op1Location = operations[i + 1];
            int op2Location = operations[i + 2];
            int outputLocation = operations[i + 3];

            switch (opCode)
            {
              case 1:
                operations[outputLocation] = operations[op1Location] + operations[op2Location];
                break;
              case 2:
                operations[outputLocation] = operations[op1Location] * operations[op2Location];
                break;
              default:
                Console.WriteLine($"Invalid code");
                break;
            }
          }
          Console.WriteLine($"Position 0 value: {operations[0]}");
        }
      }
    }
  }
}
