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
          fuelRequirements += calculateFuelRequiredRec(mass);
        }
        Console.WriteLine($"Fuel requirements: {fuelRequirements}");
      }
    }
  }

  static int calculateFuelRequiredRec(int mass)
  {
    int fuelRequired = mass / 3 - 2;
    if (fuelRequired < 0) return 0;
    return fuelRequired + calculateFuelRequiredRec(fuelRequired);
  }
}
