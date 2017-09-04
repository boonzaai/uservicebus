using System;

namespace Icris.uServiceBus.Core
{
    public interface IPubSubBroker
    {
        IPubSubBroker Publish(object message);

        IPubSubBroker Subscribe(ISubscriber subscriber);
    }
}
