using EmailService;
using MassTransit;
using Messaging.InterfacesConstants.Events;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Consumer
{
    public class OrderProcessedEventConsumer : IConsumer<IOrderProcessedEvent>
    {

        private readonly IEmailSender _emailSender;

        public OrderProcessedEventConsumer(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public async Task Consume(ConsumeContext<IOrderProcessedEvent> context)
        {
            //await ConsumeImagem(context);
            await ConsumeImageSharp(context);
        }

        private async Task ConsumeImageSystem(ConsumeContext<IOrderProcessedEvent> context) 
        {
            var rootFolder = AppContext.BaseDirectory.Substring(0, AppContext.BaseDirectory.IndexOf("bin"));
            var result = context.Message;
            var fileData = result.Files;
            if (fileData.Count < 1)
            {
                await Console.Out.WriteLineAsync("No Faces Detected");
            }
            else
            {
                fileData.Select((v, i) => new { item = v, index = i }).ToList()
                .ForEach(f =>
                {
                    //loading byte[] to memory stream
                    MemoryStream memoryStream = new MemoryStream(f.item);
                    //create image from memorystream
                    var image = System.Drawing.Image.FromStream(memoryStream);
                    //saving image in root folder
                    image.Save($"{rootFolder}/Images/face{f.index}.jpg", ImageFormat.Jpeg);
                });
            }
            //Here We will Add the EmailSending code
            string[] mailAddresses = { result.Email };


            await _emailSender.SendEmailAsync(new Message(mailAddresses, "Order " + result.Id, "From FacesAndFaces", result.Files));

            await context.Publish<IOrderDispatchedEvent>(new
            {
                context.Message.Id,
                DispatchDataTime = DateTime.UtcNow
            });
        }

        private async Task ConsumeImageSharp(ConsumeContext<IOrderProcessedEvent> context)
        {
            var rootFolder = AppContext.BaseDirectory.Substring(0, AppContext.BaseDirectory.IndexOf("bin"));
            var result = context.Message;
            var fileData = result.Files;
            ////For Image saving on the server, for testing pupopses
            //if (fileData.Count < 1)
            //{
            //    await Console.Out.WriteLineAsync("No Faces Detected");
            //}
            //else
            //{
            //    fileData.Select((v, i) => new { item = v, index = i }).ToList()
            //    .ForEach(f =>
            //    {
            //        //loading byte[] to memory stream
            //        MemoryStream memoryStream = new MemoryStream(f.item);
            //        //create image from memorystream
            //        var image = SixLabors.ImageSharp.Image.Load(memoryStream.ToArray());
            //        //saving image in root folder
            //        image.Save($"{rootFolder}/Images/face{f.index}.jpg");
            //    });
            //}
            //Here We will Add the EmailSending code
            string[] mailAddresses = { result.Email };


            await _emailSender.SendEmailAsync(new Message(mailAddresses, "Order " + result.Id, "From FacesAndFaces", result.Files));

            await context.Publish<IOrderDispatchedEvent>(new
            {
                context.Message.Id,
                DispatchDataTime = DateTime.UtcNow
            });
        }


    }
}
