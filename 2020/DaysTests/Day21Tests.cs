using AdventOfCode.Y2020.Days;

using Microsoft.Extensions.Logging;

using NSubstitute;

using NUnit.Framework;

namespace AdventOfCode.Y2020.DaysTests;

[TestFixture]
public sealed class Day21Tests
{
    [Test]
    public void TestSolver()
    {
        var input = @"mxmxvkd kfcds sqjhc nhms (contains dairy, fish)
trh fvjkl sbzzf mxmxvkd (contains dairy)
sqjhc fvjkl (contains soy)
sqjhc mxmxvkd sbzzf (contains fish)
";

        var loggerMock = Substitute.For<ILogger>();
        var solver = new Day21();

        (string part1, string part2) = solver.Solve(loggerMock, input);

        Assert.That(part1, Is.EqualTo("5"));
        Assert.That(part2, Is.EqualTo("mxmxvkd,sqjhc,fvjkl"));
    }
}