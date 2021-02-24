using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrdersApi.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string FileUrl { get; set; }
        public byte[] FileData { get; set; }
        public Status Status { get; set; }
        public List<OrderDetails> OrderDetails { get; set; }

        public Order()
        {
            OrderDetails = new List<OrderDetails>();
        }

    }
}
