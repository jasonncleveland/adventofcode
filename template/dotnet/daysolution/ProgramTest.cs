using System;
using NUnit.Framework;

[TestFixture]
public class ProgramTest
{
    [TestCase("", ExpectedResult = -1)]
    public long TestPart1(string input)
    {
        return Program.SolvePart1([input]);
    }

    [TestCase("", ExpectedResult = -1)]
    public long TestPart2(string input)
    {
        return Program.SolvePart2([input]);
    }
}