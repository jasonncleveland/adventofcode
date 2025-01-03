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
        int x = 0;
        int y = 0;

        Direction direction = Direction.East;

        foreach (string line in lines)
        {
            char operation = line[0];
            int number = int.Parse(line.Substring(1));

            switch (operation)
            {
                case 'N':
                    y -= number;
                    break;
                case 'S':
                    y += number;
                    break;
                case 'W':
                    x -= number;
                    break;
                case 'E':
                    x += number;
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
                    x += delta.x * number;
                    y += delta.y * number;
                    break;
                default:
                    throw new Exception($"Found invalid operation {operation}");
            }
        }

        return Math.Abs(x) + Math.Abs(y);
    }

    static long SolvePart2(string[] lines)
    {
        long total = 0;

        // TODO: Implement logic to solve part 2

        return total;
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