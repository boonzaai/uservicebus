using System;
using System.Collections.Generic;
using System.Text;

namespace Icris.uServiceBus.Core.Brokers
{
    public class InMemoryBroker : IPubSubBroker
    {
        List<ISubscriber> subscribers = new List<ISubscriber>();

        public IPubSubBroker Publish(object message)
        {
            foreach(var subscriber in subscribers)
            {
                subscriber.Receive(message);
            }
            return this;
        }

        public IPubSubBroker Subscribe(ISubscriber subscriber)
        {
            this.subscribers.Add(subscriber);
            return this;
        }
    }
}
