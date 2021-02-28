using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OrdersApi.Models
{
    public class OrderDetails
    {
        //composite key has to be declared in DbContext in OnModelCreate
        //{orderId,OrderDetailId}
        public Guid OrderId { get; set; }
        public int OrderDetailId { get; set; }
        public byte[] FileData { get; set; }
    }
}
