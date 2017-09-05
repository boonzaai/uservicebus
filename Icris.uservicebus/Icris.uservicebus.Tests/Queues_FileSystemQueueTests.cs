using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Icris.uServiceBus.Core.Queues;
using System.IO;
using System.Threading.Tasks;

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
            Parallel.For (0,1000, i=>
                queue.Add(new testdto() { name = "name_" + i }));

            Assert.AreEqual(queue.Count, 1000);

            Parallel.For(0,1000, i =>
            {
                var message = queue.Receive();
                var content = message.Content();
                message.Complete();
            });

            Assert.AreEqual(queue.Count, 0);
        }
    }
}
