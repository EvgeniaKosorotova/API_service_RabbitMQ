using RabbitMQ.Client;
using Send.Models;


namespace Send
{
    interface ISending
    {
        static IConnection Connection { get; set; }
        static void CreateConnection() { }
        static int SendMessage(EndpointData endpoint) { return 501; }
    }
}
