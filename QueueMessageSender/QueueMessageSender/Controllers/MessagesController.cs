using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using QueueMessageSender.Logic;
using QueueMessageSender.Models;

namespace QueueMessageSender.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    [Produces("application/json")]
    public class MessagesController : ControllerBase
    {
        private readonly IQueueMessageSender _sender;

        public MessagesController(IQueueMessageSender sender)
        {
            _sender = sender;
        }

        /// <summary>
        /// A method that accepts JSON with message information and sends the message in exchange.
        /// </summary>
        [HttpPost]
        public IActionResult Send(ReceivedDataModel model)
        {
            var departureData = new DepartureDatаRMQModel
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