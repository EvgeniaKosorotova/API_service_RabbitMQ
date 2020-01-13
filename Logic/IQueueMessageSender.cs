using RabbitMQ.Client;
using Send.Models;


namespace Send
{
    /// <summary>
    /// Abstaction to publish messages to the queue.
    /// </summary>
    interface IQueueMessageSender
    {
        public const int ServerErrorResponse = 501;

        static void CreateConnection() { }
        static int SendMessage(EndpointData endpoint) { return ServerErrorResponse; }
    }
}
