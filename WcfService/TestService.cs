
namespace WcfService
{
    using System.ServiceModel;
    using System.Collections.Generic;
    using System;

    public class TestService : ITestService
    {
        private static List<int> _failedCache = new List<int>();

        public string TestCall(int value, bool simulateExceptions)
        {
            Console.Clear();
            Console.WriteLine($"Request Received for [{value}]");

            if (simulateExceptions && value % 2 > 0 && !_failedCache.Contains(value))
            {
                _failedCache.Add(value);
                throw new FaultException("Simulated exception in service.");
            }
            
            var response = HashExtentions.ShaHash($"Random [{value}] string");

            Console.WriteLine($"Request Completed. Hash [{response}]");
            return response;
        }

        public void ResetCache()
        {
            _failedCache = new List<int>();
        }
    }
}
