using System;

namespace Icris.uservicebus
{
    public interface IPubSubBroker
    {
        IPubSubBroker Publish(object message);

        IPubSubBroker Subscribe(ISubscriber subscriber);
    }
}
