using System;
using System.Collections.Generic;
using System.Linq;

using AdventOfCode.Shared.Solver;

using Microsoft.Extensions.Logging;

namespace AdventOfCode.Y2020.Days;

internal sealed class Day14 : AbstractDaySolver<IReadOnlyList<string>>
{
    protected override IReadOnlyList<string> ParseInput(ILogger logger, string fileContents)
    {
        return fileContents
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .ToList()
            .AsReadOnly();
    }

    protected override string SolvePart1(ILogger logger, IReadOnlyList<string> lines)
    {
        long total = 0;

        Dictionary<int, long> memory = new();
        List<(int index, char value)> bitmasks = new();

        foreach (string line in lines)
        {
            string[] lineParts = line.Split(" = ");
            if (line.StartsWith("mask"))
            {
                bitmasks = new();
                string mask = lineParts[1];
                for (int i = 0; i < mask.Length; i++)
                {
                    if (mask[i] == '1' || mask[i] == '0')
                    {
                        bitmasks.Add((i, mask[i]));
                    }
                }
            }
            else
            {
                int address = int.Parse(lineParts[0].Substring(4, lineParts[0].Length - 5));
                long value = long.Parse(lineParts[1]);
                char[] binary = Convert.ToString(value, 2).PadLeft(36, '0').ToCharArray();
                foreach ((int index, char value) bitmask in bitmasks)
                {
                    binary[bitmask.index] = bitmask.value;
                }
                long maskedValue = Convert.ToInt64(new string(binary), 2);
                memory[address] = maskedValue;
            }
        }

        foreach (long value in memory.Values)
        {
            total += value;
        }

        return total.ToString();
    }

    protected override string SolvePart2(ILogger logger, IReadOnlyList<string> lines)
    {
        long total = 0;

        Dictionary<long, long> memory = new();

        string mask = "";
        foreach (string line in lines)
        {
            string[] lineParts = line.Split(" = ");
            if (line.StartsWith("mask"))
            {
                mask = lineParts[1];
            }
            else
            {
                int address = int.Parse(lineParts[0].Substring(4, lineParts[0].Length - 5));
                long value = long.Parse(lineParts[1]);
                char[] binary = Convert.ToString(address, 2).PadLeft(36, '0').ToCharArray();

                List<long> possibleAddresses = GetPossibleAddressesRec(mask, binary);

                foreach (long possibleAddress in possibleAddresses)
                {
                    memory[possibleAddress] = value;
                }
            }
        }

        foreach (long value in memory.Values)
        {
            total += value;
        }

        return total.ToString();
    }

    private static List<long> GetPossibleAddressesRec(string mask, char[] address, int index = 0, long accumulatedAddress = 0)
    {
        if (index == mask.Length)
        {
            return [accumulatedAddress];
        }

        long position = (long)Math.Pow(2, mask.Length - 1 - index);
        switch (mask[index])
        {
            case '0':
                if (address[index] == '1')
                {
                    return GetPossibleAddressesRec(mask, address, index + 1, accumulatedAddress + position);
                }
                else
                {
                    return GetPossibleAddressesRec(mask, address, index + 1, accumulatedAddress);
                }
            case '1':
                return GetPossibleAddressesRec(mask, address, index + 1, accumulatedAddress + position);
            case 'X':
                List<long> possibleAddresses = new();
                possibleAddresses.AddRange(GetPossibleAddressesRec(mask, address, index + 1, accumulatedAddress + position));
                possibleAddresses.AddRange(GetPossibleAddressesRec(mask, address, index + 1, accumulatedAddress));
                return possibleAddresses;
            default:
                throw new Exception($"Invalid character found in mask {mask[index]}");
        }
    }
}