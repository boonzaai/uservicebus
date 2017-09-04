using System;
using System.Collections.Generic;
using System.Text;

namespace Icris.uservicebus
{
    public interface IPublisher
    {
        /// <summary>
        /// A publisher can't exist without a broker.
        /// </summary>
        /// <param name="broker">The broker to publish to.</param>
        /// <returns></returns>
        IPublisher IPublisher(IPubSubBroker broker);
    }
}
