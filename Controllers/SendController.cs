using System;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Send.Models;

namespace Send.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        // POST: api/Send
        [HttpPost]
        public IActionResult Post(EndpointData model)
        {
            RmqMessageSender.CreateConnection();
            int statusCode = RmqMessageSender.SendMessage(model);
            return StatusCode(statusCode);
        }
    }
}
