using MassTransit;
using Messaging.InterfacesConstants.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Consumer
{
    public class OrderProcessedEventConsumer : IConsumer<IOrderProcessedEvent>
    {
        public Task Consume(ConsumeContext<IOrderProcessedEvent> context)
        {
            throw new NotImplementedException();
        }
    }
}
