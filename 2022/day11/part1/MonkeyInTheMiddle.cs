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

          newMonkey.items = new Queue<int>(Array.ConvertAll(lines[i + 1].Trim().Replace("Starting items: ", "").Replace(" ", "").Split(','), item => int.Parse(item)));
          newMonkey.operation = lines[i + 2].Trim().Replace("Operation: new = ", "");
          newMonkey.divisor = int.Parse(lines[i + 3].Trim().Replace("Test: divisible by ", ""));
          newMonkey.onTrueResult = int.Parse(lines[i + 4].Trim().Replace("If true: throw to monkey ", ""));
          newMonkey.onFalseResult = int.Parse(lines[i + 5].Trim().Replace("If false: throw to monkey ", ""));

          monkies.Add(newMonkey);
        }

        // Perform 20 rounds
        int numRounds = 20;
        for (int i = 0; i < numRounds; i++)
        {
          foreach (MonkeyData monkey in monkies)
          {
            while (monkey.items.Count > 0)
            {
              int item = monkey.items.Dequeue();
              int[] operationResult = monkey.performOperation(item);
              monkies[operationResult[0]].items.Enqueue(operationResult[1]);
            }
          }
        }

        for (int i = 0; i < monkies.Count; i++)
        {
          Console.WriteLine($"Monkey {i} inspected {monkies[i].inspectedItems} items");
        }
      }
    }
  }
}

class MonkeyData
{
  public Queue<int> items { get; set; } = new Queue<int>();
  public string operation { get; set; }
  public int divisor { get; set; }
  public int onTrueResult { get; set; }
  public int onFalseResult { get; set; }
  public int inspectedItems { get; set; } = 0;

  public MonkeyData()
  {
  }

  public int[] performOperation(int item)
  {
    // Monkey inspects item
    inspectedItems++;
    string newExpression = operation.Replace("old", item.ToString());
    string[] operators = newExpression.Split(' ');
    
    int result = 0;
    switch (operators[1])
    {
      case "+":
        result = int.Parse(operators[0]) + int.Parse(operators[2]);
        break;
      case "*":
        result = int.Parse(operators[0]) * int.Parse(operators[2]);
        break;
    }

    // Relief deduction
    result /= 3;

    // Monkey tests item
    bool isDivisible = result % divisor == 0;
    return new int[] { isDivisible ? onTrueResult : onFalseResult, result };
  }
}
