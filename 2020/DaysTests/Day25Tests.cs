using AdventOfCode.Y2020.Days;

using Microsoft.Extensions.Logging;

using NSubstitute;

using NUnit.Framework;

namespace AdventOfCode.Y2020.DaysTests;

[TestFixture]
public sealed class Day25Tests
{
    [Test]
    public void TestSolver()
    {
        const string input = """
5764801
17807724
""";

        var loggerMock = Substitute.For<ILogger>();
        var solver = new Day25();

        (string part1, string part2) = solver.Solve(loggerMock, input);

        Assert.That(part1, Is.EqualTo("14897079"));
        Assert.That(part2, Is.EqualTo("Merry Christmas!"));
    }
}