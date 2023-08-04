using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

class BalanceBots
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        string[] lines = File.ReadAllLines(fileName);

        string valuePattern = @"value (?<value>\d+) goes to bot (?<bot>\d+)";
        string operationPattern = @"bot (?<bot>\d+) gives low to (?<outLowType>\w+) (?<outLowId>\d+) and high to (?<outHighType>\w+) (?<outHighId>\d+)";
        Regex valueRegex = new Regex(valuePattern, RegexOptions.Compiled);
        Regex operationRegex = new Regex(operationPattern, RegexOptions.Compiled);

        Dictionary<int, BalanceBot> bots = new Dictionary<int, BalanceBot>();
        Dictionary<int, Output> outputs = new Dictionary<int, Output>();

        List<string> valueInstructions = new List<string>();

        foreach (string line in lines)
        {
          if (line.StartsWith("value"))
          {
            valueInstructions.Add(line);
          }
          else if (line.StartsWith("bot"))
          {
            Match match = operationRegex.Match(line);
            int botId = int.Parse(match.Groups["bot"].Value);
            string outLowType = match.Groups["outLowType"].Value;
            int outLowId = int.Parse(match.Groups["outLowId"].Value);
            string outHighType = match.Groups["outHighType"].Value;
            int outHighId = int.Parse(match.Groups["outHighId"].Value);
            BalanceBot newBot = new BalanceBot(botId, outLowType, outLowId, outHighType, outHighId, bots, outputs);
            bots.Add(botId, newBot);
          }
          else
          {
            throw new Exception($"Invalid instruction: {line}");
          }
        }

        foreach (string instruction in valueInstructions)
        {
          Match match = valueRegex.Match(instruction);
          int botId = int.Parse(match.Groups["bot"].Value);
          int value = int.Parse(match.Groups["value"].Value);
          BalanceBot bot = bots[botId];
          if (bot != null)
          {
            bot.Value = value;
          }
          else
          {
            throw new Exception($"No bot found for id {botId}");
          }
        }

        if (outputs.ContainsKey(0) && outputs.ContainsKey(1) && outputs.ContainsKey(2))
        {
          Output output0 = outputs[0];
          Output output1 = outputs[1];
          Output output2 = outputs[2];
          Console.WriteLine($"{output0.Value} * {output1.Value} * {output2.Value} = {output0.Value * output1.Value * output2.Value}");
        }
      }
    }
  }
}

class ValueContainer
{
  public int Id { get; set; }
  protected int _value = 0;
  public virtual int Value {
    get => _value;
    set => _value = value;
  }

  public ValueContainer()
  {
  }

  public ValueContainer(int id)
  {
    Id = id;
  }
}

class BalanceBot : ValueContainer
{
  public string LowOutputType { get; set; }
  public int LowOutputId { get; set; }
  public string HighOutputType { get; set; }
  public int HighOutputId { get; set; }
  public Dictionary<int, BalanceBot> Bots { get; set; }
  public Dictionary<int, Output> Outputs { get; set; }

  public override int Value {
    get { return _value; }
    set
    {
      if (_value != 0)
      {
        int lowValue;
        int highValue;
        if (_value > value)
        {
          lowValue = value;
          highValue = _value;
        }
        else
        {
          lowValue = _value;
          highValue = value;
        }
        _value = 0;
        setValue(LowOutputType, LowOutputId, lowValue, Bots, Outputs);
        setValue(HighOutputType, HighOutputId, highValue, Bots, Outputs);
      }
      else
      {
        _value = value;
      }
    }
  }

  public BalanceBot(int id, string lowOutputType, int lowOutputId, string highOutputType, int highOutputId, Dictionary<int, BalanceBot> bots, Dictionary<int, Output> outputs) : base(id)
  {
    LowOutputType = lowOutputType;
    LowOutputId = lowOutputId;
    HighOutputType = highOutputType;
    HighOutputId = highOutputId;
    Bots = bots;
    Outputs = outputs;
  }

  private void setValue(string outputType, int outputId, int value, Dictionary<int, BalanceBot> bots, Dictionary<int, Output> outputs)
  {
    switch (outputType)
    {
      case "bot":
        bots[outputId].Value = value;
        return;
      case "output":
        if (!outputs.ContainsKey(outputId))
        {
          outputs.Add(outputId, new Output(outputId));
        }
        outputs[outputId].Value = value;
        return;
      default:
        throw new Exception($"Invalid output type: {outputType}");
    }
  }
}

class Output : ValueContainer
{
  public Output(int id) : base(id)
  {

  }
}
