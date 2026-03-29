using System;
using System.Collections.Generic;
using System.Linq;

using AdventOfCode.Shared.Solver;

using Microsoft.Extensions.Logging;

namespace AdventOfCode.Y2020.Days;

internal sealed class Day20 : AbstractDaySolver<IReadOnlyList<TileInfo>>
{
    protected override IReadOnlyList<TileInfo> ParseInput(ILogger logger, string fileContents)
    {
        var lines = fileContents.Split("\n\n", StringSplitOptions.RemoveEmptyEntries);
        var tiles = new List<TileInfo>();

        foreach (string tileString in lines)
        {
            var tileLines = tileString.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var tileId = int.Parse(tileLines.First().Substring(5, 4));
            var grid = tileLines
                .Skip(1)
                .Select(line => line.ToCharArray().ToList().AsReadOnly())
                .ToList()
                .AsReadOnly();
            tiles.Add(new TileInfo(tileId, grid));
        }

        return tiles.AsReadOnly();
    }

    protected override string SolvePart1(ILogger logger, IReadOnlyList<TileInfo> tiles)
    {
        long total = 1;

        Dictionary<string, List<int>> edgePairs = new();

        // The tiles are 10x10 grids
        foreach (var tile in tiles)
        {
            List<string> edges = new();

            // Find left and left reversed edges
            string left = "";
            string leftReversed = "";

            for (int row = 0; row < 10; row++)
            {
                left += tile.Grid[row][0];
                leftReversed += tile.Grid[9 - row][0];
            }

            edges.Add(left);
            edges.Add(leftReversed);

            // Find right and right reversed edges
            string right = "";
            string rightReversed = "";

            for (int row = 0; row < 10; row++)
            {
                right += tile.Grid[row][9];
                rightReversed += tile.Grid[9 - row][9];
            }

            edges.Add(right);
            edges.Add(rightReversed);

            // Find top and top reversed edges
            string top = "";
            string topReversed = "";

            for (int column = 0; column < 10; column++)
            {
                top += tile.Grid[0][column];
                topReversed += tile.Grid[0][9 - column];
            }

            edges.Add(top);
            edges.Add(topReversed);

            // Find bottom and bottom reversed edges
            string bottom = "";
            string bottomReversed = "";

            for (int column = 0; column < 10; column++)
            {
                bottom += tile.Grid[9][column];
                bottomReversed += tile.Grid[9][9 - column];
            }

            edges.Add(bottom);
            edges.Add(bottomReversed);


            foreach (string edge in edges)
            {
                if (!edgePairs.ContainsKey(edge))
                {
                    edgePairs.Add(edge, new());
                }
                edgePairs[edge].Add(tile.TileId);
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
        foreach ((int tileId, int outsideEdgeCount) in outsideEdges)
        {
            if (outsideEdgeCount == 4)
            {
                total *= tileId;
            }
        }

        return total.ToString();
    }

    protected override string SolvePart2(ILogger logger, IReadOnlyList<TileInfo> tiles)
    {
        return 0.ToString();
    }
}

internal sealed class TileInfo(int tileId, IReadOnlyList<IReadOnlyList<char>> grid)
{
    public int TileId { get; } = tileId;
    public IReadOnlyList<IReadOnlyList<char>> Grid { get; } = grid;
}