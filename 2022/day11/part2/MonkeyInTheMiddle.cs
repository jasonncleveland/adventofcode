using System;
using System.Collections.Generic;
using System.IO;

class MonkeyInTheMiddle
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        string fileContents = File.ReadAllText(fileName);
        string[] lines = fileContents.Split('\n');

        List<MonkeyData> monkies = new List<MonkeyData>();

        for (int i = 0; i < lines.Length; i += 7)
        {
          // Create monkey
          MonkeyData newMonkey = new MonkeyData();

          newMonkey.items = new Queue<long>(Array.ConvertAll(lines[i + 1].Trim().Replace("Starting items: ", "").Replace(" ", "").Split(','), item => long.Parse(item)));
          newMonkey.operation = lines[i + 2].Trim().Replace("Operation: new = ", "");
          newMonkey.divisor = long.Parse(lines[i + 3].Trim().Replace("Test: divisible by ", ""));
          newMonkey.onTrueResult = int.Parse(lines[i + 4].Trim().Replace("If true: throw to monkey ", ""));
          newMonkey.onFalseResult = int.Parse(lines[i + 5].Trim().Replace("If false: throw to monkey ", ""));

          monkies.Add(newMonkey);
        }

        // Calculate LCM
        // The lowest common multiple is used too keep the worry level within
        // the bounds of a long. By using the LCM, we guarantee that every value
        // can be divided by any of the monkey's divisor while keeping the value
        // from increasing out of control and overflowing.
        long leastCommonMultiple = 1;
        foreach (MonkeyData monkey in monkies)
        {
          leastCommonMultiple *= monkey.divisor;
        }
        // Console.WriteLine($"LCM: {leastCommonMultiple}");

        // Perform 10,000 rounds
        int numRounds = 10000;
        for (int i = 0; i < numRounds; i++)
        {
          foreach (MonkeyData monkey in monkies)
          {
            while (monkey.items.Count > 0)
            {
              long item = monkey.items.Dequeue();
              long[] operationResult = monkey.performOperation(item, leastCommonMultiple);
              monkies[(int)operationResult[0]].items.Enqueue(operationResult[1]);
            }
          }

          // if ((i + 1) == 1 || (i + 1) == 20 || (i + 1) % 1000 == 0)
          // {
          //   Console.WriteLine($"== After round {i + 1} ==");
          //   for (int j = 0; j < monkies.Count; j++)
          //   {
          //     Console.WriteLine($"Monkey {j} inspected {monkies[j].inspectedItems} items");
          //   }
          // }
        }

        long[] topValues = new long[] { 0, 0 };
        for (int i = 0; i < monkies.Count; i++)
        {
          // Console.WriteLine($"Monkey {i} inspected {monkies[i].inspectedItems} items");
          if (monkies[i].inspectedItems > topValues[0])
          {
            topValues[1] = topValues[0];
            topValues[0] = monkies[i].inspectedItems;
          }
          else if (monkies[i].inspectedItems > topValues[1])
          {
            topValues[1] = monkies[i].inspectedItems;
          }
        }
        Console.WriteLine($"Total monkey buisness: {topValues[0] * topValues[1]}");
      }
    }
  }
}

class MonkeyData
{
  public Queue<long> items { get; set; } = new Queue<long>();
  public string operation { get; set; }
  public long divisor { get; set; }
  public int onTrueResult { get; set; }
  public int onFalseResult { get; set; }
  public long inspectedItems { get; set; } = 0;

  public MonkeyData()
  {
  }

  public long[] performOperation(long item, long lcm)
  {
    // Monkey inspects item
    inspectedItems++;
    string newExpression = operation.Replace("old", item.ToString());
    string[] operators = newExpression.Split(' ');
    
    long result = 0;
    switch (operators[1])
    {
      case "+":
        result = long.Parse(operators[0]) + long.Parse(operators[2]);
        break;
      case "*":
        result = long.Parse(operators[0]) * long.Parse(operators[2]);
        break;
    }

    // LCM voodoo
    result = result % lcm;

    // Monkey tests item
    bool isDivisible = result % divisor == 0;
    return new long[] { isDivisible ? onTrueResult : onFalseResult, result };
  }
}
