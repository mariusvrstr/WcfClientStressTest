
namespace Spike.Tests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using WcfServiceProxy;
    
    [TestClass]
    public class VolumeTests
    {
        /* Testing notes.
            * ===========================================
            * Please note that due to hosting the service tests only pass when debugging        * 
            * 
            * */

        [TestMethod]
        public void TestVolumeNoRetryNoExceptions()
        {
            var failures = 0;
            var itemsToDo = 30000;
            var completed = 0;

            Console.WriteLine($"Attempting to hash [{itemsToDo}] values in stress test:");
            Console.WriteLine();

            for (var k = 0; k < itemsToDo; k++)
            {
                try
                {
                    var proxy = new TestProxy();
                    var newValue = proxy.TestCall(k, false);
                    Console.WriteLine($"Index [{k}] Hash [{newValue}]");
                    completed++;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    failures++;
                }
            }

            Assert.AreEqual(itemsToDo, completed, "Not all the expected items where registered");
            Assert.IsTrue(failures == 0, "There are no expected errorss");
        }
    }
}
