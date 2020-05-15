using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using QueueMessageSender.Logic;
using QueueMessageSender.Models;
using System.Threading.Tasks;

namespace QueueMessageSender.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class UsersController : ControllerBase
    {
        private readonly IUserManager _userManager;
        private readonly ITokenManager _tokenManager;

        public UsersController(IUserManager userManager, ITokenManager tokenManager)
        {
            _userManager = userManager;
            _tokenManager = tokenManager;
        }

        /// <summary>
        /// A method of registering and saving new credentials in a database.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> RegisterAsync(AuthenticationModel authData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorModel
                {
                    Error = "Incoming data is not valid."
                });
            }

            UserModel user = await _userManager.GetByUsernameAsync(username: authData.Username);

            if (user == null)
            {
                UserModel userNew = await _userManager.CreateAsync(authData.Username, authData.Password);

                return Created(string.Empty, userNew);
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
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync([FromRoute]int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorModel
                {
                    Error = "Incoming data is not valid."
                });
            }

            UserModel user = await _userManager.GetByIdAsync(id: id);

            if (user != null)
            {
                await _userManager.DeleteAsync(id);
                await _tokenManager.DeleteTokensAsync(userId: id);

                return Ok(
                    new MessageModel
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