using System;
using RabbitMQ.Client;
using System.Text;
using QueueMessageSender.Models;
using System.Threading;

namespace QueueMessageSender
{
    class RmqMessageSender : IQueueMessageSender
    {
        protected IModel channel;
        protected IConnection connection;
        protected ConnectionFactory factory;

        public void CreateConnection()
        {
            if (factory == null) 
            {
                factory = new ConnectionFactory() { HostName = "localhost" };
                factory.AutomaticRecoveryEnabled = true;
                connection = factory.CreateConnection();
                channel = connection.CreateModel();
                Timer timer = new Timer(CreateChannel, 0, Convert.ToInt32(channel.ContinuationTimeout.TotalMilliseconds), Convert.ToInt32(channel.ContinuationTimeout.TotalMilliseconds));
            }
        }

        private void CreateChannel(object status) 
        {
            channel.Close();
            channel = connection.CreateModel();
        }

        public void SendMessage(DepartureData data)
        {
            try
            {
                channel.ExchangeDeclare(exchange: data.NameExchange, type: ExchangeType.Fanout);

                var body = Encoding.UTF8.GetBytes((char[])data.Message);

                channel.BasicPublish(exchange: data.NameExchange,
                                     routingKey: data.RoutingKey,
                                     basicProperties: null,
                                     body: body);
            }
            catch (Exception e){}
        }
    }
}
