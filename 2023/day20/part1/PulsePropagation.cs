using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

class PulsePropagation
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      int buttonPushes = args.Length > 1 ? int.Parse(args[1]) : 1000;
      if (File.Exists(fileName))
      {
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();

        string[] lines = File.ReadAllLines(fileName);

        Dictionary<string, Module> modules = new Dictionary<string, Module>();

        foreach (string line in lines)
        {
          string[] lineParts = line.Split(" -> ", StringSplitOptions.RemoveEmptyEntries);
          ModuleType moduleType;
          string moduleName = lineParts[0];
          if (moduleName.StartsWith("%"))
          {
            moduleType = ModuleType.FLIP_FLOP;
            moduleName = moduleName.Substring(1);
          }
          else if (moduleName.StartsWith("&"))
          {
            moduleType = ModuleType.CONJUNCTION;
            moduleName = moduleName.Substring(1);
          }
          else if (moduleName == "broadcaster")
          {
            moduleType = ModuleType.BROADCAST;
          }
          else
          {
            throw new Exception($"Found invalid module: {moduleName}");
          }
          string[] outputs = lineParts[1].Split(", ", StringSplitOptions.RemoveEmptyEntries);
          modules.Add(moduleName, new Module(moduleName, moduleType, new List<string>(outputs)));
        }

        // Set expected inputs for all modules
        foreach (Module module in modules.Values)
        {
          foreach (string outputName in module.Outputs)
          {
            if (!modules.ContainsKey(outputName))
            {
              continue;
            }
            Module outputModule = modules[outputName];
            outputModule.addInput(module.Name);
          }
        }

        int lowPulseCount = 0;
        int highPulseCount = 0;
        for (int i = 0; i < buttonPushes; i++)
        {
          pushTheButton(modules, ref lowPulseCount, ref highPulseCount);
        }
        Console.WriteLine($"Low pulses: {lowPulseCount} High pulses: {highPulseCount} Product: {lowPulseCount * highPulseCount}");

        stopWatch.Stop();
        Console.WriteLine($"Elapsed Time: {stopWatch.ElapsedMilliseconds} ms");
      }
    }
  }

  static void pushTheButton(Dictionary<string, Module> modules, ref int lowPulseCount, ref int highPulseCount)
  {
    Queue<PulseDelivery> pulsesToDeliver = new Queue<PulseDelivery>();

    pulsesToDeliver.Enqueue(new PulseDelivery("button", "broadcaster", PulseType.LOW));
    lowPulseCount += 1;

    while (pulsesToDeliver.Count > 0)
    {
      PulseDelivery currentPulse = pulsesToDeliver.Dequeue();
      if (!modules.ContainsKey(currentPulse.DestinationName))
      {
        continue;
      }
      Module destinationModule = modules[currentPulse.DestinationName];
      PulseType nextPulse = destinationModule.receivePulse(currentPulse.SourceName, currentPulse.Pulse);
      if (nextPulse != PulseType.INVALID)
      {
        foreach (string outputName in destinationModule.Outputs)
        {
          switch (nextPulse)
          {
            case PulseType.LOW:
              lowPulseCount += 1;
              break;
            case PulseType.HIGH:
              highPulseCount += 1;
              break;
            default:
              throw new Exception($"Invalid pulse type received {nextPulse}");
          }
          pulsesToDeliver.Enqueue(new PulseDelivery(destinationModule.Name, outputName, nextPulse));
        }
      }
    }
  }
}

enum ModuleType
{
  FLIP_FLOP,
  CONJUNCTION,
  BROADCAST,
}

enum PulseType
{
  LOW,
  HIGH,
  INVALID,
}

class Module
{
  public string Name { get; set; }
  public ModuleType Type { get; set; }
  public List<string> Outputs { get; set; }
  public HashSet<string> Inputs { get; set; }
  public PulseType Pulse { get; set; }
  // Used for flip-flop modules
  public bool State { get; set; }
  // Used for conjunction modules
  public Dictionary<string, PulseType> ReceivedPulses { get; set; }

  public Module(string name, ModuleType type, List<string> outputs)
  {
    Name = name;
    Type = type;
    Outputs = outputs;
    Inputs = new HashSet<string>();

    switch (type)
    {
      case ModuleType.FLIP_FLOP:
        State = false;
        break;
      case ModuleType.CONJUNCTION:
        ReceivedPulses = new Dictionary<string, PulseType>();
        break;
      case ModuleType.BROADCAST:
        break;
      default:
        throw new Exception($"Invalid type found: {type}");
    }
  }

  public void addInput(string sourceName)
  {
    Inputs.Add(sourceName);
  }

  public PulseType receivePulse(string sourceName, PulseType pulse)
  {
    switch (Type)
    {
      case ModuleType.FLIP_FLOP:
        if (pulse == PulseType.LOW)
        {
          // Do something
          State = !State;
          return State ? PulseType.HIGH : PulseType.LOW;
        }
        else
        {
          // Do nothing
          return PulseType.INVALID;
        }
      case ModuleType.CONJUNCTION:
        ReceivedPulses[sourceName] = pulse;

        bool allHigh = true;
        if (ReceivedPulses.Keys.Count == Inputs.Count)
        {
          foreach (PulseType receivedPulse in ReceivedPulses.Values)
          {
            if (receivedPulse != PulseType.HIGH)
            {
              allHigh = false;
            }
          }
        }
        else
        {
          allHigh = false;
        }
        return allHigh ? PulseType.LOW : PulseType.HIGH;
      case ModuleType.BROADCAST:
        return pulse;
      default:
        throw new Exception($"Invalid type found: {Type}");
    }
  }
}

class PulseDelivery
{
  public string SourceName { get; set; }
  public string DestinationName { get; set; }
  public PulseType Pulse { get; set; }

  public PulseDelivery(string sourceName, string destinationName, PulseType pulse)
  {
    SourceName = sourceName;
    DestinationName = destinationName;
    Pulse = pulse;
  }
}
