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

        /**
         * This puzzle required inspecting the problem input manually to find a pattern.
         * The target is a low pulse to the module rx.
         * The module rx is fed from a conjunction module which requires a high pulse from all of its inputs.
         * We need to calculate how many button presses it takes before a high pulse is sent from each input.
         * Once these lengths are found, we can calculate the LCM to get the fewest number of button presses.
         */
        List<string> hardcodedTargetModules = new List<string>() { "vd", "ns", "bh", "dl" };
        Dictionary<string, long> loopLengths = new Dictionary<string, long>();
        for (int buttonPress = 0;; buttonPress++)
        {
          try
          {
            pushTheButton(modules, hardcodedTargetModules, ref loopLengths, buttonPress + 1);
          }
          catch (FoundDesiredState)
          {
            long total = 1;
            foreach (long loopLength in loopLengths.Values)
            {
              total = calculateLeastCommonMultiple(total, loopLength);
            }
            Console.WriteLine($"Total value: {total}");
            break;
          }
        }

        stopWatch.Stop();
        Console.WriteLine($"Elapsed Time: {stopWatch.ElapsedMilliseconds} ms");
      }
    }
  }

  static void pushTheButton(Dictionary<string, Module> modules, List<string> targetModules, ref Dictionary<string, long> loopLengths, long pushCount)
  {
    Queue<PulseDelivery> pulsesToDeliver = new Queue<PulseDelivery>();

    pulsesToDeliver.Enqueue(new PulseDelivery("button", "broadcaster", PulseType.LOW));

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
        if (destinationModule.Type == ModuleType.CONJUNCTION && targetModules.Contains(destinationModule.Name) && nextPulse == PulseType.HIGH)
        {
          if (!loopLengths.ContainsKey(destinationModule.Name))
          {
            loopLengths.Add(destinationModule.Name, pushCount);
          }
          if (loopLengths.Keys.Count == targetModules.Count)
          {
            // We have found all the loop lengths and can stop
            throw new FoundDesiredState();
          }
        }
        foreach (string outputName in destinationModule.Outputs)
        {
          pulsesToDeliver.Enqueue(new PulseDelivery(destinationModule.Name, outputName, nextPulse));
        }
      }
    }
  }

  static long calculateLeastCommonMultiple(long a, long b)
  {
    return Math.Abs(a * b) / calculateGreatestCommonDivisor(a, b);
  }

  static long calculateGreatestCommonDivisor(long a, long b)
  {
    if (b == 0)
    {
      return a;
    }

    if (b > a)
    {
      return calculateGreatestCommonDivisor(a, b % a);
    }
    else
    {
      return calculateGreatestCommonDivisor(b, a % b);
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

class FoundDesiredState : Exception
{
}
