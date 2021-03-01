using MassTransit;
using Messaging.InterfacesConstants.Events;
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
        public async Task Consume(ConsumeContext<IOrderProcessedEvent> context)
        {
            var rootFolder = AppContext.BaseDirectory.Substring(0, AppContext.BaseDirectory.IndexOf("bin"));
            var result = context.Message;
            var fileData = result.Files;
            if (fileData.Count<1)
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
                    var image = Image.FromStream(memoryStream);
                    //saving image in root folder
                    image.Save($"{rootFolder}/Images/face{f.index}.jpg",ImageFormat.Jpeg);
                });
            }
            //Here We will Add the EmailSending code
            //--
            //--    
            //--

            await context.Publish<IOrderDispatchedEvent>(new 
            {
                context.Message.Id,
                DispatchDataTime = DateTime.UtcNow
            });

        }
    }
}
