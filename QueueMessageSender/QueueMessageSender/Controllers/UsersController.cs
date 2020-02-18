using Microsoft.AspNetCore.Mvc;
using QueueMessageSender.Controllers.Models;
using QueueMessageSender.Logic;
using QueueMessageSender.Logic.Models;
using System.Threading.Tasks;

namespace QueueMessageSender.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class UsersController : ControllerBase
    {
        private readonly IUserManager _userManager;

        public UsersController(IUserManager userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// A method of registering and saving new credentials in a database.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> RegisterAsync(AuthenticationModel authData)
        {
            UserModel user = _userManager.GetAsync(username: authData.Username).GetAwaiter().GetResult();

            if (!(user != null && user.Username == authData.Username && user.Password == _userManager.GetHash(authData.Password)))
                if (await _userManager.CreateAsync(authData.Username, authData.Password))
                {
                    return Created(string.Empty,
                        new AuthenticationResultModel
                        {
                            Message = "Credentials were recorded and saved."
                        });
                }
            return BadRequest(
                new ErrorModel
                {
                    Error = "Credentials have been created earlier."
                });
        }

        /// <summary>
        /// Method to delete a record from the database.
        /// </summary>
        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(AuthenticationModel authData)
        {
            UserModel user = _userManager.GetAsync(username: authData.Username, password: authData.Password).GetAwaiter().GetResult();
            if (user != null)
                if (await _userManager.DeleteAsync(authData.Username))
                {
                    return Ok(
                        new AuthenticationResultModel
                        {
                            Message = "Credentials have been deleted."
                        });
                }
            return NotFound(
                new ErrorModel
                {
                    Error = "No credentials were found or deleted."
                });
        }
    }
}