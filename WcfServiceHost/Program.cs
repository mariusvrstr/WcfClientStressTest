using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WcfService;

namespace WcfServiceHost
{
    class Program
    {
        static void Main(string[] args)
        {
            var servicehost = new ServiceHost(typeof(TestService));
            try
            {
                servicehost.Open();
                Console.WriteLine("Service is available:");
                Console.WriteLine(servicehost.Description.Endpoints.First().Address);
                Console.WriteLine();
                Console.WriteLine("Press <ENTER> to exit.");
                Console.ReadLine();
                servicehost.Close();
            }
            catch (CommunicationException commProblem)
            {
                Console.WriteLine("There was a communication problem. " + commProblem.Message);
                Console.Read();
            }

        }
    }
}
