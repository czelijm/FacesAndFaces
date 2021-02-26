using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrdersApi.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrdersApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        public OrdersController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync() 
        {
            var data = await _orderRepository.GetOrdersAsync();
            return Ok(data);
        }
        [HttpGet]
        public async Task<IActionResult> GetOrderByIdAsync(Guid id) 
        {
            var data = await _orderRepository.GetOrderAsync(id);
            return data is null? NotFound() : Ok(data);
        }        
        [HttpGet]
        public async Task<IActionResult> GetOrderByIdAsync(string id) 
        {
            var data = await _orderRepository.GetOrderAsync(Guid.Parse(id));
            return data is null ? NotFound() : Ok(data);
        }

    }
}
