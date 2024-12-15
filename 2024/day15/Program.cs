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
        long total = 0;

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

        List<string> instructionsList = new();
        for (int row = splitIndex + 1; row < lines.Length; row++)
        {
            instructionsList.Add(lines[row]);
        }
        string instructions = string.Join("", instructionsList);

        foreach (char instruction in instructions)
        {
            switch (instruction)
            {
                case '^':
                {
                    (int row, int column) next = (robot.row - 1, robot.column);
                    if (inBounds(grid, next))
                    {
                        if (grid[next.row][next.column] == 'O')
                        {
                            // Attempt to move box
                            bool result = moveBox(grid, next, (-1, 0));
                            if (result)
                            {
                                grid[robot.row][robot.column] = '.';
                                robot.row = next.row;
                                grid[robot.row][robot.column] = '@';
                            }
                        }
                        else
                        {
                            grid[robot.row][robot.column] = '.';
                            robot.row = next.row;
                            grid[robot.row][robot.column] = '@';
                        }
                    }
                    break;
                }
                case 'v':
                {
                    (int row, int column) next = (robot.row + 1, robot.column);
                    if (inBounds(grid, next))
                    {
                        if (grid[next.row][next.column] == 'O')
                        {
                            // Attempt to move box
                            bool result = moveBox(grid, next, (1, 0));
                            if (result)
                            {
                                grid[robot.row][robot.column] = '.';
                                robot.row = next.row;
                                grid[robot.row][robot.column] = '@';
                            }
                        }
                        else
                        {
                            grid[robot.row][robot.column] = '.';
                            robot.row = next.row;
                            grid[robot.row][robot.column] = '@';
                        }
                    }
                    break;
                }
                case '<':
                {
                    (int row, int column) next = (robot.row, robot.column - 1);
                    if (inBounds(grid, next))
                    {
                        if (grid[next.row][next.column] == 'O')
                        {
                            // Attempt to move box
                            bool result = moveBox(grid, next, (0, -1));
                            if (result)
                            {
                                grid[robot.row][robot.column] = '.';
                                robot.column = next.column;
                                grid[robot.row][robot.column] = '@';
                            }
                        }
                        else
                        {
                            grid[robot.row][robot.column] = '.';
                            robot.column = next.column;
                            grid[robot.row][robot.column] = '@';
                        }
                    }
                    break;
                }
                case '>':
                {
                    (int row, int column) next = (robot.row, robot.column + 1);
                    if (inBounds(grid, next))
                    {
                        if (grid[next.row][next.column] == 'O')
                        {
                            // Attempt to move box
                            bool result = moveBox(grid, next, (0, 1));
                            if (result)
                            {
                                grid[robot.row][robot.column] = '.';
                                robot.column = next.column;
                                grid[robot.row][robot.column] = '@';
                            }
                        }
                        else
                        {
                            grid[robot.row][robot.column] = '.';
                            robot.column = next.column;
                            grid[robot.row][robot.column] = '@';
                        }
                    }
                    break;
                }
            }
        }

        for (int row = 0; row < grid.Count; row++)
        {
            for (int column = 0; column < grid[row].Count; column++)
            {
                if (grid[row][column] == 'O')
                {
                    total += 100 * row + column;
                }
            }
        }

        return total;
    }

    static long SolvePart2(string[] lines)
    {
        long total = 0;

        // TODO: Implement logic to solve part 2

        return total;
    }

    static void printGrid(List<List<char>> grid)
    {
        foreach (List<char> row in grid)
        {
            foreach (char item in row)
            {
                Console.Write(item);
            }
            Console.WriteLine();
        }
    }

    static bool inBounds(List<List<char>> grid, (int row, int column) position)
    {
        if (position.row < 0 || position.row > grid.Count - 1 || position.column < 0 || position.column > grid[position.row].Count)
        {
            return false;
        }
        return grid[position.row][position.column] != '#';
    }

    static bool moveBox(List<List<char>> grid, (int row, int column) current, (int row, int column) delta)
    {
        // Multiple boxes are moved so need to recurse
        (int row, int column) next = (current.row + delta.row, current.column + delta.column);
        if (!inBounds(grid, next))
        {
            return false;
        }

        if (grid[next.row][next.column] == 'O')
        {
            bool result = moveBox(grid, next, delta);
            if (!result)
            {
                return false;
            }
        }

        grid[current.row][current.column] = '.';
        grid[next.row][next.column] = 'O';

        return true;
    }
}