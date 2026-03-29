using Microsoft.Extensions.Logging;

namespace AdventOfCode.Shared.Solver;

public interface IDaySolver
{
    public (string, string) Solve(ILogger logger, string input);
}