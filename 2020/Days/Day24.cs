using System;
using System.Collections.Generic;
using System.Linq;

using AdventOfCode.Shared.Solver;

using Microsoft.Extensions.Logging;

namespace AdventOfCode.Y2020.Days;

internal sealed class Day24 : AbstractDaySolver<IReadOnlyList<IReadOnlyList<HexDirection>>>
{
    protected override IReadOnlyList<IReadOnlyList<HexDirection>> ParseInput(ILogger logger, string fileContents)
    {
        return fileContents
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(line =>
            {
                var directions = new List<HexDirection>();
                for (var i = 0; i < line.Length; i++)
                {
                    directions.Add(line[i] switch
                    {
                        'e' => HexDirection.East,
                        'w' => HexDirection.West,
                        'n' => line[i + 1] switch
                        {
                            'e' => HexDirection.NorthEast,
                            'w' => HexDirection.NorthWest,
                            _ => throw new ArgumentOutOfRangeException()
                        },
                        's' => line[i + 1] switch
                        {
                            'e' => HexDirection.SouthEast,
                            'w' => HexDirection.SouthWest,
                            _ => throw new ArgumentOutOfRangeException()
                        },
                        _ => throw new ArgumentOutOfRangeException()
                    });
                    if (line[i] == 'n' || line[i] == 's')
                    {
                        i += 1;
                    }
                }

                return directions.AsReadOnly();
            })
            .ToList()
            .AsReadOnly();
    }

    protected override string SolvePart1(ILogger logger, IReadOnlyList<IReadOnlyList<HexDirection>> input)
    {
        var tiles = new Dictionary<HexCoordinate, bool>();
        foreach (var directions in input)
        {
            var position = new HexCoordinate(0, 0, 0);
            position = directions.Aggregate(position, (current, direction) => current.GetNextCoordinate(direction));

            var tile = tiles.GetValueOrDefault(position, false);
            tiles[position] = !tile;
        }

        var total = tiles.Values.Count(tile => tile);
        return total.ToString();
    }

    protected override string SolvePart2(ILogger logger, IReadOnlyList<IReadOnlyList<HexDirection>> input)
    {
        // Get starting grid tile
        // Store only black tiles
        var tiles = new HashSet<HexCoordinate>();
        foreach (var directions in input)
        {
            // Follow directions to tile to flip
            var position = directions.Aggregate(new HexCoordinate(0, 0, 0), (current, direction) => current.GetNextCoordinate(direction));
            // Flip tile by adding it or removing if already existing
            if (!tiles.Add(position))
            {
                tiles.Remove(position);
            }
        }

        // Apply flipping rules
        for (var day = 0; day < 100; day++)
        {
            var nextTiles = new HashSet<HexCoordinate>();
            foreach (var tile in tiles)
            {
                var blackNeighbours = tile.GetNeighbours().Where(tiles.Contains).ToArray();
                // Check neighbours with black rules
                // If zero or more than 2 neighbours are black, flip to white
                // De Morgan's Law: If 1 or 2 neighbours are black, remain black
                if (blackNeighbours.Length is 1 or 2)
                {
                    nextTiles.Add(tile);
                }

                foreach (var neighbour in tile.GetNeighbours())
                {
                    // Check current tile with white rules
                    // If exactly 2 neighbours are black, flip to black
                    if (neighbour.GetNeighbours().Where(tiles.Contains).Count() == 2)
                    {
                        nextTiles.Add(neighbour);
                    }
                }
            }
            tiles = nextTiles;
        }

        return tiles.Count.ToString();
    }
}

internal enum HexDirection
{
    NorthEast,
    NorthWest,
    East,
    West,
    SouthEast,
    SouthWest
}

internal sealed record HexCoordinate
{
    public int Q { get; set; }
    public int R { get; set; }
    public int S { get; set; }

    public HexCoordinate(int q, int r, int s)
    {
        Q = q;
        R = r;
        S = s;
    }

    public override string ToString()
    {
        return $"{Q},{R},{S}";
    }
}

internal static class HexCoordinateExtensions
{
    public static HexCoordinate GetNextCoordinate(this HexCoordinate coordinate, HexDirection direction)
    {
        return direction switch
        {
            HexDirection.NorthEast => new HexCoordinate(coordinate.Q, coordinate.R - 1, coordinate.S + 1),
            HexDirection.NorthWest => new HexCoordinate(coordinate.Q + 1, coordinate.R - 1, coordinate.S),
            HexDirection.East => new HexCoordinate(coordinate.Q - 1, coordinate.R, coordinate.S + 1),
            HexDirection.West => new HexCoordinate(coordinate.Q + 1, coordinate.R, coordinate.S - 1),
            HexDirection.SouthEast => new HexCoordinate(coordinate.Q - 1, coordinate.R + 1, coordinate.S),
            HexDirection.SouthWest => new HexCoordinate(coordinate.Q, coordinate.R + 1, coordinate.S - 1),
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }

    public static IEnumerable<HexCoordinate> GetNeighbours(this HexCoordinate coordinate)
    {
        yield return coordinate.GetNextCoordinate(HexDirection.NorthEast);
        yield return coordinate.GetNextCoordinate(HexDirection.NorthWest);
        yield return coordinate.GetNextCoordinate(HexDirection.East);
        yield return coordinate.GetNextCoordinate(HexDirection.West);
        yield return coordinate.GetNextCoordinate(HexDirection.SouthEast);
        yield return coordinate.GetNextCoordinate(HexDirection.SouthWest);
    }
}