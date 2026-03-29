using System;
using System.IO;

namespace AdventOfCode.Shared.IO;

public static class Files
{
    public static string ReadFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new ArgumentException("Given file does not exist");
        }

        return File.ReadAllText(filePath);
    }
}