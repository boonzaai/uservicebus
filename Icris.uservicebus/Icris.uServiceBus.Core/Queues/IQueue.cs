using Icris.uServiceBus.Core.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Icris.uServiceBus.Core.Queues
{
    public interface IQueue<T>
    {        
        void Send(T payload);
        IMessage<T> Receive();
        void Clear();
        void Subscribe(Action<IMessage<T>> notification);
    }
}
