using Microsoft.EntityFrameworkCore;
using OrdersApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrdersApi.Persistence
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrdersContext _ordersContext;

        public OrderRepository(OrdersContext ordersContext)
        {
            _ordersContext = ordersContext;
        }

        public Order GetOrder(Guid id)
        {
            return _ordersContext.Orders.Include("OrderDetails").FirstOrDefault(o => o.Id == id);
        }

        public async Task<Order> GetOrderAsync(Guid id)
        {
            //return await _ordersContext.Orders.Include("OrderDetails").FirstOrDefaultAsync(o=>o.Id == id);


            var result = await _ordersContext.Orders.Include(o=>o.OrderDetails).FirstOrDefaultAsync(o=>o.Id == id);
            //await _ordersContext.Entry(result).Collection(o => o.OrderDetails).LoadAsync();
            return result;
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync()
        {
            return await _ordersContext.Orders.Include("OrderDetails").ToListAsync();
        }

        public async Task RegisterOrderAsync(Order order)
        {
            await _ordersContext.Orders.AddAsync(order);
            await _ordersContext.SaveChangesAsync();
            //return Task.FromResult(true);
        }

        //public void UpdateOrder(Order order)
        //{
        //    _ordersContext.Entry(order)
        //    _ordersContext.SaveChangesAsync().GetAwaiter().GetResult();
        //}

        public void UpdateOrder(Order order)
        {
            _ordersContext.Entry(order).State = EntityState.Modified;
            _ordersContext.SaveChangesAsync().GetAwaiter().GetResult();
        }


    }
}
