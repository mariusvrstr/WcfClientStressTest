
namespace TestClient.Threading
{
    using Spike.Common;

    public class Counters
    {
        public Counters()
        {
            Stopwatch = new StopwatchWorker();
           // CompletedItems = new List<int>();
        }

        public StopwatchWorker Stopwatch { get; set; }

        public int SuccessCount { get; set; }

        public int FailureCount { get; set; }

        public int ItemCount => SuccessCount + FailureCount;

       // public List<int> CompletedItems { get; set; }

        public int ThreadCount { get; set; }
    }
}
