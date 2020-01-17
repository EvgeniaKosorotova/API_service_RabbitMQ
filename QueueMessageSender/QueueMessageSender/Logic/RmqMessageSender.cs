using QueueMessageSender.Logic.Entities;
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
    public class RMQMessageSender : IQueueMessageSender
    {
        public RMQMessageSender()
        {
            CreateConnection();
        }

        #region Factory field and property
        private Lazy<ConnectionFactory> _factoryLazy = new Lazy<ConnectionFactory>(() =>
        {
            return null;
        });
        private ConnectionFactory Factory
        {
            get
            {
                return _factoryLazy.Value;
            }
        }
        #endregion

        #region Connection field and property
        private Lazy<IConnection> _connectionLazy = new Lazy<IConnection>(() =>
        {
            return null;
        });
        private IConnection Connection
        {
            get
            {
                return _connectionLazy.Value;
            }
        }
        #endregion

        #region Channel field and property
        private Lazy<IModel> _channelLazy = new Lazy<IModel>(() =>
        {
            return null;
        });
        private IModel Channel
        {
            get
            {
                return _channelLazy.Value;
            }
        }
        #endregion

        #region Channel field and property
        private Lazy<List<string>> _namesExchangeLazy = new Lazy<List<string>>(() =>
        {
            return new List<string>();
        });
        private List<string> NamesExchange
        {
            get
            {
                return _namesExchangeLazy.Value;
            }
        }
        #endregion

        private void CreateConnection()
        {
            _factoryLazy = new Lazy<ConnectionFactory>(() =>
            {
                ConnectionFactory factory = new ConnectionFactory() { HostName = "localhost" };
                factory.AutomaticRecoveryEnabled = true;

                return factory;
            });

            _connectionLazy = new Lazy<IConnection>(() =>
            {
                return Factory.CreateConnection();
            });

            _channelLazy = new Lazy<IModel>(() =>
            {
                return Connection.CreateModel();
            });

            Timer timer = new Timer(CreateChannel, 0, Convert.ToInt32(Channel.ContinuationTimeout.TotalMilliseconds), Convert.ToInt32(Channel.ContinuationTimeout.TotalMilliseconds));
        }

        private void CreateChannel(object status)
        {
            Channel.Close();
            _channelLazy = new Lazy<IModel>(() =>
            {
                return Connection.CreateModel();
            });
        }

        private void VerifyExchangeCreation(string nameExchange) 
        {
            if (!NamesExchange.Contains(nameExchange))
            {
                try
                {
                    Channel.ExchangeDeclare(exchange: nameExchange, type: ExchangeType.Fanout);
                    NamesExchange.Add(nameExchange);
                }
                catch (RabbitMQ.Client.Exceptions.AlreadyClosedException)
                {
                    CreateConnection();
                }
            }
        }

        public void SendMessage(DepartureDatаRMQModel data)
        {
            if (Factory == null || Connection == null || Channel == null)
            {
                CreateConnection();
            }

            VerifyExchangeCreation(data.NameExchange);

            try
            {
                Channel.BasicPublish(exchange: data.NameExchange,
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
