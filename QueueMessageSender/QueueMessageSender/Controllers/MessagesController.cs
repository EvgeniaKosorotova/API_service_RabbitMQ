using Microsoft.AspNetCore.Mvc;
using QueueMessageSender.Logic;
using QueueMessageSender.Models;

namespace QueueMessageSender.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        // POST: api/Send
        [HttpPost]
        public IActionResult Post([FromBody]EndpointData model)
        {
            var departureData = new DepartureData
            {
                NameExchange = model.Exchange,
                RoutingKey = model.Key,
                Message = model.Message
            };
            RmqMessageSender sender = RmqMessageSender.Instance;
            sender.SendMessage(departureData);
            return Ok();
        }
    }
}