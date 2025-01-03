using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

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
                long part1 = SolvePart1(lines[0]);
                part1Timer.Stop();
                Console.WriteLine($"Part 1: {part1} ({part1Timer.Elapsed.TotalMilliseconds} ms)");
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

    static long SolvePart1(string line)
    {
        string pattern = @"row (?<row>\d+), column (?<column>\d+)";
        Regex regex = new Regex(pattern, RegexOptions.Compiled);

        Match match = regex.Match(line);
        int targetRow = int.Parse(match.Groups["row"].Value);
        int targetColumn = int.Parse(match.Groups["column"].Value);

        long firstCode = 20151125;

        int row = 1;
        int column = 1;
        int maxRow = 1;

        long previousCode = firstCode;

        while (true)
        {
            if (row == targetRow && column == targetColumn)
            {
                return previousCode;
            }

            // The next position is up and right from the current position
            int nextRow = row - 1;
            int nextColumn = column + 1;
            if (nextRow == 0)
            {
                // We have reached the top, reset to the next unchecked row and the first column
                maxRow += 1;
                nextRow = maxRow;
                nextColumn = 1;
            }

            // Calculate the next code
            long nextCode = (previousCode * 252533) % 33554393;
            previousCode = nextCode;

            row = nextRow;
            column = nextColumn;
        }
    }
}