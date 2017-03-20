
namespace TestClient.Threading
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    public class WorkItem
    {
        public WorkItem(int id, Func<int, string> work)
        {
            this.Id = id;
            this.Work = work;
        }

        public int Id { get; set; }

        private Func<int, string> Work { get; set; }

        public void Execute()
        {
            Work(Id);
        }
    }

    public class ThreadWorker
    {
        private Counters _internalCounter;
        public List<WorkItem> WorkQueue { get; set; }
        private List<int> ThreadsUsed { get; set; }
       
        public void WorkWrapper(object obj)
        {
                var workItem = (WorkItem)obj;

                try
                {
                    workItem.Execute();

                    lock (_internalCounter)
                    {
                        _internalCounter.SuccessCount++;
                    }
                }
                catch (Exception ex)
                {
                    lock (_internalCounter)
                    {
                        _internalCounter.FailureCount--;
                    }
                    
                    Console.WriteLine($"Failed to excecute work: {ex.Message}", _internalCounter.FailureCount);
                }

                if (!ThreadsUsed.Contains(Thread.CurrentThread.ManagedThreadId))
                {
                    _internalCounter.ThreadCount++;
                    ThreadsUsed.Add(Thread.CurrentThread.ManagedThreadId);
                    Console.WriteLine("New thread is joining the party. Thread {0} consumes {1}", Thread.CurrentThread.GetHashCode(), ((WorkItem)obj).Id);
                }
        }
        
        public void ProcessWorkItem(WorkItem workItem)
        {
            ThreadPool.QueueUserWorkItem(WorkWrapper, workItem);
            WorkQueue.Remove(workItem);
        }

        public void ProcessAllWorkItems(ref Counters counter)
        {
            _internalCounter = counter;
            ThreadsUsed = new List<int>();

            while (WorkQueue.Any())
            {
                lock (WorkQueue)
                {
                    var workItem = WorkQueue.FirstOrDefault();
                    ProcessWorkItem(workItem);
                }
            }
        }
    }
}
