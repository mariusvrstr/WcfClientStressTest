
namespace TestClient
{
    using System;

    public static class FeedbackWorker
    {
        public static void DisplayResultSummary(int success, int failures, int threads, string time)
        {
            Console.WriteLine();
            Console.WriteLine("--------------------------------------------------------------------------------");
            Console.WriteLine($"Batch Completed. Success [{success}] Failed [{failures}] using [{threads}] Threads. Duration [{time}]");
        }
    }
}
