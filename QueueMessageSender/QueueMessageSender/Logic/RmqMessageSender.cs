using QueueMessageSender.Models;
using RabbitMQ.Client;
using System;
using System.Text.Json;
using System.Threading;

namespace QueueMessageSender.Logic
{
    class RmqMessageSender : IQueueMessageSender
    {
        private static IModel channel;
        private static IConnection connection;
        private static ConnectionFactory factory;

        //private ConnectionFactory factory;
        //private static readonly object padlock = new object();

        //public static ConnectionFactory Factory
        //{
        //    get
        //    {
        //        if (factory == null)
        //        {
        //            lock (padlock)
        //            {
        //                if (factory == null)
        //                {
        //                    factory = new ConnectionFactory() { HostName = "localhost" };
        //                    factory.AutomaticRecoveryEnabled = true;
        //                    connection = factory.CreateConnection();
        //                    channel = connection.CreateModel();
        //                    Timer timer = new Timer(CreateChannel, 0, Convert.ToInt32(channel.ContinuationTimeout.TotalMilliseconds), Convert.ToInt32(channel.ContinuationTimeout.TotalMilliseconds));
        //                }
        //            }
        //        }
        //        return factory;
        //    }
        //}

        /// <summary>
        /// Class to publish messages to the queue RabbitMQ.
        /// </summary>
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

        private static void CreateChannel(object status)
        {
            channel.Close();
            channel = connection.CreateModel();
        }

        public void SendMessage(DepartureData data)
        {
            channel.ExchangeDeclare(exchange: data.NameExchange, type: ExchangeType.Fanout);

            channel.BasicPublish(exchange: data.NameExchange,
                                 routingKey: data.RoutingKey,
                                 basicProperties: null,
                                 body: JsonSerializer.SerializeToUtf8Bytes(data.Message));
        }
    }
}
