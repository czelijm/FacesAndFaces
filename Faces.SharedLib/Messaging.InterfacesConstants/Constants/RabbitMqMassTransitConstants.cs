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

    }
}
