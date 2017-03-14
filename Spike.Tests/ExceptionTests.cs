
namespace Spike.Tests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using WcfServiceProxy;
    
    [TestClass]
    public class ExceptionTests
    {
        /* Testing notes.
         * ===========================================
         * Please note that due to hosting the service tests only pass when debugging        * 
         * 
         * */

        private const int RetryQty = 3;

        [TestMethod]
        public void TestExceptionsNoRetry()
        {
            var failures = 0;
            var itemsToDo = 10;
            var completed = 0;

            Console.WriteLine($"Attempting to hash [{itemsToDo}] values in stress test:");
            Console.WriteLine();

            for (var k = 0; k < itemsToDo; k++)
            {
                try
                {
                    var proxy = new TestProxy();
                    var newValue = proxy.TestCall(k, true);
                    Console.WriteLine($"Index [{k}] Hash [{newValue}]");
                    completed++;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    failures++;
                }
            }
            
            Assert.AreEqual(itemsToDo/2, completed, "Not all the expected items where registered");
            Assert.IsTrue(failures == itemsToDo / 2, "There must be 50% errors of attempts");
        }


        [TestMethod]
        public void TestExceptionsWithRetry()
        {
            var failures = 0;
            var itemsToDo = 10;
            var completed = 0;

            Console.WriteLine($"Attempting to hash [{itemsToDo}] values in stress test:");
            Console.WriteLine();

            for (var k = 0; k < itemsToDo; k++)
            {
                try
                {
                    var proxy = new TestProxy();
                    var newValue = proxy.TestCall(k, true, RetryQty);
                    Console.WriteLine($"Index [{k}] Hash [{newValue}]");
                    completed++;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    failures++;
                }
            }

            Assert.AreEqual(itemsToDo, completed, "Not all tems are registered");
            Assert.IsTrue(failures == 0, "There are items that did not success in list");
        }
    }
}
