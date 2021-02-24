using Faces.WebMvc.Models;
using Faces.WebMvc.Models.ViewModels;
using MassTransit;
using Messaging.InterfacesConstants.Commnads;
using Messaging.InterfacesConstants.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Faces.WebMvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBusControl _busControl;
        public HomeController(ILogger<HomeController> logger, IBusControl busControl)
        {
            _logger = logger;
            _busControl = busControl;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> RegisterOrder()
        {
            return View();
        }

        
        [HttpPost]
        [ActionName("RegisterOrder")]
        public async Task<IActionResult> RegisterOrderPost(OrderViewModel model)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (var uploadFile = model.File.OpenReadStream())
                {
                    await uploadFile.CopyToAsync(memoryStream);
                }

                model.FileData = memoryStream.ToArray();
            }

            model.FileUrl = model.File.FileName;
            model.Id = Guid.NewGuid();
            var sendToUri = new Uri($"{RabbitMqMassTransitConstants.RabbitMquri}{RabbitMqMassTransitConstants.RegisterOrderCommandQueue}");
            var endPoint = await _busControl.GetSendEndpoint(sendToUri);
            await endPoint.Send<IRegisterOrderCommand>
            (
                new 
                {
                    model.Id,
                    model.Email,
                    model.FileData,
                    model.FileUrl
                }
            );
            ViewData["Id"] = model.Id;
            return View("Thanks");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
