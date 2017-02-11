
namespace WcfService
{
    using System.ServiceModel;
    using Properties;
    using System.Collections.Generic;

    public class TestService : ITestService
    {
        private static List<int> _failedCache = new List<int>();

        public string TestCall(int value, bool simulateExceptions)
        {
            if (simulateExceptions && value % 2 > 0 && !_failedCache.Contains(value))
            {
                _failedCache.Add(value);
                throw new CommunicationException("Simulated exception in service.");
            }
            
            var response = HashExtentions.ShaHash($"Random [{value}] string");
            return response;
        }

        public void ResetCache()
        {
            _failedCache = new List<int>();
        }
    }
}
