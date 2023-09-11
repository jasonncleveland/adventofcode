using System;
using System.Collections.Generic;
using System.IO;

class DigitalPlumber
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        string[] lines = File.ReadAllLines(fileName);

        Graph network = new Graph();

        // Input the nodes and set up the network
        foreach (string line in lines)
        {
          string[] lineParts = line.Split("<->");
          int nodeId = int.Parse(lineParts[0]);
          int[] nodeEdgeIds = Array.ConvertAll<string, int>(lineParts[1].Split(','), edgeNodeId => int.Parse(edgeNodeId));
          Node currentNode = network.Nodes.Find(node => node.Id == nodeId);
          if (currentNode == null)
          {
            currentNode = new Node(nodeId);
            network.addNode(currentNode);
          }
          foreach (int edgeNodeId in nodeEdgeIds)
          {
            Node edgeNode = network.Nodes.Find(node => node.Id == edgeNodeId);
            if (edgeNode == null)
            {
              edgeNode = new Node(edgeNodeId);
              network.addNode(edgeNode);
            }
            currentNode.addEdge(edgeNode);
          }
        }

        // Find the number of isolated node groups
        Console.WriteLine($"Total number of nodes: {network.Nodes.Count}");

        HashSet<int> visitedNodes = new HashSet<int>();
        int numGroups = 0;
        foreach (Node node in network.Nodes)
        {
          if (visitedNodes.Contains(node.Id)) continue;

          HashSet<int> connectedNodes = traverseNetwork(node);
          visitedNodes.UnionWith(connectedNodes);
          numGroups += 1;
        }
        Console.WriteLine($"Total number of groups: {numGroups}");
      }
    }
  }

  static HashSet<int> traverseNetwork(Node startNode)
  {
    Queue<Node> nodesToVisit = new Queue<Node>();
    HashSet<int> visitedNodes = new HashSet<int>();

    nodesToVisit.Enqueue(startNode);
    visitedNodes.Add(startNode.Id);

    while (nodesToVisit.Count > 0)
    {
      Node currentNode = nodesToVisit.Dequeue();

      foreach (Node childNode in currentNode.Edges)
      {
        if (!visitedNodes.Contains(childNode.Id))
        {
          nodesToVisit.Enqueue(childNode);
          visitedNodes.Add(childNode.Id);
        }
      }
    }

    return visitedNodes;
  }
}

class Graph
{
  public List<Node> Nodes { get; }

  public Graph()
  {
    Nodes = new List<Node>();
  }

  public void addNode(Node newNode)
  {
    Nodes.Add(newNode);
  }
}

class Node
{
  public int Id { get; }
  public List<Node> Edges { get; }

  public Node(int id)
  {
    Id = id;
    Edges = new List<Node>();
  }

  public void addEdge(Node connectedNode)
  {
    Edges.Add(connectedNode);
  }
}
