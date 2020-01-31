using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NLog;
using QueueMessageSender.Controllers.Models;
using QueueMessageSender.Logic;
using QueueMessageSender.Logic.Models;

namespace QueueMessageSender.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly ILogger<RMQMessageSender> _logger;
        private readonly IQueueMessageSender _sender;

        public MessagesController(IQueueMessageSender sender, ILogger<RMQMessageSender> logger)
        {
            _sender = sender;
            _logger = logger;
        }

        /// <summary>
        /// A method that accepts JSON with message information and sends the message in exchange.
        /// </summary>
        [HttpPost]
        public IActionResult Post(ReceivedDataModel model)
        {
            var departureData = new DepartureDatаRMQModel
            {
                NameExchange = model.Exchange,
                RoutingKey = model.Key,
                Message = model.Message
            };
            _logger.LogInformation($"Post method. Exchange: {departureData.NameExchange}, routing key: {departureData.RoutingKey}, message: {departureData.Message}");
            _sender.SendMessage(departureData);

            return Ok();
        }
    }
}