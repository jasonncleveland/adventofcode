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
                ulong part1 = SolvePart1(lines[0]);
                part1Timer.Stop();
                Console.WriteLine($"Part 1: {part1} ({part1Timer.Elapsed.TotalMilliseconds} ms)");

                Stopwatch part2Timer = new Stopwatch();
                part2Timer.Start();
                ulong part2 = SolvePart2(lines[0]);
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

    static ulong SolvePart1(string line)
    {
        List<int> output = new();
        int fileId = 0;
        for (int i = 0; i < line.Length; i++)
        {
            bool isFile = i % 2 == 0;
            int number = line[i] - '0';
            for (int j = 0; j < number; j++)
            {
                if (isFile)
                {
                    output.Add(fileId);
                }
                else
                {
                    output.Add(-1);
                }
            }
            if (isFile)
            {
                fileId++;
            }
        }

        int dotIndex = output.IndexOf(-1);
        if (dotIndex >= 0)
        {
            for (int i = output.Count - 1; i >= 0; i--)
            {
                if (dotIndex < 0 || i <= dotIndex)
                {
                    break;
                }

                if (output[i] != -1)
                {
                    output[dotIndex] = output[i];
                    output[i] = -1;
                    dotIndex = output.IndexOf(-1, dotIndex);
                }
            }
        }

        ulong checksum = 0;
        for (int i = 0; i < output.Count; i++)
        {
            if (output[i] != -1)
            {
                // The checksum is the file block position multiplied by the file id
                checksum += (ulong) i * (ulong) output[i];
            }
        }
        return checksum;
    }

    static ulong SolvePart2(string line)
    {
        // Store the metadata about file and free blocks for easy manipulation
        List<(int size, int startIndex, int fileId)> fileBlocks = new();
        List<(int size, int startIndex)> freeBlocks = new();
        int totalLength = 0;
        int fileId = 0;
        for (int i = 0; i < line.Length; i++)
        {
            bool isFile = i % 2 == 0;
            int number = line[i] - '0';
            if (isFile)
            {
                int startIndex = totalLength;
                fileBlocks.Add((number, startIndex, fileId));
                fileId++;
            }
            else
            {
                int startIndex = totalLength;
                freeBlocks.Add((number, startIndex));
            }
            totalLength += number;
        }

        // Work backwards through the file blocks
        fileBlocks.Reverse();
        for (int i = 0; i < fileBlocks.Count; i++)
        {
            (int size, int startIndex, int fileId) fileBlock = fileBlocks[i];
            // Search for the first free block that is large enough to hold the current file block
            int index = freeBlocks.FindIndex(block => block.size >= fileBlock.size);
            if (index > -1)
            {
                (int size, int startIndex) freeBlock = freeBlocks[index];
                if (freeBlock.startIndex < fileBlock.startIndex)
                {
                    // Move the file block to the start of the free block
                    fileBlock.startIndex = freeBlock.startIndex;
                    fileBlocks[i] = fileBlock;
                    // Reduce the free block by the moved file size and increment the start index
                    freeBlock.size -= fileBlock.size;
                    freeBlock.startIndex += fileBlock.size;
                    freeBlocks[index] = freeBlock;
                }
            }
        }

        ulong checksum = 0;
        foreach ((int size, int startIndex, int fileId) fileBlock in fileBlocks)
        {
            for (int i = 0; i < fileBlock.size; i++)
            {
                // The checksum is the file block position multiplied by the file id
                checksum += (ulong) (fileBlock.startIndex + i) * (ulong) fileBlock.fileId;
            }
        }
        return checksum;
    }
}