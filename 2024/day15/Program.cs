using System;
using System.Collections.Generic;
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
        int splitIndex = Array.FindIndex(lines, line => string.IsNullOrEmpty(line));

        (int row, int column) robot = (0, 0);
        List<List<char>> grid = new();
        for (int row = 0; row < splitIndex; row++)
        {
            List<char> line = new();
            for (int column = 0; column < lines[row].Length; column++)
            {
                if (lines[row][column] == '@')
                {
                    robot = (row, column);
                }

                line.Add(lines[row][column]);
            }
            grid.Add(line);
        }

        string instructions = ParseInstructions(lines, splitIndex + 1);
        ProcessInstructions(grid, robot, instructions, ['O'], MoveSingleBox);

        return CalculateTotal(grid, 'O');
    }

    static long SolvePart2(string[] lines)
    {
        int splitIndex = Array.FindIndex(lines, line => string.IsNullOrEmpty(line));

        (int row, int column) robot = (0, 0);
        List<List<char>> grid = new();
        for (int row = 0; row < splitIndex; row++)
        {
            List<char> line = new();
            for (int column = 0; column < lines[row].Length; column++)
            {
                switch (lines[row][column])
                {
                    case '#':
                        line.Add('#');
                        line.Add('#');
                        break;
                    case 'O':
                        line.Add('[');
                        line.Add(']');
                        break;
                    case '.':
                        line.Add('.');
                        line.Add('.');
                        break;
                    case '@':
                        line.Add('@');
                        line.Add('.');
                        robot = (row, column * 2);
                        break;
                }
            }
            grid.Add(line);
        }

        string instructions = ParseInstructions(lines, splitIndex + 1);
        ProcessInstructions(grid, robot, instructions, ['[', ']'], MoveDoubleBox);

        return CalculateTotal(grid, '[');
    }

    static void PrintGrid(List<List<char>> grid)
    {
        foreach (List<char> row in grid)
        {
            foreach (char item in row)
            {
                if (item == '@')
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                else if (item == '[' || item == ']')
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                }
                else if (item == '#')
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                }
                Console.Write(item);
                Console.ForegroundColor = ConsoleColor.White;
            }
            Console.WriteLine();
        }
    }

    static string ParseInstructions(string[] lines, int startIndex)
    {
        List<string> instructionsList = new();
        for (int row = startIndex; row < lines.Length; row++)
        {
            instructionsList.Add(lines[row]);
        }
        return string.Join("", instructionsList);
    }

    static bool InBounds(List<List<char>> grid, (int row, int column) position)
    {
        if (position.row < 0 || position.row > grid.Count - 1 || position.column < 0 || position.column > grid[position.row].Count)
        {
            return false;
        }
        return grid[position.row][position.column] != '#';
    }

    static bool IsBox(List<List<char>> grid, (int row, int column) position, List<char> boxes)
    {
        foreach (char box in boxes)
        {
            if (grid[position.row][position.column] == box)
            {
                return true;
            }
        }
        return false;
    }

    static void ProcessInstructions(List<List<char>> grid, (int row, int column) robot, string instructions, List<char> boxCharacters, MoveBox moveBoxFunc)
    {
        foreach (char instruction in instructions)
        {
            (int row, int column) next = (robot.row, robot.column);
            (int row, int column) delta = (0, 0);
            switch (instruction)
            {
                case '^':
                    next = (robot.row - 1, robot.column);
                    delta = (-1, 0);
                    break;
                case 'v':
                    next = (robot.row + 1, robot.column);
                    delta = (1, 0);
                    break;
                case '<':
                    next = (robot.row, robot.column - 1);
                    delta = (0, -1);
                    break;
                case '>':
                    next = (robot.row, robot.column + 1);
                    delta = (0, 1);
                    break;
                default:
                    throw new Exception($"Invalid direction given: {instruction}");
            }
            if (InBounds(grid, next))
            {
                if (IsBox(grid, next, boxCharacters))
                {
                    // Attempt to move box
                    bool dryRunSuccess = moveBoxFunc(grid, next, delta, true);
                    if (dryRunSuccess)
                    {
                        // Only perform the move if the dry run succeeds and all applicable boxes can be moved
                        moveBoxFunc(grid, next, delta);
                        grid[robot.row][robot.column] = '.';
                        robot.row = next.row;
                        robot.column = next.column;
                        grid[robot.row][robot.column] = '@';
                    }
                }
                else
                {
                    grid[robot.row][robot.column] = '.';
                    robot.row = next.row;
                    robot.column = next.column;
                    grid[robot.row][robot.column] = '@';
                }
            }
        }
    }

    delegate bool MoveBox(List<List<char>> grid, (int row, int column) current, (int row, int column) delta, bool dryRun = false);

    static bool MoveSingleBox(List<List<char>> grid, (int row, int column) current, (int row, int column) delta, bool dryRun = false)
    {
        // Multiple boxes are moved so need to recurse
        (int row, int column) next = (current.row + delta.row, current.column + delta.column);
        if (!InBounds(grid, next))
        {
            return false;
        }

        if (grid[next.row][next.column] == 'O')
        {
            bool result = MoveSingleBox(grid, next, delta);
            if (!result)
            {
                return false;
            }
        }

        if (!dryRun)
        {
            grid[current.row][current.column] = '.';
            grid[next.row][next.column] = 'O';
        }

        return true;
    }

    static bool MoveDoubleBox(List<List<char>> grid, (int row, int column) current, (int row, int column) delta, bool dryRun = false)
    {
        // Multiple boxes can be moved so need to recurse
        (int row, int column) next = (current.row + delta.row, current.column + delta.column);
        if (!InBounds(grid, next))
        {
            return false;
        }

        (int row, int column) left = (0, 0);
        (int row, int column) right = (0, 0);
        // We need to move both sides of the box so get the position of the other half
        if (grid[current.row][current.column] == '[')
        {
            left = current;
            right = (current.row, current.column + 1);
        }
        else if (grid[current.row][current.column] == ']')
        {
            right = current;
            left = (current.row, current.column - 1);
        }

        // Check if the next posiitions for the box parts are valid
        (int row, int column) nextLeft = (left.row + delta.row, left.column + delta.column);
        (int row, int column) nextRight = (right.row + delta.row, right.column + delta.column);
        if (!InBounds(grid, nextLeft) || !InBounds(grid, nextRight))
        {
            return false;
        }

        // Attempt to move the box parts
        if (delta.row != 0)
        {
            // We are moving vertically so push both
            bool result = true;
        
            // If the next row contains another box, recursively move them
            if (grid[nextLeft.row][nextLeft.column] == '[' || grid[nextLeft.row][nextLeft.column] == ']')
            {
                result = result && MoveDoubleBox(grid, nextLeft, delta, dryRun);
            }
            if (grid[nextRight.row][nextRight.column] == '[' || grid[nextRight.row][nextRight.column] == ']')
            {
                result = result && MoveDoubleBox(grid, nextRight, delta, dryRun);
            }
            if (!result)
            {
                return false;
            }
        }
        else if (delta.column != 0)
        {
            // We are moving horizontally so only check the leading edge of the box
            if (delta.column < 0 && grid[nextLeft.row][nextLeft.column] == ']')
            {
                // We are moving to the left so check if the next position for the left edge of the box is valid
                bool result = MoveDoubleBox(grid, nextLeft, delta, dryRun);
                if (!result)
                {
                    return false;
                }
            }
            else if (delta.column > 0 && grid[nextRight.row][nextRight.column] == '[')
            {
                // We are moving to the right so check if the next position for the right edge of the box is valid
                bool result = MoveDoubleBox(grid, nextRight, delta, dryRun);
                if (!result)
                {
                    return false;
                }
            }
        }

        if (!dryRun)
        {
            grid[left.row][left.column] = '.';
            grid[right.row][right.column] = '.';
            grid[nextLeft.row][nextLeft.column] = '[';
            grid[nextRight.row][nextRight.column] = ']';
        }

        return true;
    }

    static long CalculateTotal(List<List<char>> grid, char box)
    {
        long total = 0;

        for (int row = 0; row < grid.Count; row++)
        {
            for (int column = 0; column < grid[row].Count; column++)
            {
                if (grid[row][column] == box)
                {
                    total += 100 * row + column;
                }
            }
        }

        return total;
    }
}