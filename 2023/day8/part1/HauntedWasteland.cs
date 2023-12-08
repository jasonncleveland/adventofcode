using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

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
        Queue<char> directions = new Queue<char>(lines[0]);

        // Parse node data
        for (int i = 2; i < lines.Length; i++)
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

        int total = 0;
        Node currentNode = nodes["AAA"];
        while (currentNode.Name != "ZZZ")
        {
          total += 1;
          char nextDirection = directions.Dequeue();
          switch (nextDirection)
          {
            case 'L':
              currentNode = currentNode.Left;
              break;
            case 'R':
              currentNode = currentNode.Right;
              break;
          }
          directions.Enqueue(nextDirection);
          if (currentNode.Name == "ZZZ")
          {
            Console.WriteLine($"Found target node {currentNode.Name} in {total} steps");
            break;
          }
        }
        Console.WriteLine($"Total value: {total}");

        stopWatch.Stop();
        Console.WriteLine($"Elapsed Time: {stopWatch.ElapsedMilliseconds} ms");
      }
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
