
namespace WcfServiceProxy
{
    using TestService;

    public class TestProxy
    {
        public string TestCall(int value, bool simulateExceptions, int attempts = 1)
        {
            var consumer = new ServiceClientWrapper<TestServiceClient, ITestService>();

            if (simulateExceptions)
            {
                consumer.Excecute(c => c.ResetCache());
            }

            return consumer.Excecute(c => c.TestCall(value, simulateExceptions), attempts);
        }
    }
}
