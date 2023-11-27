using System;
using System.Collections.Generic;
using System.IO;

class UniversalOrbitMap
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        string[] lines = File.ReadAllLines(fileName);

        Dictionary<string, Planet> planets = new Dictionary<string, Planet>();

        foreach (string line in lines)
        {
          string[] orbitMap = line.Split(')');

          Planet orbitedPlanet, orbitingPlanet;
          if (!planets.ContainsKey(orbitMap[0]))
          {
            planets.Add(orbitMap[0], new Planet(orbitMap[0]));
          }
          orbitedPlanet = planets[orbitMap[0]];
          if (!planets.ContainsKey(orbitMap[1]))
          {
            planets.Add(orbitMap[1], new Planet(orbitMap[1]));
          }
          orbitingPlanet = planets[orbitMap[1]];
          orbitingPlanet.Orbits = orbitedPlanet;
        }

        int orbitCount = 0;
        foreach (Planet planet in planets.Values)
        {
          Planet orbitedPlanetRec = planet.Orbits;
          while (orbitedPlanetRec != null)
          {
            orbitCount += 1;
            orbitedPlanetRec = orbitedPlanetRec.Orbits;
          }
        }
        Console.WriteLine($"Total number of orbits: {orbitCount}");
      }
    }
  }
}

class Planet
{
  public string Name { get; }
  public Planet Orbits { get; set; }

  public Planet(string name)
  {
    Name = name;
    Orbits = null;
  }
}
