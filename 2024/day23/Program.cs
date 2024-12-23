using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

public class Program
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
                stopWatch.Stop();
                Console.WriteLine($"File read ({stopWatch.Elapsed.TotalMilliseconds} ms)");

                Stopwatch part1Timer = new Stopwatch();
                part1Timer.Start();
                long part1 = SolvePart1(lines);
                part1Timer.Stop();
                Console.WriteLine($"Part 1: {part1} ({part1Timer.Elapsed.TotalMilliseconds} ms)");

                Stopwatch part2Timer = new Stopwatch();
                part2Timer.Start();
                string part2 = SolvePart2(lines);
                part2Timer.Stop();
                Console.WriteLine($"Part 2: {part2} ({part2Timer.Elapsed.TotalMilliseconds} ms)");
            }
            else
            {
                throw new ArgumentException("Invalid file name provided. Please provide a valid file name.");
            }
        }
        else
        {
            throw new ArgumentException("Input data file name not provided. Please provide the file name as an argument: dotnet run <file-name>");
        }
    }

    static long SolvePart1(string[] lines)
    {
        Dictionary<string, List<string>> connections = parseInput(lines);

        HashSet<string> sets = new();
        foreach ((string first, List<string> neighbours) in connections)
        {
            if (neighbours.Count >= 2)
            {
                foreach (string second in neighbours)
                {
                    foreach (string third in neighbours)
                    {
                        if (second == third)
                        {
                            continue;
                        }

                        if (first.StartsWith('t') || second.StartsWith('t') || third.StartsWith('t'))
                        {
                            List<string> secondConnections = connections[second];
                            List<string> thirdConnections = connections[third];
            
                            if (secondConnections.Contains(third) && thirdConnections.Contains(second))
                            {
                                List<string> group = new() { first, second, third };
                                group.Sort();
                                sets.Add(string.Join(",", group));
                            }
                        }
                    }
                }
            }
        }

        return sets.Count;
    }

    static string SolvePart2(string[] lines)
    {
        Dictionary<string, List<string>> connections = parseInput(lines);

        // Find all maximal cliques using the Bron-Kerbosch algorithm
        List<HashSet<string>> cliques = BronKerbosch(connections, [], new(connections.Keys), []);

        // Find the largest clique
        HashSet<string> maxClique = [];
        foreach (HashSet<string> clique in cliques)
        {
            if (clique.Count > maxClique.Count)
            {
                maxClique = clique;
            }
        }

        // Sort the clique alphabetically and join with commas
        List<string> output = new(maxClique);
        output.Sort();
        return string.Join(",", output);
    }

    static Dictionary<string, List<string>> parseInput(string[] lines)
    {
        Dictionary<string, List<string>> connections = new();

        foreach (string line in lines)
        {
            string[] lineParts = line.Split('-');
            if (!connections.ContainsKey(lineParts[0]))
            {
                connections.Add(lineParts[0], new());
            }
            connections[lineParts[0]].Add(lineParts[1]);
            if (!connections.ContainsKey(lineParts[1]))
            {
                connections.Add(lineParts[1], new());
            }
            connections[lineParts[1]].Add(lineParts[0]);
        }

        return connections;
    }

    /**
     * The Bron-Kerbosh algorithm is used to find all maximal cliques in a graph
     * A clique is a subset of verticies where every node is connected to each other
     *
     * https://en.wikipedia.org/wiki/Bron%E2%80%93Kerbosch_algorithm
     *
     * Return a list of sets containing the maximal clique nodes
     */
    static List<HashSet<string>> BronKerbosch(Dictionary<string, List<string>> connections, HashSet<string> R, HashSet<string> P, HashSet<string> X)
    {
        List<HashSet<string>> cliques = new();

        if (P.Count == 0 && X.Count == 0)
        {
            cliques.Add(R);
            return cliques;
        }

        foreach (string v in P)
        {
            HashSet<string> newR = new(R);
            newR.UnionWith(new HashSet<string>() { v });
            HashSet<string> newP = new(P);
            newP.IntersectWith(new HashSet<string>(connections[v]));
            HashSet<string> newX = new(X);
            newX.IntersectWith(new HashSet<string>(connections[v]));
            cliques.AddRange(BronKerbosch(connections, newR, newP, newX));
            P.Remove(v);
            X.Add(v);
        }

        return cliques;
    }
}