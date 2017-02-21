
namespace Spike.Common
{
    using System.Diagnostics;

    public class StopwatchWorker
    {
        private Stopwatch StopWatch { get; set; } = new Stopwatch();


        public void Start()
        {
            StopWatch.Reset();
            StopWatch.Start();
        }


        public void Stop()
        {
            StopWatch.Stop();
        }

        public string ElapsedTime()
        {
            var ts = StopWatch.Elapsed;
            return $"{ts.Hours}h:{ts.Minutes}m:{ts.Seconds}s:{ts.Milliseconds}ms";
        }
    }
}
