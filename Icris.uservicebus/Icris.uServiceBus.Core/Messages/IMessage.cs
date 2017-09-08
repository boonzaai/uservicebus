using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Icris.uServiceBus.Core.Messages
{
    public interface IMessage<T>
    {
        void Complete();
        void RenewLock();
        void Release();
        T Content();
    }
}
