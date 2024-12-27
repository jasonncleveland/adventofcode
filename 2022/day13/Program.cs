using System;
using System.Collections.Generic;
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
        List<JsonArray> packets = ParseInput(lines);

        long total = 0;

        for (int i = 0; i < packets.Count; i += 2)
        {
            JsonArray left = packets[i];
            JsonArray right = packets[i + 1];
            int result = CompareNodesRec(left, right);
            if (result != 1)
            {
                total += i / 2 + 1;
            }
        }

        return total;
    }

    static long SolvePart2(string[] lines)
    {
        // Divider packates need to be added to the list of packets
        // [[2]]
        // [[6]]
        List<JsonArray> dividerPackets = new()
        {
            new JsonArray([
                new JsonArray([
                    JsonValue.Create(2)
                ])
            ]),
            new JsonArray([
                new JsonArray([
                    JsonValue.Create(6)
                ])
            ])
        };

        List<JsonArray> packets = ParseInput(lines);
        packets.AddRange(dividerPackets);

        // Sprt the packets using the custom sort function
        packets.Sort(CompareNodesRec);

        long total = 1;

        foreach (JsonArray dividerPacket in dividerPackets)
        {
            int index = packets.FindIndex(packet => packet == dividerPacket);
            total *= index + 1;
        }

        return total;
    }

    static List<JsonArray> ParseInput(string[] lines)
    {
        List<JsonArray> packets = new();

        for (int i = 0; i < lines.Length; i += 3)
        {
            packets.Add(JsonSerializer.Deserialize<JsonArray>(lines[i]));
            packets.Add(JsonSerializer.Deserialize<JsonArray>(lines[i + 1]));
        }

        return packets;
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
                // We ran out of items in the right array so the packets are not in order
                return 1;
            }

            if (left[i].GetType().Equals(typeof(JsonArray)) || right[i].GetType().Equals(typeof(JsonArray)))
            {
                JsonArray leftArray, rightArray;

                // If one of the items is an array and the other a number, convert to an array and compare
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
                    // If the recursive check found a non-zero result, return early
                    return comparisonResult;
                }
            }
            else
            {
                int leftValue = int.Parse(left[i].ToString());
                int rightValue = int.Parse(right[i].ToString());

                if (leftValue == rightValue)
                {
                    // The values are eqyal so continue checking
                    continue;
                }
                else if (leftValue < rightValue)
                {
                    // The left value is smaller than the right so the packets are in order
                    return -1;
                }
                else
                {
                    // The left value is greater than the right so the packets are not in order
                    return 1;
                }
            }
        }

        // We have reached the end of the array so the arrays are either the same or the left ran out of items
        // If the left ran out of items then the packets are in order
        return left.Count == right.Count ? 0 : -1;
    }
}