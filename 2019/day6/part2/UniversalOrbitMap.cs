using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

        // Generate a list of all nodes orbited by a given planet
        List<Planet> orbitedPlanetsYou = generateOrbitPath(planets["YOU"]);
        List<Planet> orbitedPlanetsSan = generateOrbitPath(planets["SAN"]);

        // Find the shared planet orbits to determine where the two orbits diverge
        IEnumerable<Planet> sharedOrbits = orbitedPlanetsYou.Intersect(orbitedPlanetsSan);
        int lengthYou = orbitedPlanetsYou.IndexOf(sharedOrbits.First());
        int lengthSan = orbitedPlanetsSan.IndexOf(sharedOrbits.First());
        Console.WriteLine($"Length from YOU to shared node {sharedOrbits.First().Name}: {lengthYou}");
        Console.WriteLine($"Length from SAN to shared node {sharedOrbits.First().Name}: {lengthSan}");
        Console.WriteLine($"Total length from YOU to SAN: {lengthYou + lengthSan}");
      }
    }
  }

  static List<Planet> generateOrbitPath(Planet planet)
  {
    List<Planet> orbitedPlanets = new List<Planet>();
    Planet orbitedPlanetRec = planet.Orbits;
    while (orbitedPlanetRec != null)
    {
      orbitedPlanets.Add(orbitedPlanetRec);
      orbitedPlanetRec = orbitedPlanetRec.Orbits;
    }
    return orbitedPlanets;
  }

  static void printOrbitPath(Planet planet)
  {
    Console.Write($"Orbit Path: {planet.Name}");
    Planet orbitedPlanetRec = planet.Orbits;
    while (orbitedPlanetRec != null)
    {
      Console.Write($" -> {orbitedPlanetRec.Name}");
      orbitedPlanetRec = orbitedPlanetRec.Orbits;
    }
    Console.WriteLine();
  }
}

class OrbitPath
{
  public Planet Planet { get; set; }
  public int VisitedPlanetsCount { get; set; }

  public OrbitPath(Planet planet, int visitedPlanetsCount = 0)
  {
    Planet = planet;
    VisitedPlanetsCount = visitedPlanetsCount;
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
