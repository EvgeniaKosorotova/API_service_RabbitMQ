using QueueMessageSender.Logic.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;

namespace QueueMessageSender.Logic
{
    /// <summary>
    /// Class to publish messages to the queue RabbitMQ.
    /// </summary>
    public class RMQMessageSender : IQueueMessageSender
    {
        private static readonly string hostname = "localhost";
        private ConnectionFactory Factory;
        private IConnection Connection;
        private IModel Channel;
        private readonly List<string> NamesExchange = new List<string>();
        private readonly object lockList = new object();
        private readonly object lockSend = new object();
        private DepartureDatаRMQModel datаRMQ;
        public RMQMessageSender()
        {
            Create();
        }

        private void Create() 
        {
            Factory = new ConnectionFactory() { HostName = hostname };
            Factory.AutomaticRecoveryEnabled = true;
            CreateConnection();
            CreateChannel();
            //Console.WriteLine("Создание (Create)");
            //Connection.CallbackException += CreateConnection;
            //Channel.ModelShutdown += CreateChannel;
        }

        private void CreateConnection(object sender = null, CallbackExceptionEventArgs e = null) 
        {
            Connection = Factory.CreateConnection();
            //Console.WriteLine("Создание Подключения (Connection)");
        }

        private void CreateChannel(object sender = null, ShutdownEventArgs e = null)
        {
            Channel = Connection.CreateModel();
            //Console.WriteLine("Создание Канала (Channel)");
        }

        /// <summary>
        /// A method that checks the creation of an exchange name earlier. 
        /// If the exchange name has not been created, then it is created.
        /// </summary>
        private void CreateExchange(string nameExchange) 
        {
            lock (lockList) 
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
                        SendMessage(datаRMQ);
                    }
                }
            }
        }

        public void SendMessage(DepartureDatаRMQModel data)
        {
            lock (lockSend)
            {
                datаRMQ = data;
                CreateExchange(data.NameExchange);
                try
                {
                    Channel.BasicPublish(exchange: data.NameExchange,
                                    routingKey: data.RoutingKey,
                                    basicProperties: null,
                                    body: JsonSerializer.SerializeToUtf8Bytes(data.Message));
                }
                catch (RabbitMQ.Client.Exceptions.AlreadyClosedException)
                {
                    SendMessage(data);
                }
            }
        }
    }
}
