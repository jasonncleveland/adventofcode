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
          int[] inputMemory = Array.ConvertAll<string, int>(line.Split(','), data => int.Parse(data));
          int[] memory = new int[inputMemory.Length];

          for (int noun = 0; noun < 100; noun++)
          {
            for (int verb = 0; verb < 100; verb++)
            {
              // Reset memory
              Array.Copy(inputMemory, memory, inputMemory.Length);

              // Pre-run steps
              memory[1] = noun;
              memory[2] = verb;

              for (int instructionPointer = 0; instructionPointer < memory.Length; instructionPointer += 4)
              {
                int opCode = memory[instructionPointer];
                if (opCode == 99) break;

                int value1Address = memory[instructionPointer + 1];
                int value2Address = memory[instructionPointer + 2];
                int outputAddress = memory[instructionPointer + 3];

                switch (opCode)
                {
                  case 1:
                    memory[outputAddress] = memory[value1Address] + memory[value2Address];
                    break;
                  case 2:
                    memory[outputAddress] = memory[value1Address] * memory[value2Address];
                    break;
                  default:
                    Console.WriteLine($"wtf code");
                    break;
                }
              }
              if (memory[0] == 19690720)
              {
                Console.WriteLine($"Found desired value: {memory[0]}");
                Console.WriteLine($"Noun: {noun}, Verb: {verb}");
                Console.WriteLine($"Result: {100 * noun + verb}");
                return;
              }
            }
          }
        }
      }
    }
  }
}
