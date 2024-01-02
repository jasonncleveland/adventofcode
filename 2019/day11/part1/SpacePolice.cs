using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

class SpacePolice
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

        foreach (string line in lines)
        {
          IntCodeComputer computer = new IntCodeComputer(line);

          Dictionary<string, long> paintedSquares = new Dictionary<string, long>();

          // Start painting at 0,0 on a black panel facing up
          int currentX = 0;
          int currentY = 0;

          Direction currentDirection = Direction.UP;
          computer.InputValues.Enqueue(0);

          while (true)
          {
            try
            {
              computer.processNextInstruction();
            }
            catch (NoInputValues)
            {
              // Stop processing if there are no input values
              long paintColour = computer.OutputValues.Dequeue();
              long directionChange = computer.OutputValues.Dequeue();

              // Paint the current square based on the output
              if (!paintedSquares.ContainsKey($"{currentY},{currentX}"))
              {
                paintedSquares.Add($"{currentY},{currentX}", paintColour);
              }
              else
              {
                paintedSquares[$"{currentY},{currentX}"] = paintColour;
              }

              // Figure out which direction to move next
              currentDirection = getNextDirection(currentDirection, directionChange);
              switch (currentDirection)
              {
                case Direction.LEFT:
                  currentX -= 1;
                  break;
                case Direction.RIGHT:
                  currentX += 1;
                  break;
                case Direction.UP:
                  currentY -= 1;
                  break;
                case Direction.DOWN:
                  currentY += 1;
                  break;
                default:
                  throw new Exception($"Invalid direction received: {currentDirection}");
              }

              // Get the colour of the next square and provide it as input
              long nextPaintColour = 0;
              if (paintedSquares.ContainsKey($"{currentY},{currentX}"))
              {
                nextPaintColour = paintedSquares[$"{currentY},{currentX}"];
              }
              computer.InputValues.Enqueue(nextPaintColour);
              continue;
            }
            catch (EndOfProgram)
            {
              // The program has halted
              break;
            }
          }
          Console.WriteLine($"Number of painted squares: {paintedSquares.Keys.Count}");
        }

        stopWatch.Stop();
        Console.WriteLine($"Elapsed Time: {stopWatch.ElapsedMilliseconds} ms");
      }
    }
  }

  static Direction getNextDirection(Direction currentDirection, long directionChange)
  {
    if (directionChange == 0)
    {
      // Turn left 90 degrees
      switch (currentDirection)
      {
        case Direction.LEFT:
          return Direction.DOWN;
        case Direction.RIGHT:
          return Direction.UP;
        case Direction.UP:
          return Direction.LEFT;
        case Direction.DOWN:
          return Direction.RIGHT;
        default:
          throw new Exception($"Invalid direction received: {currentDirection}");
      }
    }
    else if (directionChange == 1)
    {
      // Turn right 90 degrees
      switch (currentDirection)
      {
        case Direction.LEFT:
          return Direction.UP;
        case Direction.RIGHT:
          return Direction.DOWN;
        case Direction.UP:
          return Direction.RIGHT;
        case Direction.DOWN:
          return Direction.LEFT;
        default:
          throw new Exception($"Invalid direction received: {currentDirection}");
      }
    }
    else
    {
      throw new Exception($"Invalid direction change received: {directionChange}");
    }
  }
}

enum Direction
{
  LEFT,
  RIGHT,
  UP,
  DOWN,
}

class IntCodeComputer
{
  private bool Debug { get; set; } = false;

  private long[] Memory { get; set; }
  private long InstructionPointer { get; set; }
  private long RelativeBasePointer { get; set; }

  public Queue<long> InputValues { get; set; }
  public Queue<long> OutputValues { get; set; }

  public IntCodeComputer(string program)
  {
    // The computer's available memory should be much larger than the initial program.
    Memory = new long[10000];
    long[] instructions = Array.ConvertAll<string, long>(program.Split(','), operation => long.Parse(operation));
    Array.Copy(instructions, Memory, instructions.Length);
    InstructionPointer = 0;
    RelativeBasePointer = 0;
    InputValues = new Queue<long>();
    OutputValues = new Queue<long>();
  }

  private long getParameterAddress(long mode, long offset)
  {
    switch (mode)
    {
      case 0:
        // Position mode
        if (Debug) Console.WriteLine($"A. Position mode: Memory[{InstructionPointer} + {offset}] {Memory[InstructionPointer + offset]}");
        return Memory[InstructionPointer + offset];
      case 1:
        // Immediate mode
        if (Debug) Console.WriteLine($"A. Immediiate mode: {InstructionPointer} + {offset} {InstructionPointer + offset}");
        return InstructionPointer + offset;
      case 2:
        // Relative mode
        if (Debug) Console.WriteLine($"A. Relative mode: {RelativeBasePointer} + Memory[{InstructionPointer} + {offset}] {RelativeBasePointer + Memory[InstructionPointer + offset]}");
        return RelativeBasePointer + Memory[InstructionPointer + offset];
      default:
        throw new Exception($"Invalid mode {mode}");
    }
  }

