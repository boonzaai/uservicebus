using System;
using System.Collections.Generic;
using System.Text;

namespace Icris.uServiceBus.Core
{
    public interface ISubscriber
    {
        ISubscriber Receive(object Message);
    }
}
