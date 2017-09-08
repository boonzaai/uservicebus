using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Icris.uServiceBus.Core.Messages;
using System.Collections.Concurrent;
using System.Threading;

namespace Icris.uServiceBus.Core.Queues
{
    public class MemoryQueue<T> : IQueue<T>
    {
        ConcurrentBag<IMessage<T>> messages = new ConcurrentBag<IMessage<T>>();
        ConcurrentBag<Tuple<DateTime, IMessage<T>>> lockedmessages = new ConcurrentBag<Tuple<DateTime, IMessage<T>>>();

        public void Clear()
        {
            lockedmessages = new ConcurrentBag<Tuple<DateTime, IMessage<T>>>();
            messages = new ConcurrentBag<IMessage<T>>();
        }

        public IMessage<T> Receive()
        {
            IMessage<T> value;
            while (!messages.TryTake(out value))
            {
                Thread.Sleep(1);
                return Receive();
            }
            lockedmessages.Add(new Tuple<DateTime, IMessage<T>>(DateTime.Now,value));
            return value;
        }

        public void Send(T payload)
        {
            messages.Add(new MemoryMessage<T>(payload));
        }

        public void Subscribe(Action<IMessage<T>> notification)
        {
            throw new NotImplementedException();
        }
    }
}
