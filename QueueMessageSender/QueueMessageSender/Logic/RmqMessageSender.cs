using Microsoft.Extensions.Logging;
using Polly;
using QueueMessageSender.Logic.Models;
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
        private static readonly string hostname = "localhost";
        private ConnectionFactory Factory = null;
        private IConnection Connection = null;
        private IModel Channel = null;
        private readonly List<string> NamesExchange = new List<string>();
        private readonly object lockList = new object();
        private readonly object lockFactory = new object();
        private readonly object lockConnection = new object();
        private readonly object lockChannel = new object();
        private readonly object lockConnect = new object();
        private DepartureDatаRMQModel datаRMQ;
        private readonly ILogger<RMQMessageSender> _logger;
        private readonly Policy retry = Policy
            .Handle<Exception> ()
            .WaitAndRetry(3, retryAttempt => TimeSpan.FromSeconds(5));

        public RMQMessageSender(ILogger<RMQMessageSender> logger)
        {
            _logger = logger;
            if (Factory == null || Connection?.CloseReason != null || Channel?.CloseReason != null)
            {
                Factory = new ConnectionFactory() { HostName = hostname };
                Factory.AutomaticRecoveryEnabled = true;
                Connection = Factory.CreateConnection();
                Channel = Connection.CreateModel();
                _logger.LogInformation("Connection factory, connection, channel were created.");

                Connection.ConnectionShutdown += Reconnect;
                Channel.ModelShutdown += Reconnect;

                Thread.Sleep(5000);
            }
        }

        private void Reconnect(object sender = null, ShutdownEventArgs e = null)
        {
            retry.Execute(() =>
            {
                try
                {
                    if (Factory == null)
                    {
                        lock (lockFactory)
                        {
                            if (Factory == null)
                            {
                                _logger.LogInformation("Connection factory has recreated.");
                                Channel?.Close();
                                Channel = null;
                                Connection?.Close();
                                Connection = null;
                                Factory = new ConnectionFactory() { HostName = hostname };
                                Factory.AutomaticRecoveryEnabled = true;
                                NamesExchange.Clear();
                                _logger.LogInformation("!Connection factory has recreated.");
                            }
                        }
                    }
                    if (Connection == null || Connection?.CloseReason != null)
                    {
                        lock (lockConnection)
                        {
                            if (Connection == null || Connection?.CloseReason != null)
                            {
                                _logger.LogInformation("Connection has recreated.");
                                Connection?.Close();
                                Connection = Factory.CreateConnection();
                                _logger.LogInformation("!Connection has recreated.");
                            }
                        }
                    }
                    if (Channel == null || Channel?.CloseReason != null)
                    {
                        lock (lockChannel)
                        {
                            if (Channel == null || Channel?.CloseReason != null)
                            {
                                _logger.LogInformation("Connection factory has recreated.");
                                Channel?.Close();
                                Channel = Connection.CreateModel();
                                _logger.LogInformation("!Connection factory has recreated.");
                            }
                        }
                    }
                    _logger.LogInformation("Try reconnect.");
                }
                catch (RabbitMQ.Client.Exceptions.BrokerUnreachableException ex) 
                {
                    _logger.LogWarning(ex, "Error in Reconnect method");
                }
            });
        }

        /// <summary>
        /// A method that checks the creation of an exchange name earlier. 
        /// If the exchange name has not been created, then it is created.
        /// </summary>
        private void InitExchange(string nameExchange) 
        {
            if (!NamesExchange.Contains(nameExchange))
            {
                lock (lockList)
                {
                    if (!NamesExchange.Contains(nameExchange))
                    {
                        try
                        {
                            Channel.ExchangeDeclare(exchange: nameExchange, type: ExchangeType.Fanout);
                            NamesExchange.Add(nameExchange);
                            _logger.LogInformation($"An exchange with a new name \"{nameExchange}\" has been announced.");
                        }
                        catch (RabbitMQ.Client.Exceptions.OperationInterruptedException ex)
                        {
                            _logger.LogWarning(ex, "Error in InitExchange method");
                            Reconnect();
                            SendMessage(datаRMQ);
                        }
                    }
                }
            }
        }

        public void SendMessage(DepartureDatаRMQModel data)
        {
            if (Factory == null || Connection?.CloseReason != null || Channel?.CloseReason != null) 
            {
                lock (lockConnect)
                {
                    if (Factory == null || Connection?.CloseReason != null || Channel?.CloseReason != null)
                    {
                        Reconnect();
                    }
                }
            }
            datаRMQ = data;
            InitExchange(data.NameExchange);
            try
            {
                Channel.BasicPublish(exchange: data.NameExchange,
                                    routingKey: data.RoutingKey,
                                    basicProperties: null,
                                    body: JsonSerializer.SerializeToUtf8Bytes(data.Message));
                _logger.LogInformation($"Message has been sent. {data.RoutingKey}");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error in SendMessage method");
                Reconnect();
                SendMessage(datаRMQ);
            }
        }
    }
}
