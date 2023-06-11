// See https://aka.ms/new-console-template for more information
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Thrading;

class Program
{
    static void Main(string[] args)
    {
        Tasks.TaskCheck();
        ForeachWithLocal();
        ForWithLocal();
        CheckFor();


        var numbers = new List<int>();
        for (int i = 0; i < 100000; i++)
        {
            numbers.Add(i);
        }
        Stopwatch stopwatch1 = new Stopwatch();
        Stopwatch stopwatch2 = new Stopwatch();

        stopwatch1.Start();
        CheckNormal(numbers);

        stopwatch1.Stop();
        Console.WriteLine("t1 = " + stopwatch1.ElapsedMilliseconds);

        stopwatch2.Start();
        CheckParallel(numbers);

        stopwatch2.Stop();
        Console.WriteLine("t2 = " + stopwatch2.ElapsedMilliseconds);
        Console.WriteLine("total effct = " + 1.0 * stopwatch1.ElapsedMilliseconds / stopwatch2.ElapsedMilliseconds);
    }



    static void CheckNormal(List<int> sourceCollection)
    {
        // Sequential version
        foreach (var item in sourceCollection)
        {
            Process(item);
        }

    }
    static void CheckParallel(List<int> sourceCollection)
    {
        // Parallel equivalent
        Parallel.ForEach(sourceCollection, item => Process(item));

    }

    static void Process(int num)
    {
        for (int i = 0; i < num; i++)
        {
            var a = 100;
        }
    }

    static void ForWithLocal()
    {
        int[] nums = Enumerable.Range(0, 1_000_000).ToArray();
        long total = 0;

        // Use type parameter to make subtotal a long, not an int
        Parallel.For<long>(0, nums.Length, () => 0, (j, loop, subtotal) =>
        {
            subtotal += nums[j];
            return subtotal;
        },
            subtotal => Interlocked.Add(ref total, subtotal)
        );

        Console.WriteLine("The total is {0:N0}", total);
        Console.WriteLine("Press any key to exit");
        Console.ReadKey();
    }
    static void ForeachWithLocal()
    {
        int[] nums = Enumerable.Range(0, 1000000).ToArray();
        long total = 0;

        // First type parameter is the type of the source elements
        // Second type parameter is the type of the thread-local variable (partition subtotal)
        Parallel.ForEach<int, long>(nums, // source collection
                                    () => 0, // method to initialize the local variable
                                    (j, loop, subtotal) => // method invoked by the loop on each iteration
                                    {
                                        subtotal += j; //modify local variable
                                        return subtotal; // value to be passed to next iteration
                                    },
                                    // Method to be executed when each partition has completed.
                                    // finalResult is the final value of subtotal for a particular partition.
                                    (finalResult) => Interlocked.Add(ref total, finalResult)
                                    );

        Console.WriteLine("The total from Parallel.ForEach is {0:N0}", total);
    }
    static void ForeachCancel()
    {
        int[] nums = Enumerable.Range(0, 10000000).ToArray();
        CancellationTokenSource cts = new CancellationTokenSource();

        // Use ParallelOptions instance to store the CancellationToken
        ParallelOptions po = new ParallelOptions();
        po.CancellationToken = cts.Token;
        po.MaxDegreeOfParallelism = System.Environment.ProcessorCount;
        Console.WriteLine("Press any key to start. Press 'c' to cancel.");
        Console.ReadKey();

        // Run a task so that we can cancel from another thread.
        Task.Factory.StartNew(() =>
        {
            if (Console.ReadKey().KeyChar == 'c')
                cts.Cancel();
            Console.WriteLine("press any key to exit");
        });

        try
        {
            Parallel.ForEach(nums, po, (num) =>
            {
                double d = Math.Sqrt(num);
                Console.WriteLine("{0} on {1}", d, Thread.CurrentThread.ManagedThreadId);
            });
        }
        catch (OperationCanceledException e)
        {
            Console.WriteLine(e.Message);
        }
        finally
        {
            cts.Dispose();
        }

        Console.ReadKey();
    }


    static void CheckFor()
    {
        int colCount = 10;
        int rowCount = 20;
        int colCount2 = 27;
        double[,] m1 = InitializeMatrix(rowCount, colCount);
        double[,] m2 = InitializeMatrix(colCount, colCount2);
        double[,] result = new double[rowCount, colCount2];

        // First do the sequential version.
        Console.Error.WriteLine("Executing sequential loop...");
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        MultiplyMatricesSequential(m1, m2, result);
        stopwatch.Stop();
        Console.Error.WriteLine("Sequential loop time in milliseconds: {0}",
                                stopwatch.ElapsedMilliseconds);

        // For the skeptics.
        OfferToPrint(rowCount, colCount2, result);

        // Reset timer and results matrix.
        stopwatch.Reset();
        result = new double[rowCount, colCount2];

        // Do the parallel loop.
        Console.Error.WriteLine("Executing parallel loop...");
        stopwatch.Start();
        MultiplyMatricesParallel(m1, m2, result);
        stopwatch.Stop();
        Console.Error.WriteLine("Parallel loop time in milliseconds: {0}",
                                stopwatch.ElapsedMilliseconds);
        OfferToPrint(rowCount, colCount2, result);

        // Keep the console window open in debug mode.
        Console.Error.WriteLine("Press any key to exit.");
        Console.ReadKey();
    }

    static double[,] InitializeMatrix(
        int rows, 
        int cols)
    {
        double[,] matrix = new double[rows, cols];

        Random r = new Random();
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                matrix[i, j] = r.Next(100);
            }
        }
        return matrix;
    }

    private static void OfferToPrint(
        int rowCount, 
        int colCount, 
        double[,] matrix)
    {
        Console.Error.Write("Computation complete. Print results (y/n)? ");
        char c = Console.ReadKey(true).KeyChar;
        Console.Error.WriteLine(c);
        if (Char.ToUpperInvariant(c) == 'Y')
        {
            if (!Console.IsOutputRedirected &&
                RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Console.WindowWidth = 180;
            }

            Console.WriteLine();
            for (int x = 0; x < rowCount; x++)
            {
                Console.WriteLine("ROW {0}: ", x);
                for (int y = 0; y < colCount; y++)
                {
                    Console.Write("{0:#.##} ", matrix[x, y]);
                }
                Console.WriteLine();
            }
        }
    }
    static void MultiplyMatricesParallel(
        double[,] matA, 
        double[,] matB, 
        double[,] result)
    {
        int matACols = matA.GetLength(1);
        int matBCols = matB.GetLength(1);
        int matARows = matA.GetLength(0);

        // A basic matrix multiplication.
        // Parallelize the outer loop to partition the source array by rows.
        Parallel.For(0, matARows, i =>
        {
            for (int j = 0; j < matBCols; j++)
            {
                double temp = 0;
                for (int k = 0; k < matACols; k++)
                {
                    temp += matA[i, k] * matB[k, j];
                }
                result[i, j] = temp;
            }
        }); // Parallel.For
    }
    static void MultiplyMatricesSequential(
        double[,] matA, 
        double[,] matB,
        double[,] result)
    {
        int matACols = matA.GetLength(1);
        int matBCols = matB.GetLength(1);
        int matARows = matA.GetLength(0);

        for (int i = 0; i < matARows; i++)
        {
            for (int j = 0; j < matBCols; j++)
            {
                double temp = 0;
                for (int k = 0; k < matACols; k++)
                {
                    temp += matA[i, k] * matB[k, j];
                }
                result[i, j] += temp;
            }
        }
    
    }
}