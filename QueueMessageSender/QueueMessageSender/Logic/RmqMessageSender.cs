using QueueMessageSender.Models;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;

namespace QueueMessageSender.Logic
{
    public class RmqMessageSender : IQueueMessageSender
    {
        public RmqMessageSender()
        {
            CreateConnection();
        }

        private static IModel channel;
        private static IConnection connection;
        private static ConnectionFactory factory;
        private static readonly object padlock = new object();
        private static List<string> namesExchange = new List<string>();

        private void CreateConnection()
        {
            if (factory == null)
            {
                lock (padlock)
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
            }
        }

        private static void CreateChannel(object status)
        {
            channel.Close();
            channel = connection.CreateModel();
        }

        public void SendMessage(DepartureData data)
        {
            if (factory == null || connection == null || channel == null)
            {
                CreateConnection();
            }

            if (!namesExchange.Contains(data.NameExchange))
            {
                channel.ExchangeDeclare(exchange: data.NameExchange, type: ExchangeType.Fanout);
                namesExchange.Add(data.NameExchange);
            }

            channel.BasicPublish(exchange: data.NameExchange,
                                 routingKey: data.RoutingKey,
                                 basicProperties: null,
                                 body: JsonSerializer.SerializeToUtf8Bytes(data.Message));
        }
    }
}
