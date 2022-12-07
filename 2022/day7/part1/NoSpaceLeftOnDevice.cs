using System;
using System.Collections.Generic;
using System.IO;

class NoSpaceLeftOnDevice
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

        DirectoryData root = new DirectoryData(null, "/");
        DirectoryData currentDirectory = root;

        foreach (string line in lines)
        {
          if (line.StartsWith("$"))
          {
            // This is a command
            string[] commands = line.Split(' ');
            switch (commands[1])
            {
              case "cd":
                if (commands[2] == "/")
                {
                  currentDirectory = root;
                }
                else if (commands[2] == ".." && currentDirectory.Parent != null)
                {
                  currentDirectory = currentDirectory.Parent;
                }
                else
                {
                  DirectoryData newDirectory = currentDirectory.Directories.Find(d => d.Name == commands[2]);
                  currentDirectory = newDirectory;
                }
                break;
              case "ls":
                break;
              default:
                Console.WriteLine("How did you get here?");
                Console.WriteLine(line);
                break;
            }
          }
          else
          {
            // We are in command output
            string[] metadata = line.Split(' ');
            switch (metadata[0])
            {
              case "dir":
                DirectoryData newDirectory = new DirectoryData(currentDirectory, metadata[1]);
                currentDirectory.Directories.Add(newDirectory);
                break;
              default:
                FileData newFile = new FileData(metadata[1], int.Parse(metadata[0]));
                currentDirectory.Files.Add(newFile);
                break;
            }
          }
        }

        calculateDirectorySize(root);

        List<DirectoryData> flattenedDirectories = new List<DirectoryData>();
        flattenDirectories(root, flattenedDirectories);

        int maxSize = 100000;
        int totalSize = 0;

        foreach (DirectoryData directory in flattenedDirectories)
        {
          if (directory.SizeInBytes < maxSize)
          {
            totalSize += directory.SizeInBytes;
          }
        }

        Console.WriteLine($"Total size of directories not larger than {maxSize}: {totalSize}");
      }
    }
  }

  static int calculateDirectorySize(DirectoryData directory)
  {
    int totalDirectorySize = 0;
    
    foreach (FileData file in directory.Files)
    {
      totalDirectorySize += file.SizeInBytes;
    }

    foreach (DirectoryData childDirectory in directory.Directories)
    {
      int childDirectorySize = calculateDirectorySize(childDirectory);
      childDirectory.SizeInBytes = childDirectorySize;
      totalDirectorySize += childDirectorySize;
    }

    return totalDirectorySize;
  }

  static void flattenDirectories(DirectoryData directory, List<DirectoryData> flattenedDirectories)
  {
    foreach (DirectoryData childDirectory in directory.Directories)
    {
      flattenedDirectories.Add(childDirectory);
      flattenDirectories(childDirectory, flattenedDirectories);
    }
  }
}

public class DirectoryData
{
  public string Name { get; set; }
  public int SizeInBytes { get; set; }
  public DirectoryData Parent { get; set; }
  public List<DirectoryData> Directories { get; set; } = new List<DirectoryData>();
  public List<FileData> Files { get; set; } = new List<FileData>();

  public DirectoryData(DirectoryData parent, string name)
  {
    Parent = parent;
    Name = name;
  }
}

public class FileData
{
  public string Name { get; set; }
  public int SizeInBytes { get; set; }

  public FileData(string name, int sizeInBytes)
  {
    Name = name;
    SizeInBytes = sizeInBytes;
  }
}
