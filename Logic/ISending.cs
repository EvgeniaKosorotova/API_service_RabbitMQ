using RabbitMQ.Client;
using Send.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Send
{
    interface ISending
    {
        public IModel channel { get; set; }
        public static IModel ConnectionToChannel();
        public static void SendMessage(IModel channel, EndpointData endpoint) { }
    }
}
