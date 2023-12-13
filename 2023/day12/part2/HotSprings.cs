using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

class HotSprings
{
  static Dictionary<string, long> cache;

  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      int crumpleFactor = args.Length > 1 ? int.Parse(args[1]) : 5;
      if (File.Exists(fileName))
      {
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();

        string[] lines = File.ReadAllLines(fileName);

        long total = 0;
        foreach (string line in lines)
        {
          // Clear the cache for each line
          cache = new Dictionary<string, long>();

          string[] lineParts = line.Split(' ');
          string record = lineParts[0];
          List<int> checksums = new List<int>(Array.ConvertAll(lineParts[1].Split(','), item => int.Parse(item)));

          // Unfold the record
          string expandedRecord = string.Join('?', Enumerable.Repeat(record, crumpleFactor));
          List<int> expandedChecksums = new List<int>();
          for (int fold = 0; fold < crumpleFactor; fold++)
          {
            expandedChecksums.AddRange(checksums);
          }

          long validCombinations = calculateUniqueCombinations(expandedRecord, new Queue<int>(expandedChecksums));
          total += validCombinations;
        }
        Console.WriteLine($"Total value: {total}");

        stopWatch.Stop();
        Console.WriteLine($"Elapsed Time: {stopWatch.ElapsedMilliseconds} ms");
      }
    }
  }

  static long calculateUniqueCombinations(string record, Queue<int> checksums)
  {
    // If we have already seen this condition, return the previously computed value
    string cacheKey = $"{record};{string.Join(",", checksums)}";
    if (cache.ContainsKey(cacheKey))
    {
      return cache[cacheKey];
    }

    long validCombinations;

    // Stop when we run out of springs and return whether there are any checksums left
    if (record.Length == 0)
    {
      validCombinations = checksums.Count == 0 ? 1 : 0;
      cache.Add(cacheKey, validCombinations);
      return validCombinations;
    }
    // Stop if we run out checksums and return whether there are any remaining broken springs
    if (checksums.Count == 0)
    {
      validCombinations = !record.Contains('#') ? 1 : 0;
      cache.Add(cacheKey, validCombinations);
      return validCombinations;
    }

    if (record[0] == '?')
    {
      // Replace the question mark with a '.' or '#' and check both versions
      validCombinations = 0;
      string restOfString = record.Substring(1);
      validCombinations += calculateUniqueCombinations($"#{restOfString}", new Queue<int>(checksums));
      validCombinations += calculateUniqueCombinations($".{restOfString}", new Queue<int>(checksums));
      cache.Add(cacheKey, validCombinations);
      return validCombinations;
    }
    else if (record[0] == '#')
    {
      // If we don't expect a broken spring but find one, the entire record is invalid
      if (checksums.Count == 0)
      {
        validCombinations = 0;
        cache.Add(cacheKey, validCombinations);
        return validCombinations;
      }

      int currentChecksum = checksums.Dequeue();

      // If there are not enough charaacters or there is a non-broken spring in the expected length, the entire record is invalid
      if (record.Length < currentChecksum || record.Substring(0, currentChecksum).Contains('.'))
      {
        validCombinations = 0;
        cache.Add(cacheKey, validCombinations);
        return validCombinations;
      }

      if (record.Length > currentChecksum)
      {
        // The record length is larger than the checksum so check if we have a valid group
        string potentialGroup = record.Substring(0, currentChecksum);
        bool isCompleteGroup = potentialGroup.All(spring => spring == '#');
        // If the group of broken springs is the correct size, check the following spring
        if (isCompleteGroup)
        {
          switch (record[currentChecksum])
          {
            case '#':
              // If the group of broken springs is too large, the entire record is invalid
              validCombinations = 0;
              cache.Add(cacheKey, validCombinations);
              return validCombinations;
            case '.':
            case '?':
              // If the group is the correct size and we have a non-broken spring or wildcard, the wildcard must be a non-broken spring
              validCombinations = calculateUniqueCombinations(record.Substring(currentChecksum + 1), new Queue<int>(checksums));
              cache.Add(cacheKey, validCombinations);
              return validCombinations;
            default:
              throw new Exception($"Invalid character '{record[currentChecksum]}' found in record {record}");
          }
        }
        else
        {
          // There is a wildcard in the group of springs.
          potentialGroup = potentialGroup.Replace('?', '#');
          if (potentialGroup.All(spring => spring == '#'))
          {
            switch (record[currentChecksum])
            {
              case '#':
                // If the group of broken springs is too large, the entire record is invalid
                validCombinations = 0;
                cache.Add(cacheKey, validCombinations);
                return validCombinations;
              case '.':
              case '?':
                // If the group is the correct size and we have a non-broken spring or wildcard, the wildcard must be a non-broken spring
                validCombinations = calculateUniqueCombinations(record.Substring(currentChecksum + 1), new Queue<int>(checksums));
                cache.Add(cacheKey, validCombinations);
                return validCombinations;
              default:
                throw new Exception($"Invalid character '{record[currentChecksum]}' found in record {record}");
            }
          }
          else
          {
            // A previous check should prevent '.' from being in the group and we should never get here
            throw new Exception("Should never be here");
          }
        }
      }
      else
      {
        // The record length is equal to the checksum count so replace any ? and check if the group is valid
        validCombinations = record.Replace('?', '#').All(spring => spring == '#') && checksums.Count == 0 ? 1 : 0;
        cache.Add(cacheKey, validCombinations);
        return validCombinations;
      }
    }
    else
    {
      // If we find a non-broken spring, skip and continue with the rest of the record
      validCombinations = calculateUniqueCombinations(record.Substring(1), new Queue<int>(checksums));
      cache.Add(cacheKey, validCombinations);
      return validCombinations;
    }
  }
}
