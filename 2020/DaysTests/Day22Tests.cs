using AdventOfCode.Y2020.Days;

using Microsoft.Extensions.Logging;

using NSubstitute;

using NUnit.Framework;

namespace AdventOfCode.Y2020.DaysTests;

[TestFixture]
public sealed class Day22Tests
{
    [Test]
    public void TestSolver()
    {
        const string input = """
Player 1:
9
2
6
3
1

Player 2:
5
8
4
7
10

""";

        var loggerMock = Substitute.For<ILogger>();
        var solver = new Day22();

        (string part1, string part2) = solver.Solve(loggerMock, input);

        Assert.That(part1, Is.EqualTo("306"));
        Assert.That(part2, Is.EqualTo("291"));
    }
}