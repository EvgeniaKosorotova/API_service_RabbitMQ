using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QueueMessageSender.Controllers.Models;
using QueueMessageSender.Logic;
using QueueMessageSender.Logic.Models;

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
        public IActionResult Register(AuthenticationModel authData)
        {
            UserModel user = _userManager.GetAsync(username: authData.Username).Result;

            if (!(user != null && user.Username == authData.Username && user.Password == authData.Password))
                if (_userManager.CreateAsync(authData.Username, authData.Password).Result)
                {
                    return Created(string.Empty,
                        new AuthenticationResultModel
                        {
                            Error = "Credentials were recorded and saved."
                        });
                }
            return BadRequest(
                new AuthenticationResultModel
                {
                    Error = "Credentials have been created earlier."
                });
        }

        /// <summary>
        /// Method to delete a record from the database.
        /// </summary>
        [HttpDelete]
        public IActionResult Delete(AuthenticationModel authData)
        {
            UserModel user = _userManager.GetAsync(username: authData.Username).Result;
            if (user != null && user.Username == authData.Username && user.Password == authData.Password)
                if (_userManager.DeleteAsync(authData.Username).Result)
                {
                    return Ok(
                        new AuthenticationResultModel
                        {
                            Error = "Credentials have been deleted."
                        });
                }
            return NotFound(
                new AuthenticationResultModel
                {
                    Error = "No credentials were found or deleted."
                });
        }
    }
}