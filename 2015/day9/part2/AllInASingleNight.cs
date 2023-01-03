using System;
using System.Collections.Generic;
using System.IO;

class AllInASingleNight
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        string[] lines = File.ReadAllLines(fileName);

        Graph graph = new Graph();

        foreach (string line in lines)
        {
          string[] lineParts = line.Split('=');
          string[] cityNames = lineParts[0].Trim().Split("to");
          Node first = graph.AddNode(cityNames[0].Trim());
          Node second = graph.AddNode(cityNames[1].Trim());
          first.AddLink(second, int.Parse(lineParts[1].Trim()));
        }

        // Find the Hamiltonian Path
        int longestPathLength = int.MinValue;
        foreach (Node node in graph.Nodes)
        {
          int pathLength = calculateHamiltonianPath(node, new List<string>());
          if (pathLength > longestPathLength)
          {
            longestPathLength = pathLength;
          }
        }
        Console.WriteLine($"Overall longest path: {longestPathLength}");
      }
    }
  }

  static int calculateHamiltonianPath(Node start, List<string> visited)
  {
    visited.Add(start.Name);
    int longestPath = int.MinValue;
    foreach (Link link in start.Links)
    {
      if (!visited.Exists(name => name == link.End.Name))
      {
        int result = calculateHamiltonianPath(link.End, new List<string>(visited));
        if (result + link.Weight > longestPath)
        {
          longestPath = result + link.Weight;
        }
      }
    }
    return (longestPath == int.MinValue) ? 0 : longestPath;
  }
}

class Graph
{
  public List<Node> Nodes = new List<Node>();

  public Node AddNode(string name)
  {
    if (!Nodes.Exists(node => node.Name == name))
    {
      Nodes.Add(new Node(name));
    }
    return Nodes.Find(node => node.Name == name);
  }
}

class Node
{
  public string Name { get; set; }
  public List<Link> Links = new List<Link>();

  public Node(string name)
  {
    Name = name;
  }

  public void AddLink(Node end, int weight)
  {
    Links.Add(new Link(this, end, weight));

    // Add return link if it doesn't already exist
    if (!end.Links.Exists(link => link.Start == end && link.End == this))
    {
      end.AddLink(this, weight);
    }
  }
}

class Link
{
  public int Weight { get; set; }
  public Node Start { get; set; }
  public Node End { get; set; }

  public Link(Node start, Node end, int weight)
  {
    Start = start;
    End = end;
    Weight = weight;
  }
}
