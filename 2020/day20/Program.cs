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
                long part2 = SolvePart2(lines);
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
        long total = 1;

        Dictionary<string, List<int>> edgePairs = new();

        // The tiles are 10x10 grids
        for (int i = 0; i < lines.Length; i += 12)
        {
            int tileId = int.Parse(lines[i].Substring(5, 4));

            List<string> edges = new();

            // Find left and left reversed edges
            string left = "";
            string leftReversed = "";

            for (int row = 1; row < 11; row++)
            {
                left += lines[i + row][0];
                leftReversed += lines[i + (11 - row)][0];
            }

            edges.Add(left);
            edges.Add(leftReversed);

            // Find right and right reversed edges
            string right = "";
            string rightReversed = "";

            for (int row = 1; row < 11; row++)
            {
                right += lines[i + row][9];
                rightReversed += lines[i + (11 - row)][9];
            }

            edges.Add(right);
            edges.Add(rightReversed);

            // Find top and top reversed edges
            string top = "";
            string topReversed = "";

            for (int column = 0; column < 10; column++)
            {
                top += lines[i + 1][column];
                topReversed += lines[i + 1][9 - column];
            }

            edges.Add(top);
            edges.Add(topReversed);

            // Find bottom and bottom reversed edges
            string bottom = "";
            string bottomReversed = "";

            for (int column = 0; column < 10; column++)
            {
                bottom += lines[i + 10][column];
                bottomReversed += lines[i + 10][9 - column];
            }

            edges.Add(bottom);
            edges.Add(bottomReversed);


            foreach (string edge in edges)
            {
                if (!edgePairs.ContainsKey(edge))
                {
                    edgePairs.Add(edge, new());
                }
                edgePairs[edge].Add(tileId);
            }
        }

        // Count the number of outside edges per tile
        Dictionary<int, int> outsideEdges = new();
        foreach ((string edge, List<int> tileIds) in edgePairs)
        {
            // If an edge only has 1 occurence, it cannot border another tile and is an outside edge
            if (tileIds.Count == 1)
            {
                if (!outsideEdges.ContainsKey(tileIds[0]))
                {
                    outsideEdges.Add(tileIds[0], 0);
                }
                outsideEdges[tileIds[0]] += 1;
            }
        }

        // Find the tiles that contain 2 outside edges
        foreach ((int tileId, int ousideEdgeCount) in outsideEdges)
        {
            if (ousideEdgeCount == 4)
            {
                total *= tileId;
            }
        }

        return total;
    }

    static long SolvePart2(string[] lines)
    {
        long total = 0;

        // TODO: Implement logic to solve part 2

        return total;
    }
}