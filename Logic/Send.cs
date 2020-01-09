using System;
using RabbitMQ.Client;
using System.Text;
using Send.Models;
using System.Threading;

namespace Send
{
    class Send : ISending
    {
        public static IModel Channel { get; set; }
        public static IConnection Connection { get; set; }
        public static ConnectionFactory Factory { get; set; }

        public static void CreateConnection()
        {
            if (Factory == null) 
            { 
                Factory = new ConnectionFactory() { HostName = "localhost" };
                Factory.AutomaticRecoveryEnabled = true;
                Connection = Factory.CreateConnection();
                Channel = Connection.CreateModel();
                Timer timer = new Timer(CreateChannel, 0, Convert.ToInt32(Channel.ContinuationTimeout.TotalMilliseconds), Convert.ToInt32(Channel.ContinuationTimeout.TotalMilliseconds));
            }
        }

        private static void CreateChannel(object status) 
        {
            Channel.Close();
            Channel = Connection.CreateModel();
        }

        public static int SendMessage(EndpointData endpointData)
        {
            if (endpointData.NameExchange != null && endpointData.RoutingKey != null)
            {
                try
                {
                    Channel.ExchangeDeclare(exchange: endpointData.NameExchange, type: ExchangeType.Fanout);

                    var message = endpointData.Message;
                    var body = Encoding.UTF8.GetBytes(Convert.ToString(message));

                    Channel.BasicPublish(exchange: endpointData.NameExchange,
                                            routingKey: endpointData.RoutingKey,
                                            basicProperties: null,
                                            body: body);
                    return 200;
                }
                catch (Exception e)
                {
                    return 500;
                }
            }
            else 
            {
                return 400;

            }
        }
    }
}
