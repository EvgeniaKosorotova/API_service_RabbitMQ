using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Send.Models;

namespace Send.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SendController : ControllerBase
    {
        //// GET: api/Send
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET: api/Send/5
        //[HttpGet("{id}", Name = "Get")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST: api/Send
        [HttpPost]
        public void Post([FromBody] object value)
        {
            EndpointData endpoint = JsonSerializer.Deserialize<EndpointData>(Convert.ToString(value));
            Send.SendMessage(Send.ConnectionToChannel(), endpoint);
        }

        //// PUT: api/Send/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE: api/ApiWithActions/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
