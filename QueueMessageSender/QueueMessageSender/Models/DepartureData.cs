namespace QueueMessageSender.Models
{
    /// <summary>
    /// The data model includes a message and data for sending messages for RabbitMQ.
    /// </summary>
    public class DepartureData
    {
        public string NameExchange;
        public string RoutingKey;
        public object Message;
    }
}
