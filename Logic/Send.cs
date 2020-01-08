using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RabbitMQ.Client;
using System.Text;
using Send.Models;

namespace Send
{
    class Send : ISending
    {
        public IModel channel { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public static void SendMessage(IModel channel, EndpointData endpoint)
        {
                channel.ExchangeDeclare(exchange: endpoint.NameExchange, type: ExchangeType.Fanout);

                var message = endpoint.Message;
                var body = Encoding.UTF8.GetBytes(Convert.ToString(message));

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                channel.BasicPublish(exchange: endpoint.NameExchange,
                                        routingKey: endpoint.RoutingKey,
                                        basicProperties: properties,
                                        body: body);
        }

        public static IModel ConnectionToChannel() 
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            return channel;
        }
    }
}
