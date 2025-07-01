using System;
using NUnit.Framework;

[TestFixture]
public class ProgramTest
{
    [TestCase("9 players; last marble is worth 25 points", 32)]
    [TestCase("10 players; last marble is worth 1618 points", 8317)]
    [TestCase("13 players; last marble is worth 7999 points", 146373)]
    [TestCase("17 players; last marble is worth 1104 points", 2764)]
    [TestCase("21 players; last marble is worth 6111 points", 54718)]
    [TestCase("30 players; last marble is worth 5807 points", 37305)]
    public void TestPart1(string input, long expected)
    {
        var result = Program.SolvePart1([input]);
        Assert.That(result, Is.EqualTo(expected));
    }

    [TestCase("9 players; last marble is worth 25 points", 22563)]
    [TestCase("10 players; last marble is worth 1618 points", 74765078)]
    [TestCase("13 players; last marble is worth 7999 points", 1406506154)]
    [TestCase("17 players; last marble is worth 1104 points", 20548882)]
    [TestCase("21 players; last marble is worth 6111 points", 507583214)]
    [TestCase("30 players; last marble is worth 5807 points", 320997431)]
    public void TestPart2(string input, long expected)
    {
        var result = Program.SolvePart2([input]);
        Assert.That(result, Is.EqualTo(expected));
    }
}