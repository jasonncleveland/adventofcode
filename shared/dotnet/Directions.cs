using System;

namespace AdventOfCode.Shared.Directions;

public enum Direction
{
    Up,
    North,
    Down,
    South,
    Left,
    West,
    Right,
    East,
    Forward
}

public static class DirectionExtensions
{
    public static Direction GetNextDirection(this Direction direction, Direction turn)
    {
        return turn switch
        {
            Direction.Left => direction switch
            {
                Direction.Up => Direction.Left,
                Direction.North => Direction.West,
                Direction.Down => Direction.Right,
                Direction.South => Direction.East,
                Direction.Left => Direction.Down,
                Direction.West => Direction.South,
                Direction.Right => Direction.Up,
                Direction.East => Direction.North,
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, "invalid direction")
            },
            Direction.Right => direction switch
            {
                Direction.Up => Direction.Right,
                Direction.North => Direction.East,
                Direction.Down => Direction.Left,
                Direction.South => Direction.West,
                Direction.Left => Direction.Up,
                Direction.West => Direction.North,
                Direction.Right => Direction.Down,
                Direction.East => Direction.South,
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, "invalid direction")
            },
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, "invalid turn direction")
        };
    }
}