using System;
using System.Collections.Generic;
using System.Linq;

using AdventOfCode.Shared.Solver;

using Microsoft.Extensions.Logging;

namespace AdventOfCode.Y2020.Days;

internal sealed class Day16 : AbstractDaySolver<TicketInfo>
{
    protected override TicketInfo ParseInput(ILogger logger, string fileContents)
    {
        var sections = fileContents.Split("\n\n").ToList();
        var ticketFields = new List<TicketField>();

        // Ticket Fields
        foreach (var line in sections[0].Split('\n', StringSplitOptions.RemoveEmptyEntries))
        {
            string[] lineParts = line.Split(": ");
            var ticketField = new TicketField(lineParts[0]);
            foreach (string range in lineParts[1].Split(" or "))
            {
                string[] rangeParts = range.Split('-');
                ticketField.Ranges.Add((int.Parse(rangeParts[0]), int.Parse(rangeParts[1])));
            }

            ticketFields.Add(ticketField);
        }

        // Your Ticket
        var yourTicket = sections[1]
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Last()
            .Split(',')
            .Select(int.Parse)
            .ToList()
            .AsReadOnly();

        // Nearby Tickets
        var nearbyTickets = sections[2]
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Skip(1)
            .Select(line => line.Split(',').Select(int.Parse).ToList().AsReadOnly())
            .ToList()
            .AsReadOnly();

        return new TicketInfo(ticketFields, yourTicket, nearbyTickets);
    }

    protected override string SolvePart1(ILogger logger, TicketInfo ticketInfo)
    {
        long total = 0;

        // Parse nearby tickets
        foreach (var nearbyTicket in ticketInfo.NearbyTickets)
        {
            foreach (int number in nearbyTicket)
            {
                bool isValid = false;
                foreach (var ticketField in ticketInfo.TicketFields)
                {
                    foreach ((int lower, int upper) in ticketField.Ranges)
                    {
                        if (number >= lower && number <= upper)
                        {
                            isValid = true;
                            break;
                        }
                    }

                    if (isValid)
                    {
                        break;
                    }
                }

                if (!isValid)
                {
                    total += number;
                }
            }
        }

        return total.ToString();
    }

    protected override string SolvePart2(ILogger logger, TicketInfo ticketInfo)
    {
        // Parse nearby tickets
        var validTickets = new List<IReadOnlyList<int>>();
        foreach (var nearbyTicket in ticketInfo.NearbyTickets)
        {
            int validFieldCount = 0;
            foreach (int number in nearbyTicket)
            {
                bool isValid = false;
                foreach (TicketField ticketField in ticketInfo.TicketFields)
                {
                    foreach ((int lower, int upper) in ticketField.Ranges)
                    {
                        if (number >= lower && number <= upper)
                        {
                            isValid = true;
                            break;
                        }
                    }

                    if (isValid)
                    {
                        break;
                    }
                }

                if (isValid)
                {
                    validFieldCount++;
                }
            }

            // Keep only possibly valid tickets
            if (validFieldCount == ticketInfo.TicketFields.Count)
            {
                validTickets.Add(nearbyTicket);
            }
        }

        Dictionary<TicketField, List<int>> possibleColumnMapping = new();
        foreach (TicketField ticketField in ticketInfo.TicketFields)
        {
            List<int> possibleColumns = new();
            for (int column = 0; column < ticketInfo.TicketFields.Count; column++)
            {
                bool isColumnValid = true;
                foreach (var validTicket in validTickets)
                {
                    int number = validTicket[column];
                    bool isInRange = false;
                    foreach ((int lower, int upper) in ticketField.Ranges)
                    {
                        if (number >= lower && number <= upper)
                        {
                            isInRange = true;
                            break;
                        }
                    }

                    if (!isInRange)
                    {
                        isColumnValid = false;
                        break;
                    }
                }

                if (isColumnValid)
                {
                    possibleColumns.Add(column);
                }
            }

            possibleColumnMapping[ticketField] = possibleColumns;
        }

        Dictionary<TicketField, int> fieldMapping = new();

        // Find which column each field maps to if there are multiple options for a field (sudoku method)
        while (true)
        {
            if (possibleColumnMapping.Keys.Count == 0)
            {
                break;
            }

            foreach ((TicketField ticketField, List<int> possibleColumns) in possibleColumnMapping)
            {
                // If we find a field with only one possible column then lock it in and remove option from other fields
                if (possibleColumns.Count == 1)
                {
                    int column = possibleColumns[0];
                    fieldMapping[ticketField] = column;
                    possibleColumnMapping.Remove(ticketField);
                    foreach ((TicketField ticketField1, List<int> possibleColumns1) in possibleColumnMapping)
                    {
                        possibleColumnMapping[ticketField1].Remove(column);
                    }
                }
            }
        }

        long total = 1;

        foreach ((TicketField ticketField, int column) in fieldMapping)
        {
            if (ticketField.Name.StartsWith("departure"))
            {
                total *= ticketInfo.YourTicket[column];
            }
        }

        return total.ToString();
    }
}

internal sealed class TicketInfo(
    IReadOnlyList<TicketField> ticketFields,
    IReadOnlyList<int> yourTicket,
    IReadOnlyList<IReadOnlyList<int>> nearbyTickets)
{
    public IReadOnlyList<TicketField> TicketFields { get; } = ticketFields;
    public IReadOnlyList<int> YourTicket { get; } = yourTicket;
    public IReadOnlyList<IReadOnlyList<int>> NearbyTickets { get; } = nearbyTickets;
}

internal sealed class TicketField(string name)
{
    public string Name { get; } = name;
    public List<(int lower, int upper)> Ranges { get; set; } = [];

    public override string ToString()
    {
        return $"{Name}: {string.Join(" or ", Ranges)}";
    }
}