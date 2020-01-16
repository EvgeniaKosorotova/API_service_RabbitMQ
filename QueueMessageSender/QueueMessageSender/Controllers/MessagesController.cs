using Microsoft.AspNetCore.Mvc;
using QueueMessageSender.Logic;
using QueueMessageSender.Models;

namespace QueueMessageSender.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IQueueMessageSender _sender;

        public MessagesController(IQueueMessageSender sender)
        {
            _sender = sender;
        }

        // POST: Messages
        [HttpPost]
        public IActionResult Post([FromBody]EndpointData model)
        {
            var departureData = new DepartureData
            {
                NameExchange = model.Exchange,
                RoutingKey = model.Key,
                Message = model.Message
            };
            _sender.SendMessage(departureData);
            return Ok();
        }
    }
}