using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

class IfYouGiveASeedAFertilizer
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();

        string[] lines = File.ReadAllLines(fileName);

        Dictionary<string, AlmanacEntry> almanac = new Dictionary<string, AlmanacEntry>();
        List<long> seeds = new List<long>();

        // Parse input
        bool isInSection = false;
        string currentSection = null;
        foreach (string line in lines)
        {
          if (line.StartsWith("seeds"))
          {
            string[] seedValues = line.Split(':')[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            foreach (string seedValue in seedValues)
            {
              seeds.Add(long.Parse(seedValue));
            }
          }
          else if (!isInSection && line.Contains('-'))
          {
            string[] lineParts = line.Split(' ')[0].Split('-');
            string sourceType = lineParts[0];
            string destinationType = lineParts[2];
            isInSection = true;
            currentSection = sourceType;
            almanac.Add(sourceType, new AlmanacEntry(sourceType, destinationType));
          }
          else if (isInSection && line.Length == 0)
          {
            isInSection = false;
          }
          else if (isInSection)
          {
            string[] lineParts = line.Split(' ');
            long sourceStartIndex = long.Parse(lineParts[1]);
            long destinationStartIndex = long.Parse(lineParts[0]);
            long rangeSize = long.Parse(lineParts[2]);
            
            AlmanacEntry entry = almanac[currentSection];
            entry.Mappings.Add(new MappingEntry(sourceStartIndex, sourceStartIndex + rangeSize - 1, destinationStartIndex, destinationStartIndex + rangeSize - 1));
          }
        }

        // Compute result
        long lowestLocation = long.MaxValue;
        foreach (long seed in seeds)
        {
          long locationValue = mapValueRec(seed, "seed", "location", almanac);
          if (locationValue < lowestLocation)
          {
            lowestLocation = locationValue;
          }
        }
        Console.WriteLine($"Total value: {lowestLocation}");

        stopWatch.Stop();
        Console.WriteLine($"Elapsed Time: {stopWatch.ElapsedMilliseconds} ms");
      }
    }
  }

  static long mapValueRec(long value, string source, string destination, Dictionary<string, AlmanacEntry> almanac)
  {
    AlmanacEntry entry = almanac[source];
    long mappedValue = value;
    foreach (MappingEntry mapEntry in entry.Mappings)
    {
      if (value >= mapEntry.SourceLowerBound && value <= mapEntry.SourceUpperBound)
      {
        long difference = mapEntry.DestinationLowerBound - mapEntry.SourceLowerBound;
        mappedValue += difference;
      }
    }

    if (entry.Destination == destination)
    {
      return mappedValue;
    }
    else
    {
      return mapValueRec(mappedValue, entry.Destination, destination, almanac);
    }
  }
}

class AlmanacEntry
{
  public string Source { get; set; }
  public string Destination { get; set; }
  public List<MappingEntry> Mappings { get; set; }
    
  public AlmanacEntry(string source, string destination)
  {
    Source = source;
    Destination = destination;
    Mappings = new List<MappingEntry>();
  }
}

class MappingEntry
{
  public long SourceLowerBound { get; set; }
  public long SourceUpperBound { get; set; }
  public long DestinationLowerBound { get; set; }
  public long DestinationUpperBound { get; set; }

  public MappingEntry(long sourceLowerBound, long sourceUpperBound, long destinationLowerBound, long destinationUpperBound)
  {
    SourceLowerBound = sourceLowerBound;
    SourceUpperBound = sourceUpperBound;
    DestinationLowerBound = destinationLowerBound;
    DestinationUpperBound = destinationUpperBound;
  }
}
