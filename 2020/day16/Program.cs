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

        int lineIndex = 0;

        List<TicketField> ticketFields = new();

        // Parse field ranges
        for (;; lineIndex++)
        {
            string line = lines[lineIndex];
            if (string.IsNullOrEmpty(line))
            {
                break;
            }
            string[] lineParts = line.Split(": ");
            TicketField ticketField = new(lineParts[0]);
            foreach (string range in lineParts[1].Split(" or "))
            {
                string[] rangeParts = range.Split('-');
                ticketField.Ranges.Add((int.Parse(rangeParts[0]), int.Parse(rangeParts[1])));
            }
            ticketFields.Add(ticketField);
        }

        // Parse nearby tickets
        lineIndex += 5;
        for (; lineIndex < lines.Length; lineIndex++)
        {
            string line = lines[lineIndex];
            List<int> numbers = new(Array.ConvertAll<string, int>(line.Split(','), number => int.Parse(number)));
            foreach (int number in numbers)
            {
                bool isValid = false;
                foreach (TicketField ticketField in ticketFields)
                {
                    foreach ((int lower, int upper) in ticketField.Ranges)
                    {
                        if (number >= lower && number <= upper)
                        {
                            isValid = true;
                            break;
                        }
                    }

                    if (isValid)
                    {
                        break;
                    }
                }

                if (!isValid)
                {
                    total += number;
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
}

class TicketField(string name)
{
    public string Name { get; set; } = name;
    public List<(int lower, int upper)> Ranges { get; set; } = [];

    public override string ToString()
    {
        return $"{Name}: {string.Join(" or ", Ranges)}";
    }
}