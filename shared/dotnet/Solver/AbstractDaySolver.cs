using System.Diagnostics;

using Microsoft.Extensions.Logging;

namespace AdventOfCode.Shared.Solver;

public abstract class AbstractDaySolver<TParsed> : IDaySolver
{
    public (string, string) Solve(ILogger logger, string fileContents)
    {
        var parseTimer = new Stopwatch();
        parseTimer.Start();
        var input = ParseInput(logger, fileContents);
        parseTimer.Stop();
        logger.LogInformation("File parse: ({Runtime} ms)", parseTimer.Elapsed.TotalMilliseconds);

        var part1Timer = new Stopwatch();
        part1Timer.Start();
        var part1 = SolvePart1(logger, input);
        part1Timer.Stop();
        logger.LogInformation("Part 1: {Result} ({Runtime} ms)", part1, part1Timer.Elapsed.TotalMilliseconds);

        var part2Timer = new Stopwatch();
        part2Timer.Start();
        var part2 = SolvePart2(logger, input);
        part2Timer.Stop();
        logger.LogInformation("Part 2: {Result} ({Runtime} ms)", part2, part2Timer.Elapsed.TotalMilliseconds);

        return (part1, part2);
    }

    protected abstract TParsed ParseInput(ILogger logger, string fileContents);

    protected abstract string SolvePart1(ILogger logger, TParsed input);

    protected abstract string SolvePart2(ILogger logger, TParsed input);
}