using System;
using Spike.Common;
using TestClient.Properties;
using WcfServiceProxy;

namespace TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var failures = 0;
            var itemsToDo = Settings.Default.NumberOfCalls;
            var completed = 0;
            var stopwatch = new StopwatchWorker();
            var maxAttempts = Settings.Default.RetryFailures ? 3 : 1;


            stopwatch.Start();
            Console.WriteLine($"Attempting to hash [{itemsToDo}] values in stress test:");
            Console.WriteLine();

            for (var k = 0; k < itemsToDo; k++)
            {
                try
                {
                    var proxy = new TestProxy();
                    proxy.TestCall(k, Settings.Default.IncludeExceptions, maxAttempts);
                    completed++;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    failures++;
                }
            }


            Console.WriteLine();
            Console.WriteLine("--------------------------------------------------------------------------------");
            Console.WriteLine($"Batch Completed. Success [{completed}] Failed [{failures}] Duration [{stopwatch.ElapsedTime()}]");
            Console.ReadKey();
        }
    }
}
