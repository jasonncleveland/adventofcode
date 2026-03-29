using System;

using AdventOfCode.Shared.Directions;

namespace AdventOfCode.Shared.Coordinates;

public class Point2d(long x, long y)
{
    public long X { get; set; } = x;
    public long Y { get; set; } = y;

    public override string ToString()
    {
        return $"({X}, {Y})";
    }
}

public static class Point2dExtensions
{
    public static Point2d GetNextPosition(this Point2d current, Direction direction, long distance = 1)
    {
        return direction switch
        {
            Direction.Up or Direction.North => new Point2d(current.X, current.Y - distance),
            Direction.Down or Direction.South => new Point2d(current.X, current.Y + distance),
            Direction.Left or Direction.West => new Point2d(current.X - distance, current.Y),
            Direction.Right or Direction.East => new Point2d(current.X + distance, current.Y),
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, "invalid direction")
        };
    }
}