using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

public class Program
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
                stopWatch.Stop();
                Console.WriteLine($"File read ({stopWatch.Elapsed.TotalMilliseconds} ms)");

                Stopwatch part1Timer = new Stopwatch();
                part1Timer.Start();
                long part1 = SolvePart1(lines);
                part1Timer.Stop();
                Console.WriteLine($"Part 1: {part1} ({part1Timer.Elapsed.TotalMilliseconds} ms)");

                Stopwatch part2Timer = new Stopwatch();
                part2Timer.Start();
                long part2 = SolvePart2(lines);
                part2Timer.Stop();
                Console.WriteLine($"Part 2: {part2} ({part2Timer.Elapsed.TotalMilliseconds} ms)");
            }
            else
            {
                throw new ArgumentException("Invalid file name provided. Please provide a valid file name.");
            }
        }
        else
        {
            throw new ArgumentException("Input data file name not provided. Please provide the file name as an argument: dotnet run <file-name>");
        }
    }

    static long SolvePart1(string[] lines)
    {
        HashSet<(int x, int y, int z)> activeCubes = new();
        HashSet<(int x, int y, int z)> locationsToCheck = new();

        for (int y = 0; y < lines.Length; y++)
        {
            for (int x = 0; x < lines[y].Length; x++)
            {
                if (lines[y][x] == '#')
                {
                    activeCubes.Add((x, y, 0));
                }
            }
        }

        for (int y = -1; y <= lines.Length; y++)
        {
            for (int x = -1; x <= lines.Length; x++)
            {
                locationsToCheck.Add((x, y, 0));
            }
        }

        // Add locations in z - 1 and z + 1 to check
        foreach ((int x, int y, int z) location in new HashSet<(int x, int y, int z)>(locationsToCheck))
        {
            locationsToCheck.Add((location.x, location.y, location.z - 1));
            locationsToCheck.Add((location.x, location.y, location.z + 1));
        }

        for (int i = 0; i < 6; i++)
        {
            HashSet<(int x, int y, int z)> activeCubesCopy = new(activeCubes);
            HashSet<(int x, int y, int z)> locationsToCheckCopy = new(locationsToCheck);
            foreach ((int x, int y, int z) location in locationsToCheck)
            {
                HashSet<(int x, int y, int z)> checkedLocations = CountActiveNeighbours(activeCubes, location, out int count);
                locationsToCheckCopy.UnionWith(checkedLocations);
                if (activeCubes.Contains(location))
                {
                    // Current cube is active
                    if (count < 2 || count > 3)
                    {
                        activeCubesCopy.Remove(location);
                    }
                }
                else
                {
                    // Current cube is inactive
                    if (count == 3)
                    {
                        activeCubesCopy.Add(location);
                    }
                }
            }

            activeCubes = activeCubesCopy;
            locationsToCheck = locationsToCheckCopy;
        }

        return activeCubes.Count;
    }

    static long SolvePart2(string[] lines)
    {
        HashSet<(int x, int y, int z, int w)> activeCubes = new();

        for (int y = 0; y < lines.Length; y++)
        {
            for (int x = 0; x < lines[y].Length; x++)
            {
                if (lines[y][x] == '#')
                {
                    activeCubes.Add((x, y, 0, 0));
                }
            }
        }
        
        for (int i = 0; i < 6; i++)
        {
            // Find all neighbours of active cubes
            // An inactive cube can only change state if it's touching an active cube
            HashSet<(int x, int y, int z, int w)> neighbours = new();
            foreach ((int x, int y, int z, int w) location in activeCubes)
            {
                neighbours.Add(location);
                neighbours.UnionWith(GetNeighboursRec(location, location, 0));
            }

            HashSet<(int x, int y, int z, int w)> activeCubesCopy = new(activeCubes);
            foreach ((int x, int y, int z, int w) location in neighbours)
            {
                int activeNeighboursCount = CountActiveNeighboursRec(activeCubes, location, location, 0);
                if (activeCubes.Contains(location))
                {
                    // Current cube is active
                    if (activeNeighboursCount < 2 || activeNeighboursCount > 3)
                    {
                        activeCubesCopy.Remove(location);
                    }
                }
                else
                {
                    // Current cube is inactive
                    if (activeNeighboursCount == 3)
                    {
                        activeCubesCopy.Add(location);
                    }
                }
            }

            activeCubes = activeCubesCopy;
        }

        return activeCubes.Count;
    }

    static HashSet<(int x, int y, int z, int w)> GetNeighboursRec((int x, int y, int z, int w) cube, (int x, int y, int z, int w) neighbour, int index)
    {
        HashSet<(int x, int y, int z, int w)> neighbours = new();

        if (index == 4)
        {
            if (cube != neighbour)
            {
                neighbours.Add(neighbour);
            }
            return neighbours;
        }

        for (int i = -1; i <= 1; i++)
        {
            switch (index)
            {
                case 0:
                    neighbours.UnionWith(GetNeighboursRec(cube, (cube.x + i, neighbour.y, neighbour.z, neighbour.w), index + 1));
                    break;
                case 1:
                    neighbours.UnionWith(GetNeighboursRec(cube, (neighbour.x, cube.y + i, neighbour.z, neighbour.w), index + 1));
                    break;
                case 2:
                    neighbours.UnionWith(GetNeighboursRec(cube, (neighbour.x, neighbour.y, cube.z + i, neighbour.w), index + 1));
                    break;
                case 3:
                    neighbours.UnionWith(GetNeighboursRec(cube, (neighbour.x, neighbour.y, neighbour.z, cube.w + i), index + 1));
                    break;
                default:
                    throw new Exception($"Invalid index {index}");
            }
        }

        return neighbours;
    }

    static int CountActiveNeighboursRec(HashSet<(int x, int y, int z, int w)> activeCubes, (int x, int y, int z, int w) cube, (int x, int y, int z, int w) neighbour, int index)
    {
        if (index == 4)
        {
            return cube != neighbour && activeCubes.Contains(neighbour) ? 1 : 0;
        }

        int total = 0;

        for (int i = -1; i <= 1; i++)
        {
            switch (index)
            {
                case 0:
                    total += CountActiveNeighboursRec(activeCubes, cube, (cube.x + i, neighbour.y, neighbour.z, neighbour.w), index + 1);
                    break;
                case 1:
                    total += CountActiveNeighboursRec(activeCubes, cube, (neighbour.x, cube.y + i, neighbour.z, neighbour.w), index + 1);
                    break;
                case 2:
                    total += CountActiveNeighboursRec(activeCubes, cube, (neighbour.x, neighbour.y, cube.z + i, neighbour.w), index + 1);
                    break;
                case 3:
                    total += CountActiveNeighboursRec(activeCubes, cube, (neighbour.x, neighbour.y, neighbour.z, cube.w + i), index + 1);
                    break;
                default:
                    throw new Exception($"Invalid index {index}");
            }
        }

        return total;
    }

    static HashSet<(int, int, int)> CountActiveNeighbours(HashSet<(int, int, int)> activeCubes, (int x, int y, int z) cube, out int activeNeighbours)
    {
        activeNeighbours = 0;
        HashSet<(int, int, int)> checkedCubes = new();

        // Check -1 plane
        // NW
        checkedCubes.Add((cube.x - 1, cube.y - 1, cube.z - 1));
        if (activeCubes.Contains((cube.x - 1, cube.y - 1, cube.z - 1)))
        {
            activeNeighbours++;
        }
        // N
        checkedCubes.Add((cube.x, cube.y - 1, cube.z - 1));
        if (activeCubes.Contains((cube.x, cube.y - 1, cube.z - 1)))
        {
            activeNeighbours++;
        }
        // NE
        checkedCubes.Add((cube.x + 1, cube.y - 1, cube.z - 1));
        if (activeCubes.Contains((cube.x + 1, cube.y - 1, cube.z - 1)))
        {
            activeNeighbours++;
        }
        // W
        checkedCubes.Add((cube.x - 1, cube.y, cube.z - 1));
        if (activeCubes.Contains((cube.x - 1, cube.y, cube.z - 1)))
        {
            activeNeighbours++;
        }
        // Same
        checkedCubes.Add((cube.x, cube.y, cube.z - 1));
        if (activeCubes.Contains((cube.x, cube.y, cube.z - 1)))
        {
            activeNeighbours++;
        }
        // E
        checkedCubes.Add((cube.x + 1, cube.y, cube.z - 1));
        if (activeCubes.Contains((cube.x + 1, cube.y, cube.z - 1)))
        {
            activeNeighbours++;
        }
        // SW
        checkedCubes.Add((cube.x - 1, cube.y + 1, cube.z - 1));
        if (activeCubes.Contains((cube.x - 1, cube.y + 1, cube.z - 1)))
        {
            activeNeighbours++;
        }
        // S
        checkedCubes.Add((cube.x, cube.y + 1, cube.z - 1));
        if (activeCubes.Contains((cube.x, cube.y + 1, cube.z - 1)))
        {
            activeNeighbours++;
        }
        // SE
        checkedCubes.Add((cube.x + 1, cube.y + 1, cube.z - 1));
        if (activeCubes.Contains((cube.x + 1, cube.y + 1, cube.z - 1)))
        {
            activeNeighbours++;
        }

        // Check 0 plane
        // NW
        checkedCubes.Add((cube.x - 1, cube.y - 1, cube.z));
        if (activeCubes.Contains((cube.x - 1, cube.y - 1, cube.z)))
        {
            activeNeighbours++;
        }
        // N
        checkedCubes.Add((cube.x, cube.y - 1, cube.z));
        if (activeCubes.Contains((cube.x, cube.y - 1, cube.z)))
        {
            activeNeighbours++;
        }
        // NE
        checkedCubes.Add((cube.x + 1, cube.y - 1, cube.z));
        if (activeCubes.Contains((cube.x + 1, cube.y - 1, cube.z)))
        {
            activeNeighbours++;
        }
        // W
        checkedCubes.Add((cube.x - 1, cube.y, cube.z));
        if (activeCubes.Contains((cube.x - 1, cube.y, cube.z)))
        {
            activeNeighbours++;
        }
        // E
        checkedCubes.Add((cube.x + 1, cube.y, cube.z));
        if (activeCubes.Contains((cube.x + 1, cube.y, cube.z)))
        {
            activeNeighbours++;
        }
        // SW
        checkedCubes.Add((cube.x - 1, cube.y + 1, cube.z));
        if (activeCubes.Contains((cube.x - 1, cube.y + 1, cube.z)))
        {
            activeNeighbours++;
        }
        // S
        checkedCubes.Add((cube.x, cube.y + 1, cube.z));
        if (activeCubes.Contains((cube.x, cube.y + 1, cube.z)))
        {
            activeNeighbours++;
        }
        // SE
        checkedCubes.Add((cube.x + 1, cube.y + 1, cube.z));
        if (activeCubes.Contains((cube.x + 1, cube.y + 1, cube.z)))
        {
            activeNeighbours++;
        }

        // Check 1 plane
        // NW
        checkedCubes.Add((cube.x - 1, cube.y - 1, cube.z + 1));
        if (activeCubes.Contains((cube.x - 1, cube.y - 1, cube.z + 1)))
        {
            activeNeighbours++;
        }
        // N
        checkedCubes.Add((cube.x, cube.y - 1, cube.z + 1));
        if (activeCubes.Contains((cube.x, cube.y - 1, cube.z + 1)))
        {
            activeNeighbours++;
        }
        // NE
        checkedCubes.Add((cube.x + 1, cube.y - 1, cube.z + 1));
        if (activeCubes.Contains((cube.x + 1, cube.y - 1, cube.z + 1)))
        {
            activeNeighbours++;
        }
        // W
        checkedCubes.Add((cube.x - 1, cube.y, cube.z + 1));
        if (activeCubes.Contains((cube.x - 1, cube.y, cube.z + 1)))
        {
            activeNeighbours++;
        }
        // Same
        checkedCubes.Add((cube.x, cube.y, cube.z + 1));
        if (activeCubes.Contains((cube.x, cube.y, cube.z + 1)))
        {
            activeNeighbours++;
        }
        // E
        checkedCubes.Add((cube.x + 1, cube.y, cube.z + 1));
        if (activeCubes.Contains((cube.x + 1, cube.y, cube.z + 1)))
        {
            activeNeighbours++;
        }
        // SW
        checkedCubes.Add((cube.x - 1, cube.y + 1, cube.z + 1));
        if (activeCubes.Contains((cube.x - 1, cube.y + 1, cube.z + 1)))
        {
            activeNeighbours++;
        }
        // S
        checkedCubes.Add((cube.x, cube.y + 1, cube.z + 1));
        if (activeCubes.Contains((cube.x, cube.y + 1, cube.z + 1)))
        {
            activeNeighbours++;
        }
        // SE
        checkedCubes.Add((cube.x + 1, cube.y + 1, cube.z + 1));
        if (activeCubes.Contains((cube.x + 1, cube.y + 1, cube.z + 1)))
        {
            activeNeighbours++;
        }

        return checkedCubes;
    }
}