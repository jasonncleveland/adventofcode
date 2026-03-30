using System.Diagnostics;

using AdventOfCode.Shared.IO;
using AdventOfCode.Y2020.Days;

using Microsoft.Extensions.Logging;

namespace AdventOfCode.Y2020;

public sealed class Program
{
    public static void Main(string[] args)
    {
        using var factory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = factory.CreateLogger("Program");

        int? selectedDay = null;
        if (args.Length > 0)
        {
            selectedDay = int.Parse(args[0]);
        }

        if (selectedDay is not null)
        {
            RunSingleDay(logger, 2020, selectedDay!.Value);
        }
        else
        {
            RunAllDays(logger, 2020);
        }
    }

    private static void RunAllDays(ILogger logger, int year)
    {
        var allDaysTimer = new Stopwatch();
        allDaysTimer.Start();

        for (var day = 1; day <= DayConstants.MaxDay; day++)
        {
            RunSingleDay(logger, year, day);
        }

        allDaysTimer.Stop();
        logger.LogInformation("Total runtime ({Runtime} ms)", allDaysTimer.Elapsed.TotalMilliseconds);
    }

    private static void RunSingleDay(ILogger logger, int year, int day)
    {
        logger.LogInformation("Attempting to run year {Year} day {Day}", year, day);

        if (DayConstants.Solvers.TryGetValue(day, out var solver))
        {
            var fileReadTimer = new Stopwatch();
            fileReadTimer.Start();

            var filePath = string.Format("../inputs/{0}/day/{1}/input", year, day);
            logger.LogInformation("Attempting to read file at {FilePath}", filePath);

            var fileContents = Files.ReadFile(filePath);
            fileReadTimer.Stop();
            logger.LogInformation("File read ({Runtime} ms)", fileReadTimer.Elapsed.TotalMilliseconds);

            var solveTimer = new Stopwatch();
            solveTimer.Start();
            var (part1, part2) = solver.Solve(logger, fileContents);
            solveTimer.Stop();
            logger.LogInformation("Day {Day}: ({Part1}, {Part2}) ({Runtime} ms)", day, part1, part2, solveTimer.Elapsed.TotalMilliseconds);
        }
    }
}