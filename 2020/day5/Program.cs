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
        long max = long.MinValue;

        foreach (string line in lines)
        {
            (long seatColumn, long seatRow) = parseBoardingPass(line);
            long seatId = seatColumn * 8 + seatRow;
            if (seatId > max)
            {
                max = seatId;
            }
        }

        return max;
    }

    static long SolvePart2(string[] lines)
    {
        long total = 0;

        // TODO: Implement logic to solve part 2

        return total;
    }

    static (long, long) parseBoardingPass(string boardingPass)
    {
        int columnCount = 128;
        int columnLowerBound = 0;
        int columnUpperBound = columnCount - 1;
        int rowCount = 8;
        int rowLowerBound = 0;
        int rowUpperBound = rowCount - 1;
        foreach (char character in boardingPass)
        {
            switch (character)
            {
                case 'F':
                    // Take the lower half of the column range
                    columnCount /= 2;
                    columnUpperBound -= columnCount;
                    break;
                case 'B':
                    // Take the upper half of the column range
                    columnCount /= 2;
                    columnLowerBound += columnCount;
                    break;
                case 'L':
                    // Take the lower half of the row range
                    rowCount /= 2;
                    rowUpperBound -= rowCount;
                    break;
                case 'R':
                    // Take the upper half of the row range
                    rowCount /= 2;
                    rowLowerBound += rowCount;
                    break;
                default:
                    throw new Exception($"Invalid letter received: '{character}'");
            }
        }

        // The upper and lower bounds must be equal at the end of the processing
        if (columnLowerBound != columnUpperBound || rowLowerBound != rowUpperBound)
        {
            throw new Exception($"Error processing boarding pass {boardingPass}. First 5 characters must be B or F and final 3 must be L or R");
        }
        return (columnLowerBound, rowLowerBound);
    }
}