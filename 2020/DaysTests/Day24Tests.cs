using AdventOfCode.Y2020.Days;

using Microsoft.Extensions.Logging;

using NSubstitute;

using NUnit.Framework;

namespace AdventOfCode.Y2020.DaysTests;

[TestFixture]
public class Day24Tests
{
    [Test]
    public void TestSolver()
    {
        const string input = """
 sesenwnenenewseeswwswswwnenewsewsw
 neeenesenwnwwswnenewnwwsewnenwseswesw
 seswneswswsenwwnwse
 nwnwneseeswswnenewneswwnewseswneseene
 swweswneswnenwsewnwneneseenw
 eesenwseswswnenwswnwnwsewwnwsene
 sewnenenenesenwsewnenwwwse
 wenwwweseeeweswwwnwwe
 wsweesenenewnwwnwsenewsenwwsesesenwne
 neeswseenwwswnwswswnw
 nenwswwsewswnenenewsenwsenwnesesenew
 enewnwewneswsewnwswenweswnenwsenwsw
 sweneswneswneneenwnewenewwneswswnese
 swwesenesewenwneswnwwneseswwne
 enesenwswwswneneswsenwnewswseenwsese
 wnwnesenesenenwwnenwsewesewsesesew
 nenewswnwewswnenesenwnesewesw
 eneswnwswnwsenenwnwnwwseeswneewsenese
 neswnwewnwnwseenwseesewsenwsweewe
 wseweeenwnesenwwwswnew
 """;

        var loggerMock = Substitute.For<ILogger>();
        var solver = new Day24();

        (string part1, string part2) = solver.Solve(loggerMock, input);

        Assert.That(part1, Is.EqualTo("10"));
        Assert.That(part2, Is.EqualTo("2208"));
    }
}