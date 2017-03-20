
namespace TestClient
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Spike.Common;
    using Properties;
    using Threading;
    using WcfServiceProxy;

    class Program
    {
        private const int Retries = 3;

        private static readonly Func<int, string> ProcessTestCall = num =>
        {
            var proxy = new TestProxy();
            return proxy.TestCall(num, Settings.Default.IncludeExceptions, Settings.Default.RetryFailures ? Retries : 1);
        };

        private static List<WorkItem> WorkItems { get; set; }
        
        private static List<WorkItem> CreateWorkList()
        {
            var list = new List<WorkItem>();

            for (var i = 1; i <= Settings.Default.NumberOfCalls; i++)
            {
                list.Add(new WorkItem(i, ProcessTestCall));
            }

            return list;
        }

        private static void ProcessInSingleThread(ref int success, ref int failures)
        {
            Console.WriteLine($"Attempting to hash [{Settings.Default.NumberOfCalls}] values in stress test:");
            Console.WriteLine();

            while (WorkItems.Any())
            {
                try
                {
                    var workItem = WorkItems.FirstOrDefault();
                    workItem.Execute();
                    success++;

                    WorkItems.Remove(workItem);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    failures++;
                }
            }
        }

        private static void ProcessMultipleThreads(ref int success, ref int failures, ref int threads)
        {
            var lastCount = 0;
            var unchangedTics = 0;
            var waitBufferInTics = 5;

            var threadWorker = new ThreadWorker
            {
                WorkQueue = WorkItems
            };

            var counters = new Counters();

            counters.Stopwatch.Start();
            threadWorker.ProcessAllWorkItems(ref counters);

            while (counters.ItemCount < Settings.Default.NumberOfCalls && unchangedTics < waitBufferInTics)
            {
                if (lastCount == counters.ItemCount && lastCount > 0)
                {
                    unchangedTics++;
                }

                lastCount = counters.ItemCount;

                if (unchangedTics == waitBufferInTics)
                {
                    Console.WriteLine("Exiting all pending work has been completed. Warning some items have not been processed!");
                }

                Thread.Sleep(1000);
            }

            counters.Stopwatch.Stop();
            FeedbackWorker.DisplayResultSummary(counters.SuccessCount, counters.FailureCount, counters.ThreadCount, counters.Stopwatch.ElapsedTime());
        }

        static void Main(string[] args)
        {
            var failures = 0;
            var success = 0;
            var threads = 1;
            
            WorkItems = CreateWorkList();
            
            if (Settings.Default.UseMultipleThreads)
            {
                ProcessMultipleThreads(ref success, ref failures, ref threads);
            }
            else
            {
                var stopwatch = new StopwatchWorker();
                stopwatch.Start();
                ProcessInSingleThread(ref success, ref failures);

                FeedbackWorker.DisplayResultSummary(success, failures, threads, stopwatch.ElapsedTime());
            }

            Console.ReadKey();
        }
    }
}
