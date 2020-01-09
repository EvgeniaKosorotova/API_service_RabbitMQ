using System;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Send.Models;

namespace Send.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SendController : ControllerBase
    {
        // POST: api/Send
        [HttpPost]
        public IActionResult Post([FromBody] object value)
        {
            EndpointData endpointData = JsonSerializer.Deserialize<EndpointData>(Convert.ToString(value));
            Send.CreateConnection();
            int statusCode = Send.SendMessage(endpointData);
            return StatusCode(statusCode);
        }
    }
}
