using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

class SupplyStacks
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        string fileContents = File.ReadAllText(fileName);
        string[] lines = fileContents.Split('\n');

        int columnNum = lines[0].Length / 4 + 1;
        List<List<char>> inputData = new List<List<char>>();
        List<Stack<char>> problemData = new List<Stack<char>>();
        for (int i = 0; i < columnNum; i++)
        {
          inputData.Add(new List<char>());
        }

        
        string pattern = @"move (\d+) from (\d+) to (\d+)";
        Regex regex = new Regex(pattern, RegexOptions.Compiled);

        bool endOfFirstInput = false;
        foreach (string line in lines)
        {
          // We have reached the end of the input
          if (line.Length == 0)
          {
            endOfFirstInput = true;
            foreach (var column in inputData)
            {
              column.Reverse();
              problemData.Add(new Stack<char>(column));
            }
            continue;
          }

          if (!endOfFirstInput)
          {
            int currentColumn = 0;
            for (int i = 0; i < line.Length; i += 4)
            {
              if (char.IsDigit(line, i + 1))
              {
                // We've reached the end of the input
                break;
              }
              char letter = line[i + 1];
              if (char.IsLetter(letter))
              {
                inputData[currentColumn].Add(letter);
              }
              currentColumn++;
            }
          }
          else
          {
            MatchCollection matches = regex.Matches(line);

            foreach(Match match in matches)
            {
              GroupCollection groups = match.Groups;
              int numToMove = int.Parse(groups[1].Value);
              int startIndex = int.Parse(groups[2].Value) - 1;
              int endIndex = int.Parse(groups[3].Value) - 1;

              Stack<char> startStack = problemData[startIndex];
              Stack<char> endStack = problemData[endIndex];

              List<char> removedContainers = new List<char>();
              for (int i = 0; i < numToMove; i++)
              {
                removedContainers.Add(startStack.Pop());
              }
              removedContainers.Reverse();
              foreach (char container in removedContainers)
              {
                endStack.Push(container);
              }
            }
          }
        }

        foreach (var column in problemData)
        {
          Console.Write(column.Peek());
        }
        Console.WriteLine();
      }
    }
  }
}
