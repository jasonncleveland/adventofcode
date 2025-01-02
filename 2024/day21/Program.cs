using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

public class Program
{
    static Dictionary<(char startKey, char targetKey, int keyPadIndex), long> cache;

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
        long total = 0;

        List<KeyPad> keyPads = new()
        {
            new NumericalKeyPad(),
        };

        // Add the robot controlled keypads
        for (int i = 0; i < 2; i++)
        {
            keyPads.Add(new DirectionalKeyPad());
        }

        // Initialize the cache
        cache = new();

        foreach (string doorCode in lines)
        {
            int number = (doorCode[0] - '0') * 100 + (doorCode[1] - '0') * 10 + (doorCode[2] - '0');

            char lastKey = 'A';
            long shortestSequenceLength = 0;
            foreach (char key in doorCode)
            {
                shortestSequenceLength += FindMoveSequencesRec(keyPads, 0, lastKey, key);
                lastKey = key;
            }
            total += shortestSequenceLength * number;
        }

        return total;
    }

    static long SolvePart2(string[] lines)
    {
        long total = 0;

        List<KeyPad> keyPads = new()
        {
            new NumericalKeyPad(),
        };

        // Add the robot controlled keypads
        for (int i = 0; i < 25; i++)
        {
            keyPads.Add(new DirectionalKeyPad());
        }

        // Initialize the cache
        cache = new();

        foreach (string doorCode in lines)
        {
            int number = (doorCode[0] - '0') * 100 + (doorCode[1] - '0') * 10 + (doorCode[2] - '0');

            char lastKey = 'A';
            long shortestSequenceLength = 0;
            foreach (char key in doorCode)
            {
                shortestSequenceLength += FindMoveSequencesRec(keyPads, 0, lastKey, key);
                lastKey = key;
            }
            total += shortestSequenceLength * number;
        }

        return total;
    }

    static long FindMoveSequencesRec(List<KeyPad> keyPads, int keyPadIndex, char startKey, char targetKey)
    {
        if (keyPadIndex == keyPads.Count)
        {
            return 1;
        }

        if (cache.ContainsKey((startKey, targetKey, keyPadIndex)))
        {
            return cache[(startKey, targetKey, keyPadIndex)];
        }

        KeyPad keyPad = keyPads[keyPadIndex];

        long shortestSequenceLength = long.MaxValue;
        foreach (List<char> moveSequence in keyPad.GetShortestSequences(startKey, targetKey))
        {
            long sequenceLength = 0;
            char lastKey = 'A';
            foreach (char move in moveSequence)
            {
                long value = FindMoveSequencesRec(keyPads, keyPadIndex + 1, lastKey, move);
                if (value != 0)
                {
                    sequenceLength += value;
                }
                lastKey = move;
            }

            if (sequenceLength < shortestSequenceLength)
            {
                shortestSequenceLength = sequenceLength;
            }
        }

        cache.Add((startKey, targetKey, keyPadIndex), shortestSequenceLength);
        return shortestSequenceLength;
    }
}

enum Direction
{
    Up = '^',
    Down = 'v',
    Left = '<',
    Right = '>'
}

static class DirectionMethods
{
    public static (int row, int column) GetNextPosition(this Direction direction, (int row, int column) position)
    {
        switch (direction)
        {
            case Direction.Up:
                return (position.row - 1, position.column);
            case Direction.Down:
                return (position.row + 1, position.column);
            case Direction.Left:
                return (position.row, position.column - 1);
            case Direction.Right:
                return (position.row, position.column + 1);
            default:
                throw new Exception($"Invalid directiomn given {direction}");
        }
    }
}

internal abstract class KeyPad
{
    protected Dictionary<(int, int), char> _keys;
    protected Dictionary<char, (int, int)> _keyPositions;

    public IEnumerable<List<char>> GetShortestSequences(char startKey, char targetKey)
    {
        Queue<((int, int) position, List<char> moves)> positionsToCheck = new();

        positionsToCheck.Enqueue((_keyPositions[startKey], new()));

        int shortestPathLength = int.MaxValue;

        while (positionsToCheck.Count > 0)
        {
            ((int, int) position, List<char> moves) current = positionsToCheck.Dequeue();

            if (current.moves.Count > shortestPathLength)
            {
                continue;
            }

            if (_keys[current.position] == targetKey)
            {
                shortestPathLength = current.moves.Count;
                current.moves.Add('A');
                yield return current.moves;
                continue;
            }

            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                (int row, int column) next = direction.GetNextPosition(current.position);
                if (_keys.ContainsKey(next))
                {
                    List<char> moves = new(current.moves);
                    moves.Add((char) direction);
                    positionsToCheck.Enqueue((next, moves));
                }
            }
        }
    }
}

internal class NumericalKeyPad : KeyPad
{
    public NumericalKeyPad()
    {
        _keys = new()
        {
            { (0, 0), '7' },
            { (0, 1), '8' },
            { (0, 2), '9' },
            { (1, 0), '4' },
            { (1, 1), '5' },
            { (1, 2), '6' },
            { (2, 0), '1' },
            { (2, 1), '2' },
            { (2, 2), '3' },
            { (3, 1), '0' },
            { (3, 2), 'A' },
        };
        _keyPositions = new()
        {
            { '7', (0, 0) },
            { '8', (0, 1) },
            { '9', (0, 2) },
            { '4', (1, 0) },
            { '5', (1, 1) },
            { '6', (1, 2) },
            { '1', (2, 0) },
            { '2', (2, 1) },
            { '3', (2, 2) },
            { '0', (3, 1) },
            { 'A', (3, 2) },
        };
    }
}

internal class DirectionalKeyPad : KeyPad
{
    public DirectionalKeyPad()
    {
        _keys = new()
        {
            { (0, 1), '^' },
            { (0, 2), 'A' },
            { (1, 0), '<' },
            { (1, 1), 'v' },
            { (1, 2), '>' },
        };
        _keyPositions = new()
        {
            { '^', (0, 1) },
            { 'A', (0, 2) },
            { '<', (1, 0) },
            { 'v', (1, 1) },
            { '>', (1, 2) },
        };
    }
}