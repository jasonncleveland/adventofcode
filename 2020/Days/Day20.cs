using System;
using System.Collections.Generic;
using System.Linq;

using AdventOfCode.Shared.Grid;
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
                .Select(line => line.ToCharArray().ToList())
                .ToList();
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
            var edges = new List<string>
            {
                // Find left and left reversed edges
                GetLeftEdge(tile.Grid),
                GetLeftEdge(tile.Grid, true),
                // Find right and right reversed edges
                GetRightEdge(tile.Grid),
                GetRightEdge(tile.Grid, true),
                // Find top and top reversed edges
                GetTopEdge(tile.Grid),
                GetTopEdge(tile.Grid, true),
                // Find bottom and bottom reversed edges
                GetBottomEdge(tile.Grid),
                GetBottomEdge(tile.Grid, true)
            };

            foreach (string edge in edges)
            {
                edgePairs.TryAdd(edge, []);
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
        var edgePairs = new Dictionary<string, List<int>>();
        var tileLookup = tiles.ToDictionary(tile => tile.TileId);

        // The tiles are 10x10 grids
        foreach (var tile in tiles)
        {
            var edges = new List<string>
            {
                // Find left and left reversed edges
                GetLeftEdge(tile.Grid),
                GetLeftEdge(tile.Grid, true),
                // Find right and right reversed edges
                GetRightEdge(tile.Grid),
                GetRightEdge(tile.Grid, true),
                // Find top and top reversed edges
                GetTopEdge(tile.Grid),
                GetTopEdge(tile.Grid, true),
                // Find bottom and bottom reversed edges
                GetBottomEdge(tile.Grid),
                GetBottomEdge(tile.Grid, true)
            };

            foreach (string edge in edges)
            {
                edgePairs.TryAdd(edge, []);
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
                outsideEdges.TryAdd(tileIds[0], 0);
                outsideEdges[tileIds[0]] += 1;
            }
        }

        // If an edge has four occurrences then it is a corner. Any edge can be used so we take the first one
        var cornerTileId = outsideEdges.First(edge => edge.Value == 4).Key;
        var currentTile = tileLookup[cornerTileId];

        // Rotate tile so that left side and top edge are outside
        RotateCornerTile(edgePairs, currentTile);

        var outputGrids = new List<List<List<List<char>>>>();
        while (true)
        {
            // Rotate right neighbours
            var grids = RotateRightTileRec(edgePairs, tileLookup, currentTile);
            outputGrids.Add(grids);

            // Rotate bottom neighbour
            var bottomNeighbourTileId = RotateBottomTile(edgePairs, tileLookup, currentTile);
            if (bottomNeighbourTileId is null)
            {
                break;
            }
            // Move down to next tile
            currentTile = tileLookup[bottomNeighbourTileId!.Value];
        }

        // Combine the correctly ordered tiles into a combined tile ignoring individual tile edges
        var combinedGrid = new List<List<char>>();
        foreach (var gridRow in outputGrids)
        {
            for (var row = 1; row < currentTile.Grid.Count - 1; row++)
            {
                var combinedRow = new List<char>();
                for (var g = 0; g < gridRow.Count; g++)
                {
                    for (var column = 1; column < currentTile.Grid[row].Count - 1; column++)
                    {
                        combinedRow.Add(gridRow[g][row][column]);
                    }
                }
                combinedGrid.Add(combinedRow);
            }
        }

        // Rotate tile until top edge matches right edge
        int result;
        while (true)
        {
            // Try square as given
            if (CheckForSeaMonster(combinedGrid, out result))
            {
                break;
            }

            // Flip horizontally
            combinedGrid.Flip();
            if (CheckForSeaMonster(combinedGrid, out result))
            {
                break;
            }

            // Flip back for next rotation
            // Memory allocation is so expensive that it's faster to rotate in place twice vs allocating
            combinedGrid.Flip();

            // Rotate 90 degrees right for next iteration
            combinedGrid.Rotate();
        }

        return result.ToString();
    }

    private string GetLeftEdge(IReadOnlyList<IReadOnlyList<char>> grid, bool isReverse = false)
    {
        var edge = "";

        if (isReverse)
        {
            for (int row = grid.Count - 1; row >= 0; row--)
            {
                edge += grid[row][0];
            }
        }
        else
        {
            for (int row = 0; row < grid.Count; row++)
            {
                edge += grid[row][0];
            }
        }

        return edge;
    }

    private string GetRightEdge(IReadOnlyList<IReadOnlyList<char>> grid, bool isReverse = false)
    {
        var edge = "";

        if (isReverse)
        {
            for (int row = grid.Count - 1; row >= 0; row--)
            {
                edge += grid[row][^1];
            }
        }
        else
        {
            for (int row = 0; row < grid.Count; row++)
            {
                edge += grid[row][^1];
            }
        }

        return edge;
    }

    private string GetTopEdge(IReadOnlyList<IReadOnlyList<char>> grid, bool isReverse = false)
    {
        var edge = "";

        if (isReverse)
        {
            for (int column = grid[^1].Count - 1; column >= 0; column--)
            {
                edge += grid[0][column];
            }
        }
        else
        {
            for (int column = 0; column < grid[0].Count; column++)
            {
                edge += grid[0][column];
            }
        }

        return edge;
    }

    private string GetBottomEdge(IReadOnlyList<IReadOnlyList<char>> grid, bool isReverse = false)
    {
        var edge = "";

        if (isReverse)
        {
            for (int column = grid[^1].Count - 1; column >= 0; column--)
            {
                edge += grid[^1][column];
            }
        }
        else
        {
            for (int column = 0; column < grid[^1].Count; column++)
            {
                edge += grid[^1][column];
            }
        }

        return edge;
    }

    private void RotateCornerTile(Dictionary<string, List<int>> edgePairs, TileInfo tile)
    {
        // Rotate tile so that left side and top edge are outside
        while (true)
        {
            // Try square as given
            if (edgePairs[GetLeftEdge(tile.Grid)].Count == 1 && edgePairs[GetTopEdge(tile.Grid)].Count == 1)
            {
                break;
            }

            // Flip horizontally
            tile.Grid.Flip();
            if (edgePairs[GetLeftEdge(tile.Grid)].Count == 1 && edgePairs[GetTopEdge(tile.Grid)].Count == 1)
            {
                break;
            }

            // Flip back for next rotation
            // Memory allocation is so expensive that it's faster to rotate in place twice vs allocating
            tile.Grid.Flip();

            // Rotate 90 degrees right for next iteration
            tile.Grid.Rotate();
        }
    }

    private int? RotateBottomTile(Dictionary<string, List<int>> edgePairs, Dictionary<int, TileInfo> tileLookup, TileInfo tile)
    {
        var bottomEdge = GetBottomEdge(tile.Grid);
        if (edgePairs[bottomEdge].Count == 1)
        {
            // No bottom neighbour to check
            return null;
        }

        var bottomNeighbourTileId = edgePairs[bottomEdge].First(tileId => tileId != tile.TileId);
        var currentTile = tileLookup[bottomNeighbourTileId];

        // Rotate tile until top edge matches right edge
        while (true)
        {
            // Try square as given
            if (GetTopEdge(currentTile.Grid) == bottomEdge)
            {
                break;
            }

            // Flip horizontally
            currentTile.Grid.Flip();
            if (GetTopEdge(currentTile.Grid) == bottomEdge)
            {
                break;
            }

            // Flip back for next rotation
            // Memory allocation is so expensive that it's faster to rotate in place twice vs allocating
            currentTile.Grid.Flip();

            // Rotate 90 degrees right for next iteration
            currentTile.Grid.Rotate();
        }

        return bottomNeighbourTileId;
    }

    private List<List<List<char>>> RotateRightTileRec(Dictionary<string, List<int>> edgePairs, Dictionary<int, TileInfo> tileLookup, TileInfo tile)
    {
        var rightEdge = GetRightEdge(tile.Grid);
        if (edgePairs[rightEdge].Count == 1)
        {
            // No right neighbour to check
            return [tile.Grid];
        }

        var rightNeighbourTileId = edgePairs[rightEdge].First(tileId => tileId != tile.TileId);
        var currentTile = tileLookup[rightNeighbourTileId];

        // Rotate tile until left edge matches right edge
        while (true)
        {
            // Try square as given
            if (GetLeftEdge(currentTile.Grid) == rightEdge)
            {
                var neighbours = RotateRightTileRec(edgePairs, tileLookup, currentTile);
                // Combine current tile with right neighbours
                return [tile.Grid, .. neighbours];
            }

            // Flip horizontally
            currentTile.Grid.Flip();
            if (GetLeftEdge(currentTile.Grid) == rightEdge)
            {
                var neighbours = RotateRightTileRec(edgePairs, tileLookup, currentTile);
                // Combine current tile with right neighbours
                return [tile.Grid, .. neighbours];
            }

            // Flip back for next rotation
            // Memory allocation is so expensive that it's faster to rotate in place twice vs allocating
            currentTile.Grid.Flip();

            // Rotate 90 degrees right for next iteration
            currentTile.Grid.Rotate();
        }
    }

    private bool CheckForSeaMonster(List<List<char>> grid, out int result)
    {
        var checkedCoordinates = new HashSet<(int row, int column)>();
        var seaMonsterCoordinates = new HashSet<(int row, int column)>();
        var foundSeaMonster = false;
        for (var row = 0; row < grid.Count; row++)
        {
            for (var column = 0; column < grid[row].Count; column++)
            {
                if (grid[row][column] == '#')
                {
                    checkedCoordinates.Add((row, column));
                    try
                    {
                        if (grid[row][column] == '#' &&
                            grid[row + 1][column + 1] == '#' &&
                            grid[row + 1][column + 4] == '#' &&
                            grid[row][column + 5] == '#' &&
                            grid[row][column + 6] == '#' &&
                            grid[row + 1][column + 7] == '#' &&
                            grid[row + 1][column + 10] == '#' &&
                            grid[row][column + 11] == '#' &&
                            grid[row][column + 12] == '#' &&
                            grid[row + 1][column + 13] == '#' &&
                            grid[row + 1][column + 16] == '#' &&
                            grid[row][column + 17] == '#' &&
                            grid[row][column + 18] == '#' &&
                            grid[row - 1][column + 18] == '#' &&
                            grid[row][column + 19] == '#')
                        {
                            foundSeaMonster = true;
                            seaMonsterCoordinates.Add((row, column));
                            seaMonsterCoordinates.Add((row + 1, column + 1));
                            seaMonsterCoordinates.Add((row + 1, column + 4));
                            seaMonsterCoordinates.Add((row, column + 5));
                            seaMonsterCoordinates.Add((row, column + 6));
                            seaMonsterCoordinates.Add((row + 1, column + 7));
                            seaMonsterCoordinates.Add((row + 1, column + 10));
                            seaMonsterCoordinates.Add((row, column + 11));
                            seaMonsterCoordinates.Add((row, column + 12));
                            seaMonsterCoordinates.Add((row + 1, column + 13));
                            seaMonsterCoordinates.Add((row + 1, column + 16));
                            seaMonsterCoordinates.Add((row, column + 17));
                            seaMonsterCoordinates.Add((row, column + 18));
                            seaMonsterCoordinates.Add((row - 1, column + 18));
                            seaMonsterCoordinates.Add((row, column + 19));
                        }
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        // Impossible to fit sea monster starting a current position
                    }
                }
            }
        }

        result = checkedCoordinates.Count - checkedCoordinates.Intersect(seaMonsterCoordinates).Count();
        return foundSeaMonster;
    }
}

internal sealed class TileInfo(int tileId, List<List<char>> grid)
{
    public int TileId { get; } = tileId;
    public List<List<char>> Grid { get; } = grid;
}