  private long getParameterValue(long mode, long offset)
  {
    switch (mode)
    {
      case 0:
        // Position mode
        if (Debug) Console.WriteLine($"V. Position mode: Memory[{InstructionPointer} + {offset}] {Memory[Memory[InstructionPointer + offset]]}");
        return Memory[Memory[InstructionPointer + offset]];
      case 1:
        // Immediate mode
        if (Debug) Console.WriteLine($"V. Immediiate mode: {InstructionPointer} + {offset} {Memory[InstructionPointer + offset]}");
        return Memory[InstructionPointer + offset];
      case 2:
        // Relative mode
        if (Debug) Console.WriteLine($"V. Relative mode: {RelativeBasePointer} + {InstructionPointer + Memory[InstructionPointer + offset]} {Memory[RelativeBasePointer + Memory[InstructionPointer + offset]]}");
        return Memory[RelativeBasePointer + Memory[InstructionPointer + offset]];
      default:
        throw new Exception($"Invalid mode {mode}");
    }
  }

  
  public void processNextInstruction()
  {
    long instruction = Memory[InstructionPointer];
    if (Debug) Console.WriteLine($"Getting instruction at location {InstructionPointer}: {instruction}");
    if (instruction == 99) throw new EndOfProgram();

    // Parse the instruction to get the op code and modes
    long opCode = instruction % 100;
    long parameter1Mode = instruction % 1000 / 100;
    long parameter2Mode = instruction % 10000 / 1000;
    long parameter3Mode = instruction % 100000 / 10000;

    long parameter1Value, parameter2Value;
    long outputAddress, jumpAddress;

    switch (opCode)
    {
      case 1:
        // Add
        parameter1Value = getParameterValue(parameter1Mode, 1);
        parameter2Value = getParameterValue(parameter2Mode, 2);
        outputAddress = getParameterAddress(parameter3Mode, 3);
        if (Debug) Console.WriteLine($"> Computing {parameter1Value} + {parameter2Value} = {parameter1Value + parameter2Value} and storing at address {outputAddress}");
        Memory[outputAddress] = parameter1Value + parameter2Value;
        InstructionPointer += 4;
        break;
      case 2:
        // Multiply
        parameter1Value = getParameterValue(parameter1Mode, 1);
        parameter2Value = getParameterValue(parameter2Mode, 2);
        outputAddress = getParameterAddress(parameter3Mode, 3);
        if (Debug) Console.WriteLine($"> Computing {parameter1Value} * {parameter2Value} = {parameter1Value * parameter2Value} and storing at address {outputAddress}");
        Memory[outputAddress] = parameter1Value * parameter2Value;
        InstructionPointer += 4;
        break;
      case 3:
        // Store
        if (InputValues.Count > 0)
        {
          outputAddress = getParameterAddress(parameter1Mode, 1);
          if (Debug) Console.WriteLine($"> Storing {InputValues.Peek()} at address {outputAddress}");
          Memory[outputAddress] = InputValues.Dequeue();
          InstructionPointer += 2;
          break;
        }
        else
        {
          throw new NoInputValues();
        }
      case 4:
        // Load
        parameter1Value = getParameterValue(parameter1Mode, 1);
        if (Debug) Console.WriteLine($"> Fetching value {parameter1Value}");
        OutputValues.Enqueue(parameter1Value);
        InstructionPointer += 2;
        break;
      case 5:
        // Jump if True
        parameter1Value = getParameterValue(parameter1Mode, 1);
        jumpAddress = getParameterValue(parameter2Mode, 2);
        if (Debug) Console.WriteLine($"> Checking {parameter1Value} != 0 => {parameter1Value != 0} and jumping to address {jumpAddress} if true");
        if (parameter1Value != 0)
        {
          InstructionPointer = jumpAddress;
        }
        else
        {
          InstructionPointer += 3;
        }
        break;
      case 6:
        // Jump if False
        parameter1Value = getParameterValue(parameter1Mode, 1);
        jumpAddress = getParameterValue(parameter2Mode, 2);
        if (Debug) Console.WriteLine($"> Checking {parameter1Value} == 0 => {parameter1Value == 0} and jumping to address {jumpAddress} if true");
        if (parameter1Value == 0)
        {
          InstructionPointer = jumpAddress;
        }
        else
        {
          InstructionPointer += 3;
        }
        break;
      case 7:
        // Less Than
        parameter1Value = getParameterValue(parameter1Mode, 1);
        parameter2Value = getParameterValue(parameter2Mode, 2);
        outputAddress = getParameterAddress(parameter3Mode, 3);
        if (Debug) Console.WriteLine($"> Comparing {parameter1Value} < {parameter2Value} => {parameter1Value < parameter2Value} and storing at address {outputAddress}");
        Memory[outputAddress] = parameter1Value < parameter2Value ? 1 : 0;
        InstructionPointer += 4;
        break;
      case 8:
        // Equals
        parameter1Value = getParameterValue(parameter1Mode, 1);
        parameter2Value = getParameterValue(parameter2Mode, 2);
        outputAddress = getParameterAddress(parameter3Mode, 3);
        if (Debug) Console.WriteLine($"> Comparing {parameter1Value} == {parameter2Value} => {parameter1Value == parameter2Value} and storing at address {outputAddress}");
        Memory[outputAddress] = parameter1Value == parameter2Value ? 1 : 0;
        InstructionPointer += 4;
        break;
      case 9:
        // Relative Base
        parameter1Value = getParameterValue(parameter1Mode, 1);
        if (Debug) Console.WriteLine($"> Setting relative base value {RelativeBasePointer} + {parameter1Value} = {RelativeBasePointer + parameter1Value}");
        RelativeBasePointer += parameter1Value;
        InstructionPointer += 2;
        break;
      default:
        throw new Exception($"Invalid code {opCode}");
    }
  }

  public void run()
  {
    while (true)
    {
      try
      {
        processNextInstruction();
      }
      catch (NoInputValues)
      {
        // Stop processing if there are no input values
        return;
      }
      catch (EndOfProgram)
      {
        // The program has halted
        return;
      }
    }
  }

  public void dumpMemory()
  {
    Console.WriteLine(string.Join(',', Memory));
  }
}

class NoInputValues : Exception
{
}

class EndOfProgram : Exception
{
}
