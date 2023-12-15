using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

class LensLibrary
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

        foreach (string line in lines)
        {
          Dictionary<int, List<Lens>> boxes = new Dictionary<int, List<Lens>>();
          string[] operations = line.Split(',');

          foreach (string operation in operations)
          {
            string label = null;
            int focalLength = -1;

            if (operation.Contains('='))
            {
              string[] stepParts = operation.Split('=');
              label = stepParts[0];
              focalLength = int.Parse(stepParts[1]);
            }
            else if (operation.Contains('-'))
            {
              string[] stepParts = operation.Split('-');
              label = stepParts[0];
            }

            int hashIndex = hashLabel(label);
            if (!boxes.ContainsKey(hashIndex))
            {
              boxes.Add(hashIndex, new List<Lens>());
            }

            if (focalLength > 0)
            {
              Lens existingLens = boxes[hashIndex].Find(lens => lens.Label == label);
              if (existingLens != null)
              {
                existingLens.FocalLength = focalLength;
              }
              else
              {
                boxes[hashIndex].Add(new Lens(label, focalLength));
              }
            }
            else
            {
              boxes[hashIndex].RemoveAll(lens => lens.Label == label);
            }
          }

          int total = 0;
          foreach (int boxIndex in boxes.Keys)
          {
            if (boxes[boxIndex].Count > 0)
            {
              for (int i = 0; i < boxes[boxIndex].Count; i++)
              {
                // Focusing power = Box number * Slot number * Focal length
                int focusingPower = (boxIndex + 1) * (i + 1) * boxes[boxIndex][i].FocalLength;
                total += focusingPower;
              }
            }
          }
          Console.WriteLine($"Total value: {total}");
        }

        stopWatch.Stop();
        Console.WriteLine($"Elapsed Time: {stopWatch.ElapsedMilliseconds} ms");
      }
    }
  }

  static int hashLabel(string label)
  {
    int labelTotal = 0;
    foreach (char character in label)
    {
      labelTotal += character;
      labelTotal *= 17;
      labelTotal %= 256;
    }
    return labelTotal;
  }
}

class Lens
{
  public string Label { get; set; }
  public int FocalLength { get; set; }

  public Lens(string label, int focalLength)
  {
    Label = label;
    FocalLength = focalLength;
  }
}
