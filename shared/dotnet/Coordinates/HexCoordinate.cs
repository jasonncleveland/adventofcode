using System;
using System.Collections.Generic;

using AdventOfCode.Shared.Directions;

namespace AdventOfCode.Shared.Coordinates;

public record HexCoordinate(int Q, int R, int S)
{
    public override string ToString()
    {
        return $"{Q},{R},{S}";
    }
}

public static class HexCoordinateExtensions
{
    extension(HexCoordinate coordinate)
    {
        public HexCoordinate GetNextCoordinate(HexDirection direction)
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

        public IEnumerable<HexCoordinate> GetNeighbours()
        {
            yield return coordinate.GetNextCoordinate(HexDirection.NorthEast);
            yield return coordinate.GetNextCoordinate(HexDirection.NorthWest);
            yield return coordinate.GetNextCoordinate(HexDirection.East);
            yield return coordinate.GetNextCoordinate(HexDirection.West);
            yield return coordinate.GetNextCoordinate(HexDirection.SouthEast);
            yield return coordinate.GetNextCoordinate(HexDirection.SouthWest);
        }
    }
}