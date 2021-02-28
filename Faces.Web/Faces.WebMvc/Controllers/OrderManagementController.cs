using Faces.WebMvc.RestClients;
using Faces.WebMvc.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Faces.WebMvc.Controllers
{
    public class OrderManagementController : Controller
    {
        private  IOrderManagementApi _orderManagementApi; 

        public OrderManagementController(IOrderManagementApi orderManagementApi)
        {
            _orderManagementApi = orderManagementApi;
        }
       
        
        public async Task<IActionResult> Index()
        {
            var result = await _orderManagementApi.GetOrders();
            //image in byte[] -> base64String
            result.ForEach(order=> 
            {
                //instead making new view model we will send image url as imagestring !!! XD
                order.FileUrl = ConvertAndFormatToString(order.FileData); 
            });

            return View(result);
        }

        [Route("/details/{orderId}")]
        public async Task<IActionResult> Details(string orderId) 
        {
            var order = await _orderManagementApi.GetOrderbyId(Guid.Parse(orderId));
            if (order == null) return NotFound();
            order.FileUrl = ConvertAndFormatToString(order.FileData);
            order.OrderDetails.ForEach(detail => 
            {
                detail.FileUrl = ConvertAndFormatToString(detail.FileData);
            });

            return View(order);
        }

        private string ConvertAndFormatToString(byte[] imageData)
        {
            string imageBase64Data = Convert.ToBase64String(imageData);

            return string.Format("data:image/png;base64, {0}",imageBase64Data);
        }
    }
}
