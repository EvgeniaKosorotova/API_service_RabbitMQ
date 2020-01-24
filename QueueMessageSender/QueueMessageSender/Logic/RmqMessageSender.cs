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
        private ConnectionFactory Factory = null;
        private IConnection Connection = null;
        private IModel Channel = null;
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
            Reconnect();
            Console.WriteLine("Создание (Create)");
            Connection.ConnectionShutdown += Reconnect;
            Channel.ModelShutdown += Reconnect;
        }

        private void Reconnect(object sender = null, ShutdownEventArgs e = null)
        {
            try
            {
                Console.WriteLine("!Создание (Reconnect)");
                if (Connection == null || !Connection.IsOpen)
                {
                    Console.WriteLine("!Создание (Connection)");
                    Connection = Factory.CreateConnection();
                }
                if (Channel == null || Channel.IsClosed)
                {
                    Console.WriteLine("!Создание (Channel)");
                    Channel = Connection.CreateModel();
                }
            }
            catch (RabbitMQ.Client.Exceptions.BrokerUnreachableException) 
            {
                Console.WriteLine("catch BrokerUnreachableException (Reconnect)");
                Reconnect();
            }
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
                    catch (RabbitMQ.Client.Exceptions.OperationInterruptedException)
                    {
                        Console.WriteLine("catch OperationInterruptedException (CreateExchange)");
                        Reconnect();
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
                catch (Exception)
                {
                    Console.WriteLine("catch Exception Reconnect(); SendMessage(datаRMQ); (SendMessage) catch");
                    Reconnect();
                    SendMessage(datаRMQ);
                }
            }
        }
    }
}
