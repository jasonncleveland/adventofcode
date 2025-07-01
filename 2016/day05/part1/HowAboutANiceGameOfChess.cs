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

        foreach (string line in lines)
        {
          HashAlgorithm mD5 = MD5.Create();

          List<char> password = new List<char>();
          int number = 0;
          while (password.Count < 8)
          {
            byte[] inputBytes = Encoding.ASCII.GetBytes($"{line}{number}");
            byte[] hash = mD5.ComputeHash(inputBytes);
            string hashString = BitConverter.ToString(hash).Replace("-", String.Empty);
            if (hashString.StartsWith("00000"))
            {
              password.Add(hashString[5]);
            }
            number++;
          }

          Console.WriteLine($"Password: {new string(password.ToArray())}");
        }
      }
    }
  }
}
