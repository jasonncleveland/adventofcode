using System;
using System.Collections.Generic;
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
        }

        HashSet<string> molecules = new HashSet<string>();
        foreach (KeyValuePair<string, List<string>> elementReplacements in replacements)
        {
          List<int> indexes = new List<int>();
          List<char> currentElement = new List<char>();
          string elementString;
          for (int i = 0; i < input.Length; i++)
          {
            // Elements start with a capital letter
            if (currentElement.Count > 0 && input[i] >= 'A' && input[i] <= 'Z')
            {
              elementString = new string(currentElement.ToArray());
              if (elementString == elementReplacements.Key)
              {
                indexes.Add(i - elementString.Length);
              }
              currentElement.Clear();
            }
            currentElement.Add(input[i]);
          }
          // Handle the last element
          elementString = new string(currentElement.ToArray());
          if (elementString == elementReplacements.Key)
          {
            indexes.Add(input.Length - elementString.Length);
          }

          foreach (string replacement in elementReplacements.Value)
          {
            foreach (int index in indexes)
            {
              List<char> newMolecule = new List<char>(input);
              newMolecule.RemoveRange(index, elementReplacements.Key.Length);
              newMolecule.InsertRange(index, replacement.ToCharArray());
              molecules.Add(new string(newMolecule.ToArray()));
            }
          }
        }

        Console.WriteLine($"Number of unique molecules: {molecules.Count}");
      }
    }
  }
}
