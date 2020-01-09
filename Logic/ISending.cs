using RabbitMQ.Client;
using Send.Models;


namespace Send
{
    interface ISending
    {
        static void CreateConnection() { }
        static int SendMessage(EndpointData endpoint) { return 501; }
    }
}
