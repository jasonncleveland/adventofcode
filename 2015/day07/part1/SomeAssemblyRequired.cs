using System;
using System.IO;
using System.Collections.Generic;

class TheIdealStockingStuffer
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        string[] lines = File.ReadAllLines(fileName);

        Dictionary<string, ushort> wires = new Dictionary<string, ushort>();
        Queue<string> instructions = new Queue<string>(lines);

        while (instructions.Count > 0)
        {
          string instruction = instructions.Dequeue();
          string[] instructionParts = instruction.Split("->");
          string target = instructionParts[1].Trim();
          string operation = instructionParts[0].Trim();
          
          ushort value;
          if (ushort.TryParse(operation, out value))
          {
            // If we have just a value then store it
            wires[target] = value;
          }
          else
          {
            // Try to perform the operation
            if (operation.IndexOf("AND") > -1)
            {
              // AND
              string[] operands = operation.Split("AND");
              string leftSide = operands[0].Trim();
              string righttSide = operands[1].Trim();
              ushort left, right;
              if (!ushort.TryParse(leftSide, out left))
              {
                if (wires.ContainsKey(leftSide))
                {
                  left = wires[leftSide];
                }
                else
                {
                  // We don't have all of the required values yet
                  instructions.Enqueue(instruction);
                  continue;
                }
              }
              if (!ushort.TryParse(righttSide, out right))
              {
                if (wires.ContainsKey(righttSide))
                {
                  right = wires[righttSide];
                }
                else
                {
                  // We don't have all of the required values yet
                  instructions.Enqueue(instruction);
                  continue;
                }
              }
              wires[target] = (ushort) (left & right);
            }
            else if (operation.IndexOf("OR") > -1)
            {
              // OR
              string[] operands = operation.Split("OR");
              string leftSide = operands[0].Trim();
              string righttSide = operands[1].Trim();
              ushort left, right;
              if (!ushort.TryParse(leftSide, out left))
              {
                if (wires.ContainsKey(leftSide))
                {
                  left = wires[leftSide];
                }
                else
                {
                  // We don't have all of the required values yet
                  instructions.Enqueue(instruction);
                  continue;
                }
              }
              if (!ushort.TryParse(righttSide, out right))
              {
                if (wires.ContainsKey(righttSide))
                {
                  right = wires[righttSide];
                }
                else
                {
                  // We don't have all of the required values yet
                  instructions.Enqueue(instruction);
                  continue;
                }
              }
              wires[target] = (ushort) (left | right);
            }
            else if (operation.IndexOf("NOT") > -1)
            {
              // NOT
              string[] operands = operation.Split("NOT");
              string righttSide = operands[1].Trim();
              ushort right;
              if (!ushort.TryParse(righttSide, out right))
              {
                if (wires.ContainsKey(righttSide))
                {
                  right = wires[righttSide];
                }
                else
                {
                  // We don't have all of the required values yet
                  instructions.Enqueue(instruction);
                  continue;
                }
              }
              wires[target] = (ushort) (~right);
            }
            else if (operation.IndexOf("LSHIFT") > -1)
            {
              // LSHIFT
              string[] operands = operation.Split("LSHIFT");
              string leftSide = operands[0].Trim();
              string righttSide = operands[1].Trim();
              ushort left, right;
              if (!ushort.TryParse(leftSide, out left))
              {
                if (wires.ContainsKey(leftSide))
                {
                  left = wires[leftSide];
                }
                else
                {
                  // We don't have all of the required values yet
                  instructions.Enqueue(instruction);
                  continue;
                }
              }
              if (!ushort.TryParse(righttSide, out right))
              {
                if (wires.ContainsKey(righttSide))
                {
                  right = wires[righttSide];
                }
                else
                {
                  // We don't have all of the required values yet
                  instructions.Enqueue(instruction);
                  continue;
                }
              }
              wires[target] = (ushort) (left << right);
            }
            else if (operation.IndexOf("RSHIFT") > -1)
            {
              // RSHIFT
              string[] operands = operation.Split("RSHIFT");
              string leftSide = operands[0].Trim();
              string righttSide = operands[1].Trim();
              ushort left, right;
              if (!ushort.TryParse(leftSide, out left))
              {
                if (wires.ContainsKey(leftSide))
                {
                  left = wires[leftSide];
                }
                else
                {
                  // We don't have all of the required values yet
                  instructions.Enqueue(instruction);
                  continue;
                }
              }
              if (!ushort.TryParse(righttSide, out right))
              {
                if (wires.ContainsKey(righttSide))
                {
                  right = wires[righttSide];
                }
                else
                {
                  // We don't have all of the required values yet
                  instructions.Enqueue(instruction);
                  continue;
                }
              }
              wires[target] = (ushort) (left >> right);
            }
            else
            {
              // No operation so we must be storing one value in another
              string operand = operation.Trim();
              if (!ushort.TryParse(operand, out value))
              {
                if (wires.ContainsKey(operand))
                {
                  value = wires[operand];
                }
                else
                {
                  // We don't have all of the required values yet
                  instructions.Enqueue(instruction);
                  continue;
                }
              }
              wires[target] = value;
            }
          }
        }

        ushort aValue;
        if (wires.TryGetValue("a", out aValue))
        {
          Console.WriteLine($"Value on wire 'a': {aValue}");
        }
      }
    }
  }
}
