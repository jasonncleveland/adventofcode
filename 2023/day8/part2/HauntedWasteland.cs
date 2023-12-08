using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

class HauntedWasteland
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

        Dictionary<string, Node> nodes = new Dictionary<string, Node>();
        string directions = lines[0];

        // Parse node data
        for (long i = 2; i < lines.Length; i++)
        {
          string line = lines[i];
          string[] lineParts = line.Split(" = ");
          string nodeName = lineParts[0];
          string left = lineParts[1].Substring(1, 3);
          string right = lineParts[1].Substring(6, 3);
          Node node, leftNode, rightNode;
          if (!nodes.ContainsKey(left))
          {
            nodes.Add(left, new Node(left));
          }
          leftNode = nodes[left];
          if (!nodes.ContainsKey(right))
          {
            nodes.Add(right, new Node(right));
          }
          rightNode = nodes[right];
          if (!nodes.ContainsKey(nodeName))
          {
            nodes.Add(nodeName, new Node(nodeName));
          }
          node = nodes[nodeName];
          node.Left = leftNode;
          node.Right = rightNode;
        }

        List<Node> startingNodes = nodes.Values.Where(node => node.Name.EndsWith('A')).ToList();
        List<long> stepsList = new List<long>();
        foreach (Node startNode in startingNodes)
        {
          long stepsRequired = getPathLength(startNode, new Queue<char>(directions));
          stepsList.Add(stepsRequired);
        }
        long total = calculateLeastCommonMultiple(stepsList);
        Console.WriteLine($"Total: {total}");

        stopWatch.Stop();
        Console.WriteLine($"Elapsed Time: {stopWatch.ElapsedMilliseconds} ms");
      }
    }
  }

  static long getPathLength(Node startNode, Queue<char> directions)
  {
    Node node = startNode;
    long steps = 0;
    while (true)
    {
      steps += 1;
      char nextDirection = directions.Dequeue();
      switch (nextDirection)
      {
        case 'L':
          node = node.Left;
          break;
        case 'R':
          node = node.Right;
          break;
      }
      directions.Enqueue(nextDirection);
      if (node.Name.EndsWith('Z'))
      {
        return steps;
      }
    }
  }

  static long calculateLeastCommonMultiple(List<long> numbers)
  {
    long total = 1;
    foreach (long number in numbers)
    {
      total = calculateLeastCommonMultiple(total, number);
    }

    return total;
  }

  static long calculateLeastCommonMultiple(long a, long b)
  {
    return Math.Abs(a * b) / calculateGreatestCommonDivisor(a, b);
  }

  static long calculateGreatestCommonDivisor(long a, long b)
  {
    if (b == 0)
    {
      return a;
    }

    if (b > a)
    {
      return calculateGreatestCommonDivisor(a, b % a);
    }
    else
    {
      return calculateGreatestCommonDivisor(b, a % b);
    }
  }
}

class Node
{
  public string Name { get; set; }
  public Node Left { get; set; }
  public Node Right { get; set; }

  public Node(string name, Node left = null, Node right = null)
  {
    Name = name;
    Left = left;
    Right = right;
  }
}
