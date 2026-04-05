using AdventOfCode.Y2020.Days;

using Microsoft.Extensions.Logging;

using NSubstitute;

using NUnit.Framework;

namespace AdventOfCode.Y2020.DaysTests;

[TestFixture]
public class Day23Tests
{
    [Test]
    public void TestSolver()
    {
        const string input = "389125467";

        var loggerMock = Substitute.For<ILogger>();
        var solver = new Day22();

        (string part1, string part2) = solver.Solve(loggerMock, input);

        Assert.That(part1, Is.EqualTo("67384529"));
        Assert.That(part2, Is.EqualTo("149245887792"));
    }
}