﻿using MassTransit;
using Messaging.InterfacesConstants.Commnads;
using Messaging.InterfacesConstants.Events;
using Messaging.InterfacesConstants.SignalR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OrdersApi.Hubs;
using OrdersApi.Models;
using OrdersApi.Persistence;
using OrdersApi.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace OrdersApi.Messages.Consumers
{
    //class that will intercept any message comes to our register order command queue
    public class RegisterOrderCommandConsumer : IConsumer<IRegisterOrderCommand>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHubContext<OrderHub> _hubContext;
        private readonly IOptions<OrderSettings> _settings;

        public RegisterOrderCommandConsumer(IOrderRepository orderRepository,IHttpClientFactory httpClientFactory, IHubContext<OrderHub> hubContext, IOptions<OrderSettings> settings)
        {
            _orderRepository = orderRepository;
            _httpClientFactory = httpClientFactory;
            _hubContext = hubContext;
            _settings = settings;
        }

        public async Task Consume(ConsumeContext<IRegisterOrderCommand> context)
        {
            var result = context.Message;
            if (result != null 
                && result.Email!=null && !(result.Id.Equals(Guid.Empty)) //Guid is always not null. for testing We have sended empty guid,
                && result.FileData!=null && result.FileUrl!=null
                )
            {
                await SaveOrder(result);

                //SignalR part
                //Method "UpdateOrders" Should be on client side, eventName (parameter), other_parameter 
                await _hubContext.Clients.All.SendAsync(ClientsMethodsName.UpdateOrdersOnClient, EventName.NewOrderCreated, result.Id);

                //connetct to api via http, no messagebroker part
                var client = _httpClientFactory.CreateClient();
                Tuple<List<byte[]>,Guid> orderDetailData = await GetFacesFromFaceApiAsync(client, result.FileData, result.Id);
                List<byte[]> faces = orderDetailData.Item1;
                Guid orderId = orderDetailData.Item2;
                SaveOrderDeteils(orderId,faces);

                //SignalR part
                await _hubContext.Clients.All.SendAsync(ClientsMethodsName.UpdateOrdersOnClient, EventName.OrderProcessed, result.Id);

                //Send Order Processed Event massage to MessageBroker
                await context.Publish<IOrderProcessedEvent>(new 
                {
                    Id = orderId,
                    FileUrl = result.FileUrl,
                    Files = faces,
                    Email = result.Email
                });
            }
            //return Task.FromResult();
        }

        private void SaveOrderDeteils(Guid orderId, List<byte[]> faces)
        {
            var order = _orderRepository.GetOrderAsync(orderId).Result;
            if (order!=null)
            {
                order.Status = Status.Processed;
                faces.Select((x,i)=> new
                {
                    item = x,
                    index = i
                })
                .ToList()
                .ForEach(face=> 
                {
                    order.OrderDetails.Add(new OrderDetails 
                    {
                        OrderId=orderId,
                        FileData=face.item,
                        OrderDetailId=face.index
                    });
                });
                _orderRepository.UpdateOrder(order);
            }
        }

        private async Task<Tuple<List<byte[]>, Guid>> GetFacesFromFaceApiAsync(HttpClient client, byte[] fileData, Guid orderId)
        {
            var byteArrayContent = new ByteArrayContent(fileData);
            Tuple<List<byte[]>, Guid> orderDetailDataFromResponse = null;
            byteArrayContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octete-stream");

            //string requestUri = Messaging.InterfacesConstants.Constants.RabbitMqMassTransitConstants.HostAddress
            //   + Messaging.InterfacesConstants.Constants.RabbitMqMassTransitConstants.FaceApiUri 
            //   + "?orderId=" + orderId.ToString();

            string requestUri = _settings.Value.FacesApiUrl
               + Messaging.InterfacesConstants.Constants.RabbitMqMassTransitConstants.FaceApiUri 
               + "?orderId=" + orderId.ToString();
            
            using (var response = await client.PostAsync(requestUri,byteArrayContent))
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                orderDetailDataFromResponse = JsonConvert.DeserializeObject<Tuple<List<byte[]>, Guid>>(apiResponse);
            }
            return orderDetailDataFromResponse;
        }

        private async Task SaveOrder(IRegisterOrderCommand result)
        {
            Order order = new Order
            {
                Email = result.Email,
                Id = result.Id,
                FileData = result.FileData,
                FileUrl = result.FileUrl,
                Status = Status.Registered
            };
           await _orderRepository.RegisterOrderAsync(order);
        }
    }
}
