using Faces.WebMvc.Models.ViewModels;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Faces.WebMvc.RestClients
{
    public interface IOrderManagementApi
    {
        //refit library was used to make things easier; For http request.
        //it's not recomendet to use refit for complex objects like in ex. tuple used in orderApi. 
        //Using Refit we don't have to deserialize Json file

        //for reaching getAllAsync method in controler
        [Get("/orders")] // from refit library
        Task<List<OrderViewModel>> GetOrders();
        
        [Get("/orders/{orderId}")] // from refit library
        Task<OrderViewModel> GetOrderbyId(Guid orderId);
        

    }
}
