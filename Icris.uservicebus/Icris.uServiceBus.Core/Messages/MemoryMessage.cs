using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Icris.uServiceBus.Core.Messages
{
    public class MemoryMessage<T> : IMessage<T>
    {
        T value;
        public MemoryMessage(T value)
        {
            this.value = value;
        }
        public void Complete()
        {
            throw new NotImplementedException();
        }

        public T Content()
        {
            return value;
        }

        public void Release()
        {
            throw new NotImplementedException();
        }

        public void RenewLock()
        {
            throw new NotImplementedException();
        }
    }
}
