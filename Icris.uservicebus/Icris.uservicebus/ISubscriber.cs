using System;
using System.Collections.Generic;
using System.Text;

namespace Icris.uservicebus
{
    public interface ISubscriber
    {
        ISubscriber Receive(object Message);
    }
}
