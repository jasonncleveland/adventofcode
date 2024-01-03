using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

class Snowverload
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

        Graph network = new Graph();

        foreach (string line in lines)
        {
          string[] lineParts = line.Split(':');
          string nodeName = lineParts[0];
          string[] nodeEdgeNames = lineParts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);

          Node currentNode = network.Nodes.Find(node => node.Name == nodeName);
          if (currentNode == null)
          {
            currentNode = new Node(nodeName);
            network.addNode(currentNode);
          }

          foreach (string edgeNodeName in nodeEdgeNames)
          {
            Node edgeNode = network.Nodes.Find(node => node.Name == edgeNodeName);
            if (edgeNode == null)
            {
              edgeNode = new Node(edgeNodeName);
              network.addNode(edgeNode);
            }

            // Graph is bi-directional
            currentNode.addEdge(edgeNode);
            edgeNode.addEdge(currentNode);
          }
        }

        Console.WriteLine($"Total number of nodes: {network.Nodes.Count}");

        int numGroups;
        HashSet<string> visitedNodes;

        numGroups = 0;
        visitedNodes = new HashSet<string>();
        foreach (Node node in network.Nodes)
        {
          if (visitedNodes.Contains(node.Name)) continue;

          HashSet<string> connectedNodes = traverseNetwork(node);
          visitedNodes.UnionWith(connectedNodes);
          numGroups += 1;
        }
        Console.WriteLine($"Total number of groups: {numGroups}");

        int total = 1;
        
        // We need to find the 3 edges to remove so run the algorithm 3 times to remove the most traversed edge
        findAndRemoveBridge(network);
        findAndRemoveBridge(network);
        findAndRemoveBridge(network);

        numGroups = 0;
        visitedNodes = new HashSet<string>();
        // Find the number of nodes in each group
        foreach (Node node in network.Nodes)
        {
          if (visitedNodes.Contains(node.Name)) continue;

          HashSet<string> connectedNodes = traverseNetwork(node);
          visitedNodes.UnionWith(connectedNodes);
          numGroups += 1;
          total *= connectedNodes.Count;
        }
        Console.WriteLine($"Total number of groups: {numGroups}");
        if (numGroups != 2)
        {
          throw new Exception($"Invalid number of groups");
        }
        Console.WriteLine($"Total value: {total}");

        stopWatch.Stop();
        Console.WriteLine($"Elapsed Time: {stopWatch.ElapsedMilliseconds} ms");
      }
    }
  }

  static string findAndRemoveBridge(Graph network)
  {
    // Use Girvan-Newman Algorithm to find the most traversed edge

    Dictionary<string, int> betweenness = new Dictionary<string, int>();
    HashSet<string> checkedConnections = new HashSet<string>();

    foreach (Node startNode in network.Nodes)
    {
      foreach (Node endNode in network.Nodes)
      {
        if (startNode == endNode)
        {
          continue;
        }

        // Prevent checking the same path multiple times
        string firstKey = $"{startNode.Name},{endNode.Name}";
        string secondKey = $"{endNode.Name},{startNode.Name}";
        if (checkedConnections.Contains(firstKey) || checkedConnections.Contains(secondKey))
        {
          continue;
        }
        checkedConnections.Add(firstKey);
        checkedConnections.Add(secondKey);

        markShortestPath(startNode, endNode, betweenness);
      }
    }

    string mostVisitedEdge = null;
    int highestBetweenness = int.MinValue;
    foreach (KeyValuePair<string, int> data in betweenness)
    {
      if (data.Value > highestBetweenness)
      {
        highestBetweenness = data.Value;
        mostVisitedEdge = data.Key;
      }
    }
    Console.WriteLine($"Most used edge: {mostVisitedEdge}: {highestBetweenness}");
    string[] edgeNodes = mostVisitedEdge.Split(',');
    string firstNodeName = edgeNodes[0];
    string secondNodeName = edgeNodes[1];

    Node firstNode = network.Nodes.Find(node => node.Name == firstNodeName);
    Node secondNode = network.Nodes.Find(node => node.Name == secondNodeName);

    firstNode.Edges.Remove(secondNode);
    secondNode.Edges.Remove(firstNode);

    return null;
  }

  static void markShortestPath(Node startNode, Node endNode, Dictionary<string, int> betweeness)
  {
    Queue<NodeWithPath> nodesToVisit = new Queue<NodeWithPath>();
    HashSet<string> visitedNodes = new HashSet<string>();

    nodesToVisit.Enqueue(new NodeWithPath(startNode, new HashSet<string>()));
    visitedNodes.Add(startNode.Name);

    int fewestVisitedEdges = int.MaxValue;
    while (nodesToVisit.Count > 0)
    {
      NodeWithPath currentNode = nodesToVisit.Dequeue();

      if (currentNode.Node == endNode)
      {
        if (currentNode.VisitedEdges.Count <= fewestVisitedEdges)
        {
          fewestVisitedEdges = currentNode.VisitedEdges.Count;
          foreach (string visitedEdge in currentNode.VisitedEdges)
          {
            if (!betweeness.ContainsKey(visitedEdge))
            {
              betweeness.Add(visitedEdge, 0);
            }

            betweeness[visitedEdge] += 1;
          }
        }
        break;
      }

      foreach (Node childNode in currentNode.Node.Edges)
      {
        if (!visitedNodes.Contains(childNode.Name))
        {
          HashSet<string> visitedEdges = new HashSet<string>(currentNode.VisitedEdges);
          visitedEdges.Add($"{currentNode.Node.Name},{childNode.Name}");
          nodesToVisit.Enqueue(new NodeWithPath(childNode, visitedEdges));
          visitedNodes.Add(childNode.Name);
        }
      }
    }
  }

  static HashSet<string> traverseNetwork(Node startNode)
  {
    Queue<Node> nodesToVisit = new Queue<Node>();
    HashSet<string> visitedNodes = new HashSet<string>();

    nodesToVisit.Enqueue(startNode);
    visitedNodes.Add(startNode.Name);

    while (nodesToVisit.Count > 0)
    {
      Node currentNode = nodesToVisit.Dequeue();

      foreach (Node childNode in currentNode.Edges)
      {
        if (!visitedNodes.Contains(childNode.Name))
        {
          nodesToVisit.Enqueue(childNode);
          visitedNodes.Add(childNode.Name);
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
  public string Name { get; }
  public HashSet<Node> Edges { get; }

  public Node(string name)
  {
    Name = name;
    Edges = new HashSet<Node>();
  }

  public void addEdge(Node connectedNode)
  {
    Edges.Add(connectedNode);
  }
}

class NodeWithPath
{
  public Node Node { get; set; }
  public HashSet<string> VisitedEdges { get; set; }

  public NodeWithPath(Node node, HashSet<string> visitedEdges)
  {
    Node = node;
    VisitedEdges = visitedEdges;
  }
}
