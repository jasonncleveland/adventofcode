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
        Dictionary<string, AlmanacEntry> reverseAlmanac = new Dictionary<string, AlmanacEntry>();
        List<long> seeds = new List<long>();

        // Parse input
        bool isInSection = false;
        string currentSection = null;
        foreach (string line in lines)
        {
          if (line.StartsWith("seeds"))
          {
            string[] seedValues = line.Split(':')[1].Trim().Split(' ');
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
        
        // Generate reverse almanac
        foreach (KeyValuePair<string, AlmanacEntry> almanacEntryValue in almanac)
        {
          AlmanacEntry entry = almanacEntryValue.Value;
          AlmanacEntry reverseEntry = new AlmanacEntry(entry.Destination, entry.Source);
          foreach (MappingEntry mapEntry in entry.Mappings)
          {
            reverseEntry.Mappings.Add(new MappingEntry(mapEntry.DestinationLowerBound, mapEntry.DestinationUpperBound, mapEntry.SourceLowerBound, mapEntry.SourceUpperBound));
          }
          reverseAlmanac.Add(entry.Destination, reverseEntry);
        }

        // Compute result
        long lowestLocation = 0;
        while (true)
        {
          // Map the location to seed
          long mappedSeed = mapValueRec(lowestLocation, "location", "seed", reverseAlmanac);
          // Check seeds to see if we have a match
          bool foundMatch = false;
          for (int i = 1; i < seeds.Count; i += 2)
          {
            long startSeed = seeds[i - 1];
            long seedCount = seeds[i];
            if (mappedSeed >= startSeed && mappedSeed < startSeed + seedCount)
            {
              Console.WriteLine($"Found seed {mappedSeed} in range [{startSeed}, {startSeed + seedCount})");
              foundMatch = true;
              break;
            }
          }
          if (foundMatch) break;

          lowestLocation++;
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
