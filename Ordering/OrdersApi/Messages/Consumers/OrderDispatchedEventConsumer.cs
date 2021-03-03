using MassTransit;
using Messaging.InterfacesConstants.Events;
using Messaging.InterfacesConstants.SignalR;
using Microsoft.AspNetCore.SignalR;
using OrdersApi.Hubs;
using OrdersApi.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrdersApi.Messages.Consumers
{
    public class OrderDispatchedEventConsumer : IConsumer<IOrderDispatchedEvent>
    { 
        private readonly IOrderRepository _orderRepository;
        private readonly IHubContext<OrderHub> _hubContext;

        public OrderDispatchedEventConsumer(IOrderRepository orderRepository, IHubContext<OrderHub> hubContext)
        {
            _orderRepository = orderRepository;
            _hubContext = hubContext;
        }

        public async Task Consume(ConsumeContext<IOrderDispatchedEvent> context)
        {
            var message = context.Message;
            Guid id = message.Id;
            await UpdateDatabaseAsync(id);
            //await _hubContext.Clients.All.SendAsync(ClientsMethodsName.UpdateOrdersOnClient, new object[] {EventName.OrderDispatched, id });
            await _hubContext.Clients.All.SendAsync(ClientsMethodsName.UpdateOrdersOnClient, EventName.OrderDispatched, id);
            //return Task.CompletedTask;
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
