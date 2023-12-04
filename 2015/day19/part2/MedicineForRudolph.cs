using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

class MedecineForRudolph
{
  static void Main(string[] args)
  {
    if (args.Length > 1)
    {
      string mappingsFileName = args[0];
      string inputFileName = args[1];
      if (File.Exists(mappingsFileName) && File.Exists(inputFileName))
      {
        string[] lines = File.ReadAllLines(mappingsFileName);
        string input = File.ReadAllText(inputFileName);

        Dictionary<string, List<string>> replacements = new Dictionary<string, List<string>>();
        Dictionary<string, string> inverseReplacements = new Dictionary<string, string>();

        foreach (string line in lines)
        {
          string[] lineParts = line.Split("=>");
          string original = lineParts[0].Trim();
          string replacement = lineParts[1].Trim();
          if (!replacements.ContainsKey(original))
          {
            replacements.Add(original, new List<string>());
          }
          replacements[original].Add(replacement);
          inverseReplacements.Add(replacement, original);
        }

        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();

        int replaceCount = 0;
        string replacedMolecule = input;
        while (replacedMolecule != "e")
        {
          foreach (KeyValuePair<string, string> elementReplacements in inverseReplacements)
          {
            if (elementReplacements.Value == "e" && replacedMolecule.Length != elementReplacements.Key.Length)
            {
              continue;
            }
            if (replacedMolecule.Contains(elementReplacements.Key))
            {
              int index = replacedMolecule.IndexOf(elementReplacements.Key);
              replacedMolecule = replacedMolecule.Substring(0, index) + elementReplacements.Value + replacedMolecule.Substring(index + elementReplacements.Key.Length);
              replaceCount++;
            }
          }
        }
        Console.WriteLine($"Steps required to reach target molecule: {replaceCount}");
  
        stopWatch.Stop();
        Console.WriteLine($"Elapsed Time: {stopWatch.ElapsedMilliseconds} ms");
      }
    }
  }
}
