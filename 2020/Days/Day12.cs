using System;
using System.Collections.Generic;
using System.Linq;

using AdventOfCode.Shared.Coordinates;
using AdventOfCode.Shared.Directions;
using AdventOfCode.Shared.Solver;

using Microsoft.Extensions.Logging;

namespace AdventOfCode.Y2020.Days;

internal sealed class Day12 : AbstractDaySolver<IReadOnlyList<(Direction direction, int distance)>>
{
    protected override IReadOnlyList<(Direction direction, int distance)> ParseInput(ILogger logger, string fileContents)
    {
        return fileContents
            .Split("\n", StringSplitOptions.RemoveEmptyEntries)
            .Select(line =>
            {
                var direction = line[0] switch
                {
                    'N' => Direction.North,
                    'S' => Direction.South,
                    'E' => Direction.East,
                    'W' => Direction.West,
                    'L' => Direction.Left,
                    'R' => Direction.Right,
                    'F' => Direction.Forward,
                    _ => throw new ArgumentOutOfRangeException()
                };
                var distance = int.Parse(line[1..]);
                return (direction, distance);
            })
            .ToList()
            .AsReadOnly();
    }

    protected override string SolvePart1(ILogger logger, IReadOnlyList<(Direction direction, int distance)> operations)
    {
        var shipPosition = new Point2d(0, 0);
        var currentDirection = Direction.East;

        foreach ((Direction direction, int distance) in operations)
        {
            switch (direction)
            {
                case Direction.North:
                    shipPosition.Y -= distance;
                    break;
                case Direction.South:
                    shipPosition.Y += distance;
                    break;
                case Direction.West:
                    shipPosition.X -= distance;
                    break;
                case Direction.East:
                    shipPosition.X += distance;
                    break;
                case Direction.Left:
                case Direction.Right:
                    for (int i = 0; i < distance / 90; i++)
                    {
                        currentDirection = currentDirection.GetNextDirection(direction);
                    }
                    break;
                case Direction.Forward:
                    shipPosition = shipPosition.GetNextPosition(currentDirection, distance);
                    break;
                default:
                    throw new Exception($"Found invalid direction {direction}");
            }
        }

        return (Math.Abs(shipPosition.X) + Math.Abs(shipPosition.Y)).ToString();
    }

    protected override string SolvePart2(ILogger logger, IReadOnlyList<(Direction direction, int distance)> operations)
    {
        var shipPosition = new Point2d(0, 0);
        var waypointPosition = new Point2d(10, -1);

        foreach ((Direction direction, int distance) in operations)
        {
            long deltaX = waypointPosition.X - shipPosition.X;
            long deltaY = waypointPosition.Y - shipPosition.Y;
            switch (direction)
            {
                case Direction.North:
                    waypointPosition.Y -= distance;
                    break;
                case Direction.South:
                    waypointPosition.Y += distance;
                    break;
                case Direction.West:
                    waypointPosition.X -= distance;
                    break;
                case Direction.East:
                    waypointPosition.X += distance;
                    break;
                case Direction.Left:
                case Direction.Right:
                    // Calculate and perform the correct number of 90 degree rotations
                    for (int i = 0; i < distance / 90; i++)
                    {
                        // When rotating, the x and y coordinates swap positions
                        if (direction == Direction.Left)
                        {
                            // When rotating left, the y becomes the negated x
                            long tmp = deltaX * -1;
                            deltaX = deltaY;
                            deltaY = tmp;
                        }
                        else
                        {
                            // When rotating left, the x becomes the negated y
                            long tmp = deltaX;
                            deltaX = deltaY * -1;
                            deltaY = tmp;
                        }
                    }
                    waypointPosition.X = shipPosition.X + deltaX;
                    waypointPosition.Y = shipPosition.Y + deltaY;
                    break;
                case Direction.Forward:
                    // Move the waypoint the given number of times
                    waypointPosition.X += deltaX * distance;
                    waypointPosition.Y += deltaY * distance;

                    // Move the ship to the waypoint minus the delta
                    shipPosition.X = waypointPosition.X - deltaX;
                    shipPosition.Y = waypointPosition.Y - deltaY;
                    break;
                default:
                    throw new Exception($"Found invalid direction {direction}");
            }
        }

        return (Math.Abs(shipPosition.X) + Math.Abs(shipPosition.Y)).ToString();
    }
}