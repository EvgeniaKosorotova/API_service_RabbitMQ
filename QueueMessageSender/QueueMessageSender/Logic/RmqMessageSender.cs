using QueueMessageSender.Models;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;

namespace QueueMessageSender.Logic
{
    /// <summary>
    /// Class to publish messages to the queue RabbitMQ.
    /// </summary>
    public class RmqMessageSender : IQueueMessageSender
    {
        public RmqMessageSender()
        {
            CreateConnection();
        }

        private static IModel _channel;
        private static IConnection _connection;
        private static ConnectionFactory _factory;
        private static readonly object _padlock = new object();
        private static List<string> _namesExchange = new List<string>();

        private void CreateConnection()
        {
            if (_factory == null)
            {
                lock (_padlock)
                {
                    if (_factory == null)
                    {
                        _factory = new ConnectionFactory() { HostName = "localhost" };
                        _factory.AutomaticRecoveryEnabled = true;
                        _connection = _factory.CreateConnection();
                        _channel = _connection.CreateModel();
                        Timer timer = new Timer(CreateChannel, 0, Convert.ToInt32(_channel.ContinuationTimeout.TotalMilliseconds), Convert.ToInt32(_channel.ContinuationTimeout.TotalMilliseconds));
                    }
                }
            }
        }

        private static void CreateChannel(object status)
        {
            _channel.Close();
            _channel = _connection.CreateModel();
        }

        public void SendMessage(DepartureData data)
        {
            if (_factory == null || _connection == null || _channel == null)
            {
                CreateConnection();
            }

            if (!_namesExchange.Contains(data.NameExchange))
            {
                _channel.ExchangeDeclare(exchange: data.NameExchange, type: ExchangeType.Fanout);
                _namesExchange.Add(data.NameExchange);
            }

            _channel.BasicPublish(exchange: data.NameExchange,
                                 routingKey: data.RoutingKey,
                                 basicProperties: null,
                                 body: JsonSerializer.SerializeToUtf8Bytes(data.Message));
        }
    }
}
