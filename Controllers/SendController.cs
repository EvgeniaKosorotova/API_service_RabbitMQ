using Microsoft.AspNetCore.Mvc;
using QueueMessageSender.Models;

namespace QueueMessageSender.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        // POST: api/Send
        [HttpPost]
        public IActionResult Post(EndpointData model)
        {
            var departureData = new DepartureData
            {
                NameExchange = model.NameExchange,
                RoutingKey = model.RoutingKey,
                Message = model.Message
            };
            var sender = new RmqMessageSender();
            sender.CreateConnection();
            sender.SendMessage(departureData);
            return Ok();
        }
    }
}
