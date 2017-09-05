using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Icris.uServiceBus.Core.Queues;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
using System.Linq;

namespace Icris.uservicebus.Tests
{
    [TestClass]
    public class Queues_FileSystemQueueTests
    {
        class testdto
        {
            public string name { get; set; }
        }
        [TestMethod]
        public void TestCreate1000Messages()
        {
            var queue = new FileSystemQueue<testdto>(Path.GetTempPath() + "queue");
            queue.Clear();
            Parallel.For(0, 1000, i =>
                queue.Add(new testdto() { name = "" + i }));

            Assert.AreEqual(queue.Count, 1000);
            ConcurrentBag<int> processed = new ConcurrentBag<int>();
            Parallel.For(0, 1000, i =>
             {
                 var message = queue.Receive(60);
                 var content = message.Content();
                 if (processed.ToList().Contains(int.Parse(content.name)))
                 {
                     System.Diagnostics.Debugger.Break();
                 }
                 processed.Add(int.Parse(content.name));
                 message.Complete();
             });
            Console.WriteLine(processed);
            Assert.AreEqual(queue.Count, 0);
        }
        [TestMethod]
        public void TestReceiveTimeout()
        {
            var queue = new FileSystemQueue<testdto>(Path.GetTempPath() + "queue");
            queue.Clear();
            queue.Add(new testdto() { name = Guid.NewGuid().ToString() });
            var message = queue.Receive(30);
            //Thread.Sleep(4000);
            var message2 = queue.Receive(500);
            try
            {
                message.RenewLock();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Lock expired", e.Message);
            }

        }
    }
}
