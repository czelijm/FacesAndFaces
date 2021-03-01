using MassTransit;
using Messaging.InterfacesConstants.Events;
using OrdersApi.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrdersApi.Messages.Consumers
{
    public class OrderDispatchedEventConsumer : IConsumer<IOrderDispatchedEvent>
    { 
        private readonly OrderRepository _orderRepository;

        public OrderDispatchedEventConsumer(OrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task Consume(ConsumeContext<IOrderDispatchedEvent> context)
        {
            var message = context.Message;
            Guid id = message.Id;
            await UpdateDatabaseAsync(id);
        }

        private async Task UpdateDatabaseAsync(Guid id)
        {
            var order = await _orderRepository.GetOrderAsync(id);
            if (order!=null)
            {
                order.Status = Models.Status.Sent;
                _orderRepository.UpdateOrder(order);
            }
        }
    }
}
