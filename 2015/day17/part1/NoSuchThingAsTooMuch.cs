using System;
using System.Collections.Generic;
using System.IO;

class NoSuchThingAsTooMuch
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        string[] lines = File.ReadAllLines(fileName);

        int litersOfEggnog = 150;

        List<Container> availableContainers = new List<Container>();
        int containerCount = 0;
        foreach (string line in lines)
        {
          availableContainers.Add(new Container(containerCount++, int.Parse(line)));
        }

        HashSet<string> combinations = new HashSet<string>();
        findCombinations(availableContainers, litersOfEggnog, new List<Container>(), combinations);
        Console.WriteLine($"Unique combinations: {combinations.Count}");
      }
    }
  }

  static void printList(List<Container> containers)
  {
    foreach (Container container in containers)
    {
      Console.Write($"{container.Volume} ({container.Id}) ");
    }
    Console.WriteLine();
  }

  static int sumList(List<Container> containers)
  {
    int total = 0;
    foreach (Container container in containers)
    {
      total += container.Volume;
    }
    return total;
  }

  static void findCombinations(List<Container> remainingContainers, int targetVolume, List<Container> combination, HashSet<string> combinations)
  {
    int totalVolume = sumList(combination);

    if (totalVolume == targetVolume)
    {
      combination.Sort((a, b) => a.Id - b.Id);
      List<string> indexes = new List<string>();
      foreach (Container container in combination)
      {
        indexes.Add(container.Id.ToString());
      }
      string combinationId = string.Join(",", indexes.ToArray());
      combinations.Add(combinationId);
      return;
    }
    if (totalVolume > targetVolume || remainingContainers.Count == 0)
    {
      return;
    }

    for (int i = 0; i < remainingContainers.Count; i++)
    {
      Container currentContainer = remainingContainers[i];
      List<Container> newRemainingContainers = new List<Container>(remainingContainers);
      newRemainingContainers.Remove(currentContainer);
      List<Container> newCombination = new List<Container>(combination);
      newCombination.Add(currentContainer);
      findCombinations(newRemainingContainers, targetVolume, newCombination, combinations);
    }
  }
}

class Container
{
  public int Id { get; set; }
  public int Volume { get; set; }

  public Container(int id, int volume)
  {
    Id = id;
    Volume = volume;
  }
}
