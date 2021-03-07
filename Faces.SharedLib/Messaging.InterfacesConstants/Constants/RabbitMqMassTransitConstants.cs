using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.InterfacesConstants.Constants
{
    public class RabbitMqMassTransitConstants
    {
        //if you do not speify port number
        //masstransit assumes that you use defal port anyway
        public const string RabbitMquriWithPort = "rabbitmq://rabbitmq:5672";
        public const string RabbitMquri = "rabbitmq://rabbitmq/";
        public const string UserName = "guest";
        public const string Password = "guest";
        //this is the point, where our MVC project will send the data to
        public const string RegisterOrderCommandQueue = "register.order.command";
        public const string NotificationServiceQueue = "notification.service.queue";
        public const string OrderDispatchedServiceQueue = "orfder.dispatch.service.queue";
        //public const string HostName = "localhost";
        //public const string Port = "6001";
        public const string HostName = "rabbitmq";
        public const string Port = "6100";
        public static string HostAddress = $"http://{HostName}:{Port}";
        public const string FaceApiUri = "/api/Faces";
        public const int RetryNumber = 2;
        public const int ItervalWaitTimeInSeconds = 10;
        public const int PerfectchCount = 16;



    }
}
