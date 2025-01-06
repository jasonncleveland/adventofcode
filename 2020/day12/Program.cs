using System;
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
        CartesianPoint shipPosition = new(0, 0);
        Direction direction = Direction.East;

        foreach (string line in lines)
        {
            char operation = line[0];
            int number = int.Parse(line.Substring(1));

            switch (operation)
            {
                case 'N':
                    shipPosition.Y -= number;
                    break;
                case 'S':
                    shipPosition.Y += number;
                    break;
                case 'W':
                    shipPosition.X -= number;
                    break;
                case 'E':
                    shipPosition.X += number;
                    break;
                case 'L':
                case 'R':
                    for (int i = 0; i < number / 90; i++)
                    {
                        direction = direction.GetNextDirection(operation);
                    }
                    break;
                case 'F':
                    (int x, int y) delta = direction.GetDelta();
                    shipPosition.X += delta.x * number;
                    shipPosition.Y += delta.y * number;
                    break;
                default:
                    throw new Exception($"Found invalid operation {operation}");
            }
        }

        return Math.Abs(shipPosition.X) + Math.Abs(shipPosition.Y);
    }

    static long SolvePart2(string[] lines)
    {
        CartesianPoint shipPosition = new(0, 0);
        CartesianPoint waypointPosition = new(10, -1);

        foreach (string line in lines)
        {
            char operation = line[0];
            int number = int.Parse(line.Substring(1));

            long deltaX = waypointPosition.X - shipPosition.X;
            long deltaY = waypointPosition.Y - shipPosition.Y;
            switch (operation)
            {
                case 'N':
                    waypointPosition.Y -= number;
                    break;
                case 'S':
                    waypointPosition.Y += number;
                    break;
                case 'W':
                    waypointPosition.X -= number;
                    break;
                case 'E':
                    waypointPosition.X += number;
                    break;
                case 'L':
                case 'R':
                    // Calculate and perform the correct number of 90 degree rotations
                    for (int i = 0; i < number / 90; i++)
                    {
                        // When rotating, the x and y coordinates swap positions
                        if (operation == 'L')
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
                case 'F':
                    // Move the waypoint the given number of times
                    waypointPosition.X += deltaX * number;
                    waypointPosition.Y += deltaY * number;

                    // Move the ship to the waypoint minus the delta
                    shipPosition.X = waypointPosition.X - deltaX;
                    shipPosition.Y = waypointPosition.Y - deltaY;
                    break;
                default:
                    throw new Exception($"Found invalid operation {operation}");
            }
        }

        return Math.Abs(shipPosition.X) + Math.Abs(shipPosition.Y);
    }
}

class CartesianPoint(long x, long y)
{
    public long X { get; set; } = x;
    public long Y { get; set; } = y;

    public override string ToString()
    {
        return $"({X}, {Y})";
    }
}

enum Direction
{
    North,
    South,
    West,
    East
}

static class DirectionMethods
{
    public static (int x, int y) GetDelta(this Direction direction)
    {
        switch (direction)
        {
            case Direction.North:
                return (0, -1);
            case Direction.South:
                return (0, 1);
            case Direction.West:
                return (-1, 0);
            case Direction.East:
                return (1, 0);
            default:
                throw new Exception($"Invalid directiomn given {direction}");
        }
    }

    public static Direction GetNextDirection(this Direction direction, char operation)
    {
        switch (direction)
        {
            case Direction.North:
                return operation == 'L' ? Direction.West : Direction.East;
            case Direction.South:
                return operation == 'L' ? Direction.East : Direction.West;
            case Direction.West:
                return operation == 'L' ? Direction.South : Direction.North;
            case Direction.East:
                return operation == 'L' ? Direction.North : Direction.South;
            default:
                throw new Exception($"Invalid directiomn given {direction}");
        }
    }
}