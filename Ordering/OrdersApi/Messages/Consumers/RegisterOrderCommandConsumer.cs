using MassTransit;
using Messaging.InterfacesConstants.Commnads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrdersApi.Messages.Consumers
{
    //class that will intercept any message comes to our register order command queue
    public class RegisterOrderCommandConsumer : IConsumer<IRegisterOrderCommand>
    {
        public Task Consume(ConsumeContext<IRegisterOrderCommand> context)
        {
            throw new NotImplementedException();
        }
    }
}
