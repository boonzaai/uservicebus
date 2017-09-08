using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Icris.uServiceBus.Core.Queues;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
using System.Linq;
using Icris.uServiceBus.Core.Messages;

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
                queue.Send(new testdto() { name = "" + i }));

            Assert.AreEqual(queue.Count, 1000);
            Parallel.For(0, 1000, new ParallelOptions() { MaxDegreeOfParallelism = 1 }, i =>
             {
                 var message = queue.Receive();
                 var content = message.Content();
                 message.Complete();
             });
            Assert.AreEqual(queue.Count, 0);
        }
        [TestMethod]
        public void TestReceiveTimeout()
        {
            var queue = new FileSystemQueue<testdto>(Path.GetTempPath() + "queue");
            queue.Clear();
            queue.Send(new testdto() { name = Guid.NewGuid().ToString() });
            var message = queue.Receive();
            //Thread.Sleep(4000);
            var message2 = queue.Receive();
            Assert.AreEqual(null, message2);
            try
            {
                message.RenewLock();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Lock expired", e.Message);
            }

        }

        [TestMethod]
        public void TestSubscription()
        {
            var messagecount = 0;
            var counter = new Action<IMessage<testdto>>(x => { messagecount++; });
            var queue = new FileSystemQueue<testdto>(Path.GetTempPath() + "queue");
            queue.Subscribe(counter);
            Parallel.For(0, 100, x => { queue.Send(new testdto()); });

            Assert.AreEqual(100, messagecount);

        }
    }
}
