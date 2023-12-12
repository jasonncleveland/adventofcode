using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

class HotSprings
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

        int total = 0;
        foreach (string line in lines)
        {
          string[] lineParts = line.Split(' ');
          string record = lineParts[0];
          List<int> checksums = new List<int>(Array.ConvertAll(lineParts[1].Split(','), item => int.Parse(item)));
          total += generateSpringCombinationsRec(new List<char>(record), checksums);
        }
        Console.WriteLine($"Total value: {total}");

        stopWatch.Stop();
        Console.WriteLine($"Elapsed Time: {stopWatch.ElapsedMilliseconds} ms");
      }
    }
  }

  static int generateSpringCombinationsRec(List<char> record, List<int> checksums, int index = 0)
  {
    // Stop if there are no more unknowns to replace or we reached the end of the record
    if (index == record.Count || !record.Any(spring => spring == '?'))
    {
      return isValidArrangement(record, checksums) ? 1 : 0;
    }

    if (record[index] == '?')
    {
      // Replace the question mark with a '.' or '#' and check both versions
      List<char> workingSpring = new List<char>(record);
      workingSpring[index] = '.';
      List<char> brokenSpring = new List<char>(record);
      brokenSpring[index] = '#';

      int validCombinations = 0;
      validCombinations += generateSpringCombinationsRec(workingSpring, checksums, index + 1);
      validCombinations += generateSpringCombinationsRec(brokenSpring, checksums, index + 1);
      return validCombinations;
    }
    else
    {
      return generateSpringCombinationsRec(record, checksums, index + 1);
    }
  }

  static bool isValidArrangement(List<char> record, List<int> checksums)
  {
    if (record.Count(spring => spring == '#') != checksums.Sum())
    {
      return false;
    }
    Queue<int> checksumsQueue = new Queue<int>(checksums);
    int currentChecksum = checksumsQueue.Dequeue();
    bool checkingGroup = false;
    int index = 0;
    foreach (char spring in record)
    {
      // Broken spring
      if (spring == '#')
      {
        if (!checkingGroup)
        {
          // Denote when we start checking a spring group
          checkingGroup = true;
        }

        if (currentChecksum == 0)
        {
          // If we don't expect a broken spring but find one, the entire record is invalid
          return false;
        }

        if (currentChecksum > 0)
        {
          // If we expect a broken spring and find one, decrement the 
          currentChecksum--;
        }
      }
      // Working spring
      else if (spring == '.')
      {
        if (checkingGroup)
        {
          if (currentChecksum > 0)
          {
            // If we are checking a spring group and find a non-broken spring, the entire record is invalid
            return false;
          }

          // Denote when we stop checking a spring group
          checkingGroup = false;
        }
      }
      else
      {
        throw new Exception($"Invalid character '{spring}' found in record {string.Join("", record)}");
      }

      if (!checkingGroup && currentChecksum == 0)
      {
        // If we have successfully reached the end of a group, get the next checksum
        if (checksumsQueue.Count > 0)
        {
          currentChecksum = checksumsQueue.Dequeue();
        }
        else
        {
          currentChecksum = 0;
        }
      }

      index++;
    }

    return checksumsQueue.Count == 0;
  }
}
