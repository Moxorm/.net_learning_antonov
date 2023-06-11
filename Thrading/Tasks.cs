using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thrading
{
    public class Tasks
    {
        public static void TaskCheck() 
        {
            Thread.CurrentThread.Name = "Main";

            // Create a task and supply a user delegate by using a lambda expression.
            Task taskA = new Task(() => Console.WriteLine("Hello from taskA."));
            // Start the task.
            taskA.Start();

            // Output a message from the calling thread.
            Console.WriteLine("Hello from thread '{0}'.",
                              Thread.CurrentThread.Name);
            taskA.Wait();
        }
        public static void RunImmediately()
        {
            Thread.CurrentThread.Name = "Main";

            // Define and run the task.
            Task taskA = Task.Run(() => Console.WriteLine("Hello from taskA."));

            // Output a message from the calling thread.
            Console.WriteLine("Hello from thread '{0}'.",
                                Thread.CurrentThread.Name);
            taskA.Wait();
        }

        public static void TaskFactory()
        {
            Task[] taskArray = new Task[10];
            for (int i = 0; i < taskArray.Length; i++)
            {
                taskArray[i] = Task.Factory.StartNew((Object obj) =>
                {
                    CustomData data = obj as CustomData;
                    if (data == null) return;

                    data.ThreadNum = Thread.CurrentThread.ManagedThreadId;
                },
                new CustomData() { Name = i, CreationTime = DateTime.Now.Ticks });
            }
            Task.WaitAll(taskArray);
            foreach (var task in taskArray)
            {
                var data = task.AsyncState as CustomData;
                if (data != null)
                    Console.WriteLine("Task #{0} created at {1}, ran on thread #{2}.",
                                      data.Name, data.CreationTime, data.ThreadNum);
            }
        }
        public static void TaskList()
        {
            Task<Double>[] taskArray = { Task<Double>.Factory.StartNew(() => DoComputation(1.0)),
                                     Task<Double>.Factory.StartNew(() => DoComputation(100.0)),
                                     Task<Double>.Factory.StartNew(() => DoComputation(1000.0)) };
            
            var results = new Double[taskArray.Length];
            Double sum = 0;

            for (int i = 0; i < taskArray.Length; i++)
            {
                results[i] = taskArray[i].Result;
                Console.Write("{0:N1} {1}", results[i],
                                  i == taskArray.Length - 1 ? "= " : "+ ");
                sum += results[i];
            }
            Console.WriteLine("{0:N1}", sum);
        }
        public static void ExpampleForRefIbDelegate()
        {
            // Create the task object by using an Action(Of Object) to pass in the loop
            // counter. This produces an unexpected result.
            Task[] taskArray = new Task[10];
            for (int i = 0; i < taskArray.Length; i++)
            {
                taskArray[i] = Task.Factory.StartNew((Object obj) => {
                    var data = new CustomData() { Name = i, CreationTime = DateTime.Now.Ticks };
                    data.ThreadNum = Thread.CurrentThread.ManagedThreadId;
                    Console.WriteLine("Task #{0} created at {1} on thread #{2}.",
                                      data.Name, data.CreationTime, data.ThreadNum);
                },
                                                     i);
            }
            Task.WaitAll(taskArray);
        }
        public static void ExpampleForRefIbDelegateFix()
        {
            // Create the task object by using an Action(Of Object) to pass in custom data
            // to the Task constructor. This is useful when you need to capture outer variables
            // from within a loop.
            Task[] taskArray = new Task[10];
            for (int i = 0; i < taskArray.Length; i++)
            {
                taskArray[i] = Task.Factory.StartNew((Object obj) => {
                    CustomData data = obj as CustomData;
                    if (data == null)
                        return;

                    data.ThreadNum = Thread.CurrentThread.ManagedThreadId;
                    Console.WriteLine("Task #{0} created at {1} on thread #{2}.",
                                     data.Name, data.CreationTime, data.ThreadNum);
                },
                                                      new CustomData() { Name = i, CreationTime = DateTime.Now.Ticks });
            }
            Task.WaitAll(taskArray);
        }
        public static void ContinueWith()
        {
            var getData = Task.Factory.StartNew(() => {
                Random rnd = new Random();
                int[] values = new int[100];
                for (int ctr = 0; ctr <= values.GetUpperBound(0); ctr++)
                    values[ctr] = rnd.Next();

                return values;
            });
            var processData = getData.ContinueWith((x) => {
                int n = x.Result.Length;
                long sum = 0;
                double mean;

                for (int ctr = 0; ctr <= x.Result.GetUpperBound(0); ctr++)
                    sum += x.Result[ctr];

                mean = sum / (double)n;
                return Tuple.Create(n, sum, mean);
            });
            var displayData = processData.ContinueWith((x) => {
                return String.Format("N={0:N0}, Total = {1:N0}, Mean = {2:N2}",
                                     x.Result.Item1, x.Result.Item2,
                                     x.Result.Item3);
            });
            Console.WriteLine(displayData.Result);


            var displayData2 = Task.Factory.StartNew(() => {
                Random rnd = new Random();
                int[] values = new int[100];
                for (int ctr = 0; ctr <= values.GetUpperBound(0); ctr++)
                    values[ctr] = rnd.Next();

                return values;
            }).
                        ContinueWith((x) => {
                            int n = x.Result.Length;
                            long sum = 0;
                            double mean;

                            for (int ctr = 0; ctr <= x.Result.GetUpperBound(0); ctr++)
                                sum += x.Result[ctr];

                            mean = sum / (double)n;
                            return Tuple.Create(n, sum, mean);
                        }).
                        ContinueWith((x) => {
                            return String.Format("N={0:N0}, Total = {1:N0}, Mean = {2:N2}",
                                                 x.Result.Item1, x.Result.Item2,
                                                 x.Result.Item3);
                        });
            Console.WriteLine(displayData2.Result);
        }

        public static void SeparateTask()
        {
            var outer = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Outer task beginning.");

                var child = Task.Factory.StartNew(() =>
                {
                    Thread.SpinWait(5000000);
                    Console.WriteLine("Detached task completed.");
                });
            });

            outer.Wait();
            Console.WriteLine("Outer task completed.");
            // The example displays the following output:
            //    Outer task beginning.
            //    Outer task completed.
            //    Detached task completed.
        }
        public static void UnitedTask()
        {
            var parent = Task.Factory.StartNew(() => {
                Console.WriteLine("Parent task beginning.");
                for (int ctr = 0; ctr < 10; ctr++)
                {
                    int taskNo = ctr;
                    Task.Factory.StartNew((x) => {
                        Thread.SpinWait(5000000);
                        Console.WriteLine("Attached child #{0} completed.",
                                          x);
                    },
                                          taskNo, TaskCreationOptions.AttachedToParent);
                }
            });

            parent.Wait();
            Console.WriteLine("Parent task completed.");
        }
        class CustomData
        {
            public long CreationTime;
            public int Name;
            public int ThreadNum;
        }
        private static Double DoComputation(Double start)
        {
            Double sum = 0;
            for (var value = start; value <= start + 10; value += .1)
                sum += value;

            return sum;
        }
    }
}
