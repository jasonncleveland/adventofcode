using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

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

    public static long SolvePart1(string[] lines)
    {
        long total = 0;

        var lineParts = lines[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var playerCount = long.Parse(lineParts[0]);
        var highestMarble = long.Parse(lineParts[6]);

        return Simulate(playerCount, highestMarble);
    }

    public static long SolvePart2(string[] lines)
    {
        long total = 0;

        var lineParts = lines[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var playerCount = long.Parse(lineParts[0]);
        var highestMarble = long.Parse(lineParts[6]) * 100;

        return Simulate(playerCount, highestMarble);

        return total;
    }

    static long Simulate(long playerCount, long highestMarble)
    {
        var playerScores = new Dictionary<long, long>();
        for (var i = 0; i < playerCount; i++)
        {
            playerScores.Add(i, 0);
        }

        var removedNumbers = new List<long>();

        var head = new CustomLinkedListNode(0);
        head.Next = head;
        head.Previous = head;

        var currentNode = head;
        var currentPlayer = 0;

        for (var i = 1; i <= highestMarble; i++)
        {
            if (currentPlayer == playerCount)
            {
                currentPlayer = 0;
            }
            var nextMarble = i;
            if (nextMarble % 23 == 0)
            {
                var tempCurrent = currentNode;
                for (var j = 0; j < 7; j++)
                {
                    tempCurrent = tempCurrent.Previous;
                }
                removedNumbers.Add(tempCurrent.Value);
                currentNode = tempCurrent.Next;
                tempCurrent.Remove();
                playerScores[currentPlayer] += nextMarble + tempCurrent.Value;
            }
            else
            {
                currentNode = currentNode.Next;
                var newNode = new CustomLinkedListNode(nextMarble);
                currentNode.AddAfter(newNode);
                currentNode = newNode;
            }
            currentPlayer += 1;
        }

        var maxScore = long.MinValue;
        foreach ((long player, long score) in playerScores)
        {
            if (score > maxScore)
            {
                maxScore = score;
            }
        }

        return maxScore;
    }

    static void PrintList(CustomLinkedListNode head, CustomLinkedListNode highlight = null)
    {
        var currentNode = head;
        while (currentNode != null)
        {
            if (currentNode == highlight)
            {
                var currentColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($"({currentNode.Value}) ");
                Console.ForegroundColor = currentColor;
            }
            else
            {
                Console.Write($"{currentNode.Value} ");
            }
            currentNode = currentNode.Next;
            if (currentNode == head)
            {
                break;
            }
        }
        Console.WriteLine();
    }
}