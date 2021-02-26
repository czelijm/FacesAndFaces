using Faces.WebMvc.Models.ViewModels;
using Microsoft.Extensions.Configuration;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Faces.WebMvc.RestClients
{
    public class OrderManagementApi : IOrderManagementApi
    {
        private readonly IOrderManagementApi _restClient;
        public OrderManagementApi(IConfiguration configuration, HttpClient httpClient)
        {
            //let now http client about remote api location
            string apiHostAndPort = configuration.GetSection("ApiServiceLocations")
                .GetValue<string>("OrdersApiLocations");
            httpClient.BaseAddress = new Uri($"http://{apiHostAndPort}/api");
            _restClient = RestService.For<IOrderManagementApi>(httpClient);
        }

        public async Task<List<OrderViewModel>> GetOrderbyId(Guid orderId)
        {
            try
            {
                return await _restClient.GetOrderbyId(orderId);
            }
            catch (ApiException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
                else throw;
            }
        }

        public async Task<List<OrderViewModel>> GetOrders()
        {
            return await _restClient.GetOrders();
        }
    }
}
