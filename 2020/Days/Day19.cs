using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using AdventOfCode.Shared.Solver;

using Microsoft.Extensions.Logging;

namespace AdventOfCode.Y2020.Days;

internal sealed class Day19 : AbstractDaySolver<MessageInfo>
{
    protected override MessageInfo ParseInput(ILogger logger, string fileContents)
    {
        var lines = fileContents.Split("\n\n");

        // Read matching rules
        Dictionary<int, string> rulesMap = new();
        foreach (var line in lines.First().Split('\n', StringSplitOptions.RemoveEmptyEntries))
        {
            string[] lineParts = line.Split(": ");
            int ruleId = int.Parse(lineParts[0]);
            if (lineParts[1].Contains('"'))
            {
                rulesMap.Add(ruleId, lineParts[1][1].ToString());
            }
            else
            {
                string[] ruleOptions = lineParts[1].Split(" | ");
                List<string> ruleOptionsList = new();
                foreach (string ruleOption in ruleOptions)
                {
                    string[] ruleOptionParts = ruleOption.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    ruleOptionsList.Add(string.Join("", ruleOptionParts.Select(r => $"{{{r}}}")));
                }

                rulesMap.Add(ruleId, string.Join("|", ruleOptionsList));
            }
        }

        // Read messages
        var messages = lines.Last().Split('\n', StringSplitOptions.RemoveEmptyEntries).ToList().AsReadOnly();

        return new MessageInfo(rulesMap, messages);
    }

    protected override string SolvePart1(ILogger logger, MessageInfo messageInfo)
    {
        // Convert rules to regex
        string regexString = ConvertRulesToRegexString(messageInfo.Rules);

        // Check messages
        var total = messageInfo.Messages.Count(message => Regex.IsMatch(message, regexString));

        return total.ToString();
    }

    protected override string SolvePart2(ILogger logger, MessageInfo messageInfo)
    {
        var rulesCopy = new Dictionary<int, string>(messageInfo.Rules)
        {
            // Replace 8 with 8: 42 | 42 8
            // This is equivalent to 42+
            [8] = "{42}+", // Replace 11 with 11: 42 31 | 42 11 31
            // This is looking for the same number of 42 as 31
            // Hard code up to 4 pairs because laziness
            [11] = "{42}{31}|{42}{42}{31}{31}|{42}{42}{42}{31}{31}{31}|{42}{42}{42}{42}{31}{31}{31}{31}"
        };

        // Convert rules to regex
        string regexString = ConvertRulesToRegexString(rulesCopy);

        // Check messages
        var total = messageInfo.Messages.Count(message => Regex.IsMatch(message, regexString));

        return total.ToString();
    }

    private static string ConvertRulesToRegexString(IReadOnlyDictionary<int, string> rules)
    {
        string pattern = @"\{(?<ruleId>\d+)\}";
        string outputRegex = $"^{rules[0]}$";

        // Replace rules until no changes are made
        bool changesMade = true;
        while (changesMade)
        {
            changesMade = false;
            foreach (Match match in Regex.Matches(outputRegex, pattern))
            {
                int nextRuleId = int.Parse(match.Groups["ruleId"].Value);
                if (rules.TryGetValue(nextRuleId, out string rule))
                {
                    outputRegex = outputRegex.Replace($"{{{nextRuleId}}}", $"(?:{rule})");
                    changesMade = true;
                }
            }
        }

        return outputRegex;
    }
}

internal sealed class MessageInfo(Dictionary<int, string> rules, IReadOnlyList<string> messages)
{
    public IReadOnlyDictionary<int, string> Rules { get; } = rules;
    public IReadOnlyList<string> Messages { get; } = messages;
}