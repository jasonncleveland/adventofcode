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

        HashSet<(double, double)> visited = new();

        for (int row = 0; row < lines.Length; row++)
        {
            for (int column = 0; column < lines[row].Length; column++)
            {
                if (!visited.Contains((row, column)))
                {
                    (int area, int perimeter) result = getGroupSizeRec(lines, row, column, visited);
                    total += result.area * result.perimeter;
                }
            }
        }

        return total;
    }

    static long SolvePart2(string[] lines)
    {
        long total = 0;

        HashSet<(double, double)> visited = new();

        for (int row = 0; row < lines.Length; row++)
        {
            for (int column = 0; column < lines[row].Length; column++)
            {
                if (!visited.Contains((row, column)))
                {
                    HashSet<(double, double)> visitedSides = new();
                    (int area, int perimeter) result = getGroupSizeRec(lines, row, column, visitedSides);
                    HashSet<(double, double)> points = new();
                    foreach ((double row, double column) point in visitedSides)
                    {
                        points.Add((point.row - 0.5, point.column - 0.5));
                        points.Add((point.row - 0.5, point.column + 0.5));
                        points.Add((point.row + 0.5, point.column - 0.5));
                        points.Add((point.row + 0.5, point.column + 0.5));
                    }
                    // Find valid points
                    HashSet<(double, double)> validPoints = new();
                    List<Line2d> validLines = new();
                    HashSet<((double, double), (double, double))> checkedPoints = new();
                    foreach ((double row, double column) first in points)
                    {
                        foreach ((double row, double column) second in points)
                        {
                            if (first == second || checkedPoints.Contains((first, second)) || checkedPoints.Contains((second, first))) continue;
                            checkedPoints.Add((first, second));
                            checkedPoints.Add((second, first));
                            double rowDelta = second.row - first.row;
                            double columnDelta = second.column - first.column;
                            if (Math.Abs(rowDelta) + Math.Abs(columnDelta) == 1)
                            {
                                if (rowDelta != 0)
                                {
                                    if (visitedSides.Contains((second.row - rowDelta / 2, second.column - 0.5)) &&
                                        visitedSides.Contains((second.row - rowDelta / 2, second.column + 0.5)))
                                    {
                                        // Line crosses an internal point
                                        continue;
                                    }
                                    if (!visitedSides.Contains((second.row - rowDelta / 2, second.column - 0.5)) &&
                                        !visitedSides.Contains((second.row - rowDelta / 2, second.column + 0.5)))
                                    {
                                        // Line is between two outside points
                                        continue;
                                    }
                                }
                                else if (columnDelta != 0)
                                {
                                    if (visitedSides.Contains((second.row - 0.5, second.column - columnDelta / 2)) &&
                                        visitedSides.Contains((second.row + 0.5, second.column - columnDelta / 2)))
                                    {
                                        // Line crosses an internal point
                                        continue;
                                    }
                                    if (!visitedSides.Contains((second.row - 0.5, second.column - columnDelta / 2)) &&
                                        !visitedSides.Contains((second.row + 0.5, second.column - columnDelta / 2)))
                                    {
                                        // Line is between two outside points
                                        continue;
                                    }
                                }
                                validPoints.Add(first);
                                validPoints.Add(second);
                                Line2d line = new Line2d(first, second);
                                validLines.Add(line);
                            }
                        }
                    }

                    // Find angles
                    HashSet<(double row, double column, double angle)> uniqueAngles = new();
                    Dictionary<(double, double, double), int> visitedAnglesMap = new();
                    HashSet<(Line2d, Line2d)> checkedLines = new();
                    foreach (Line2d first in validLines)
                    {
                        foreach (Line2d second in validLines)
                        {
                            if (first == second) continue;
                            checkedLines.Add((first, second));
                            checkedLines.Add((second, first));
                            HashSet<(double, double)> uniquePoints = new();
                            uniquePoints.Add(first.First);
                            uniquePoints.Add(first.Second);
                            uniquePoints.Add(second.First);
                            uniquePoints.Add(second.Second);
                            if (uniquePoints.Count != 3) continue;
                            (double row, double column) p1;
                            (double row, double column) p2;
                            (double row, double column) p3;
                            if (first.First == second.First)
                            {
                                p1 = first.First;
                                p2 = first.Second;
                                p3 = second.Second;
                            }
                            else if (first.First == second.Second)
                            {
                                p1 = first.First;
                                p2 = first.Second;
                                p3 = second.First;
                            }
                            else if (first.Second == second.Second)
                            {
                                p1 = first.Second;
                                p2 = first.First;
                                p3 = second.First;
                            }
                            else if (first.Second == second.First)
                            {
                                p1 = first.Second;
                                p2 = first.First;
                                p3 = second.Second;
                            }
                            else
                            {
                                throw new Exception($"Should not be here");
                            }
                            double angleInRadians = Math.Atan2(p3.row - p1.row, p3.column - p1.column) - Math.Atan2(p2.row - p1.row, p2.column - p1.column);
                            double angleInDegrees = (180 / Math.PI) * angleInRadians;
                            if (Math.Abs(angleInDegrees) == 90 || Math.Abs(angleInDegrees) == 270)
                            {
                                uniqueAngles.Add((p1.row, p1.column, angleInDegrees));
                            }
                        }
                    }
                    visited.UnionWith(visitedSides);
                    total += result.area * (uniqueAngles.Count / 2);
                }
            }
        }

        return total;
    }

    static (int area, int perimeter) getGroupSizeRec(string[] lines, int row, int column, HashSet<(double, double)> visited)
    {
        visited.Add((row, column));

        char label = lines[row][column];

        int area = 1;
        int perimeter = 0;
        foreach ((int nextRow, int nextColumn) in getNeighbours(row, column))
        {
            if (nextRow >= 0 && nextRow < lines.Length && nextColumn >= 0 && nextColumn < lines[nextRow].Length)
            {
                if (label != lines[nextRow][nextColumn])
                {
                    // Increase the perimeter when we touch a group that doesn't have the same label
                    perimeter++;
                }
                else
                {
                    // Recursively check neighbours to calculate area and perimeter
                    if (!visited.Contains((nextRow, nextColumn)))
                    {
                        visited.Add((nextRow, nextColumn));
                        (int area, int perimeter) result = getGroupSizeRec(lines, nextRow, nextColumn, visited);
                        area += result.area;
                        perimeter += result.perimeter;
                    }
                }
            }
            else
            {
                // Perimeter should still increase when on the outside edges
                perimeter++;
            }
        }

        return (area, perimeter);
    }

    static IEnumerable<(int, int)> getNeighbours(int row, int column)
    {
        yield return (row - 1, column);
        yield return (row + 1, column);
        yield return (row, column - 1);
        yield return (row, column + 1);
    }
}

public class Line2d
{
    public (double row, double column) First { get; set; }
    public (double row, double column) Second { get; set; }

    public Line2d((double x, double y) first, (double x, double y) second)
    {
        First = first;
        Second = second;
    }

    public override string ToString()
    {
        return $"{First} <-> {Second}";
    }
}