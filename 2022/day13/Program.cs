using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

public class Program
{
    static void Main(string[] args)
    {
        if (args.Length > 0)
        {
            string fileName = args[0];
            if (File.Exists(fileName))
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                string[] lines = File.ReadAllLines(fileName);
                stopWatch.Stop();
                Console.WriteLine($"File read ({stopWatch.Elapsed.TotalMilliseconds} ms)");

                Stopwatch part1Timer = new Stopwatch();
                part1Timer.Start();
                long part1 = SolvePart1(lines);
                part1Timer.Stop();
                Console.WriteLine($"Part 1: {part1} ({part1Timer.Elapsed.TotalMilliseconds} ms)");

                Stopwatch part2Timer = new Stopwatch();
                part2Timer.Start();
                long part2 = SolvePart2(lines);
                part2Timer.Stop();
                Console.WriteLine($"Part 2: {part2} ({part2Timer.Elapsed.TotalMilliseconds} ms)");
            }
            else
            {
                throw new ArgumentException("Invalid file name provided. Please provide a valid file name.");
            }
        }
        else
        {
            throw new ArgumentException("Input data file name not provided. Please provide the file name as an argument: dotnet run <file-name>");
        }
    }

    static long SolvePart1(string[] lines)
    {
        long total = 0;

        for (int i = 0; i < lines.Length; i += 3)
        {
            string left = lines[i];
            string right = lines[i + 1];
            JsonArray leftParsed = JsonSerializer.Deserialize<JsonArray>(left);
            JsonArray rightParsed = JsonSerializer.Deserialize<JsonArray>(right);
            int result = CompareNodesRec(leftParsed, rightParsed);
            if (result != 1)
            {
                total += i / 3 + 1;
            }
        }

        return total;
    }

    static long SolvePart2(string[] lines)
    {
        long total = 0;

        // TODO: Implement logic to solve part 2

        return total;
    }

    /**
     * Comparison results:
     * -1 - Left is smaller
     * 0 - Left and right are equal
     * 1 - Right is smaller
     */
    static int CompareNodesRec(JsonArray left, JsonArray right)
    {
        for (int i = 0; i < left.Count; i++)
        {
            if (right.Count <= i)
            {
                return 1;
            }

            if (left[i].GetType().Equals(typeof(JsonArray)) || right[i].GetType().Equals(typeof(JsonArray)))
            {
                JsonArray leftArray, rightArray;

                if (!left[i].GetType().Equals(typeof(JsonArray)))
                {
                    leftArray = new JsonArray([left[i].DeepClone()]);
                }
                else
                {
                    leftArray = left[i] as JsonArray;
                }

                if (!right[i].GetType().Equals(typeof(JsonArray)))
                {
                    rightArray = new JsonArray([right[i].DeepClone()]);
                }
                else
                {
                    rightArray = right[i] as JsonArray;
                }

                int comparisonResult = CompareNodesRec(leftArray, rightArray);
                if (comparisonResult != 0)
                {
                    return comparisonResult;
                }
            }
            else
            {
                int leftValue = int.Parse(left[i].ToString());
                int rightValue = int.Parse(right[i].ToString());

                if (leftValue == rightValue)
                {
                    continue;
                }
                else if (leftValue < rightValue)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }
        }

        return left.Count < right.Count ? -1 : left.Count == right.Count ? 0 : 1;
    }
}