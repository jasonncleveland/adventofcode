using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AdventOfCode.Shared.Solver;

using Microsoft.Extensions.Logging;

namespace AdventOfCode.Y2020.Days;

internal sealed class Day23 : AbstractDaySolver<IReadOnlyList<int>>
{
    protected override IReadOnlyList<int> ParseInput(ILogger logger, string fileContents)
    {
        return fileContents
            .Trim()
            .ToCharArray()
            .Select(c => c - '0')
            .ToList()
            .AsReadOnly();
    }

    protected override string SolvePart1(ILogger logger, IReadOnlyList<int> input)
    {
        var buffer = input.ToArray();
        var copy = new int[buffer.Length];

        var accumulator = new int[3];
        for (var move = 0; move < 100; move++)
        {
            logger.LogTrace("cups: {Cups}", string.Join(", ", buffer));

            // Pick up three cups immediately clockwise of current cup
            for (var i = 0; i < 3; i++)
            {
                accumulator[i] = buffer[i + 1];
            }
            logger.LogTrace("pick up: {Cups}", string.Join(", ", accumulator));

            // Move cups left to fill gap
            for (int i = 0, from = 4, to = 1; i < buffer.Length - 4; i++, from++, to++)
            {
                buffer[to % buffer.Length] = buffer[from % buffer.Length];
            }

            // Set empty space to max value to mark it as "empty"
            for (var i = 1; i < 4; i++)
            {
                buffer[^i] = int.MaxValue;
            }

            var target = buffer[0] - 1;
            int destination;
            do
            {
                destination = buffer.IndexOf(target);
                target--;
                if (target <= 0)
                {
                    target = 9;
                }
            } while (destination < 0);
            logger.LogTrace("destination: {Destination}", target + 1);

            // Create gap for insertion
            for (var i = buffer.Length - 4; i > destination; i--)
            {
                buffer[i + 3] = buffer[i];
                buffer[i] = int.MaxValue;
            }

            // Place removed cups after destination cup
            var insertIndex = destination + 1;
            for (var i = 0; i < 3; i++)
            {
                buffer[insertIndex + i] = accumulator[i];
            }

            // Shift left so that the current cup is always the first one
            for (var i = 0; i < buffer.Length; i++)
            {
                copy[i] = buffer[(1 + i) % buffer.Length];
            }

            for (var i = 0; i < buffer.Length; i++)
            {
                buffer[i] = copy[i];
            }
        }

        var start = buffer.IndexOf(1);
        var output = new StringBuilder();
        for (int i = 0; i < buffer.Length - 1; i++)
        {
            output.Append(buffer[(start + 1 + i) % buffer.Length]);
        }
        return output.ToString();
    }

    protected override string SolvePart2(ILogger logger, IReadOnlyList<int> input)
    {
        const int cupCount = 1_000_000;
        const int moveCount = 10_000_000;

        var expandedInput = input.Select(i => (long)i).ToList();
        for (int i = input.Count; i < cupCount; i++)
        {
            expandedInput.Add(i + 1);
        }

        // Create a linked list of the nodes with a lookup table
        var lookup = expandedInput.Select(i => new LinkedListNode<long>(i)).ToArray();
        var head = lookup[0];
        lookup.Sort();

        // Arrange the nodes correctly
        for (var i = 0; i < expandedInput.Count; i++)
        {
            var number = expandedInput[i];
            var node = lookup[number - 1];
            var previousNumber = expandedInput[(i + expandedInput.Count - 1) % expandedInput.Count];
            var previousNode = lookup[previousNumber - 1];
            var nextNumber = expandedInput[(i + 1) % expandedInput.Count];
            var nextNode = lookup[nextNumber - 1];
            node.Previous = previousNode;
            previousNode.Next = node;
            node.Next = nextNode;
            nextNode.Previous = node;
        }

        var current = head;
        for (var move = 0; move < moveCount; move++)
        {
            var removedNodeHead = current.Next;
            var removedNodeTail = removedNodeHead.Next.Next;
            var next = current.Next.Next.Next.Next;
            current.Next = next;
            next.Previous = current;
            removedNodeHead.Previous = null;
            removedNodeTail.Next = null;

            var target = current.Value - 1;
            while (true)
            {
                if (target <= 0)
                {
                    target = cupCount;
                }
                if (removedNodeHead.Value == target || removedNodeHead.Next.Value == target || removedNodeHead.Next.Next.Value == target)
                {
                    target -= 1;
                    continue;
                }
                break;
            }

            var targetNode = lookup[target - 1];
            var targetNext = targetNode.Next;
            targetNode.Next = removedNodeHead;
            removedNodeHead.Previous = targetNode;
            removedNodeTail.Next = targetNext;
            targetNext.Previous = removedNodeTail;

            current = current.Next;
        }

        var start = lookup[0].Next;
        var first = start.Value;
        var second = start.Next.Value;
        return (first * second).ToString();
    }
}

internal sealed class LinkedListNode<T> : IComparable<LinkedListNode<T>> where T : IComparable
{
    public LinkedListNode<T> Next { get; set; }
    public LinkedListNode<T> Previous { get; set; }
    public T Value { get; }

    public LinkedListNode(T value)
    {
        Value = value;
    }

    public int CompareTo(LinkedListNode<T> other)
    {
        return Value.CompareTo(other.Value);
    }
}