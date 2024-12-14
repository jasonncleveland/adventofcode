using System;
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
        long total = 0;

        for (int i = 0; i < lines.Length; i += 4)
        {
            string[] buttonAParts = lines[i].Split(":")[1].Trim().Split(", ");
            (int x, int y, int p) buttonA = (int.Parse(buttonAParts[0].Split('+')[1]), int.Parse(buttonAParts[1].Split('+')[1]), 0);
            string[] buttonBParts = lines[i + 1].Split(":")[1].Trim().Split(", ");
            (int x, int y, int p) buttonB = (int.Parse(buttonBParts[0].Split('+')[1]), int.Parse(buttonBParts[1].Split('+')[1]), 0);
            string[] prizeParts = lines[i + 2].Split(":")[1].Trim().Split(", ");
            (int x, int y) prize = (int.Parse(prizeParts[0].Split('=')[1]), int.Parse(prizeParts[1].Split('=')[1]));

            double[,] matrix =
            {
                { buttonA.x, buttonB.x, prize.x },
                { buttonA.y, buttonB.y, prize.y }
            };
            double[] buttonPresses = gaussianElimination(matrix, matrix.GetLength(1) - 1);
            if (Math.Round(buttonPresses[0], 2) % 1 == 0 && Math.Round(buttonPresses[1], 2) % 1 == 0)
            {
                total += (long) Math.Round(buttonPresses[0], 2) * 3 + (long) Math.Round(buttonPresses[1], 2);
            }
        }

        return total;
    }

    static long SolvePart2(string[] lines)
    {
        long total = 0;

        // TODO: Implement logic to solve part 2

        return total;
    }

    // Gaussian Elimination code taken from https://www.geeksforgeeks.org/gaussian-elimination/

    // Function to get matrix content
    static double[] gaussianElimination(double[,] matrix, int unkownCount)
    {
        int singular_flag = forwardElimination(matrix, unkownCount);

        // if matrix is singular
        if (singular_flag != -1) 
        {
            throw new Exception("Singular Matrix.");

            // if the RHS of equation corresponding to zero row  is 0, * system has infinitely many solutions, else inconsistent
            if (matrix[singular_flag, unkownCount] != 0)
            {
                throw new Exception("Inconsistent System.");
            }
            else
            {
                throw new Exception("May have infinitely many solutions.");
            }

            return [];
        }

        // get solution to system and print it using backward substitution
        return backSubstitution(matrix, unkownCount);
    }
     
    // Function to reduce matrix to r.e.f.
    static int forwardElimination(double[,] matrix, int unkownCount)
    {
        for (int k = 0; k < unkownCount; k++) 
        {
            // Initialize maximum value and index for pivot
            int i_max = k;
            int v_max = (int) matrix[i_max, k];

            // find greater amplitude for pivot if any
            for(int i = k + 1; i < unkownCount; i++)
            {
                if (Math.Abs(matrix[i, k]) > v_max) 
                {
                    v_max = (int) matrix[i, k];
                    i_max = i;
                }

                // If a principal diagonal element  is zero, it denotes that matrix is singular, and will lead to a division-by-zero later.
                if (matrix[k, i_max] == 0)
                {
                    // Matrix is singular
                    return k;
                }

                // Swap the greatest value row with current row
                if (i_max != k)
                {
                    swap_row(matrix, unkownCount, k, i_max);
                }
                 
                for(int a = k + 1; a < unkownCount; a++)
                {
                    // factor f to set current row kth element to 0, and subsequently remaining kth column to 0
                    double f = matrix[i, k] / matrix[k, k];

                    // subtract fth multiple of corresponding kth row element
                    for (int j = k + 1; j <= unkownCount; j++)
                    {
                        matrix[i, j] -= matrix[k, j] * f;
                    }

                    // filling lower triangular matrix with zeros
                    matrix[i, k] = 0;
                }
            }
        }
        return -1;
    }
 
    // Function for elementary operation of swapping two rows
    static void swap_row(double[,] matrix, int unkownCount, int i, int j)
    {
        for (int k = 0; k <= unkownCount; k++)
        {
            double temp = matrix[i, k];
            matrix[i, k] = matrix[j, k];
            matrix[j, k] = temp;
        }
    }
     
    // Function to calculate the values of the unknowns
    static double[] backSubstitution(double[,] matrix, int unkownCount)
    {
        // An array to store solution
        double[] solution = new double[unkownCount]; 

        // Start calculating from last equation up to the first
        for (int i = unkownCount - 1; i >= 0; i--)
        {
            // start with the RHS of the equation
            solution[i] = matrix[i, unkownCount];

            // Initialize j to i+1 since matrix is upper triangular
            for(int j = i + 1; j < unkownCount; j++) 
            {
                // subtract all the lhs values except the coefficient of the variable whose value is being calculated
                solution[i] -= matrix[i,j] * solution[j];
            }

            // divide the RHS by the coefficient of the unknown being calculated
            solution[i] = solution[i] / matrix[i,i];
        }

        return solution;
    }
}