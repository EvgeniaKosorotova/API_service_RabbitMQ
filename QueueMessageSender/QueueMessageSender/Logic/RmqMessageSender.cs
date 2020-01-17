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

        private Lazy<ConnectionFactory> _factory = new Lazy<ConnectionFactory>(() =>
        {
            return null;
        });
        private Lazy<IConnection> _connection = new Lazy<IConnection>(() =>
        {
            return null;
        });
        private Lazy<IModel> _channel = new Lazy<IModel>(() =>
        {
            return null;
        });
        private Lazy<List<string>> _namesExchange = new Lazy<List<string>>(() =>
        {
            return new List<string>();
        });

        private void CreateConnection()
        {
            _factory = new Lazy<ConnectionFactory>(() =>
            {
                ConnectionFactory factory = new ConnectionFactory() { HostName = "localhost" };
                factory.AutomaticRecoveryEnabled = true;
                return factory;
            });

            _connection = new Lazy<IConnection>(() =>
            {
                ConnectionFactory factory = _factory.Value;
                return factory.CreateConnection();
            });

            _channel = new Lazy<IModel>(() =>
            {
                IConnection connection = _connection.Value;
                return connection.CreateModel();
            });

            Timer timer = new Timer(CreateChannel, 0, Convert.ToInt32((_channel.Value).ContinuationTimeout.TotalMilliseconds), Convert.ToInt32((_channel.Value).ContinuationTimeout.TotalMilliseconds));
        }

        private void CreateChannel(object status)
        {
            (_channel.Value).Close();
            _channel = new Lazy<IModel>(() =>
            {
                IConnection connection = _connection.Value;
                return connection.CreateModel();
            });
        }

        private void VerifyExchangeCreation(string nameExchange) 
        {
            if (!(_namesExchange.Value).Contains(nameExchange))
            {
                try
                {
                    (_channel.Value).ExchangeDeclare(exchange: nameExchange, type: ExchangeType.Fanout);
                    (_namesExchange.Value).Add(nameExchange);
                }
                catch (RabbitMQ.Client.Exceptions.AlreadyClosedException)
                {
                    CreateConnection();
                }
            }
        }

        public void SendMessage(DepartureDataModel data)
        {
            if (_factory.Value == null || _connection.Value == null || _channel.Value == null)
            {
                CreateConnection();
            }

            VerifyExchangeCreation(data.NameExchange);

            try
            {
                (_channel.Value).BasicPublish(exchange: data.NameExchange,
                                 routingKey: data.RoutingKey,
                                 basicProperties: null,
                                 body: JsonSerializer.SerializeToUtf8Bytes(data.Message));
            }
            catch (RabbitMQ.Client.Exceptions.AlreadyClosedException) 
            {
                CreateConnection();
            }
        }


    }
}
