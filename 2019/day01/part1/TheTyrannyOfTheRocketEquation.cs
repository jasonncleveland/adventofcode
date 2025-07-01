using System;
using System.IO;

class TheTyrannyOfTheRocketEquation
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        string[] lines = File.ReadAllLines(fileName);

        int fuelRequirements = 0;
        foreach (string line in lines)
        {
          int mass = int.Parse(line);
          int fuelRequired = mass / 3 - 2;
          fuelRequirements += fuelRequired;
        }
        Console.WriteLine($"Fuel requirements: {fuelRequirements}");
      }
    }
  }
}
