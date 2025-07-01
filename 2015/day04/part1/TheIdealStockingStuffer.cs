using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

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

        foreach (string line in lines)
        {
          int number = 0;
          HashAlgorithm mD5 = MD5.Create();

          while (true)
          {
            byte[] inputBytes = Encoding.ASCII.GetBytes($"{line}{number}");
            byte[] hash = mD5.ComputeHash(inputBytes);
            string hashString = BitConverter.ToString(hash).Replace("-", String.Empty);
            if (hashString.StartsWith("00000"))
            {
              Console.WriteLine(hashString);
              break;
            }
            number++;
          }

          Console.WriteLine($"Lowest number: {number}");
        }
      }
    }
  }
}
