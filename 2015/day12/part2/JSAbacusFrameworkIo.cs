using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

/*
 * To compile: dotnet build part2/part2.csproj
 * To run: dotnet run --project part2/part2.csproj -- problemData.txt
 */
class JSAbacusFrameworkIo
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        string[] lines = File.ReadAllLines(fileName);

        foreach (string line in lines)
        {
          if (line.Length < 1)
          {
            continue;
          }
          JsonNode data = JsonSerializer.Deserialize<JsonNode>(line);
          int totalSum = calculateTotalSum(data);
          Console.WriteLine($"Total sum: {totalSum}");
        }
      }
    }
  }

  static int calculateTotalSum(JsonNode node)
  {
    int total = 0;

    Type nodeType = node.GetType();
    if (nodeType.Equals(typeof(JsonArray)))
    {
      foreach (JsonNode childNode in node.AsArray())
      {
        total += calculateTotalSum(childNode);
      }
    }
    else if (nodeType.Equals(typeof(JsonObject)))
    {
      foreach (KeyValuePair<string, JsonNode> childNode in node.AsObject())
      {
        Type childNodeType = childNode.Value.GetType();
        if (!childNodeType.Equals(typeof(JsonArray)) && !childNodeType.Equals(typeof(JsonObject)))
        {
          if (childNode.Value.AsValue().ToString() == "red")
          {
            return 0;
          }
        }
        total += calculateTotalSum(childNode.Value);
      }
    }
    else
    {
      int value;
      if (int.TryParse(node.AsValue().ToString(), out value))
      {
        total += value;
      }
    }

    return total;
  }
}
