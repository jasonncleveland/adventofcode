using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

class HowAboutANiceGameOfChess
{
  static void Main(string[] args)
  {
    if (args.Length > 0)
    {
      string fileName = args[0];
      if (File.Exists(fileName))
      {
        string[] lines = File.ReadAllLines(fileName);

        HashAlgorithm mD5 = MD5.Create();
        foreach (string line in lines)
        {
          var watch = System.Diagnostics.Stopwatch.StartNew();

          char[] password = new char[8];
          bool[] found = new bool[8];
          int foundCount = 0;
          int number = 0;
          while (foundCount < 8)
          {
            byte[] inputBytes = Encoding.ASCII.GetBytes($"{line}{number}");
            byte[] hash = mD5.ComputeHash(inputBytes);

            if (hash[0] == 0 && hash[1] == 0 && hash[2] < 8 && !found[(int) hash[2]])
            {
              string hashString = BitConverter.ToString(hash).Replace("-", String.Empty);
              password[hashString[5] - '0'] = hashString[6];
              found[hashString[5] - '0'] = true;
              foundCount++;
            }
            number++;
          }
          watch.Stop();

          Console.WriteLine($"Password: {new string(password)} ({watch.ElapsedMilliseconds})");
        }
      }
    }
  }
}
