using Microsoft.AspNetCore.Mvc;
using QueueMessageSender.Logic;
using QueueMessageSender.Models;
using System.Threading.Tasks;

namespace QueueMessageSender.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class RolesController : ControllerBase
    {
        private readonly IRoleManager _roleManager;

        public RolesController(IRoleManager roleManager)
        {
            _roleManager = roleManager;
        }

        /// <summary>
        /// A method of getting list roles from the database.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var roles = await _roleManager.GetAllAsync();

            return Ok(new RolesResultModel { 
                Roles = roles
            });
        }
    }
}