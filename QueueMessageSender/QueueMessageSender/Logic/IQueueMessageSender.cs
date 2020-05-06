using QueueMessageSender.Models;

namespace QueueMessageSender.Logic
{
    /// <summary>
    /// Abstraction to publish messages to the queue.
    /// </summary>
    public interface IQueueMessageSender
    {
        bool SendMessage(DepartureDatаRMQModel data);
    }
}
