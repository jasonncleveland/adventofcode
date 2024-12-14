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
            int width = args.Length > 1 ? int.Parse(args[1]) : 101;
            int height = args.Length > 2 ? int.Parse(args[2]) : 103;
            if (File.Exists(fileName))
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                string[] lines = File.ReadAllLines(fileName);
                stopWatch.Stop();
                Console.WriteLine($"File read ({stopWatch.Elapsed.TotalMilliseconds} ms)");

                Stopwatch part1Timer = new Stopwatch();
                part1Timer.Start();
                long part1 = SolvePart1(lines, width, height);
                part1Timer.Stop();
                Console.WriteLine($"Part 1: {part1} ({part1Timer.Elapsed.TotalMilliseconds} ms)");

                Stopwatch part2Timer = new Stopwatch();
                part2Timer.Start();
                long part2 = SolvePart2(lines, width, height);
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

    static long SolvePart1(string[] lines, long width, long height)
    {
        List<Robot> robots = parseInput(lines);

        return calculateSafetyFactor(robots, width, height, 100);
    }

    static long SolvePart2(string[] lines, long width, long height)
    {
        List<Robot> robots = parseInput(lines);

        // The grid will cycle back to its original state after width * height states
        for (int seconds = 1; seconds <= width * height; seconds++)
        {
            bool result = simulateAndCheckForTree(robots, width, height, seconds);
            if (result)
            {
                return seconds;
            }
        }

        return -1;
    }

    static List<Robot> parseInput(string[] lines)
    {
        List<Robot> robots = new();

        foreach (string line in lines)
        {
            string[] lineParts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string[] positionParts = lineParts[0].Split('=')[1].Split(',');
            string[] velocityParts = lineParts[1].Split('=')[1].Split(',');
            long startX = long.Parse(positionParts[0]);
            long startY = long.Parse(positionParts[1]);
            long deltaX = long.Parse(velocityParts[0]);
            long deltaY = long.Parse(velocityParts[1]);
            robots.Add(new Robot(startX, startY, deltaX, deltaY));
        }

        return robots;
    }

    static long calculateSafetyFactor(List<Robot> robots, long width, long height, long seconds)
    {
        long midX = width / 2;
        long midY = height / 2;

        long quadrant1 = 0;
        long quadrant2 = 0;
        long quadrant3 = 0;
        long quadrant4 = 0;

        foreach (Robot robot in robots)
        {
            long endX = (robot.X + (robot.DX * seconds)) % width;
            if (endX < 0)
            {
                endX = width + endX;
            }
            long endY = (robot.Y + (robot.DY * seconds)) % height;
            if (endY < 0)
            {
                endY = height + endY;
            }

            if (endX < midX && endY < midY)
            {
                quadrant1++;
            }
            else if (endX > midX && endY < midY)
            {
                quadrant2++;
            }
            else if (endX < midX && endY > midY)
            {
                quadrant3++;
            }
            else if (endX > midX && endY > midY)
            {
                quadrant4++;
            }
        }

        return quadrant1 * quadrant2 * quadrant3 * quadrant4;
    }

    static bool simulateAndCheckForTree(List<Robot> robots, long width, long height, long seconds)
    {
        Dictionary<long, long> robotsByRow = new();
        Dictionary<long, long> robotsByColumn = new();

        foreach (Robot robot in robots)
        {
            long endX = (robot.X + (robot.DX * seconds)) % width;
            if (endX < 0)
            {
                endX = width + endX;
            }
            long endY = (robot.Y + (robot.DY * seconds)) % height;
            if (endY < 0)
            {
                endY = height + endY;
            }

            if (!robotsByRow.ContainsKey(endY))
            {
                robotsByRow.Add(endY, 0);
            }
            robotsByRow[endY] += 1;

            if (!robotsByColumn.ContainsKey(endX))
            {
                robotsByColumn.Add(endX, 0);
            }
            robotsByColumn[endX] += 1;
        }

        int rowCount = 0;
        int columnCount = 0;
        foreach (long rowValue in robotsByRow.Values)
        {
            if (rowValue >= 31)
            {
                rowCount++;
            }
        }
        foreach (long columnValue in robotsByColumn.Values)
        {
            if (columnValue >= 33)
            {
                columnCount++;
            }
        }

        // If we have 2 rows with at least 31 robots and 2 columns with at least 33 robots then we have found the tree
        return rowCount >= 2 && columnCount >= 2;
    }
}

class Robot(long x, long y, long dx, long dy)
{
    public long X = x;
    public long Y = y;
    public long DX = dx;
    public long DY = dy;
}