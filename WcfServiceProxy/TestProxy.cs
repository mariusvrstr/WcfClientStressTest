
namespace WcfServiceProxy
{
    using TestService;
    using System;
    using Spike.Common;

    public class TestProxy
    {
        public string TestCall(int value, bool simulateExceptions, int attempts = 1)
        {
            var counter = new StopwatchWorker();
            var isSample = SampleWorker.IsSample(value);

            if (isSample)
            {
                counter.Start();
            }

            var consumer = new ServiceClientWrapper<TestServiceClient, ITestService>();

            if (simulateExceptions)
            {
               consumer.Excecute(c => c.ResetCache());
            }

            var response = consumer.Excecute(c => c.TestCall(value, simulateExceptions), attempts);

            if (isSample)
            {
                Console.WriteLine($"Sample [{value}] >> WCF Call Complete. Duration [{counter.ElapsedTime()}]");
                counter.Start();
            }

            return response;
        }
    }
}
